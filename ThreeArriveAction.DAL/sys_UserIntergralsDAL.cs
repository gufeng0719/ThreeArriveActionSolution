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

        public DataSet GetList(int pageSize,int pageIndex,string strWhere,string filedOrder,out int recordCount )
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select a.*,b.VillageId,b.UserName,c.VillageName,c.VillageParId ");
            strSql.Append(" from sys_UserIntegrals a inner join sys_Users b on a.UserId = b.UserId ");
            strSql.Append(" inner join sys_Villages c on b.VillageId =c.VillageId ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
            return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }

        public DataTable UserIntergralsChart(int town,int year,int month)
        {
            string strSql1 = "select VillageId,VillageName,0 as AvgIntergral from sys_Villages where VillageParId=" + town;
            DataTable dt = DbHelperSQL.Query(strSql1).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                string strSql2 = "select count(1) from sys_Users where VillageId="+dr["VillageId"].ToString();
                int unumber =Convert.ToInt32( DbHelperSQL.GetSingle(strSql2));//该村总人数
                string strSql3 = "select sum(IntegralScore)  from sys_UserIntegrals a left join sys_Users b on a.UserId =b.UserId";
                strSql3+=" where IntegralYear = "+year+" and IntegralMonth ="+month+" and b.VillageId ="+dr["VillageId"].ToString();
                int suminter = Convert.ToInt32(DbHelperSQL.GetSingle(strSql3));
                dr["AvgIntergral"] = suminter / unumber;
            }
            return dt;
        }
        #endregion
    }
}
