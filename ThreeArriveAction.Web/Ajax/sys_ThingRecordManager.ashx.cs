using System;
using System.Collections.Generic;
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
            }
        }

        private void AddThingRecord(HttpContext context)
        {
            //获取当前登录的操作者信息
            sys_UsersModel model = new ManagePage().GetUsersinfo();
            var openId = context.Request.Form["openId"];
            if (model == null && openId.IsNullOrEmpty())
            {
                context.Response.Write("<script>parent.location.href='login.html'</script>");
            }
            else
            {
                sys_ThingRecordsModel thingModel = new sys_ThingRecordsModel();
                thingModel.ThingDate = DateTime.Now;
                thingModel.ThingName = MXRequest.GetFormString("thingname");
                thingModel.ThingReason = MXRequest.GetFormString("thingreason");
                thingModel.ThingSolution = MXRequest.GetFormString("thingsolution");
                thingModel.ThingHaving = MXRequest.GetFormString("thinghaving") == "" ? "否" : "是";
                thingModel.ThingImgUrl = MXRequest.GetFormString("imgurl");
                thingModel.SubcriberId = int.Parse(MXRequest.GetFormString("slSubId"));
                thingModel.UserId = model == null ? GetUserIdByOpenId(openId) : model.UserId;
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