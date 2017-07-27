using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ThreeArriveAction.DAL;

namespace ThreeArriveAction.BLL
{
    public partial class sys_IntegralInfoBLL
    {
        private readonly sys_IntegralInfoDAL iDAL = new sys_IntegralInfoDAL();
        public sys_IntegralInfoBLL() { }
        #region 查询
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string fileOrder, out int recordCount)
        {
            DataSet ds = iDAL.GetList(pageSize, pageIndex, strWhere, fileOrder, out recordCount);
            return ds;
        }
            #endregion
        }
}
