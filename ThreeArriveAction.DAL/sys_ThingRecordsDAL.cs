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
    public partial class sys_ThingRecordsDAL
    {
        #region 添加
        public int AddThingRecord(sys_ThingRecordsModel thingModel)
        {
           using (SqlConnection conn = new SqlConnection(DbHelperSQL.connectionString)){
                conn.Open();
                using(SqlTransaction trans = conn.BeginTransaction()){
                    try
                    {
                        //添加有事马上到表数据
                        StringBuilder strSql = new StringBuilder();
                        strSql.Append("insert into sys_ThingRecords (ThingDate,ThingName,");
                        strSql.Append("ThingReason,ThingSolution,ThingHaving,ThingImgUrl,");
                        strSql.Append("SubcriberId,UserId )");
                        strSql.Append(" values (@ThingDate,@ThingName,@ThingReason,@ThingSolution,");
                        strSql.Append("@ThingHaving,@ThingImgUrl,@SubcriberId,@UserId);");
                        strSql.Append("select @@Identity ");
                        SqlParameter[] parameter1 ={
                                                       new SqlParameter("@ThingDate",SqlDbType.DateTime),
                                                       new SqlParameter("@ThingName",SqlDbType.VarChar,500),
                                                       new SqlParameter("@ThingReason",SqlDbType.VarChar,5000),
                                                       new SqlParameter("@ThingSolution",SqlDbType.VarChar,5000),
                                                       new SqlParameter("@ThingHaving",SqlDbType.VarChar,50),
                                                       new SqlParameter("@ThingImgUrl",SqlDbType.VarChar,500),
                                                       new SqlParameter("@SubcriberId",SqlDbType.Int,4),
                                                       new SqlParameter("@UserId",SqlDbType.Int,4)
                                                   };
                        parameter1[0].Value = thingModel.ThingDate;
                        parameter1[1].Value =thingModel.ThingName;
                        parameter1[2].Value = thingModel.ThingReason;
                        parameter1[3].Value = thingModel.ThingSolution;
                        parameter1[4].Value = thingModel.ThingHaving;
                        parameter1[5].Value = thingModel.ThingImgUrl;
                        parameter1[6].Value = thingModel.SubcriberId;
                        parameter1[7].Value = thingModel.UserId;
                        object obj = DbHelperSQL.GetSingle(conn, trans, strSql.ToString(), parameter1);
                        if (obj != null)
                        {
                            int thingId = Convert.ToInt32(obj);
                            sys_UserIntergralsDAL userInter = new sys_UserIntergralsDAL();
                            int integralId = userInter.ExistsUserIntegrals(thingModel.UserId);
                            //判断是否存在该有用户积分表
                            if (integralId > 0)
                            {
                                //存在积分信息，修改积分
                                StringBuilder strSql2 = new StringBuilder();
                                strSql2.Append("update sys_UserIntegrals set IntegralScore=IntegralScore+3 ");
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
                                parameters3[0].Value = thingModel.UserId;
                                parameters3[1].Value = 3;
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
                            parameters4[1].Value = thingModel.UserId;
                            parameters4[2].Value = 3;
                            parameters4[3].Value = 3;
                            parameters4[4].Value = DateTime.Now;
                            parameters4[5].Value = thingId;
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

        public DataTable GetThingModelByThingId(int thingId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 a.*,b.UserName,c.VillageName,c.VillageParId,c.VillageId,d.SubscriberName,d.SubscriberPhone,e.Dictdata_Name ");
            strSql.Append("from sys_ThingRecords a inner join sys_Users b on a.UserId = b.UserId inner join sys_Villages c ");
            strSql.Append("on b.VillageId = c.VillageId left join sys_SubscriberFamily d on a.SubcriberId = d.SubscriberId ");
            strSql.Append(" inner join(select * from sys_DictionaryData where Dict_value=1) e on d.SubscriberType =e.Dictdata_Value ");
            strSql.Append(" where ThingId ="+thingId );
            DataTable dt = DbHelperSQL.Query(strSql.ToString()).Tables[0];
            return dt;
        }

        public DataSet SearchThingRecord(int pageSize,int pageIndex,string strWhere,string fieldOrder,out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select a.*,b.UserName,c.VillageName,c.VillageParId,c.VillageId,d.SubscriberName,e.Dictdata_Name ");
            strSql.Append("from sys_ThingRecords a inner join sys_Users b on a.UserId = b.UserId ");
            strSql.Append("inner join sys_Villages c on b.VillageId = c.VillageId ");
            strSql.Append("left join sys_SubscriberFamily d on a.SubcriberId = d.SubscriberId ");
            strSql.Append("inner join(select * from sys_DictionaryData where Dict_value=1) e on d.SubscriberType =e.Dictdata_Value ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where "+strWhere);
            }
            recordCount =Convert.ToInt32( DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
            DataSet ds = DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount,pageSize,pageIndex,strSql.ToString(),fieldOrder));
            return ds;
        }
        #endregion
    }
}
