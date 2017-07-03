using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThreeArriveAction.Model;
using ThreeArriveAction.DAL;

namespace ThreeArriveAction.BLL
{
    /// <summary>
    /// 业务逻辑类:开启签到
    /// </summary>
    public partial class sys_OnSignsBLL
    {
        private sys_OnSignsDAL onSignsDAL = new sys_OnSignsDAL();
        #region 添加
        /// <summary>
        /// 添加该村今天签到信息
        /// </summary>
        /// <param name="onSignModel">开启签到信息实体</param>
        /// <returns></returns>
        public int AddOnSign(sys_OnSignsModel onSignModel)
        {
            return onSignsDAL.AddOnSign(onSignModel);
        }
        #endregion
        #region 查询
        /// <summary>
        /// 根据村居编号，查询该村今天是否开启签到
        /// </summary>
        /// <param name="villageId">村居编号</param>
        /// <returns></returns>
        public bool ExistsByVillageId(int villageId)
        {
            return onSignsDAL.ExistsByVillageId(villageId);
        }
        /// <summary>
        /// 根据村居编号与查询日期 ，查询是否开启签到
        /// </summary>
        /// <param name="villageId">村居编号</param>
        /// <param name="searchDate">查询日期</param>
        /// <returns></returns>
        public sys_OnSignsModel GetOnSignsByVillageId(int villageId, DateTime searchDate)
        {
            return onSignsDAL.GetOnSignsByVillageId(villageId, searchDate);
        }
        #endregion
    }
}
