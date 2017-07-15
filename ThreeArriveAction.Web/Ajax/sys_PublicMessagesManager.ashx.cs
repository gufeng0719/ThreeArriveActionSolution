using System;
using System.Collections;
using System.Linq;
using System.Web;
using ThreeArriveAction.BLL;
using ThreeArriveAction.Common;
using ThreeArriveAction.Model;

namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// sys_PublicMessagesManager 的摘要说明
    /// </summary>
    public class sys_PublicMessagesManager : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var type = context.Request["type"];
            if (type == "openList")
            {
                OpenList(context);
            }
            else if (type == "add")
            {
                Add(context);
            }
        }

        public void Add(HttpContext context)
        {
            var sh = new SqlHelper<sys_UsersModel>(new sys_UsersModel());
            sh.AddWhere("UserRemark", context.Request["openId"]);
            var user = sh.Select().FirstOrDefault();
            if (user == null)
            {
                LogHelper.Log("openId:" + context.Request["openId"], "openId未能查询到用户信息");
                context.Response.Write("错误的用户信息");
                return;
            }
            var paths = context.Request["paths[]"].Split(',');
            var thumbnailUrl = paths[paths.Length - 1];
            var imageUrl = "";
            foreach (var path in paths)
            {
                if (path != thumbnailUrl)
                {
                    imageUrl += path + ",";
                }
            }
            var model = new sys_PublicMessagesModel
            {
                ImageUrl = imageUrl,
                PublicType = context.Request["openType"].ToInt(),
                PublishDate = DateTime.Now,
                Remarks = context.Request["msg"],
                ThumbnailUrl = thumbnailUrl,
                UserId = user.UserId,
                VillageId = user.VillageId
            };
            LogHelper.Log(model.ToJson());
            var line = new sys_PublicMessagesBLL().Add(model);
            context.Response.Write(line);
        }

        public void OpenList(HttpContext context)
        {
            var openType = context.Request["openType"].ToInt();
            var page = context.Request["page"].ToInt();
            var size = context.Request["size"].ToInt();
            var village = context.Request["village"].ToInt();

            var sh = new SqlHelper<sys_PublicMessagesModel>(new sys_PublicMessagesModel());

            sh.PageConfig = new PageConfig
            {
                PageIndex = page == 0 ? 1 : page,
                PageSize = size,
                PageSortField = "PublishDate",
                SortEnum = SortEnum.Desc
            };
            sh.AddWhere("PublicType", openType);
            if (village > 0)
            {
                sh.AddWhere("VillageId", village);
            }
            var villageList = DataConfig.GetVillages();
            var list = new ArrayList();
            foreach (var item in sh.Select().OrderByDescending(x => x.PublishDate))
            {
                var villageModel = villageList.FirstOrDefault(v => v.VillageId == item.VillageId);
                list.Add(new
                {
                    img = item.ThumbnailUrl,
                    date = item.PublishDate.ToString("yyyy-MM-dd"),
                    village = villageModel.VillageName,
                    town = villageList.FirstOrDefault(v => v.VillageId == villageModel.VillageParId).VillageName
                });
            }

            context.Response.Write(new
            {
                page,
                totle = sh.Total,
                list
            }.ToJson());
            return;
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