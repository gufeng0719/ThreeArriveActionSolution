using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ThreeArriveAction.DBUtility;
using ThreeArriveAction.Common;

namespace ThreeArriveAction.DAL
{
   public partial  class sys_IntegralInfoDAL
    {
        public sys_IntegralInfoDAL() { }

        #region 查询
        public DataSet GetList(int pageSize,int pageIndex,string strWhere,string fileOrder,out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select IntegralInfoId,IntegralId,UserId,IntegralType,Score,Convert(varchar(100),IntegralDate,23) as IntegralDate,IntegralEventId,Remarks from sys_IntegralInfo ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where "+strWhere);
            }
            recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
            return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount,pageSize,pageIndex,strSql.ToString(),fileOrder));
        }
        #endregion
    }
}
