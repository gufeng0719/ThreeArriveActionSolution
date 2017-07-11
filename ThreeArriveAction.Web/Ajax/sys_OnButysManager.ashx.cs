using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using ThreeArriveAction.BLL;
using ThreeArriveAction.Model;
using ThreeArriveAction.Common;

namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// sys_OnButysManager 的摘要说明
    /// </summary>
    public class sys_OnButysManager : IHttpHandler,IRequiresSessionState
    {
        private readonly sys_OnButysBLL butyBLL = new sys_OnButysBLL();

        public void ProcessRequest(HttpContext context)
        {
            string type = MXRequest.GetQueryString("type");
            switch (type)
            {
                case "set":
                    AddButy(context);
                    break;
            }
        }

        #region 添加值班信息
        private void AddButy(HttpContext context)
        {
            int userid = MXRequest.GetFormInt("userid");
            int villageid = MXRequest.GetFormInt("villageid");
            sys_OnButysModel butyMode = new sys_OnButysModel();
            butyMode.VillageId = villageid;
            butyMode.UserId = userid;
            butyMode.ButyDate = DateTime.Now;
            string result = butyBLL.AddOnButys(butyMode);
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