using System;
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

        #region 分页查询
        public void GetUsersList(HttpContext context)
        {
            //获取当前登录的操作者信息
            sys_UsersModel model = new ManagePage().GetUsersinfo();
            if (model == null)
            {
                context.Response.Write("<script>parent.location.href='login.html'</script>");
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
            userModel.UserRemark = MXRequest.GetFormString("userremark").ToString();
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
            if (action.IndexOf("add") > 0)
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

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}