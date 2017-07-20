using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using ThreeArriveAction.Common;
using ThreeArriveAction.BLL;
using ThreeArriveAction.Model;
using ThreeArriveAction.Web.UI;
using System.Data;
using System.Web.SessionState;

namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// sys_SubscriberFamiliesManager 的摘要说明
    /// </summary>
    public class sys_SubscriberFamiliesManager : IHttpHandler, IRequiresSessionState
    {
        private readonly sys_SubscriberFamilyBLL subBLL = new sys_SubscriberFamilyBLL();
        public void ProcessRequest(HttpContext context)
        {
            string type = MXRequest.GetQueryString("type");
            switch (type)
            {
                case "get":
                    GetSubscriberFamilyList(context); 
                    break;
                case "edit":
                    GetSubscriberFamily(context);
                    break;
                case "save":
                    SaveSubscriberFamily(context);
                    break;
                case "del":
                    DeleteSubscriberFamily(context);
                    break;
            }
        }

        #region 获取七户信息列表
        private void GetSubscriberFamilyList(HttpContext context)
        {
            sys_UsersModel model = new ManagePage().GetUsersinfo();
            if (model == null)
            {
                context.Response.Write("<script>parent.location.href=http://'" + HttpContext.Current.Request.Url.Authority + "/login.html'</script>");
                return;
            }
            else
            {
                StringBuilder strWhere = new StringBuilder();
                int pageSize = 10;
                int page = MXRequest.GetQueryInt("page", 1);
                string key = MXRequest.GetQueryString("keywords");
                string subtype = MXRequest.GetQueryString("subtype");
                key = key.Replace("'", "");
                //区管理员 能够查询所有七户人员
                if (model.OrganizationId == 1)
                {
                    strWhere.Append(" 1=1 ");
                }
                else if (model.OrganizationId == 2)
                {
                    //镇组织委员 只能查询镇下行政村的七户人员
                    strWhere.Append("VillageId in (select ViilageId from sys_Villages where VillageId=" + model.VillageId + " or VillageParId =)" + model.VillageId);
                }
                else if (model.OrganizationId == 3)
                {
                    //村负责人 只能查询本村的七户人员
                    strWhere.Append(" VillageId =" + model.VillageId);
                }
                else
                {
                    //普通村居干部,只能查询责任七户人员
                    strWhere.Append(" UserId="+model.UserId);
                }
                if (subtype != "0")
                {
                    strWhere.Append(" and (SubscriberType ="+subtype+") ");
                }
                if (!string.IsNullOrEmpty(key))
                {
                    strWhere.Append(" and ( SubscriberName like '%" + key + "%' or SubscriberPhone like '%" + key + "%' or SubscriberAddress like '%" + key + "%' ) ");
                }
                int totalCount = 0;
                StringBuilder strJson = new StringBuilder();

                DataSet ds = subBLL.GetList(pageSize, page, strWhere.ToString(), "UserId asc", out totalCount);
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

        #region 获取该七户信息
        private void GetSubscriberFamily(HttpContext context)
        {
            int subid = int.Parse(MXRequest.GetQueryString("subid"));
            string result = subBLL.GetSubscriberFamilyJsonBySubId(subid);
            context.Response.Write(result);
        }
        #endregion

        #region 保存七户信息
        private void SaveSubscriberFamily(HttpContext context)
        {
            sys_SubscriberFamilyModel subModel = new sys_SubscriberFamilyModel();
            subModel.SubscriberName = MXRequest.GetFormString("subname");
            subModel.SubscriberPhone = MXRequest.GetFormString("subphone");
            subModel.SubscriberType = int.Parse(MXRequest.GetFormString("subtype"));
            subModel.FamilyAddress = MXRequest.GetFormString("address");
            subModel.FamilyNumber = int.Parse(MXRequest.GetFormString("famnumber"));
            subModel.Reamarks = MXRequest.GetFormString("remarks");
            subModel.VillageId = int.Parse(MXRequest.GetFormString("villageid"));
            subModel.UserId = int.Parse(MXRequest.GetFormString("userid"));
            string action = MXRequest.GetFormString("action");
            string result = "";
            if (action == "add")
            {
                result = subBLL.AddSubscriberFamily(subModel);
            }
            else
            {
                subModel.SubscriberId = int.Parse(MXRequest.GetFormString("subid"));
                result = subBLL.UpdateSubscriberFamily(subModel);
            }
            context.Response.Write(result);

        }
        #endregion

        #region 删除七户信息
        private void DeleteSubscriberFamily(HttpContext context)
        {
            string str = MXRequest.GetFormString("str");
            string result = subBLL.DeleteSubscriberFamily(str);
            context.Response.Write(result);
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