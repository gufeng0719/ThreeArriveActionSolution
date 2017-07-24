using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ThreeArriveAction.Common;
using ThreeArriveAction.Model;

namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// sys_SignInfoManager 的摘要说明
    /// </summary>
    public class sys_SignInfoManager : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var type = context.Request["type"];
            if (type == "add")
            {
                Add(context);
            }
            else if (type == "getIsSubmit")
            {
                GetIsSubmit(context);
            }
        }

        public void GetIsSubmit(HttpContext context)
        {
            var userId = context.Request["userId"].ToInt();
            var signSh = new SqlHelper<sys_SignsModel>(new sys_SignsModel());
            signSh.AddWhere("SignUserId", userId);
            signSh.AddWhere(" AND SignDate > '" + DateTime.Now.ToString("yyyy-MM-dd") + "'");
            var sign = signSh.Select().FirstOrDefault();
            if (sign == null)
            {
                context.Response.Write(new
                {
                    code = 0,
                    msg = "userId未能查询到签到相关信息"
                }.ToJson());
                return;
            }
            var signInfoSh = new SqlHelper<sys_SignInfoModel>(new sys_SignInfoModel());
            signInfoSh.AddWhere("SignId", sign.SignId);
            signInfoSh.AddWhere("SignInfoDate", DateTime.Now.ToString("yyyy-MM-dd"), RelationEnum.GreaterEqual);
            if (signInfoSh.Select().Any())
            {
                context.Response.Write(new
                {
                    code = 0,
                    msg = "已签到,且上传照片已完成,无需重复操作"
                }.ToJson());
            }
            else
            {
                context.Response.Write(new
                {
                    code = 1,
                    msg = "还未上传照片,请尽快上传."
                }.ToJson());
            }
        }

        public void Add(HttpContext context)
        {
            var userId = context.Request["userId"].ToInt();
            var path1 = context.Request["path1"];
            var path2 = context.Request["path2"];
            var path3 = context.Request["path3"];
            var path4 = context.Request["path4"];
            var msg = context.Request["msg"] ?? "";
            var signSh = new SqlHelper<sys_SignsModel>(new sys_SignsModel());
            signSh.AddWhere("SignId", userId);
            signSh.AddWhere("SignDate", DateTime.Now.ToString("yyyy-MM-dd"), RelationEnum.GreaterEqual);
            var sign = signSh.Select().FirstOrDefault();
            if (sign == null)
            {
                context.Response.Write(0);
                return;
            }
            var signInfo = new SqlHelper<sys_SignInfoModel>(new sys_SignInfoModel
            {
                SignId = sign.SignId,
                Msg = msg,
                Path1 = path1,
                Path2 = path2,
                Path3 = path3,
                Path4 = path4
            });
            context.Response.Write(signInfo.Insert());
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