﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using ThreeArriveAction.BLL;
using ThreeArriveAction.Common;
using System.Web.SessionState;
using ThreeArriveAction.Model;
using ThreeArriveAction.Web.UI;
using System.Data;
using System.Reflection;
using System.Web.UI.WebControls;

namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// sys_UsersManager 的摘要说明
    /// </summary>
    public class sys_UsersManager : IHttpHandler, IRequiresSessionState
    {
        private readonly sys_UsersBLL usersBLL = new sys_UsersBLL();
        public void ProcessRequest(HttpContext context)
        {
            string type = MXRequest.GetQueryString("type");
            switch (type)
            {
                case "login":
                    Login(context);
                    break;
                case "exit":
                    Exit(context);
                    break;
                case "get":
                    GetUsersList(context);
                    break;
                case "edit":
                    GetUserModel(context);
                    break;
                case "save":
                    SaveUser(context);
                    break;
                case "checkOpenId":
                    CheckOpenId(context);
                    break;
                case "signInPhone":
                    SignInPhone(context);
                    break;
                case "del":
                    DeleteUser(context);
                    break;
                case "select":
                    GetUserTreeJson(context);
                    break;
                case "buty":
                    GetButyUsers(context);
                    break;
                case "weixinGetButyUser":
                    WeixinGetButyUser(context);
                    break;
                case "officer":
                    GetOfficeList(context);
                    break;
                case "getUsersByVillage":
                    GetUsersByVillage(context);
                    break;
                case "getUserPage":
                    GetUserPage(context);
                    break;
                case "changePwd":
                    ChangePassword(context);
                    break;
                case "myself":
                    GetMyUserModel(context);
                    break;
                case "update":
                    UpdateUserModel(context);
                    break;
                default:
                    context.Response.Write("错误的请求");
                    return;
            }
        }
        #region 登录
        private void Login(HttpContext context)
        {
            string uphone = context.Request.Form["uphone"].ToString();
            string upwd = context.Request.Form["upwd"].ToString();
            string result = usersBLL.Login(uphone, upwd);
            context.Response.Write(result);
        }
        #endregion

        #region 退出
        private void Exit(HttpContext context)
        {
            try
            {

                context.Session[MXKeys.SESSION_ADMIN_INFO] = null;
                Utils.WriteCookie("uphone", "ThreeArriveAction", -14400);
                Utils.WriteCookie("upwd", "ThreeArriveAction", -14400);

                //context.Session["yubomId"] = null;
                // Utils.WriteCookie("yubomId", "MxWeiXinPF", -14400);
                context.Response.Write("{\"success\":true}");
            }
            catch (Exception)
            {
                context.Response.Write("{\"success\":false}");
            }



        }

        #endregion

        #region 村居干部查询
        private void GetOfficeList(HttpContext context)
        {
            //获取当前登录的操作者信息
            sys_UsersModel model = new ManagePage().GetUsersinfo();
            if (model == null)
            {
                context.Response.Write("");
            }
            else
            {
                StringBuilder strSql = new StringBuilder();
                int pageSize = 20;
                int town = MXRequest.GetQueryIntValue("town");
                int vid = MXRequest.GetQueryIntValue("vid");
                int page = MXRequest.GetQueryInt("page", 1);
                string key = MXRequest.GetQueryString("key");
                key = key.Replace("'", "");
                strSql.Append(" OrganizationId in (3,4) ");
                if (town != 0 && vid == 0)
                {//镇编号不为0，村编号为0，查询该镇所有村居干部
                    strSql.Append("and VillageId in (select ViilageId from sys_Villages where VillageParId= " + town + ")");
                }
                else if (town != 0 && vid != 0)
                {
                    //镇编号不为0，村编号不为0，查询该村所有村居干部
                    strSql.Append(" and VillageId =" + vid);
                }
                if (key != "")
                {
                    strSql.Append(" and ( UserPhone like '%" + key + "%' or UserName like '%" + key + "%' or UserDuties like '%" + key + "%' ) ");
                }


                int totalCount = 0;
                StringBuilder strJson = new StringBuilder();

                DataSet ds = usersBLL.GetList(pageSize, page, strSql.ToString(), "UserId asc", out totalCount);
                var villageList = DataConfig.GetVillages();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    var parId = villageList.FirstOrDefault(x => x.VillageId == int.Parse(dr["VillageId"].ToString())).VillageParId;
                    var parName = villageList.FirstOrDefault(x => x.VillageId == parId).VillageName;
                    dr["VillageName"] = parName + "--" + dr["VillageName"].ToString();
                }
                strJson.Append("{\"total\":" + totalCount);
                if (totalCount > 0)
                {
                    strJson.Append(",\"rows\":" + JsonHelper.ToJson(ds.Tables[0]));
                }
                string pageContent = Utils.OutPageList(pageSize, page, totalCount, "Load(__id__)", 8);
                if (pageContent == "")
                {
                    strJson.Append(",\"pageContent\":\"\"");
                }
                else
                {
                    strJson.Append(",\"pageContent\":" + pageContent);
                }

                strJson.Append("}");
                context.Response.Write(strJson.ToString());
            }
        }

        #endregion

        #region 分页查询
        public void GetUsersList(HttpContext context)
        {
            //获取当前登录的操作者信息
            sys_UsersModel model = new ManagePage().GetUsersinfo();
            if (model == null)
            {
                context.Response.Write("<script>parent.location.href='http://wx.haqdj.gov.cn'</script>");
            }
            else
            {
                StringBuilder strSql = new StringBuilder();
                int pageSize = 10;
                int page = MXRequest.GetQueryInt("page", 1);
                string key = MXRequest.GetQueryString("keywords");
                key = key.Replace("'", "");
                //区管理员 能够查询所有人员
                if (model.OrganizationId == 1)
                {
                    strSql.Append(" 1=1 ");
                }
                else if (model.OrganizationId == 2)
                {
                    //镇组织委员 只能查询镇下行政村的人员
                    strSql.Append("VillageId in (select ViilageId from sys_Villages where VillageId=" + model.VillageId + " or VillageParId =)" + model.VillageId);
                }
                else
                {
                    //村负责人 只能查询本村的人员
                    strSql.Append(" VillageId =" + model.VillageId);
                }
                if (!string.IsNullOrEmpty(key))
                {
                    strSql.Append(" and ( UserPhone like '%" + key + "%' or UserName like '%" + key + "%' or UserDuties like '%" + key + "%' ) ");
                }
                int totalCount = 0;
                StringBuilder strJson = new StringBuilder();

                DataSet ds = usersBLL.GetList(pageSize, page, strSql.ToString(), "UserId asc", out totalCount);
                strJson.Append("{\"total\":" + totalCount);
                if (totalCount > 0)
                {
                    strJson.Append(",\"rows\":" + JsonHelper.ToJson(ds.Tables[0]));
                }
                string pageContent = Utils.OutPageList(pageSize, page, totalCount, "Load(__id__)", 8);
                if (pageContent == "")
                {
                    strJson.Append(",\"pageContent\":\"\"");
                }
                else
                {
                    strJson.Append(",\"pageContent\":" + pageContent);
                }

                strJson.Append("}");
                context.Response.Write(strJson.ToString());
            }
        }
        #endregion

        #region 加载该用户信息
        private void GetUserModel(HttpContext context)
        {
            int userid = int.Parse(MXRequest.GetQueryString("userid"));
            string result = usersBLL.GetUsersModelJson(userid);
            context.Response.Write(result);
        }
        #endregion

        #region 加载本人用户信息
        private void GetMyUserModel(HttpContext context)
        {
            //获取当前登录的操作者信息
            sys_UsersModel model = new ManagePage().GetUsersinfo();
            string result = "";
            if (model == null)
            {
                result = "";
            }
            else
            {
                result = usersBLL.GetUsersModelJson(model.UserId);
            }

            context.Response.Write(result);
        }
        #endregion

        #region 保存用户信息
        private void SaveUser(HttpContext context)
        {
            sys_UsersModel userModel = new sys_UsersModel();
            sys_UsersInfoModel infoModel = new sys_UsersInfoModel();
            userModel.UserPhone = MXRequest.GetFormString("userphone").ToString();
            userModel.UserName = MXRequest.GetFormString("username").ToString();
            userModel.UserDuties = MXRequest.GetFormString("userduties").ToString();
            userModel.OrganizationId = int.Parse(MXRequest.GetFormString("organid").ToString());
            userModel.VillageId = int.Parse(MXRequest.GetFormString("villageid").ToString());
            userModel.UserBirthday = MXRequest.GetFormString("userbirthday").ToString();
            userModel.UserPassword = MXRequest.GetFormString("userpassword").ToString();

            string action = MXRequest.GetFormString("action");
            bool blinfo = false;
            infoModel.UserPhoto = MXRequest.GetFormString("userphoto");
            infoModel.UserUrl = MXRequest.GetFormString("userurl");
            infoModel.PersonalIntroduction = MXRequest.GetFormString("introduction");
            infoModel.PersonalHonor = MXRequest.GetFormString("honor");
            infoModel.UserEducation = MXRequest.GetFormString("education");
            infoModel.JoinPartyDate = MXRequest.GetFormString("joinpartydate");
            infoModel.UserRemarks = MXRequest.GetFormString("remark");
            //循环查看infoModel中是否有属性值不为空或者不为空字符串
            PropertyInfo[] propertyInfo = infoModel.GetType().GetProperties();
            for (int i = 0; i < propertyInfo.Length; i++)
            {
                object objectValue = propertyInfo[i].GetGetMethod().Invoke(infoModel, null);
                if (objectValue == null)
                {
                    continue;
                }
                if (objectValue.ToString() != "" && objectValue.ToString() != "0")
                {
                    blinfo = true;
                }
            }
            if (blinfo)
            {
                userModel.UserInfoModel = infoModel;
            }
            string result = "";
            if (action.Contains("add"))
            {
                result = usersBLL.AddUser(userModel);
            }
            else
            {
                userModel.UserId = int.Parse(MXRequest.GetFormString("userid"));
                result = usersBLL.UpdateUser(userModel);
            }
            context.Response.Write(result);
        }

        public void UpdateUserModel(HttpContext context)
        {
            sys_UsersModel userModel = new sys_UsersModel();
            sys_UsersInfoModel infoModel = new sys_UsersInfoModel();
            userModel.UserPhone = MXRequest.GetFormString("userphone").ToString();
            userModel.UserName = MXRequest.GetFormString("username").ToString();
            userModel.UserDuties = MXRequest.GetFormString("userduties").ToString();
            //userModel.OrganizationId = int.Parse(MXRequest.GetFormString("organid").ToString());
            //userModel.VillageId = int.Parse(MXRequest.GetFormString("villageid").ToString());
            userModel.UserBirthday = MXRequest.GetFormString("userbirthday").ToString();
            userModel.UserPassword = MXRequest.GetFormString("userpassword").ToString();


            bool blinfo = false;
            infoModel.UserPhoto = MXRequest.GetFormString("userphoto");
            infoModel.UserUrl = MXRequest.GetFormString("userurl");
            infoModel.PersonalIntroduction = MXRequest.GetFormString("introduction");
            infoModel.PersonalHonor = MXRequest.GetFormString("honor");
            infoModel.UserEducation = MXRequest.GetFormString("education");
            infoModel.JoinPartyDate = MXRequest.GetFormString("joinpartydate");
            infoModel.UserRemarks = MXRequest.GetFormString("remark");
            //循环查看infoModel中是否有属性值不为空或者不为空字符串
            PropertyInfo[] propertyInfo = infoModel.GetType().GetProperties();
            for (int i = 0; i < propertyInfo.Length; i++)
            {
                object objectValue = propertyInfo[i].GetGetMethod().Invoke(infoModel, null);
                if (objectValue == null)
                {
                    continue;
                }
                if (objectValue.ToString() != "" && objectValue.ToString() != "0")
                {
                    blinfo = true;
                }
            }
            if (blinfo)
            {
                userModel.UserInfoModel = infoModel;
            }

            userModel.UserId = int.Parse(MXRequest.GetFormString("userid"));
            string result = usersBLL.UpdateUserModel(userModel);

            context.Response.Write(result);
        }
        #endregion

        #region 验证人员是否已注册
        public void CheckOpenId(HttpContext context)
        {
            var sh = new SqlHelper<sys_UsersModel>(new sys_UsersModel());
            sh.AddWhere("UserRemark", context.Request["openid"], RelationEnum.Like);
            var model = sh.Select().FirstOrDefault();
            if (model == null)
            {
                context.Response.Write(new
                {
                    code = 0
                }.ToJson());
            }
            else
            {
                context.Response.Write(new
                {
                    code = 1,
                    model
                }.ToJson());
            }
        }
        #endregion

        #region 注册手机号码
        public void SignInPhone(HttpContext context)
        {
            var sh = new SqlHelper<sys_UsersModel>(new sys_UsersModel());
            sh.AddWhere("UserPhone", context.Request["phone"]);
            var model = sh.Select().FirstOrDefault();
            if (model == null)
            {
                context.Response.Write("0");
                return;
            }
            sh = new SqlHelper<sys_UsersModel>(new sys_UsersModel());
            sh.AddUpdate("UserRemark", context.Request["openid"]);
            sh.AddWhere("UserPhone", context.Request["phone"]);
            sh.Update();
            model.UserRemark = context.Request["openid"];
            context.Response.Write(model.ToJson());
            return;
        }
        #endregion

        #region 删除用户
        private void DeleteUser(HttpContext context)
        {
            string userIds = MXRequest.GetFormString("str");
            string result = usersBLL.Delete(userIds);
            context.Response.Write(result);
        }
        #endregion

        #region 用户数据用于下拉
        private void GetUserTreeJson(HttpContext context)
        {
            string villid = MXRequest.GetQueryString("villid");
            sys_UsersModel model = new ManagePage().GetUsersinfo();
            StringBuilder strJson = new StringBuilder();
            if (model == null)
            {
                context.Response.Write("<script>parent.location.href='http://wx.haqdj.gov.cn'</script>");
            }
            else
            {
                if (model.OrganizationId == 4)
                {
                    strJson.Append("{\"total\":1,\"rows\":[{\"UserId\":" + model.UserId + ",\"UserName\":" + model.UserName + "}]}");
                }
                else
                {
                    StringBuilder strWhere = new StringBuilder();
                    if (villid != "")
                    {
                        strWhere.Append("VillageId in (select VillageId from sys_Villages where VillageId=" + villid + " or VillageParId=" + villid + ")");
                    }
                    else
                    {
                        if (model.OrganizationId != 1)
                        {
                            strWhere.Append("VillageId in (select VillageId from sys_Villages where VillageId=" + model.VillageId + " or VillageParId=" + model.VillageId + ")");
                        }
                    }
                    DataSet ds = usersBLL.GetList(0, strWhere.ToString(), " UserId asc ");
                    strJson.Append("{\"total\":" + ds.Tables[0].Rows.Count + ",\"rows\":");
                    if (ds.Tables[0].Rows.Count > 0)
                    {

                        strJson.Append(JsonHelper.ToJson(ds.Tables[0]));
                    }
                    else
                    {
                        strJson.Append("[]");
                    }
                    strJson.Append("}");
                }
                context.Response.Write(strJson.ToString());
            }
        }
        #endregion

        #region 获取设置值班人员数据
        private void GetButyUsers(HttpContext context)
        {//获取当前登录的操作者信息
            sys_UsersModel model = new ManagePage().GetUsersinfo();
            if (model == null)
            {
                context.Response.Write("<script>parent.location.href='http://wx.haqdj.gov.cn'</script>");
            }
            else
            {
                string key = MXRequest.GetQueryString("keywords");
                key = key.Replace("'", "");
                StringBuilder strWhere = new StringBuilder();
                strWhere.Append(" VillageId =" + model.VillageId);
                strWhere.Append(" and UserState=1 ");
                if (!string.IsNullOrEmpty(key))
                {
                    strWhere.Append(" and ( UserPhone like '%" + key + "%' or UserName like '%" + key + "%' or UserDuties like '%" + key + "%' ) ");
                }
                string result = usersBLL.GetButyUsers(strWhere.ToString(), model.VillageId);
                context.Response.Write(result);
            }

        }
        #endregion

        #region weixin获取值班人员信息

        public void WeixinGetButyUser(HttpContext context)
        {
            var villageList = DataConfig.GetVillages();
            var user = DataConfig.GetUsers(new Dictionary<string, object> { { "UserRemark", context.Request["openId"] } }).FirstOrDefault();
            if (user == null)
            {
                return;
            }
            var isEx = new SqlHelper<sys_OnButysModel>(new sys_OnButysModel());
            isEx.AddWhere("VillageId", user.VillageId);
            isEx.AddWhere("ButyDate", DateTime.Now.ToString("yyyy-MM-dd"), RelationEnum.Greater);
            var exist = isEx.Select().Any();
            var sh = new SqlHelper<sys_UsersModel>(new sys_UsersModel());
            sh.AddWhere("VillageId", user.VillageId);
            var sysUsersModels = sh.Select();

            var infoSh = new SqlHelper<sys_UsersInfoModel>(new sys_UsersInfoModel());
            var list = sysUsersModels as sys_UsersModel[] ?? sysUsersModels.ToArray();
            infoSh.AddWhere($" AND UserId IN ({string.Join(",", list.Select(x => x.UserId))})");
            var infoList = infoSh.Select();

            var village = villageList.FirstOrDefault(x => x.VillageId == user.VillageId);
            context.Response.Write(new
            {
                town = villageList.FirstOrDefault(x => x.VillageId == village?.VillageParId)?.VillageName ?? "淮安",
                village = village?.VillageName,
                villageId = user.VillageId,
                exist,
                list = list.Select(x => new
                {
                    value = x.UserId,
                    text = x.UserName,
                    phone = x.UserPhone,
                    img = infoList.FirstOrDefault(i => i.UserId == x.UserId)?.UserPhoto ?? ""
                })
            }.ToJson());
        }

        #endregion

        #region 获取用户更具villageId

        private void GetUsersByVillage(HttpContext context)
        {
            var villageList = DataConfig.GetVillages();
            var userList = DataConfig.GetUsers(new Dictionary<string, object>());

            var list = villageList.Where(x => x.VillageParId == 1).Select(x => new
            {
                id = x.VillageId,
                label = x.VillageName,
                children = villageList.Where(v => v.VillageParId == x.VillageId).Select(v => new
                {
                    id = v.VillageId,
                    label = v.VillageName,
                    children = userList.Where(u => u.VillageId == v.VillageId).Select(u => new
                    {
                        id = u.UserRemark,
                        label = u.UserName,
                        children = new ArrayList(),
                        disabled = u.UserRemark.IsNullOrEmpty()
                    }),
                    disabled = userList.Where(u => u.VillageId == v.VillageId).All(u => u.UserRemark.IsNullOrEmpty())
                }),
            });

            context.Response.Write(list.ToJson());
        }

        #endregion

        #region 修改密码

        private void ChangePassword(HttpContext context)
        {
            string result = "";
            //获取当前登录的操作者信息
            sys_UsersModel model = new ManagePage().GetUsersinfo();
            if (model == null)
            {
                result = "{\"info\":\"当前没有登录用户\",\"status\":\"n\"}";
            }
            else
            {
                string oldPassword = context.Request.Form["txtOldPassword"].ToString();
                string newPassword = context.Request.Form["txtPassword"].ToString();
                if (string.Equals(oldPassword, model.UserPassword))
                {
                    bool bl = usersBLL.ChangePassword(model.UserId, newPassword);

                    if (bl)
                    {
                        context.Session[MXKeys.SESSION_ADMIN_INFO] = null;
                        Utils.WriteCookie("uphone", "ThreeArriveAction", -14400);
                        Utils.WriteCookie("upwd", "ThreeArriveAction", -14400);
                        result = "{\"info\":\"密码修改成功\",\"status\":\"y\"}";
                    }
                    else
                    {
                        result = "{\"info\":\"密码修改失败\",\"status\":\"n\"}";
                    }

                }
                else
                {
                    result = "{\"info\":\"原始密码不正确\",\"status\":\"n\"}";
                }
            }
            context.Response.Write(result);
        }

        #endregion

        private void GetUserPage(HttpContext context)
        {
            var village = context.Request["ddlvillage"].ToInt();

            var page = context.Request["page"].ToInt();
            var sh = new SqlHelper<UserInfo>(new UserInfo(), "UserId", "UserId", "sys_Users")
            {
                PageConfig = new PageConfig
                {
                    PageSortField = "OrganizationId",
                    PageSize = context.Request["size"].ToInt(),
                    PageIndex = page,
                    SortEnum = SortEnum.Asc
                },
                Alia = "u"
            };
            sh.AddShow("i.UserPhoto,u.UserId,u.UserPhone,u.UserName,u.UserDuties,u.VillageId");
            sh.AddWhere("u.VillageId", village);
            sh.AddJoin(" LEFT JOIN sys_UsersInfo as i ON i.UserId = u.UserId");
            var list = sh.Select();
            var villageList = DataConfig.GetVillages();
            var arr = new ArrayList();
            foreach (var x in list)
            {
                var villageModel = villageList.FirstOrDefault(v => x.VillageId == v.VillageId);
                arr.Add(new
                {
                    id = x.UserId,
                    name = x.UserName,
                    img = x.UserPhoto,
                    phone = x.UserPhone,
                    duty = x.UserDuties,
                    village = (villageList.FirstOrDefault(v => v.VillageId == villageModel?.VillageParId)?.VillageName ?? "淮安") + "--" + villageModel?.VillageName
                });
            }
            context.Response.Write(new
            {
                list = arr,
                page,
                sql = sh.SqlString.ToString(),
            }.ToJson());
        }

        class UserInfo
        {
            public int UserId { get; set; }
            public string UserPhoto { get; set; } = string.Empty;
            public string UserPhone { get; set; } = string.Empty;
            public string UserName { get; set; } = string.Empty;
            public int VillageId { get; set; }
            public string UserDuties { get; set; } = string.Empty;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}