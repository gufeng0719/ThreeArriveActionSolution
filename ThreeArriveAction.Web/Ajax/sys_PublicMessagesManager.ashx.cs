﻿using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using ThreeArriveAction.BLL;
using ThreeArriveAction.Common;
using ThreeArriveAction.Model;
using ThreeArriveAction.Web.UI;

namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// sys_PublicMessagesManager 的摘要说明
    /// </summary>
    public class sys_PublicMessagesManager : IHttpHandler, IRequiresSessionState
    {
        private sys_PublicMessagesBLL pBLL = new sys_PublicMessagesBLL();
        public void ProcessRequest(HttpContext context)
        {
            var type = context.Request["type"];
            switch (type)
            {
                case "openList":
                    OpenList(context);
                    break;
                case "add":
                    Add(context);
                    break;
                case "get":
                    GetOpenList(context);
                    break;
                case "detail":
                    GetPublishDetail(context);
                    break;
                case "getModel":
                    GetModel(context);
                    break;
                case "insert":
                    Insert(context);
                    break;
                case "del":
                    DeletePublicMessage(context);
                    break;
            }
        }

        public void GetModel(HttpContext context)
        {
            var id = context.Request["id"].ToInt();
            var sh = new SqlHelper<sys_PublicMessagesModel>(new sys_PublicMessagesModel());
            sh.AddWhere("PublicId", id);
            var model = sh.Select().FirstOrDefault();
            if (model == null)
            {
                context.Response.Write("未能获取到用户对象");
                return;
            }
            var villageList = DataConfig.GetVillages();
            var village = villageList.FirstOrDefault(x => x.VillageId == model.VillageId);
            context.Response.Write(new
            {
                type = ((OpenTypeEnum)model.PublicType).ToString(),
                gimg = model.ThumbnailUrl,
                imgs = model.ImageUrl.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries),
                village = (villageList.FirstOrDefault(x => x.VillageId == village?.VillageParId)?.VillageName ?? "淮安") + "---" + village?.VillageName,
                msg = model.Remarks
            }.ToJson());
        }


        #region 添加公开信息
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
            var line = pBLL.Add(model);
            context.Response.Write(line);
        }

        public void Insert(HttpContext context)
        {
            int ptype = MXRequest.GetFormIntValue("ptype");
            string th = MXRequest.GetFormString("thumbnail");
            string imgurl = context.Request.Form["imgurl"];

            string remark = MXRequest.GetFormString("remark");
            sys_UsersModel userModel = new ManagePage().GetUsersinfo();
            sys_PublicMessagesModel publicModel = new sys_PublicMessagesModel();
            publicModel.VillageId = userModel.VillageId;
            publicModel.PublishDate = DateTime.Now;
            publicModel.PublicType = ptype;
            publicModel.ThumbnailUrl = th;
            publicModel.ImageUrl = imgurl;
            publicModel.UserId = userModel.UserId;
            publicModel.Remarks = remark;
            int number = pBLL.Add(publicModel);
            JsonMessage json = new JsonMessage();
            if (number > 0)
            {
                json.success = true;
                json.msg = "公务发布成功";
            }
            else
            {
                json.success = false;
                json.msg = "公务发布失败";
            }

            context.Response.Write(JsonHelper.ToJson(json));
        }
        #endregion

        #region 获取公开信息列表供公众号使用
        public void OpenList(HttpContext context)
        {
            var openType = context.Request["openType"].ToInt();
            var page = context.Request["page"].ToInt();
            var size = context.Request["size"].ToInt();
            var village = context.Request["village"].ToInt();

            var sh = new SqlHelper<sys_PublicMessagesModel>(new sys_PublicMessagesModel())
            {
                PageConfig = new PageConfig
                {
                    PageIndex = page == 0 ? 1 : page,
                    PageSize = size,
                    PageSortField = "PublishDate",
                    SortEnum = SortEnum.Desc
                }
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
                    id = item.PublicId,
                    img = item.ThumbnailUrl,
                    date = item.PublishDate.ToString("yyyy-MM-dd"),
                    village = villageModel?.VillageName ?? "",
                    town = villageList.FirstOrDefault(v => v.VillageId == villageModel?.VillageParId)?.VillageName
                });
            }

            context.Response.Write(new
            {
                page,
                totle = sh.Total,
                list
            }.ToJson());
        }
        #endregion

        #region 获取公开信息列表供管理系统使用
        public void GetOpenList(HttpContext context)
        {
            //获取当前登录的操作者信息
            sys_UsersModel model = new ManagePage().GetUsersinfo();
            int pageSize = 20;
            int pageIndex = MXRequest.GetQueryIntValue("page");
            int town = MXRequest.GetQueryInt("town");
            int vid = MXRequest.GetQueryInt("vid");
            string opendate = MXRequest.GetQueryString("opendate");
            int opentype = MXRequest.GetQueryIntValue("opentype");
            StringBuilder strWhere = new StringBuilder();
            if (opentype != 0)
            {
                strWhere.Append(" PublicType = "+opentype);
            }else
            {
                strWhere.Append(" 1=1 ");
            }
            if (town != 0)
            {
                if (vid == 0)
                {
                    strWhere.Append(" and VillageParId =" + town);
                }
                else
                {
                    strWhere.Append(" and VillageId=" + vid);
                }
            }
            else
            {
                //默认查询(如果管理员，查询所有根据时间倒序查询)
                if (model.OrganizationId == 2)
                {
                    strWhere.Append(" and VillageParId=" + model.VillageId);
                }
                else if (model.OrganizationId == 3 || model.OrganizationId == 4)
                {
                    //查看本村或者本镇的公开信息
                    strWhere.Append(" and VillageId=" + model.VillageId);
                }

            }
            if (opendate != "")
            {
                strWhere.Append(" and Convert(varchar(7),PublishDate,23)='" + opendate + "'");
            }
            string result = pBLL.GetListJson(pageSize, pageIndex, strWhere.ToString(), "PublicId DESC ");
            context.Response.Write(result);

        }
        #endregion

        #region 根据公开信息编号获取详细信息
        public void GetPublishDetail(HttpContext context)
        {
            int pid = MXRequest.GetQueryIntValue("pid");
            string json = pBLL.GetModelJson(pid);
            context.Response.Output.Write(json);

        }
        #endregion

        #region 删除三务公开
        private void DeletePublicMessage(HttpContext context)
        {
            string str = MXRequest.GetFormString("str");
            string result = "";
            if (str.Trim() != "")
            {
                result = pBLL.DeletePublicMessage(str);
            }
            else
            {
                result = "{\"info\":\"请至少选择一条记录\",\"status\":\"n\"}";
            }
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
