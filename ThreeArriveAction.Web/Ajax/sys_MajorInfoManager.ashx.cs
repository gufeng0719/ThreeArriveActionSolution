using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ThreeArriveAction.Common;
using ThreeArriveAction.Model;

namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// sys_MajorInfoManager 的摘要说明
    /// </summary>
    public class sys_MajorInfoManager : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var type = context.Request["type"];
            if (type == "getUnreadNumber")
            {
                GetUnreadNumber(context);
            }
            else if (type == "getPageList")
            {
                GetPageList(context);
            }
        }

        private void GetPageList(HttpContext context)
        {
            var userId = context.Request["userId"].ToInt();
            var size = context.Request["size"].ToInt();
            var page = context.Request["page"].ToInt();
            var sh = new SqlHelper<MajorPageList>(new MajorPageList(), "", "", "sys_MajorInfo")
            {
                PageConfig = new PageConfig
                {
                    PageSize = size,
                    PageSortSql = "b.ReadUserId ASC,a.MajorDate DESC",
                    PageIndex = page
                },
                Alia = "a"
            };
            sh.AddShow("a.MajorId,a.MajorTitle,a.MajorFromUserId,a.MajorDate,b.ReadUserId");
            sh.AddJoin(@" LEFT JOIN [dbo].[sys_ReadLog] as b on  a.MajorId = b.MajorId  
                        AND b.ReadUserId = " + userId + " or b.ReadUserId is NULL");
            var majorPageLists = sh.Select();
            var list = majorPageLists as MajorPageList[] ?? majorPageLists.ToArray();

            var userSh = new SqlHelper<sys_UsersModel>(new sys_UsersModel());
            userSh.AddWhere(" AND UserId IN (" + string.Join(",", list.Select(x => x.MajorFromUserId)) + ")");
            var userList = userSh.Select();
            context.Response.Write(new
            {
                page,
                totle = sh.Total,
                list = list.Select(x => new
                {
                    id = x.MajorId,
                    userName = userList.FirstOrDefault(u => u.UserId == x.MajorFromUserId)?.UserName ?? "管理员",
                    type = x.ReadUserId > 0 ? 1 : 0,
                    time = x.MajorDate.ToString("yyyy-M-d"),
                    title = x.MajorTitle
                })
            }.ToJson());
        }

        public void GetUnreadNumber(HttpContext context)
        {
            var userId = context.Request["userId"];
            var majorSh = new SqlHelper<sys_MajorInfoModel>(new sys_MajorInfoModel());
            var majorList = majorSh.Select();
            var readSh = new SqlHelper<sys_ReadLogModel>(new sys_ReadLogModel());
            readSh.AddWhere("ReadUserId", userId);
            var readList = readSh.Select();
            context.Response.Write(majorList.Count() - readList.Count());
        }

        class MajorPageList
        {
            public int MajorId { get; set; }
            public string MajorTitle { get; set; } = string.Empty;
            public int MajorFromUserId { get; set; }
            public DateTime MajorDate { get; set; } = DateTime.Now;
            public int ReadUserId { get; set; }
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