using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ThreeArriveAction.BLL;
using ThreeArriveAction.Common;
using ThreeArriveAction.Web.UI;
using ThreeArriveAction.Model;
using System.Web.SessionState;

namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// sys_OrganizationsManager 的摘要说明
    /// </summary>
    public class sys_OrganizationsManager : IHttpHandler, IRequiresSessionState
    {
        private sys_OrganizationsBLL organBLL = new sys_OrganizationsBLL();
        public void ProcessRequest(HttpContext context)
        {
            string type = MXRequest.GetQueryString("type");
            switch (type)
            {
                case "get":
                    GetOrganizationList(context);
                    break;
                case "edit":
                    GetOrganizationByOrgId(context);
                    break;
                case "save":
                    SaveOrganization(context);
                    break;
                case "select":
                    GetOrganizationSelect(context);
                    break;
            }
        }

        private void GetOrganizationList(HttpContext context)
        {
            string strWhere = "";
            string keywords = MXRequest.GetQueryString("keywords");
            if (keywords != null && keywords != "")
            {
                strWhere += " OrganizationName like '%" + keywords + "%'";
            }
            string result = organBLL.GetListJson(strWhere);
            context.Response.Write(result);
        }

        private void GetOrganizationSelect(HttpContext context)
        {
            string result = organBLL.GetListJson(" OrganizationState=1 ");
            context.Response.Write(result);
        }

        private void GetOrganizationByOrgId(HttpContext context)
        {
            int orgid = int.Parse(MXRequest.GetQueryString("orgid"));
            string result = organBLL.GetOrganization(orgid);
            context.Response.Write(result);
        }

        private void SaveOrganization(HttpContext context)
        {
            sys_OrganizationsModel organModel = new sys_OrganizationsModel();
            string action = MXRequest.GetFormString("action");
            organModel.OrganizationName = MXRequest.GetFormString("organname").ToString();
            if (MXRequest.GetFormString("organstate") == "on")
            {
                organModel.OrganizationState = 1;
            }
            else
            {
                organModel.OrganizationState = 2;
            }
            organModel.Remarks = MXRequest.GetFormString("remarks");
            string result = "";
            if (action.IndexOf("add")>0)
            {
                result = organBLL.AddOrganization(organModel);
            }
            else
            {
                organModel.OrganizationId = int.Parse(MXRequest.GetFormString("orgid"));
                result = organBLL.UpdateOrganization(organModel);
            }
            context.Response.Write(result);
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