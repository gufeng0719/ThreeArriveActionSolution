using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using ThreeArriveAction.DBUtility;
using ThreeArriveAction.Model;
using ThreeArriveAction.Common;
namespace ThreeArriveAction.DAL
{
    /// <summary>
    /// 数据访问类:每周家家到操作类
    /// </summary>
    public partial class sys_WeekArrivesDAL
    {
        public sys_WeekArrivesDAL() { }

        #region 添加
        /// <summary>
        /// 根据每周家家到实体数据，添加每周家家到信息
        /// </summary>
        /// <param name="wModel">实体数据</param>
        /// <returns></returns>
        public int AddWeekArrives(sys_WeekArrivesModel wModel)
        {
            using(SqlConnection conn = new SqlConnection(DbHelperSQL.connectionString)){
                conn.Open();
                using(SqlTransaction trans = conn.BeginTransaction()){
                    try
                    {
                        //添加每周家家到表数据
                        StringBuilder strSql = new StringBuilder();
                        strSql.Append("insert into sys_WeekArrives (WeekArriveDate,SubcriberId,");
                        strSql.Append("ThingMessage,ThingResult,ThingImgUrl,UserId,WeekArriveState )");
                        strSql.Append(" values (@WeekArriveDate,@SubcriberId,@ThingMessage,@ThingResult,@ThingImgUrl,");
                        strSql.Append(" @UserId,@WeekArriveState );select @@Identity ");
                        SqlParameter[] parameter1 ={
                                                       new SqlParameter("@WeekArriveDate",SqlDbType.DateTime),
                                                       new SqlParameter("@SubcriberId",SqlDbType.Int,4),
                                                       new SqlParameter("@ThingMessage",SqlDbType.VarChar,5000),
                                                       new SqlParameter("@ThingResult",SqlDbType.VarChar,5000),
                                                       new SqlParameter("@ThingImgUrl",SqlDbType.VarChar,500),
                                                       new SqlParameter("@UserId",SqlDbType.Int,4),
                                                       new SqlParameter("@WeekArriveState",SqlDbType.Int,4)
                                                   };
                        parameter1[0].Value = wModel.WeekArriveDate;
                        parameter1[1].Value = wModel.SubcriberId;
                        parameter1[2].Value = wModel.ThingMessage;
                        parameter1[3].Value = wModel.ThingResult;
                        parameter1[4].Value = wModel.ThingImgUrl;
                        parameter1[5].Value = wModel.UserId;
                        parameter1[6].Value = wModel.WeekArriveState;
                        object obj = DbHelperSQL.GetSingle(conn, trans, strSql.ToString(), parameter1);
                        if (obj != null)
                        {
                            int weekArriveId = Convert.ToInt32(obj);
                            sys_UserIntergralsDAL userInter = new sys_UserIntergralsDAL();
                            int integralId = userInter.ExistsUserIntegrals(wModel.UserId);
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
                                parameters3[0].Value = wModel.UserId;
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
                            parameters4[1].Value = wModel.UserId;
                            parameters4[2].Value = 2;
                            parameters4[3].Value = 1;
                            parameters4[4].Value = DateTime.Now;
                            parameters4[5].Value = weekArriveId;
                            int number = DbHelperSQL.ExecuteSql(conn, trans, strSql4.ToString(), parameters4);
                            //修改受访户坐标
                            StringBuilder strSql5 = new StringBuilder();
                            strSql5.Append("update sys_SubscriberFamily set FamilyCoordinate=@FamilyCoordinate where SubscriberId=@SubscriberId ");
                            SqlParameter[] parameters5 ={
                                                           new SqlParameter("@FamilyCoordinate",SqlDbType.VarChar,500),
                                                           new SqlParameter("@SubscriberId",SqlDbType.Int,4)
                                                       };
                            parameters5[0].Value = wModel.Remarks;
                            parameters5[1].Value = wModel.SubcriberId;
                            DbHelperSQL.ExecuteSql(conn, trans, strSql5.ToString(), parameters5);
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
                        else
                        {
                            trans.Rollback();
                            return 0;
                        }

                    }
                    catch
                    {
                        trans.Rollback();
                        return -1;
                    }
                }
            }
        }
        #endregion

        #region 查询和统计
        public DataSet SearchWeekArrive(int pageSize,int pageIndex,string strWhere1,string strWhere2,string fieldOrder,out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select a.*,case when b.WeekArriveId > 0 then 'true' else 'false' end as blWeek,b.WeekArriveId,ThingMessage from ");
            strSql.Append("(select SubscriberId,SubscriberName,sys_SubscriberFamily.VillageId,VillageName,VillageParId,sys_SubscriberFamily.UserId,UserName");
            strSql.Append(" from sys_SubscriberFamily ");
            strSql.Append("inner join sys_Villages on sys_SubscriberFamily.VillageId =sys_Villages.VillageId ");
            strSql.Append("inner join sys_Users on sys_SubscriberFamily.UserId = sys_Users.UserId ");
            if (strWhere1.Trim() != "")
            {
                strSql.Append(" where " + strWhere1);
            }
            strSql.Append(" ) a left join (select * from sys_WeekArrives ");
            if (strWhere2.Trim() != "")
            {
                strSql.Append(" where "+strWhere2);
            }
            strSql.Append(" ) b on a.SubscriberId = b.SubcriberId ");
            recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
            DataSet ds = DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize,pageIndex,strSql.ToString(),fieldOrder));
            return ds; 
        }
        #endregion
    }
}
