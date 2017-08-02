using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ThreeArriveAction.DAL;
using ThreeArriveAction.Model;
using ThreeArriveAction.Common;

namespace ThreeArriveAction.BLL
{
    /// <summary>
    /// 业务逻辑类:每周家家到
    /// </summary>
   public partial class sys_WeekArrivesBLL
    {
       private readonly sys_WeekArrivesDAL weekDAL = new sys_WeekArrivesDAL();
        #region 添加
       public string AddWeekArrives(sys_WeekArrivesModel wModel)
       {
           int number = weekDAL.AddWeekArrives(wModel);
           if (number > 0)
           {
               return "{\"info\":\"本次拜访提交成功\",\"status\":\"y\"}";
           }
           else
           {
               return "{\"info\":\"本次拜访提交失败\",\"status\":\"n\"}";
           }
       }
        #endregion
        #region 查询与统计
        public DataSet SearchUserWeekArrive(int pageSize,int pageIndex,string strWhere1,string strWhere2,string fieldOrder,out int recordCount)
        {
            DataSet ds = weekDAL.SearchWeekArrive(pageSize,pageIndex,strWhere1,strWhere2,fieldOrder,out recordCount);
            return ds;
        }

        public DataTable StatisWeekArrive(string strSql1,string strSql2)
        {
            DataTable dt = weekDAL.StatisWeekArrive(strSql1, strSql2);
            return dt;
        }
        #endregion
    }
}
