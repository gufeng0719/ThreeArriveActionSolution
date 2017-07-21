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
    /// sys_InteractionLearnsManager 的摘要说明
    /// </summary>
    public class sys_InteractionLearnsManager : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var type = context.Request["type"];
            if (type == "getPageList")
            {
                GetPageList(context);
            }
            else if (type == "add")
            {
                Add(context);
            }
            else if (type == "getModel")
            {
                GetModel(context);
            }
            else if (type == "delete")
            {
                Delete(context);
            }
            else
            {
                context.Response.Write("错误的请求");
            }
        }

        private void Delete(HttpContext context)
        {
            var sh = new SqlHelper<sys_InteractionLearnsModel>(new sys_InteractionLearnsModel());
            var line = sh.Delete(" AND LearnId = " + context.Request["id"].ToInt());
            context.Response.Write(line);
        }

        private void GetModel(HttpContext context)
        {
            var id = context.Request["id"];
            var sh = new SqlHelper<sys_InteractionLearnsModel>(new sys_InteractionLearnsModel());
            sh.AddWhere("LearnId", id);
            var model = sh.Select().FirstOrDefault();
            if (model == null)
            {
                context.Response.Write("错误的Id:" + id);
                return;
            }
            var user = DataConfig.GetUsers(new Dictionary<string, object> { { "UserId", model.Publisher } }).FirstOrDefault();
            context.Response.Write(new
            {
                title = model.LearnTitle,
                content = model.LearnContent,
                time = model.PublishDate.ToString("yyyy-M-d dddd"),
                userName = user?.UserName ?? "管理员",
                type = ((StudyEnum)model.LearnType).ToString()
            }.ToJson());
        }

        private void Add(HttpContext context)
        {
            //var user = new ManagePage().GetUsersinfo();
            var sh = new SqlHelper<sys_InteractionLearnsModel>(new sys_InteractionLearnsModel
            {
                LearnContent = context.Request["content"],
                LearnTitle = context.Request["title"],
                LearnType = context.Request["studyType"].ToInt(),
                PublishDate = DateTime.Now,
                Publisher = 1
            });
            var line = sh.Insert();
            context.Response.Write(line);
        }

        private void GetPageList(HttpContext context)
        {
            var size = context.Request["size"].ToInt();
            var page = context.Request["page"].ToInt();
            var type = context.Request["pageType"].ToInt();
            var title = (context.Request["title"] ?? "").Replace("  ", " ").Split(' ');
            var sh = new SqlHelper<sys_InteractionLearnsModel>(new sys_InteractionLearnsModel())
            {
                PageConfig = new PageConfig
                {
                    PageSize = size,
                    SortEnum = SortEnum.Desc,
                    PageSortField = "LearnId",
                    PageIndex = page
                }
            };
            if (type > 0)
            {
                sh.AddWhere("LearnType", type);
            }
            for (var i = 0; i < title.Length; i++)
            {
                if (title[i].IsNullOrEmpty()) continue;
                if (i == 0)
                    sh.AddWhere(" AND ( LearnTitle LIKE '%" + title[i] + "%' ");
                else if (i == title.Length - 1)
                    sh.AddWhere(" OR LearnTitle LIKE '%" + title[i] + "%' ) ");
                else
                    sh.AddWhere(" OR LearnTitle LIKE '%" + title[i] + "%'  ");
            }
            var list = sh.Select();
            var sysInteractionLearnsModels = list as sys_InteractionLearnsModel[] ?? list.ToArray();
            if (!sysInteractionLearnsModels.Any())
            {
                context.Response.Write(new
                {
                    list = new ArrayList(),
                    page,
                    totle = 0,
                    sql = ""
                }.ToJson());
                return;
            }
            var userSh = new SqlHelper<sys_UsersModel>(new sys_UsersModel());
            userSh.AddWhere(" AND UserId in (" + string.Join(",", sysInteractionLearnsModels.Select(x => x.Publisher)) + ")");
            var userlist = userSh.Select();
            context.Response.Write(new
            {
                list = sysInteractionLearnsModels.Select(x => new
                {
                    id = x.LearnId,
                    title = x.LearnTitle,
                    time = x.PublishDate.ToString("yyyy-M-d"),
                    userName = userlist.FirstOrDefault(u => x.Publisher == u.UserId)?.UserName ?? "管理员",
                    type = ((StudyEnum)x.LearnType).ToString()
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