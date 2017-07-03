using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using ThreeArriveAction.DBUtility;
using ThreeArriveAction.Model;

namespace ThreeArriveAction.DAL
{
    /// <summary>
    /// 数据访问类：七户信息
    /// </summary>
    public partial class sys_SubscriberFamilyDAL
    {
        #region 添加
        #endregion

        #region 修改
        /// <summary>
        /// 根据户号编号，修改该户的坐标
        /// </summary>
        /// <param name="subscriberId">户号编号</param>
        /// <returns></returns>
        public int UpdateCoordinate(int subscriberId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update sys_SubscriberFamily set FamilyCoordinate=@FamilyCoordinate ");
            strSql.Append(" where SubscriberId=@SubscriberId ");
            SqlParameter[] parameter ={
                                          new SqlParameter("@SubscriberId",SqlDbType.Int,4)
                                     };
            parameter[0].Value = subscriberId;
            int number = DbHelperSQL.ExecuteSql(strSql.ToString(), parameter);
            return number;
        }
        #endregion

        #region 删除
        #endregion

        #region 查询
        /// <summary>
        /// 根据客户编号,查询该客户信息
        /// </summary>
        /// <param name="subId">客户编号</param>
        /// <returns></returns>
        public sys_SubscriberFamilyModel GetSubscriberFamilyBySubId(int subId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 SubscriberId,SubscriberName,SubscriberPhone,SubscriberType,");
            strSql.Append("FamilyAddress,FamilyCoordinate,FamilyNumber,VillageId,UserId,Remarks ");
            strSql.Append(" from sys_SubscriberFamily where SubscriberId=@SubscriberId  ");
            SqlParameter[] parameters ={
                                         new SqlParameter("@SubscriberId",SqlDbType.Int,4)
                                     };
            parameters[0].Value = subId;
            SqlDataReader reader = DbHelperSQL.ExecuteReader(strSql.ToString(), parameters);
            sys_SubscriberFamilyModel subModel = new sys_SubscriberFamilyModel();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    subModel.SubscriberId = reader.GetInt32(0);
                    subModel.SubscriberName = reader.GetString(1);
                    subModel.SubscriberPhone = reader.GetString(2);
                    subModel.SubscriberType = reader.GetInt32(3);
                    subModel.FamilyAddress = reader.GetString(4);
                    subModel.FamilyCoordinate = reader.GetString(5);
                    subModel.FamilyNumber = reader.GetInt32(6);
                    subModel.VillageId = reader.GetInt32(7);
                    subModel.UserId = reader.GetInt32(8);
                    subModel.Reamarks = reader.GetString(9);
                }
                reader.Close();
                reader.Dispose();
            }
            return subModel;
        }

        /// <summary>
        /// 根据用户编号，查询该用户所责任区七户信息
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <returns></returns>
        public List<sys_SubscriberFamilyModel> GetSubscriberFamilyByUserId(int userId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select SubscriberId,SubscriberName,SubscriberPhone,SubscriberType,");
            strSql.Append("FamilyAddress,FamilyCoordinate,FamilyNumber,VillageId,UserId,Remarks ");
            strSql.Append(" from sys_SubscriberFamily  where UserId=@UserId ");
            SqlParameter[] parameters ={
                                          new SqlParameter("@UserId",SqlDbType.Int,4)
                                      };
            parameters[0].Value = userId;
            List<sys_SubscriberFamilyModel> subList = new List<sys_SubscriberFamilyModel>();
            DataTable dt = DbHelperSQL.Query(strSql.ToString(),parameters).Tables[0];
            int rowscount = dt.Rows.Count;
            if (rowscount > 0)
            {
                foreach(DataRow dr in dt.Rows){
                    sys_SubscriberFamilyModel model = new sys_SubscriberFamilyModel();
                    if (dr["SubscriberId"] != null && dr["SubscriberId"].ToString() != "")
                    {
                        model.SubscriberId = int.Parse(dr["SubscriberId"].ToString());
                    }
                    if (dr["SubscriberName"].ToString() != "")
                    {
                        model.SubscriberName = dr["SubscriberName"].ToString();
                    }
                    if (dr["SubscriberPhone"].ToString() != "")
                    {
                        model.SubscriberPhone = dr["SubscriberPhone"].ToString();
                    }
                    if (dr["SubscriberType"] != null && dr["SubscriberType"].ToString() != "")
                    {
                        model.SubscriberType = int.Parse(dr["SubscriberType"].ToString());
                    }
                    if (dr["FamilyAddress"] != null)
                    {
                        model.FamilyAddress = dr["FamilyAddress"].ToString();
                    }
                    if (dr["FamilyCoordinate"] != null)
                    {
                        model.FamilyCoordinate = dr["FamilyCoordinate"].ToString();
                    }
                    if (dr["FamilyNumber"] != null && dr["FamilyNumber"].ToString() != "")
                    {
                        model.FamilyNumber = int.Parse(dr["FamilyNumber"].ToString());
                    }
                    if (dr["VillageId"] != null && dr["VillageId"].ToString() != "")
                    {
                        model.VillageId = int.Parse(dr["VillageId"].ToString());
                    }
                    if (dr["UserId"] != null && dr["UserId"].ToString() != "")
                    {
                        model.UserId = int.Parse(dr["UserId"].ToString());
                    }
                    if (dr["Remarks"] != null)
                    {
                        model.Reamarks = dr["Remarks"].ToString();
                    }
                    subList.Add(model);
                }
            }
            return subList;
        }

        /// <summary>
        /// 根据村居编号，查询该村七户信息
        /// </summary>
        /// <param name="villageId">村居编号</param>
        /// <returns></returns>
        public List<sys_SubscriberFamilyModel> GetSubscriberFamilyByVillageId(int villageId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select SubscriberId,SubscriberName,SubscriberPhone,SubscriberType,");
            strSql.Append("FamilyAddress,FamilyCoordinate,FamilyNumber,VillageId,UserId,Remarks ");
            strSql.Append(" from sys_SubscriberFamily  where VillageId=@VillageId ");
            SqlParameter[] parameters ={
                                          new SqlParameter("@VillageId",SqlDbType.Int,4)
                                      };
            parameters[0].Value = villageId;
            List<sys_SubscriberFamilyModel> subList = new List<sys_SubscriberFamilyModel>();
            DataTable dt = DbHelperSQL.Query(strSql.ToString(), parameters).Tables[0];
            int rowscount = dt.Rows.Count;
            if (rowscount > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    sys_SubscriberFamilyModel model = new sys_SubscriberFamilyModel();
                    if (dr["SubscriberId"] != null && dr["SubscriberId"].ToString() != "")
                    {
                        model.SubscriberId = int.Parse(dr["SubscriberId"].ToString());
                    }
                    if (dr["SubscriberName"].ToString() != "")
                    {
                        model.SubscriberName = dr["SubscriberName"].ToString();
                    }
                    if (dr["SubscriberPhone"].ToString() != "")
                    {
                        model.SubscriberPhone = dr["SubscriberPhone"].ToString();
                    }
                    if (dr["SubscriberType"] != null && dr["SubscriberType"].ToString() != "")
                    {
                        model.SubscriberType = int.Parse(dr["SubscriberType"].ToString());
                    }
                    if (dr["FamilyAddress"] != null)
                    {
                        model.FamilyAddress = dr["FamilyAddress"].ToString();
                    }
                    if (dr["FamilyCoordinate"] != null)
                    {
                        model.FamilyCoordinate = dr["FamilyCoordinate"].ToString();
                    }
                    if (dr["FamilyNumber"] != null && dr["FamilyNumber"].ToString() != "")
                    {
                        model.FamilyNumber = int.Parse(dr["FamilyNumber"].ToString());
                    }
                    if (dr["VillageId"] != null && dr["VillageId"].ToString() != "")
                    {
                        model.VillageId = int.Parse(dr["VillageId"].ToString());
                    }
                    if (dr["UserId"] != null && dr["UserId"].ToString() != "")
                    {
                        model.UserId = int.Parse(dr["UserId"].ToString());
                    }
                    if (dr["Remarks"] != null)
                    {
                        model.Reamarks = dr["Remarks"].ToString();
                    }
                    subList.Add(model);
                }
            }
            return subList;
        }

        #endregion
    }
}
