using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ThreeArriveAction.Common;
using ThreeArriveAction.Model;

namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// sys_LeaveMessageManager 的摘要说明
    /// </summary>
    public class sys_LeaveMessageManager : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var type = context.Request["type"];
            if (type == "add")
            {
                Add(context);
            }
            else if (type == "getPage")
            {
                GetPage(context);
            }
        }

        private void GetPage(HttpContext context)
        {
            var page = context.Request["page"].ToInt();
            var size = context.Request["size"].ToInt();
            var sh = new SqlHelper<sys_LeaveMessageModel>(new sys_LeaveMessageModel())
            {
                PageConfig = new PageConfig
                {
                    SortEnum = SortEnum.Desc,
                    PageSortField = "LeaveId",
                    PageSize = size,
                    PageIndex = page
                }
            };
            var list = sh.Select();
            var sysLeaveMessageModels = list as sys_LeaveMessageModel[] ?? list.ToArray();

            // 获取回复
            var reSh = new SqlHelper<sys_ReplaysModel>(new sys_ReplaysModel());
            reSh.AddWhere(" AND InteractionId IN (" + string.Join(",", sysLeaveMessageModels.Select(x => x.LeaveId)) + ")");
            var reList = reSh.Select();

            context.Response.Write(new
            {
                list = sysLeaveMessageModels.Select(x => new
                {
                    id = x.LeaveId,
                    img = x.LeaveImages.Split(',').FirstOrDefault() ?? "",
                    content = x.LeaveContent,
                    time = x.LeaveDateTime.ToString("yyyy-M-d"),
                    praise = x.LeavePraiseNumber,
                    comment = reList.Count(r => r.InteractionId == x.LeaveId)
                }),
                page,
                totle = sh.Total,
                sql = sh.SqlString.ToString()
            }.ToJson());
        }

        private void Add(HttpContext context)
        {
            var openId = context.Request["openId"] ?? "";
            var content = context.Request["content"] ?? "";
            var paths = context.Request["paths[]"] ?? "";
            var user = DataConfig.GetUsers(new Dictionary<string, object> {
                                                                              {
                                                                                  "UserRemark",openId
                                                                              }}).FirstOrDefault();
            if (user == null)
            {
                context.Response.Write("openId不存在与数据库中openId:" + openId);
                return;
            }
            var model = new sys_LeaveMessageModel
            {
                LeaveContent = content,
                VillageId = user.VillageId,
                UserId = user.UserId,
                LeaveDateTime = DateTime.Now,
                LeaveImages = paths
            };
            var sh = new SqlHelper<sys_LeaveMessageModel>(model);
            var line = sh.Insert();
            context.Response.Write(line.ToString());
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