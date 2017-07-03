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
           return "";
       }

       #endregion
   }
}
