using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using ThreeArriveAction.Common;
using ThreeArriveAction.Model;
using ThreeArriveAction.BLL;
using System.Web.SessionState;
using ThreeArriveAction.Web.UI;

namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// sys_LeaveMessageManager 的摘要说明
    /// </summary>
    public class sys_LeaveMessageManager : IHttpHandler, IRequiresSessionState
    {
        private readonly sys_LeaveMessageBLL lmBLL = new sys_LeaveMessageBLL();
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
            else if (type == "get")
            {
                GetLeaveList(context);
            }
            else if (type == "edit")
            {
                GetLeaveModel(context);
            }
            else if (type == "save")
            {
                PublishLeaveMessage(context);
            }
            else if (type == "praise")
            {
                SetPraiseNumber(context);
            }
            else if (type == "del")
            {
                DeleteLeave(context);
            }
            else if (type == "insert")
            {
                InsertLeave(context);
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
                imgs = (model.LeaveImages ?? "").Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries),
                time = model.LeaveDateTime.ToString("yyyy-MM-dd"),
                yetPraise = (model.LeavePraiseUserIds ?? "").Split(',').Any(x => x == user.UserId.ToString()),
                userId = model.UserId
            }.ToJson());
        }

        private void GetLeaveModel(HttpContext context)
        {
            int id = MXRequest.GetQueryIntValue("id");
            var sh = new SqlHelper<sys_LeaveMessageModel>(new sys_LeaveMessageModel());
            sh.AddWhere("LeaveId", id);
            var model = sh.Select().FirstOrDefault();
            sys_UsersModel user = new ManagePage().GetUsersinfo();
            if (model == null)
            {
                context.Response.Write("{\"info\":\"获取失败\",\"status\":\"n\"}");
                return;
            }
            context.Response.Write(new
            {
                userName = DataConfig.GetUsers(new Dictionary<string, object> { { "UserId", model.UserId } }).FirstOrDefault()?.UserName ?? "管理员",
                content = model.LeaveContent,
                praise = model.LeavePraiseNumber,
                imgs = (model.LeaveImages ?? "").Split(','),
                time = model.LeaveDateTime.ToString("yyyy-MM-dd"),
                userId = model.UserId,
                yetPraise = (model.LeavePraiseUserIds ?? "").Split(',').Any(x => x == user.UserId.ToString()),
                state = model.LeaveState,
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
            sh.AddWhere("LeaveState", 1);
            var sysLeaveMessageModels = sh.Select();
            var list = sysLeaveMessageModels as sys_LeaveMessageModel[] ?? sysLeaveMessageModels.ToArray();

            // 获取回复
            var reSh = new SqlHelper<sys_ReplaysModel>(new sys_ReplaysModel());
            var sqlIn = string.Join(",", list.Select(x => x.LeaveId));
            sqlIn = sqlIn.IsNullOrEmpty() ? "0" : sqlIn;
            reSh.AddWhere(" AND InteractionId IN (" + sqlIn + ")");
            var reList = reSh.Select();

            // 获取人员
            var userSh = new SqlHelper<sys_UsersModel>(new sys_UsersModel());
            reSh.AddWhere(" AND UserId IN (" + string.Join(",", list.Select(x => x.UserId)) + ")");
            var userList = userSh.Select();

            context.Response.Write(new
            {
                list = list.Select(x => new
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

        private void GetLeaveList(HttpContext context)
        {
            string key = MXRequest.GetQueryString("keywords");
            int leavestate = MXRequest.GetQueryInt("leavestate");
            int pageSize = 20;
            int pageIndex = MXRequest.GetQueryInt("page");
            StringBuilder strWhere = new StringBuilder();
            strWhere.Append(" 1=1 ");
            if (key != "")
            {
                strWhere.Append(" and LeaveContent like '%" + key + "%'");
            }
            if (leavestate != 0)
            {
                strWhere.Append(" and LeaveState =" + leavestate);
            }

            string fieldOrder = "LeaveDateTime DESC,LeaveState DESC ";
            StringBuilder strJson = new StringBuilder();
            int recordCound = 0;
            DataSet ds = lmBLL.GetList(pageSize, pageIndex, strWhere.ToString(), fieldOrder, out recordCound);
            strJson.Append("{\"total\":" + recordCound);
            if (ds.Tables[0].Rows.Count > 0)
            {
                strJson.Append(",\"rows\":" + JsonHelper.ToJson(ds.Tables[0]));
            }
            else
            {
                strJson.Append(",\"rows\":[]");
            }
            string pageContent = Utils.OutPageList(pageSize, pageIndex, recordCound, "Load(__id__)", 8);
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

        private void PublishLeaveMessage(HttpContext context)
        {
            int id = MXRequest.GetFormIntValue("id");
            int score = MXRequest.GetFormIntValue("score");
            string result = lmBLL.PublishLeaveMessage(id, score);
            context.Response.Output.Write(result);
        }

        #region 添加留言
        private void InsertLeave(HttpContext context)
        {
            string content = MXRequest.GetFormString("content");
            string imgurl = MXRequest.GetFormString("imgurl");
            sys_LeaveMessageModel lmModel = new sys_LeaveMessageModel();
            sys_UsersModel user = new ManagePage().GetUsersinfo();
            lmModel.LeaveContent = content;
            lmModel.LeaveImages = imgurl;
            lmModel.UserId = user.UserId;
            lmModel.VillageId = user.VillageId;
            lmModel.LeavePraiseNumber = 0;
            lmModel.LeavePraiseUserIds = "";
            lmModel.LeaveState = 0;
            string result = lmBLL.AddLeave(lmModel);
            context.Response.Output.Write(result);
        }
        #endregion

        #region 点赞
        private void SetPraiseNumber(HttpContext context)
        {
            int lid = MXRequest.GetFormInt("lid");
            int userid = new ManagePage().GetUsersinfo().UserId;
            string result = lmBLL.SetPraiseNumber(lid, userid);
            context.Response.Output.Write(result);

        }
        #endregion

        #region 删除留言
        private void DeleteLeave(HttpContext context)
        {
            string str = MXRequest.GetFormString("str");
            string result = lmBLL.DeleteLeaves(str);
            context.Response.Output.Write(result);
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