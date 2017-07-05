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
       #endregion
       #region 修改
       #endregion

       #region 删除
       #endregion

       #region 查询
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

       public string GetListJson(int parId)
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
                   int grage =int.Parse(dr["VillageGrage"].ToString());
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
