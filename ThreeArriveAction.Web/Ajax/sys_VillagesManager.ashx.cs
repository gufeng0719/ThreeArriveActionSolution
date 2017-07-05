using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using ThreeArriveAction.BLL;
using ThreeArriveAction.Common;
using ThreeArriveAction.Web.UI;
using ThreeArriveAction.Model;

namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// sys_VillagesManager 的摘要说明
    /// </summary>
    public class sys_VillagesManager : IHttpHandler, IRequiresSessionState
    {
        private readonly sys_VillagesBLL villageBLL = new sys_VillagesBLL();
        public void ProcessRequest(HttpContext context)
        {
            string type = MXRequest.GetQueryString("type");
            switch(type){
                case "select":
                    GetVillageTreeJson(context);
                    break;

            }
        }

        private void GetVillageTreeJson(HttpContext context)
        {
            string result = villageBLL.GetListJson(0);
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