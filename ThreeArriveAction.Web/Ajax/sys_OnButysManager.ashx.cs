﻿using System;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.SessionState;
using ThreeArriveAction.BLL;
using ThreeArriveAction.Model;
using ThreeArriveAction.Common;

namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// sys_OnButysManager 的摘要说明
    /// </summary>
    public class sys_OnButysManager : IHttpHandler, IRequiresSessionState
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
                case "buty":
                    GetButyUserList(context);
                    break;
                case "getOnButysList":
                    GetViweButysList(context);
                    break;
            }
        }

        #region 添加值班信息
        private void AddButy(HttpContext context)
        {
            int userid = MXRequest.GetFormInt("userid");
            int villageid = MXRequest.GetFormInt("villageid");
            sys_OnButysModel butyMode = new sys_OnButysModel
            {
                VillageId = villageid,
                UserId = userid,
                ButyDate = DateTime.Now
            };
            string result = butyBLL.AddOnButys(butyMode);
            context.Response.Write(result);

        }
        #endregion

        #region 查询值班人员
        private void GetButyUserList(HttpContext context)
        {
            int pageSize = 20;
            int pageIndex = MXRequest.GetQueryIntValue("page");
            int town = MXRequest.GetQueryInt("town");
            int vid = MXRequest.GetQueryInt("vid");
            string butydate = MXRequest.GetQueryString("butydate");

            StringBuilder strWhere = new StringBuilder();
            if (town != 0)
            {
                if (vid == 0)
                {
                    strWhere.Append(" VillageParId =" + town);
                }
                else
                {
                    strWhere.Append("VillageId=" + vid);
                }
            }
            else
            {
                strWhere.Append(" 1=1 ");
            }
            strWhere.Append(" and Convert(varchar(100),ButyDate,23)='" + butydate + "'");
            string result = butyBLL.GetListJson(pageSize, pageIndex, strWhere.ToString(), "OnbutyId DESC ");
            context.Response.Write(result);
        }
        #endregion

        public void GetViweButysList(HttpContext context)
        {
            var page = context.Request["page"].ToInt();
            var size = context.Request["size"].ToInt();
            var ddlvillage = context.Request["ddlvillage"].ToInt();
            var villageList = DataConfig.GetVillages();
            var sh = new SqlHelper<v_OnButyUsers>(new v_OnButyUsers(), "OnbutyId", "OnbutyId")
            {
                PageConfig = new PageConfig
                {
                    PageSortField = "OnbutyId",
                    PageSize = size,
                    SortEnum = SortEnum.Desc,
                    PageIndex = page
                }
            };
            if (ddlvillage > 0)
            {
                sh.AddWhere("VillageId", ddlvillage);
            }
            var list = sh.Select();
            context.Response.Write(new
            {
                list = list.Select(x => new
                {
                    img = x.UserPhoto,
                    name = x.UserName,
                    village = (villageList.FirstOrDefault(v => v.VillageId == x.VillageParId)?.VillageName ?? "淮安") + 
                            " --" + 
                            villageList.FirstOrDefault(v => v.VillageId == x.VillageId)?.VillageName,
                    phone = x.UserPhone,
                    time = x.ButyDate.ToString("yyyy-M-d")
                }),
                page,
                totle = sh.Total,
                sql = sh.SqlString.ToString()
            }.ToJson());

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