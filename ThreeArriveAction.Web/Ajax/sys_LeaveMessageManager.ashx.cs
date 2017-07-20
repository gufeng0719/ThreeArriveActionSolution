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
            else if (type == "getModel")
            {
                GetModel(context);
            }
            else if (type == "pushPraise")
            {
                PushPraise(context);
            }
        }

        private void PushPraise(HttpContext context)
        {
            var id = context.Request["id"].ToInt();
            var openId = context.Request["openId"];
            var user = DataConfig.GetUsers(new Dictionary<string, object> { { "UserRemark", openId } }).FirstOrDefault();
            if (user == null)
            {
                context.Response.Write("openId错误 ； openId： " + openId);
                return;
            }
            var sh = new SqlHelper<sys_LeaveMessageModel>(new sys_LeaveMessageModel());
            sh.AddWhere("LeaveId", id);
            var model = sh.Select().FirstOrDefault();
            if (model == null)
            {
                context.Response.Write("Id错误 ； Id： " + id);
                return;
            }
            var modelSh = new SqlHelper<sys_LeaveMessageModel>(new sys_LeaveMessageModel());
            modelSh.AddUpdate("LeavePraiseNumber", model.LeavePraiseNumber + 1);
            modelSh.AddUpdate("LeavePraiseUserIds", model.LeavePraiseUserIds + "," + user.UserId);
            modelSh.AddWhere("LeaveId", id);
            modelSh.Update();

            context.Response.Write(model.LeavePraiseNumber + 1);

        }

        private void GetModel(HttpContext context)
        {
            var id = context.Request["id"].ToInt();
            var openId = context.Request["openId"];
            var sh = new SqlHelper<sys_LeaveMessageModel>(new sys_LeaveMessageModel());
            sh.AddWhere("LeaveId", id);
            var model = sh.Select().FirstOrDefault();
            var user = DataConfig.GetUsers(new Dictionary<string, object> { { "UserRemark", openId } }).FirstOrDefault();
            if (model == null || user == null)
            {
                context.Response.Write("未能查询信息Id:" + id + "；openId:" + openId);
                return;
            }
            context.Response.Write(new
            {
                userName = DataConfig.GetUsers(new Dictionary<string, object> { { "UserId", model.UserId } }).FirstOrDefault()?.UserName ?? "管理员",
                content = model.LeaveContent,
                praise = model.LeavePraiseNumber,
                imgs = (model.LeaveImages ?? "").Split(','),
                time = model.LeaveDateTime.ToString("yyyy-M-d h:m"),
                yetPraise = (model.LeavePraiseUserIds ?? "").Split(',').Any(x => x == user.UserId.ToString()),
                userId = model.UserId
            }.ToJson());
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

            // 获取人员
            var userSh = new SqlHelper<sys_UsersModel>(new sys_UsersModel());
            reSh.AddWhere(" AND UserId IN (" + string.Join(",", sysLeaveMessageModels.Select(x => x.UserId)) + ")");
            var userList = userSh.Select();

            context.Response.Write(new
            {
                list = sysLeaveMessageModels.Select(x => new
                {
                    id = x.LeaveId,
                    img = x.LeaveImages.Split(',').FirstOrDefault() ?? "",
                    content = x.LeaveContent,
                    time = x.LeaveDateTime.ToString("yyyy-M-d"),
                    praise = x.LeavePraiseNumber,
                    comment = reList.Count(r => r.InteractionId == x.LeaveId),
                    userName = userList.FirstOrDefault(u => u.UserId == x.UserId)?.UserName ?? "管理员"
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
            LogHelper.Log(model.ToJson());
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