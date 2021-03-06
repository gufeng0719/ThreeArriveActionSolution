﻿using System;
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
    /// 每日早报道数据访问类
    /// </summary>
    public partial class sys_SignsDAL
    {
        public sys_SignsDAL() { }

        #region 添加
        /// <summary>
        /// 用户点击签到
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <returns></returns>
        public int AddSign(int userId)
        {
            using (SqlConnection conn = new SqlConnection(DbHelperSQL.connectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        StringBuilder strSql = new StringBuilder();
                        strSql.Append("insert into sys_Signs (SignDate,SignUserId) values (@SignDate,@SignUserId);select @@IDENTITY; ");
                        SqlParameter[] parameters = {
                                            new SqlParameter("@SignDate",SqlDbType.DateTime),
                                            new SqlParameter("@SignUserId",SqlDbType.Int,4)
                                       };
                        parameters[0].Value = DateTime.Now;
                        parameters[1].Value = userId;
                        object obj = DbHelperSQL.GetSingle(conn, trans, strSql.ToString(), parameters);//带事务
                        int signId = Convert.ToInt32(obj);

                        sys_UserIntergralsDAL userInter = new sys_UserIntergralsDAL();
                        int integralId = userInter.ExistsUserIntegrals(userId);
                        //判断是否存在该有用户积分表
                        if (integralId > 0)
                        {
                            //存在积分信息，修改积分
                            StringBuilder strSql2 = new StringBuilder();
                            strSql2.Append("update sys_UserIntegrals set IntegralScore=IntegralScore+1 ");
                            strSql2.Append(" where IntegralId=@IntegralId");
                            SqlParameter[] parameters2 ={
                                               new SqlParameter("@IntegralId",SqlDbType.Int,4)
                                           };
                            parameters2[0].Value = integralId;
                            DbHelperSQL.ExecuteSql(conn, trans, strSql2.ToString(), parameters2);
                        }
                        else
                        {
                            //不存在积分信息，添加积分信息
                            StringBuilder strSql3 = new StringBuilder();
                            strSql3.Append("insert into sys_UserIntegrals (UserId,IntegralScore,IntegralYear,IntegralMonth )");
                            strSql3.Append(" values(@UserId,@IntegralScore,@IntegralYear,@IntegralMonth ); select @@Identity;");
                            SqlParameter[] parameters3 ={
                                               new SqlParameter("@UserId",SqlDbType.Int,4),
                                               new SqlParameter("@IntegralScore",SqlDbType.Int,4),
                                               new SqlParameter("@IntegralYear",SqlDbType.Int,4),
                                               new SqlParameter("@IntegralMonth",SqlDbType.Int,4)
                                           };
                            parameters3[0].Value = userId;
                            parameters3[1].Value = 1;
                            parameters3[2].Value = DateTime.Now.Year;
                            parameters3[3].Value = DateTime.Now.Month;
                            object obj2 = DbHelperSQL.GetSingle(conn, trans, strSql3.ToString(), parameters3);
                            integralId = Convert.ToInt32(obj2);
                        }
                        //添加积分详细信息
                        StringBuilder strSql4 = new StringBuilder();
                        strSql4.Append("insert into sys_IntegralInfo (IntegralId,UserId,IntegralType,");
                        strSql4.Append("Score,IntegralDate,IntegralEventId ) values (");
                        strSql4.Append("@IntegralId,@UserId,@IntegralType,@Score,@IntegralDate,@IntegralEventId)");
                        SqlParameter[] parameters4 ={
                                           new SqlParameter("@IntegralId",SqlDbType.Int,4),
                                           new SqlParameter("@UserId",SqlDbType.Int,4),
                                           new SqlParameter("@IntegralType",SqlDbType.Int,4),
                                           new SqlParameter("@Score",SqlDbType.Int,4),
                                           new SqlParameter("@IntegralDate",SqlDbType.DateTime),
                                           new SqlParameter("@IntegralEventId",SqlDbType.Int,4)
                                       };
                        parameters4[0].Value = integralId;
                        parameters4[1].Value = userId;
                        parameters4[2].Value = 1;
                        parameters4[3].Value = 1;
                        parameters4[4].Value = DateTime.Now;
                        parameters4[5].Value = signId;
                        int number = DbHelperSQL.ExecuteSql(conn, trans, strSql4.ToString(), parameters4);
                        if (number > 0)
                        {
                            trans.Commit();
                            return number;
                        }
                        else
                        {
                            trans.Rollback();
                            return number;
                        }
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return 0;
                    }
                }
            }
        }
        #endregion

        #region 查询与统计
        public bool Exists(int userId, DateTime signDate)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select SignId from sys_Signs ");
            strSql.Append(" where SignUserId=@SignUserId ");
            strSql.Append(" and Convert(varchar(100),SignDate,23)=@SignDate ");
            SqlParameter[] parameters ={
                                           new SqlParameter("@SignUserId",SqlDbType.Int,4),
                                           new SqlParameter("@SignDate",SqlDbType.VarChar,15)
                                      };
            parameters[0].Value = userId;
            parameters[1].Value = signDate.ToString("yyyy-MM-dd");
            bool bl = DbHelperSQL.Exists(strSql.ToString(), parameters);
            return bl;
        }

        public DataSet SearchUserSign(int pageSize,int pageIndex,string strWhere1,string strWhere2,string fieldOrder,out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select a.*,case  when b.SignId>0 then 'true' else 'false' end  as blSign,b.SignId,SignDate from  ");
            strSql.Append("(select UserId,UserName,sys_Users.VillageId,VillageName,VillageParId  from sys_Users  inner join sys_Villages on sys_Users.VillageId = sys_Villages.VillageId ");
            if (strWhere1.Trim() != ""){
                strSql.Append(" where "+strWhere1);
            }
            strSql.Append(" ) a left join (select * from sys_Signs ");
            if (strWhere2.Trim() != "")
            {
                strSql.Append(" where "+strWhere2);
            }
            strSql.Append(" ) b on a.UserId = b.SignUserId  ");
            recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
            DataSet ds = DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), fieldOrder));
            return ds;
        }

        /// <summary>
        /// 统计每日早报道
        /// </summary>
        /// <param name="strSql1">用户统计语句</param>
        /// <param name="strSql2">早报道签到表统计语句/param>
        /// <returns></returns>
        public DataTable StatisticsSign(string strSql1,string strSql2)
        {
            //得到基础的统计数据
            DataTable dt = DbHelperSQL.Query(strSql1.ToString()).Tables[0];
            if (dt.Rows.Count > 0)
            {
                foreach(DataRow dr in dt.Rows)
                {
                    dr["HavNumber"] = Convert.ToInt32(DbHelperSQL.GetSingle(strSql2+dr["Id"].ToString()));
                }
            }
            return dt;
        }
        #endregion
    }
}
