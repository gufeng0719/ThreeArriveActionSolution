using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using ThreeArriveAction.BLL;
using ThreeArriveAction.Common;
using ThreeArriveAction.Web.UI;
using ThreeArriveAction.Model;

namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// sys_VillagesManager 的摘要说明
    /// </summary>
    public class sys_VillagesManager : IHttpHandler, IRequiresSessionState
    {
        private readonly sys_VillagesBLL villageBLL = new sys_VillagesBLL();
        public void ProcessRequest(HttpContext context)
        {
            string type = MXRequest.GetQueryString("type");
            switch(type){
                case "select":
                    GetVillageTreeJson(context);
                    break;
                case "get":
                    GetVillagesList(context);
                    break;
                case "edit":
                    GetVillageModel(context);
                    break;
                case "save":
                    SaveVillage(context);
                    break;
                case "del":
                    DeleteVillage(context);
                    break;

            }
        }
        /// <summary>
        /// 获取村居下拉选项数据
        /// </summary>
        /// <param name="context"></param>
        private void GetVillageTreeJson(HttpContext context)
        {
            int parentId = 0;
            //获取当前登录的操作者信息
            sys_UsersModel model = new ManagePage().GetUsersinfo();
            if (model == null)
            {
                context.Response.Write("<script>parent.location.href='login.html'</script>");
            }
            else
            {
                if (model.OrganizationId == 1)
                {
                    parentId = 0;
                }
                else
                {
                    parentId = model.OrganizationId;
                }
                string result = villageBLL.GetListTreeJson(parentId);
                context.Response.Write(result);
            }
        }

        /// <summary>
        /// 获取所有村居数据列表
        /// </summary>
        /// <param name="context"></param>
        private void GetVillagesList(HttpContext context)
        {
            string result = villageBLL.GetListJson(0);
            context.Response.Write(result);
        }

        /// <summary>
        /// 获取本村居信息
        /// </summary>
        private void GetVillageModel(HttpContext context)
        {
            int villageId = int.Parse(MXRequest.GetQueryString("villageid"));
            string result = villageBLL.GetVillageJson(villageId);
            context.Response.Write(result);
        }

        private void SaveVillage(HttpContext context)
        {
            sys_VillagesModel villageModel = new sys_VillagesModel();
            villageModel.VillageName = MXRequest.GetFormString("villagename");
            villageModel.VillageParId = int.Parse(MXRequest.GetFormString("parid"));
            villageModel.Remarks = MXRequest.GetFormString("remark");
            string action = MXRequest.GetFormString("action");
            string result = "";
            if (action == "add")
            {
                result = villageBLL.AddVillage(villageModel);
            }
            else
            {
                villageModel.VillageId = int.Parse(MXRequest.GetFormString("villageid"));
                result = villageBLL.UpdateVillage(villageModel);
            }
            context.Response.Write(result);
        }

        /// <summary>
        /// 删除村居信息，包含其子级村居
        /// </summary>
        /// <param name="context"></param>
        private void DeleteVillage(HttpContext context)
        {
            string ids = MXRequest.GetFormString("str");
            string result = villageBLL.DeleteVillage(ids);
            context.Response.Write(result);
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