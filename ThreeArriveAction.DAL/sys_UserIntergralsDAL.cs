using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using ThreeArriveAction.DBUtility;
using ThreeArriveAction.Model;
using ThreeArriveAction.Common;

namespace ThreeArriveAction.DAL
{
    /// <summary>
    /// 用户积分数据访问类
    /// </summary>
    public partial class sys_UserIntergralsDAL
    {
        public sys_UserIntergralsDAL() { }

        #region 查询
        /// <summary>
        /// 查询该用户在本年本月是否存在积分记录
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int ExistsUserIntegrals(int userId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select IntegralId from sys_UserIntegrals ");
            strSql.Append(" where UserId=@UserId and IntegralYear=@IYear and IntegralMonth=@IMonth ");
            SqlParameter[] parameters ={
                                          new SqlParameter("@UserId",SqlDbType.Int,4),
                                          new SqlParameter("@IYear",SqlDbType.Int,4),
                                          new SqlParameter("@IMonth",SqlDbType.Int,4)
                                      };
            parameters[0].Value = userId;
            parameters[1].Value = DateTime.Now.Year;
            parameters[2].Value = DateTime.Now.Month;
            object obj = DbHelperSQL.GetSingle(strSql.ToString(),parameters);
            if (object.Equals(obj, null))
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }

        }
        #endregion
    }
}
