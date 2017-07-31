using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using ThreeArriveAction.Common;
using ThreeArriveAction.Model;
using System.Web.SessionState;
using ThreeArriveAction.BLL;
using ThreeArriveAction.Web.UI;

namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// sys_InteractionLearnsManager 的摘要说明
    /// </summary>
    public class sys_InteractionLearnsManager : IHttpHandler,IRequiresSessionState
    {
        private readonly sys_InteractionLearnsBLL iiBLL = new sys_InteractionLearnsBLL();
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
            else if (type == "get")
            {
                GetLearnList(context);
            }else if (type == "save")
            {
                SaveLearn(context);
            }
            else if (type == "edit")
            {
                GetModel(context);
            }
            else if (type == "del")
            {
                DeleteLearns(context);
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
                type = ((StudyEnum)model.LearnType).ToString(),
                remark = model.Remarks
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
                if (i == 0 || i == title.Length - 1)
                {
                    if (i == 0)
                        sh.AddWhere(" AND ( LearnTitle LIKE '%" + title[i] + "%' ");
                    if (i == title.Length - 1)
                        sh.AddWhere(" OR LearnTitle LIKE '%" + title[i] + "%' ) ");
                }
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

        private void GetLearnList(HttpContext context)
        {
            int pageSize = 20;
            int pageIndex = MXRequest.GetQueryIntValue("page");
            string key = MXRequest.GetQueryString("keywords");
            int learntype = MXRequest.GetQueryIntValue("learntype");
            StringBuilder strWhere = new StringBuilder();
            strWhere.Append(" 1=1 ");
            if (key != "")
            {
                string[] keyArr = (key ?? "").Replace("  ", " ").Split(' ');
                for (var i = 0; i < keyArr.Length; i++)
                {
                    if (keyArr[i].IsNullOrEmpty()) continue;
                    if (i == 0 || i == keyArr.Length - 1)
                    {
                        if (i == 0)
                            strWhere.Append(" AND ( LearnTitle LIKE '%" + keyArr[i] + "%' ");
                        if (i == keyArr.Length - 1)
                            strWhere.Append(" OR LearnTitle LIKE '%" + keyArr[i] + "%' ) ");
                    }
                    else
                        strWhere.Append(" OR LearnTitle LIKE '%" + keyArr[i] + "%'  ");
                }
              
            }
            if (learntype != 0)
            {
                strWhere.Append(" and LearnType ="+learntype);
            }
            string fieldOrder = "PublishDate DESC ";
            int recordCount = 0;
            DataSet ds = iiBLL.GetList(pageSize,pageIndex,strWhere.ToString(),fieldOrder,out recordCount);
            StringBuilder strJson = new StringBuilder();
            
            strJson.Append("{\"total\":" + recordCount);
            if (recordCount > 0)
            {
                strJson.Append(",\"rows\":" + JsonHelper.ToJson(ds.Tables[0]));
            }
            string pageContent = Utils.OutPageList(pageSize, pageIndex, recordCount, "Load(__id__)", 8);
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

        #region 保存在线学习
        private void SaveLearn(HttpContext context)
        {
            string action = MXRequest.GetFormString("action");
            sys_InteractionLearnsModel iiModel = new sys_InteractionLearnsModel();
            iiModel.LearnTitle = MXRequest.GetFormString("learntitle");
            iiModel.LearnType = MXRequest.GetFormIntValue("learntype");
            iiModel.LearnContent = MXRequest.GetFormString("learncontent");
            iiModel.Publisher = new ManagePage().GetUsersinfo().UserId;
            iiModel.PublishDate = DateTime.Now;
            iiModel.Remarks = MXRequest.GetFormString("remark");
            string json = "";
            if (action == "add")
            {
                json = iiBLL.AddLearns(iiModel);
            }
            else
            {
                iiModel.LearnId = MXRequest.GetFormIntValue("learnid");
                json = iiBLL.UpdateLearns(iiModel);
            }
            context.Response.Output.Write(json);
        }
        #endregion

        #region 删除在线学习
        private void DeleteLearns(HttpContext context)
        {
            string userIds = MXRequest.GetFormString("str");
            string result = iiBLL.DeleteLearns(userIds);
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