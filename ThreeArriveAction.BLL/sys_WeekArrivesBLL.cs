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
    }
}
