using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ThreeArriveAction.Common;
using ThreeArriveAction.Model;
using ThreeArriveAction.DAL;

namespace ThreeArriveAction.BLL
{
    public partial class sys_UsersBLL : System.Web.UI.Page
    {
        private readonly sys_UsersDAL userDAL = new sys_UsersDAL();
        #region 登录功能
        public string Login(string userPhone, string userPassword)
        {
            JsonMessage json = new JsonMessage();
            if (Session["AdminLoginSun"] == null)
            {
                Session["AdminLoginSun"] = 1;
            }
            else
            {
                Session["AdminLoginSun"] = Convert.ToInt32(Session["AdminLoginSun"]) + 1;
            }
            //判断登录错误次数
            if (Session["AdminLoginSun"] != null && Convert.ToInt32(Session["AdminLoginSun"]) > 5)
            {
                json.success = false;
                json.msg = "错误超过5次，关闭浏览器重新登录！";
            }

            sys_UsersModel userModel = GetUsersModelByUserPhone(userPhone);
            if (userModel != null)
            {
                if (string.Equals(userModel.UserPassword, userPassword))
                {
                    Session[MXKeys.SESSION_ADMIN_INFO] = userModel;
                    Session.Timeout = 45;
                    //写入登录日志
                    siteconfig siteConfig = new sys_configBLL().loadConfig();
                    if (siteConfig.logstatus > 0)
                    {
                        // new BLL.manager_log().Add(model.id, model.user_name, MXEnums.ActionEnum.Login.ToString(), "用户登录");
                    }
                    //写入Cookies
                    Utils.WriteCookie("DTRememberName",Utils.UrlEncode(userModel.UserName), 14400);
                    Utils.WriteCookie("uphone", "ThreeArriveAction", userModel.UserPhone);
                    Utils.WriteCookie("upwd", "ThreeArriveAction", userModel.UserPassword);
                    json.success = true;
                    json.msg = "登录成功";
                }
                else
                {
                    json.success = false;
                    json.msg = "密码不正确";
                }
            }
            else
            {
                json.success = false;
                json.msg = "该用户不存在";
            }
            return JsonHelper.ToJson(json);
        }
        #endregion
        #region 添加
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="userModel">用户实体</param>
        /// <returns></returns>
        public string AddUser(sys_UsersModel userModel)
        {
            userModel.UserState = 1;
            int rows = userDAL.AddUser(userModel);
            if (rows > 0)
            {
                return "{\"info\":\"用户添加成功\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"用户添加失败\",\"status\":\"n\"}";
            }
        }
        #endregion

        #region 修改
        public string UpdateUser(sys_UsersModel userModel)
        {
            bool bl = userDAL.Update(userModel);
            if (bl)
            {
                return "{\"info\":\"用户修改成功\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"用户修改失败\",\"status\":\"n\"}";
            }
        }
        #endregion

        #region 删除
        public string Delete(string userIds)
        {
            int number = userDAL.Delete(userIds);
            if (number > 0)
            {
                return "{\"info\":\"用户删除成功\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"用户删除失败\",\"status\":\"n\"}";
            }
        }
        #endregion

        #region 查询
        /// <summary>
        /// 根据用户编号查询该用户信息
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <returns></returns>
        public sys_UsersModel GetUsersModel(int userId)
        {
            sys_UsersModel model = userDAL.GetModel(userId);
            return model;
        }

        public string GetUsersModelJson(int userId)
        {
            sys_UsersInfoBLL infoBLL = new sys_UsersInfoBLL();
            sys_UsersModel model = GetUsersModel(userId);
            sys_UsersInfoModel infoModel = infoBLL.GetUserInfoModel(userId);
            StringBuilder strJson = new StringBuilder();
            strJson.Append(JsonHelper.ToJson(model));
            strJson.Replace("}",",\"info\":[");
            if (infoModel != null)
            {
                strJson.Append(JsonHelper.ToJson(infoModel));
            }
            strJson.Append("]}");
            return strJson.ToString();
        }

        /// <summary>
        /// 根据用户电话查询改用户实体信息
        /// </summary>
        /// <param name="userPhone"></param>
        /// <returns></returns>
        public sys_UsersModel GetUsersModelByUserPhone(string userPhone)
        {
            sys_UsersModel userModel = userDAL.GetModelByUserPhone(userPhone);
            return userModel;
        }
        /// <summary>
        /// 查询前几条用户信息
        /// </summary>
        /// <param name="top">条数</param>
        /// <param name="strWhere">查询条件</param>
        /// <param name="filedOrder">排序</param>
        /// <returns></returns>
        public DataSet  GetList(int top,string strWhere,string filedOrder)
        {
            DataSet ds = userDAL.GetList(top,strWhere,filedOrder);
            return ds;
        }

        /// <summary>
        /// 分页查询用户信息
        /// </summary>
        /// <param name="pageSize">每页条数</param>
        /// <param name="PageIndex">当前页码</param>
        /// <param name="strWhere">查询条件</param>
        /// <param name="filedOrder">排序条件</param>
        /// <param name="recordCount">总记录数</param>
        /// <returns></returns>
        public DataSet GetList(int pageSize, int PageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            DataSet ds = userDAL.GetList(pageSize, PageIndex, strWhere, filedOrder, out recordCount);
            return ds;
        }

        /// <summary>
        /// 根据村居编号，查询村居改天是否有值班信息与人员列表
        /// </summary>
        /// <param name="strWhere">查询条件</param>
        /// <param name="villageId">村居编号</param>
        /// <returns></returns>
        public string GetButyUsers(string strWhere,int villageId)
        {
            //获取该村所有的用户
            DataSet ds = GetList(0, strWhere, "UserId asc");
            //获取今天是否已经设置值班信息
            sys_OnButysBLL butyBLL = new sys_OnButysBLL();
            bool bl = butyBLL.ExisByVillageId(villageId,DateTime.Now);
            StringBuilder strJson = new StringBuilder();
            strJson.Append("{");
            if (bl)
            {
                strJson.Append("\"buty\":true");
            }
            else
            {
                strJson.Append("\"buty\":false");
            }
            if (ds.Tables[0].Rows.Count > 0)
            {
                strJson.Append(",\"rows\":" + JsonHelper.ToJson(ds.Tables[0]));
            }
            else
            {
                strJson.Append(",\"rows\":[]");
            }
            strJson.Append("}");
            return strJson.ToString();
        }

        #endregion
    }
}
