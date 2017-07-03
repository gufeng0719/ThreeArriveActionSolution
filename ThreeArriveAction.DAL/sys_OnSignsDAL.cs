using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using ThreeArriveAction.Model;
using ThreeArriveAction.DBUtility;

namespace ThreeArriveAction.DAL
{
    /// <summary>
    /// 数据访问操作类:
    /// </summary>
   public partial class sys_OnSignsDAL
    {
       public sys_OnSignsDAL() { }
       #region 添加
       /// <summary>
       /// 添加该村签到开启信息
       /// </summary>
       /// <param name="signModel"></param>
       /// <returns></returns>
       public int AddOnSign(sys_OnSignsModel signModel)
       {
           StringBuilder strSql = new StringBuilder();
           strSql.Append("insert into sys_OnSigns(VillageId,UserId,OnTime) values( ");
           strSql.Append("@VillageId,@UserId,@OnTime )");
           SqlParameter[] parameters ={
                                          new SqlParameter("@VillageId",SqlDbType.Int,4),
                                          new SqlParameter("@UserId",SqlDbType.Int,4),
                                          new SqlParameter("@OnTime",SqlDbType.DateTime)
                                      };
           parameters[0].Value = signModel.VillageId;
           parameters[1].Value = signModel.UserId;
           parameters[2].Value = signModel.OnTime;
           int numbers = DbHelperSQL.ExecuteSql(strSql.ToString(),parameters);
           return numbers;
       }
       #endregion
       #region 查询
       /// <summary>
       /// 根据村居编号，查询该村是否开启签到
       /// </summary>
       /// <param name="villageId"></param>
       /// <returns></returns>
       public bool ExistsByVillageId(int villageId)
       {
           StringBuilder strSql = new StringBuilder();
           strSql.Append("select OnSignId from sys_OnSigns ");
           strSql.Append(" where VillageId=@VillageId and CONVERT(varchar(100),OnTime,23) =@OnTime");
           SqlParameter[] parameters ={
                                          new SqlParameter("@VillageId",SqlDbType.Int,4),
                                          new SqlParameter("@OnTime",SqlDbType.VarChar,15)
                                     };
           parameters[0].Value = villageId;
           parameters[1].Value = DateTime.Now.ToString("yyyy-MM-dd");
           bool bl = DbHelperSQL.Exists(strSql.ToString(), parameters);
           return bl;
       }
       /// <summary>
       /// 根据村居编号与查询日期 ，查询是否开启签到
       /// </summary>
       /// <param name="villageId">村居编号</param>
       /// <param name="SearchDate">查询日期</param>
       /// <returns></returns>
       public sys_OnSignsModel GetOnSignsByVillageId(int villageId,DateTime SearchDate)
       {
           StringBuilder strSql = new StringBuilder();
           strSql.Append("select top 1 OnSignId,VillageId,UserId,OnTime ");
           strSql.Append("from sys_OnSigns where VillageId=@VillageId and CONVERT(varchar(100),OnTime,23) =@OnTime ");
           SqlParameter[] parameter ={
                                         new SqlParameter("@VillageId",SqlDbType.Int,4),
                                         new SqlParameter("@OnTime",SqlDbType.VarChar,15)
                                    };
           parameter[0].Value = villageId;
           parameter[1].Value = SearchDate.ToString("yyyy-MM-dd");
           SqlDataReader reader = DbHelperSQL.ExecuteReader(strSql.ToString(), parameter);
           sys_OnSignsModel onSignModel = new sys_OnSignsModel();
           if (reader.HasRows)
           {
               while (reader.Read())
               {
                   onSignModel.OnSignId = reader.GetInt32(0);
                   onSignModel.VillageId = reader.GetInt32(1);
                   onSignModel.UserId = reader.GetInt32(2);
                   onSignModel.OnTime = reader.GetDateTime(3);
               }
           }
           return onSignModel;
       }
        #endregion

    }
}
