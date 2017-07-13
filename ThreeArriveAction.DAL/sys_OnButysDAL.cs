using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using ThreeArriveAction.Common;
using ThreeArriveAction.DBUtility;
using ThreeArriveAction.Model;
namespace ThreeArriveAction.DAL
{
    /// <summary>
    /// 数据访问操作类:值班表信息
    /// </summary>
    public partial class sys_OnButysDAL
    {

        #region 基本操作
        /// <summary>
        /// 检索在某天某村是否已有值班信息
        /// </summary>
        /// <param name="villageid">村居编号</param>
        /// <param name="seachTime">查询日期</param>
        /// <returns></returns>
        public bool ExisByVillageId(int villageid,DateTime seachTime)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from sys_OnButys where ");
            strSql.Append(" VillageId=@VillageId and Convert(varchar(100),ButyDate,23)=@ButyDate");
            SqlParameter[] parameters ={
                                           new SqlParameter("@VillageId",SqlDbType.Int,4),
                                           new SqlParameter("@ButyDate",SqlDbType.VarChar,15)
                                      };
            parameters[0].Value = villageid;
            parameters[1].Value = seachTime.ToString("yyyy-MM-dd");
            object obj = DbHelperSQL.GetSingle(strSql.ToString(), parameters);
            if (obj != null)
            {
                if (Convert.ToInt32(obj) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }
        #endregion

        #region 添加
        /// <summary>
        /// 添加值班信息
        /// </summary>
        /// <param name="butyModel">值班实体信息</param>
        /// <returns></returns>
        public int AddOnButys(sys_OnButysModel butyModel){
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into sys_OnButys (VillageId,UserId,ButyDate)");
            strSql.Append("values (@VillageId,@UserId,@ButyDate)");
            SqlParameter[] parameters ={
                                           new SqlParameter("@VillageId",SqlDbType.Int,4),
                                           new SqlParameter("@UserId",SqlDbType.Int,4),
                                           new SqlParameter("@ButyDate",SqlDbType.DateTime)
                                      };
            parameters[0].Value = butyModel.VillageId;
            parameters[1].Value = butyModel.UserId;
            parameters[2].Value = butyModel.ButyDate;
            int number = DbHelperSQL.ExecuteSql(strSql.ToString(),parameters);
            return number;
        }
        #endregion

        #region 查询
        /// <summary>
        /// 检索在某天某村值班信息
        /// </summary>
        /// <param name="villageid">村居编号</param>
        /// <param name="butyDate">查询日期</param>
        /// <returns></returns>
        public sys_OnButysModel GetButyByVillageAndButyDate(int villageid, DateTime butyDate)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 OnbutyId,VillageId,UserId,ButyDate,Remarks ");
            strSql.Append("from sys_OnButys where VillageId=@VillageId and ");
            strSql.Append("Convert(varchar(100),ButyDate,23)=@ButyDate ");
            SqlParameter[] parameters ={
                                           new SqlParameter("@VillageId",SqlDbType.Int,4),
                                           new SqlParameter("@ButyDate",SqlDbType.VarChar,15)
                                      };
            parameters[0].Value = villageid;
            parameters[1].Value = butyDate.ToString("yyyy-MM-dd");
            DataSet ds = DbHelperSQL.Query(strSql.ToString(),parameters);
            sys_OnButysModel butyModel = new sys_OnButysModel();
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["OnbutyId"] != null && ds.Tables[0].Rows[0]["OnbutyId"].ToString() != "")
                {
                    butyModel.OnbutyId = int.Parse(ds.Tables[0].Rows[0]["OnbutyId"].ToString());
                }
                if (ds.Tables[0].Rows[0]["VillageId"] != null && ds.Tables[0].Rows[0]["VillageId"].ToString() != "")
                {
                    butyModel.VillageId = int.Parse(ds.Tables[0].Rows[0]["VillageId"].ToString());
                }
                if (ds.Tables[0].Rows[0]["UserId"] != null && ds.Tables[0].Rows[0]["UserId"].ToString() != "")
                {
                    butyModel.UserId = int.Parse(ds.Tables[0].Rows[0]["UserId"].ToString());
                }
                if (ds.Tables[0].Rows[0]["ButyDate"] != null && ds.Tables[0].Rows[0]["ButyDate"].ToString()!="")
                {
                    butyModel.ButyDate = DateTime.Parse(ds.Tables[0].Rows[0]["ButyDate"].ToString());
                }
                butyModel.Remarks = ds.Tables[0].Rows[0]["Remarks"].ToString();
            }
            return butyModel;
        }

        /// <summary>
        /// 根据日期查询，该天值班信息
        /// </summary>
        /// <param name="seachTime">查询日期</param>
        /// <returns></returns>
        public DataSet GetButyList(DateTime seachTime)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select OnbutyId,a.VillageId,a.UserId,ButyDate,Remarks,b.VillageName,c.UserName ");
            strSql.Append("from sys_OnButys a,sys_Villages b,sys_Users c ");
            strSql.Append(" where CONVERT(varchar(100),ButyDate,23) =@ButyDate");
            SqlParameter[] parameters ={
                                          new SqlParameter("@ButyDate",SqlDbType.VarChar,15)
                                      };
            parameters[0].Value = seachTime.ToString("yyyy-MM-dd");
            DataSet ds = DbHelperSQL.Query(strSql.ToString(), parameters);
            return ds;

        }

        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from v_OnButyUsers ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where "+strWhere);
            }
            recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
            return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount,pageSize,pageIndex,strSql.ToString(),filedOrder));
        }
        #endregion
    }
}
