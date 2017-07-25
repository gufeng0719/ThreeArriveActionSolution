using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using ThreeArriveAction.BLL;
using ThreeArriveAction.Common;
using ThreeArriveAction.Model;
using System.Web.SessionState;
using ThreeArriveAction.Web.UI;
namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// sys_ThingRecrodManager 的摘要说明
    /// </summary>
    public class sys_ThingRecordManager : IHttpHandler, IRequiresSessionState
    {
        private readonly sys_ThingRecordsBLL thingBLL = new sys_ThingRecordsBLL();
        public void ProcessRequest(HttpContext context)
        {
            string type = MXRequest.GetQueryString("type");
            switch (type)
            {
                case "add":
                    AddThingRecord(context);
                    break;
                case "getMsgInfo":
                    GetMsgInfo(context);
                    break;
            }
        }

        private void GetMsgInfo(HttpContext context)
        {
            var openId = context.Request["openId"];
            var user = DataConfig.GetUsers(new Dictionary<string, object> { { "UserRemark", openId } }).FirstOrDefault();
            if (user == null)
            {
                context.Response.Write("错误的OpenId");
                return;
            }
            var sh = new SqlHelper<sys_ThingRecordsModel>(new sys_ThingRecordsModel());
            sh.AddWhere("UserId", user.UserId);
            sh.AddOrder("ThingDate", SortEnum.Desc);
            var model = sh.Select().FirstOrDefault();
            if (model == null)
            {
                context.Response.Write("此用户没有任何的拜访记录");
                return;
            }
            var familyModel = new sys_SubscriberFamilyModel();
            if (model.SubcriberId > 0)
            {
                var familySh = new SqlHelper<sys_SubscriberFamilyModel>(new sys_SubscriberFamilyModel());
                familySh.AddWhere("SubscriberId", model.SubcriberId);
                familyModel = familySh.Select().FirstOrDefault();
            }
            else
            {
                familyModel.SubscriberName = model.ExtraName;
                familyModel.SubscriberPhone = model.ExtraPhone;
            }

            var toUser = DataConfig.GetUsers(new Dictionary<string, object> { { "VillageId", user.VillageId }, { "OrganizationId", 3 } }).FirstOrDefault();

            context.Response.Write(new
            {
                fromName = $"{user.UserName}({user.UserPhone})",
                toName = $"{familyModel?.SubscriberName }({familyModel?.SubscriberPhone ?? "暂无联系方式"})",
                toOpenId = toUser?.UserRemark ?? ConfigurationManager.AppSettings["OpenIdForAdmin"],
                time = model.ThingDate.ToString("yy-M-d hh:mm"),
                title = model.ThingName,
                having = model.ThingHaving.Trim() == "是" ? "已解决" : "未解决"
            }.ToJson());
        }

        private void AddThingRecord(HttpContext context)
        {
            //获取当前登录的操作者信息
            sys_UsersModel model = new ManagePage().GetUsersinfo();
            var openId = context.Request.Form["openId"];
            if (model == null && openId.IsNullOrEmpty())
            {
                context.Response.Write("<script>parent.location.href=http://'" + HttpContext.Current.Request.Url.Authority + "/login.html'</script>");
                return;
            }
            else
            {
                sys_ThingRecordsModel thingModel = new sys_ThingRecordsModel
                {
                    ThingDate = DateTime.Now,
                    ThingName = MXRequest.GetFormString("thingname") ?? "",
                    ThingReason = MXRequest.GetFormString("thingreason") ?? "",
                    ThingSolution = MXRequest.GetFormString("thingsolution") ?? "",
                    ThingHaving = MXRequest.GetFormString("thinghaving") == "" ? "否" : "是",
                    ThingImgUrl = MXRequest.GetFormString("imgurl") ?? "",
                    SubcriberId = MXRequest.GetFormString("slSubId").ToInt(),
                    UserId = model?.UserId ?? GetUserIdByOpenId(openId),
                    ExtraName = context.Request["name"] ?? "",
                    ExtraPhone = context.Request["phone"] ?? ""
                };
                string result = thingBLL.AddThingRecord(thingModel);
                context.Response.Write(result);
            }
        }

        public int GetUserIdByOpenId(string openId)
        {
            var sh = new SqlHelper<sys_UsersModel>(new sys_UsersModel());
            sh.AddWhere("UserRemark", openId, RelationEnum.Like);
            var model = sh.Select().FirstOrDefault();
            if (model == null)
            {
                LogHelper.Log("根据OpenId(" + openId + ")未能查询到人员信息");
                return 0;
            }
            else
            {
                return model.UserId;
            }
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