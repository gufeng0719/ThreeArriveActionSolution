using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ThreeArriveAction.DAL;

namespace ThreeArriveAction.BLL
{
   public partial class sys_UserIntergralsBLL
    {
        private readonly sys_UserIntergralsDAL uIDAL = new sys_UserIntergralsDAL();
        public sys_UserIntergralsBLL() { }

        #region
        /// <summary>
        /// 分页查询用户信息
        /// </summary>
        /// <param name="pageSize">每页条数</param>
        /// <param name="PageIndex">当前页码</param>
        /// <param name="strWhere">查询条件</param>
        /// <param name="filedOrder">排序条件</param>
        /// <param name="recordCount">总记录数</param>
        /// <returns></returns>
        public DataSet GetList(int pageSize, int PageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            DataSet ds = uIDAL.GetList(pageSize, PageIndex, strWhere, filedOrder, out recordCount);
            return ds;
        }
        #endregion
    }
}
