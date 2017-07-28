using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using ThreeArriveAction.BLL;
using ThreeArriveAction.Common;
using ThreeArriveAction.Model;
using ThreeArriveAction.Web.UI;

namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// sys_ReplaysManager 的摘要说明
    /// </summary>
    public class sys_ReplaysManager : IHttpHandler,IRequiresSessionState
    {
        private readonly sys_ReplaysBLL rBLL = new sys_ReplaysBLL();
        public void ProcessRequest(HttpContext context)
        {
            var type = context.Request["type"];
            if (type == "getList")
            {
                GetList(context);
            }
            else if (type == "comment")
            {
                Comment(context);
            }else if (type == "get")
            {
                GetReplayList(context);
            }else if (type == "replay")
            {
                AddReplay(context);
            }
        }

        private void Comment(HttpContext context)
        {
            var openId = context.Request["openId"];
            var user = DataConfig.GetUsers(new Dictionary<string, object> { { "UserRemark", openId } }).FirstOrDefault();
            if (user == null)
            {
                context.Response.Write("openId错误openId:" + openId);
                return;
            }
            var toPlayer = context.Request["toPlayer"].ToInt();
            var id = context.Request["id"].ToInt();
            var content = context.Request["content"];
            var model = new sys_ReplaysModel
            {
                InteractionId = id,
                ReToplayerId = toPlayer,
                RepalyDate = DateTime.Now,
                ReplayContent = content,
                ReplayerId = user.UserId,
            };
            var sh = new SqlHelper<sys_ReplaysModel>(model, "ReplayId", "ReplayId");
            var line = sh.Insert();
            var toUser = DataConfig.GetUsers(new Dictionary<string, object> { { "UserId", toPlayer } }).FirstOrDefault();
            if (line > 0)
            {
                context.Response.Write(new
                {
                    playerId = user.UserId,
                    player = "我",
                    toPlayer = toUser == null ? "管理员" : toUser.UserId == user.UserId ? "我" : toUser.UserName,
                    content
                }.ToJson());
                return;
            }
            context.Response.Write("添加数据异常");
        }

        private void GetList(HttpContext context)
        {
            var id = context.Request["id"].ToInt();
            var openId = context.Request["openId"];
            var user = DataConfig.GetUsers(new Dictionary<string, object>() { { "UserRemark", openId } }).FirstOrDefault();
            if (user == null)
            {
                context.Response.Write("当前openId，未能查询到用户信息openId：" + openId);
                return;
            }
           
            var sh = new SqlHelper<sys_ReplaysModel>(new sys_ReplaysModel());
            sh.AddWhere("InteractionId", id);
            var list = sh.Select();
            var listModel = list as sys_ReplaysModel[] ?? list.ToArray();
            if (!listModel.Any())
            {
                context.Response.Write(new
                {
                    list = new ArrayList()
                }.ToJson());
                return;
            }
            // 取出回复人Id 被回复人Id
            var userIds = listModel.Select(x => x.ReToplayerId).ToList();
            userIds.AddRange(listModel.Select(x => x.ReplayId).ToList());
            userIds = userIds.Where((x, i) => userIds.FindIndex(z => z == x) == i).ToList();
            // 查询所有涉及到的人员信息
            if (userIds.Count > 0)
            {
                var userSh = new SqlHelper<sys_UsersModel>(new sys_UsersModel());
                userSh.AddWhere(" AND UserId IN (" + string.Join(",", userIds) + ")");
                var userList = userSh.Select();
                var listUserModel = userList as sys_UsersModel[] ?? userList.ToArray();

                context.Response.Write(new
                {
                    list = listModel.Select(x => new
                    {
                        playerId = x.ReplayerId,
                        player = x.ReplayerId == user.UserId ? "我" : listUserModel.FirstOrDefault(u => u.UserId == x.ReplayerId)?.UserName ?? "管理员",
                        toPlayer = x.ReToplayerId == user.UserId ? "我" : listUserModel.FirstOrDefault(u => u.UserId == x.ReToplayerId)?.UserName ?? "管理员",
                        content = x.ReplayContent
                    })
                }.ToJson());
            }
        }

        private void GetReplayList(HttpContext context)
        {
            var id = context.Request["id"].ToInt();
            sys_UsersModel user = new ManagePage().GetUsersinfo();

            var sh = new SqlHelper<sys_ReplaysModel>(new sys_ReplaysModel());
            sh.AddWhere("InteractionId", id);
            var list = sh.Select();
            var listModel = list as sys_ReplaysModel[] ?? list.ToArray();
            if (!listModel.Any())
            {
                context.Response.Write(new
                {
                    list = new ArrayList()
                }.ToJson());
                return;
            }
            // 取出回复人Id 被回复人Id
            var userIds = listModel.Select(x => x.ReToplayerId).ToList();
            userIds.AddRange(listModel.Select(x => x.ReplayId).ToList());
            userIds = userIds.Where((x, i) => userIds.FindIndex(z => z == x) == i).ToList();
            // 查询所有涉及到的人员信息
            if (userIds.Count > 0)
            {
                var userSh = new SqlHelper<sys_UsersModel>(new sys_UsersModel());
                userSh.AddWhere(" AND UserId IN (" + string.Join(",", userIds) + ")");
                var userList = userSh.Select();
                var listUserModel = userList as sys_UsersModel[] ?? userList.ToArray();

                context.Response.Write(new
                {
                    list = listModel.Select(x => new
                    {
                        playerId = x.ReplayerId,
                        toPlayerId = x.reToplayerId,
                        player = x.ReplayerId == user.UserId ? "我" : listUserModel.FirstOrDefault(u => u.UserId == x.ReplayerId)?.UserName ?? "管理员",
                        toPlayer = x.ReToplayerId == user.UserId ? "我" : listUserModel.FirstOrDefault(u => u.UserId == x.ReToplayerId)?.UserName ?? "管理员",
                        content = x.ReplayContent
                    })
                }.ToJson());
            }
        }

        private void AddReplay(HttpContext context)
        {
            int lid = MXRequest.GetFormIntValue("lid");
            int rid = MXRequest.GetFormIntValue("rid");
            string replay = MXRequest.GetFormString("replay");
            sys_UsersModel user = new ManagePage().GetUsersinfo();
            sys_ReplaysModel rModel = new sys_ReplaysModel();
            rModel.InteractionId = lid;
            rModel.ReplayContent = replay;
            rModel.ReplayId = user.UserId;
            rModel.RepalyDate = DateTime.Now;
            rModel.reToplayerId = rid;
            string json = rBLL.AddReplay(rModel);
            context.Response.Output.Write(json);
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