using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThreeArriveAction.DAL;
using ThreeArriveAction.Common;
using ThreeArriveAction.Model;
using System.Data;

namespace ThreeArriveAction.BLL
{
   public partial class sys_NavigationsBLL
    {
       private readonly sys_NavigationsDAL navDAL = new sys_NavigationsDAL();
        #region 增加
       public string AddNavigation(sys_NavigationsModel model)
       {
           model.NavLayer = GetNavLayer(model.ParentId);
           int number = navDAL.AddNavigation(model);
           if (number > 0)
           {
               return "{\"info\":\"菜单添加成功\",\"status\":\"y\"}";
           }
           else
           {
               return "{\"info\":\"菜单添加失败\",\"status\":\"n\"}";
           }

       }
        #endregion
        #region 修改
       public string UpdateNavigation(sys_NavigationsModel model)
       {
           model.NavLayer = GetNavLayer(model.ParentId);
           int number = navDAL.UpdateNavigation(model);
           if (number > 0)
           {
               return "{\"info\":\"菜单修改成功\",\"status\":\"y\"}";
           }
           else
           {
               return "{\"info\":\"菜单修改失败\",\"status\":\"n\"}";
           }

       }
        #endregion
        #region 删除
        public string DeleteNavigation(string ids)
        {
            int number = navDAL.DeleteNavigation(ids);
            if (number > 0)
            {
                return "{\"info\":\"菜单删除成功\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"菜单删除失败\",\"status\":\"n\"}";
            }
        }
        #endregion
        #region 查询

        public int GetNavLayer(int parentId)
       {
           return navDAL.GetNavLayer(parentId);
       }

       /// <summary>
       /// 根据菜单编号，查询该菜单信息
       /// </summary>
       /// <param name="navigationId">菜单编号</param>
       /// <returns></returns>
       public sys_NavigationsModel GetNavigationsByNavId(int navigationId)
       {
           sys_NavigationsModel model = navDAL.GetNavigationsByNavId(navigationId);
           return model;
       }

       public string GetNavigationsJsonByNavId(int navigationId)
       {
           sys_NavigationsModel model = GetNavigationsByNavId(navigationId);
           if (model != null)
           {
               return JsonHelper.ToJson(model);
           }
           else
           {
               return "";
           }
       }

       /// <summary>
       /// 根据父级编号，查询所有子级菜单
       /// </summary>
       /// <param name="parnetId">父级编号</param>
       /// <returns></returns>
       public List<sys_NavigationsModel> GetNavigationsByParentId(int parnetId)
       {
           List<sys_NavigationsModel> modelList = navDAL.GetNavigationsByParentId(parnetId);
           return modelList;
       }
       /// <summary>
       /// 根据组织编号，查询该等级所拥有的菜单
       /// </summary>
       /// <param name="organizationId">组织等级编号</param>
       /// <returns></returns>
       public List<sys_NavigationsModel> GetNavigationsByOrganization(int organizationId)
       {
           List<sys_NavigationsModel> modelList = navDAL.GetNavigationsByOrganization(organizationId);
           return modelList;
       }

       public DataTable GetDataList(int parentId)
       {
           return navDAL.GetDataList(parentId);
       }

       public DataTable GetList(int parentId)
       {
           return navDAL.GetList(parentId);
       }

       /// <summary>
       /// 根据父级编号，获取所有菜单数据(已经排序好)
       /// </summary>
       /// <param name="parentId"></param>
       /// <returns></returns>
       public string GetDataListJson(int parentId)
       {
           DataTable dt = GetDataList(parentId);
           StringBuilder strJson = new StringBuilder();
           strJson.Append("{\"total\":"+dt.Rows.Count);
           strJson.Append(",\"rows\":");
           if (dt.Rows.Count > 0)
           {
               strJson.Append(JsonHelper.ToJson(dt));
           }
           else
           {
               strJson.Append("[]");
           }
           strJson.Append("}");
           return strJson.ToString();
       }
       /// <summary>
       /// 根据父级编号获取用于下拉列表的数据
       /// </summary>
       /// <param name="parentId">父级编号</param>
       /// <returns></returns>
       public string GetListTreeJson(int parentId)
       {
           DataTable dt = GetDataList(parentId);
           StringBuilder strJson = new StringBuilder();
           strJson.Append("{\"total\":");
           if (dt != null)
           {
               strJson.Append((dt.Rows.Count + 1) + "," + "\"rows\":[");
               strJson.Append("{\"title\":\"无父级导航\",\"value\":0}");
               foreach (DataRow dr in dt.Rows)
               {
                   string id = dr["NavigationId"].ToString();
                   string title = dr["NavigationName"].ToString();
                   int grage = int.Parse(dr["NavLayer"].ToString());
                   if (grage != 1)
                   {
                       title = "|- " + title;
                       title = Utils.StringOfChar(grage - 1, "&nbsp;&nbsp;&nbsp;&nbsp;") + title;
                   }
                   strJson.Append(",{\"title\":\"" + title + "\",\"value\":\"" + id + "\"}");
               }
               strJson.Append("]");
           }
           else
           {
               strJson.Append("0");
           }
           strJson.Append("}");
           return strJson.ToString();
       }

        #endregion
    }
}
