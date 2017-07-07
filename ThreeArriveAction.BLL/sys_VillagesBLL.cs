using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ThreeArriveAction.Common;
using ThreeArriveAction.DAL;
using ThreeArriveAction.Model;

namespace ThreeArriveAction.BLL
{
   public partial class sys_VillagesBLL
   {
       private readonly sys_VillagesDAL villageDAL = new sys_VillagesDAL();
       #region 添加
       /// <summary>
       /// 添加村居信息
       /// </summary>
       /// <param name="model">村居实体</param>
       /// <returns></returns>
       public string AddVillage(sys_VillagesModel model)
       {
           model.VillageGrade = GetVillageGrade(model.VillageParId);
           int number = villageDAL.AddVillage(model);
           if (number > 0)
           {
               return "{\"info\":\"村居添加成功\",\"status\":\"y\"}";
           }
           else
           {
               return "{\"info\":\"村居添加失败\",\"status\":\"n\"}";
           }
       }
       #endregion
       #region 修改
       /// <summary>
       /// 修改村居信息
       /// </summary>
       /// <param name="model">村居实体</param>
       /// <returns></returns>
       public string UpdateVillage(sys_VillagesModel model)
       {
           model.VillageGrade = GetVillageGrade(model.VillageParId);
           int number = villageDAL.UpdateVillage(model);
           if (number > 0)
           {
               return "{\"info\":\"村居修改成功\",\"status\":\"y\"}";
           }
           else
           {
               return "{\"info\":\"村居修改失败\",\"status\":\"n\"}";
           }
       }
       #endregion

       #region 删除
       public string DeleteVillage(string ids)
       {
           int number = villageDAL.DeleteVillage(ids);
           if (number > 0)
           {
               return "{\"info\":\"村居删除成功\",\"status\":\"y\"}";
           }
           else
           {
               return "{\"info\":\"村居删除失败\",\"status\":\"n\"}";
           }
       }
       #endregion

       #region 查询

       public int GetVillageGrade(int parId)
       {
           return villageDAL.GetVillageGrade(parId);
       }

       /// <summary>
       /// 根据村居编号查询该村居信息
       /// </summary>
       /// <param name="villageId">村居编号</param>
       /// <returns></returns>
       public sys_VillagesModel GetVillage(int villageId)
       {
           return villageDAL.GetVillage(villageId);
       }
       /// <summary>
       /// 根据村居编号查询该村居Json数据格式
       /// </summary>
       /// <param name="villageId">村居编号</param>
       /// <returns></returns>
       public string GetVillageJson(int villageId)
       {
           sys_VillagesModel model = GetVillage(villageId);
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
       /// 根据父级编号，查询所有子集信息
       /// </summary>
       /// <param name="parId"></param>
       /// <returns></returns>
       public DataTable GetVillageListByParId(int parId)
       {
           return villageDAL.GetVillageListByParId(parId);
       }

       /// <summary>
       /// 取得所有村居列表
       /// </summary>
       /// <param name="parId">父级编号</param>
       /// <returns></returns>
       public DataTable GetList(int parId)
       {
           return villageDAL.GetList(parId);
       }


       public string GetListTreeJson(int parId)
       {
           DataTable dt = villageDAL.GetList(parId);
           StringBuilder strJson = new StringBuilder();
           strJson.Append("{\"total\":");
           if (dt != null)
           {
               strJson.Append((dt.Rows.Count+1) + "," + "\"rows\":[");
               strJson.Append("{\"title\":\"无父级导航\",\"value\":0}");
               foreach (DataRow dr in dt.Rows)
               {
                   string id = dr["VillageId"].ToString();
                   string title = dr["VillageName"].ToString();
                   int grage =int.Parse(dr["VillageGrade"].ToString());
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

       public string GetListJson(int parId)
       {
           DataTable dt = GetList(parId);
           StringBuilder strJson = new StringBuilder();
           strJson.Append("{\"total\":"+dt.Rows.Count);
           if (dt.Rows.Count > 0)
           {
               strJson.Append(",\"rows\":"+JsonHelper.ToJson(dt));
           }
           strJson.Append("}");
           return strJson.ToString();
       }

       #endregion
   }
}
