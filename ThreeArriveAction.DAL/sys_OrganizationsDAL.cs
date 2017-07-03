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
    public class sys_OrganizationsDAL
    {
        public sys_OrganizationsDAL() { }
        #region 添加
        /// <summary>
        /// 添加组织等级
        /// </summary>
        /// <param name="organModel">组织等级实体</param>
        /// <returns></returns>
        public int AddOrganization(sys_OrganizationsModel organModel)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into sys_Organizations (OrganizationName,OrganizationState,Remarks) ");
            strSql.Append("values(@Organization,@OrganizationState,@Remarks)");
            SqlParameter[] parameter ={
                                          new SqlParameter("@Organization",SqlDbType.VarChar,100),
                                          new SqlParameter("@OrganizationState",SqlDbType.Int,4),
                                          new SqlParameter("@Remarks",SqlDbType.VarChar,500)
                                     };
            parameter[0].Value = organModel.OrganizationName;
            parameter[1].Value = organModel.OrganizationState;
            parameter[2].Value = organModel.Remarks;

            int number = DbHelperSQL.ExecuteSql(strSql.ToString(),parameter);
            return number;

        }
        #endregion

        #region 修改
        /// <summary>
        /// 根据编号修改等级信息
        /// </summary>
        /// <param name="organModel">组织等级信息</param>
        /// <returns></returns>
        public int UpdateOrganization(sys_OrganizationsModel organModel)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update sys_Organizations set OrganizationName=@OrganizationName,");
            strSql.Append("OrganizationState=@OrganizationState,Remarks=@Remarks ");
            strSql.Append(" where OrganizationId=@OrganizationId ");
            SqlParameter[] parameters ={
                                           new SqlParameter("@OrganizationName",SqlDbType.VarChar,100),
                                           new SqlParameter("@OrganizationState",SqlDbType.Int,4),
                                           new SqlParameter("@Remarks",SqlDbType.VarChar,500),
                                           new SqlParameter("@OrganizationId",SqlDbType.Int,4)
                                      };
            parameters[0].Value = organModel.OrganizationName;
            parameters[1].Value = organModel.OrganizationState;
            parameters[2].Value = organModel.Remarks;
            parameters[3].Value = organModel.OrganizationId;
            int numbers = DbHelperSQL.ExecuteSql(strSql.ToString(),parameters);
            return numbers;

        }
        #endregion

        #region 查询
        /// <summary>
        /// 根据编号，查询该组织的信息
        /// </summary>
        /// <param name="organId"></param>
        /// <returns></returns>
        public sys_OrganizationsModel GetOrganizationsModel(int organId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 OrganizationId,OrganizationName,OrganizationState,Remarks ");
            strSql.Append("from sys_Organizations where OrganizationId=@OrganizationId");
            SqlParameter[] parameters ={
                                           new SqlParameter("@OrganizationId",SqlDbType.Int,4)
                                       };
            parameters[0].Value = organId;
            sys_OrganizationsModel model = new sys_OrganizationsModel();
            DataSet ds = DbHelperSQL.Query(strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["OrganizationId"].ToString() != "")
                {
                    model.OrganizationId = int.Parse(ds.Tables[0].Rows[0]["OrganizationId"].ToString());
                }
                if (ds.Tables[0].Rows[0]["OrganizationName"] != null && ds.Tables[0].Rows[0]["OrganizationName"].ToString() != "")
                {
                    model.OrganizationName = ds.Tables[0].Rows[0]["OrganizationName"].ToString();
                }
                if (ds.Tables[0].Rows[0]["OrganizationState"].ToString() != "")
                {
                    model.OrganizationState = int.Parse(ds.Tables[0].Rows[0]["OrganizationState"].ToString());
                }
                if (ds.Tables[0].Rows[0]["Remarks"] != null && ds.Tables[0].Rows[0]["Remarks"].ToString() != "")
                {
                    model.Remarks = ds.Tables[0].Rows[0]["Remarks"].ToString();
                }
                return model;
            }
            else
            {
                return null;
            }
        }

        // <summary>
        /// 查询数据
        /// </summary>
        /// <param name="strWhere">查询条件</param>
        /// <returns></returns>
        public DataSet GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            
            strSql.Append(" OrganizationId,OrganizationName,OrganizationState,Remarks ");
            strSql.Append(" from sys_Organizations ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DbHelperSQL.Query(strSql.ToString());
        }

        /// <summary>
        /// 查询前几行数据
        /// </summary>
        /// <param name="top">条数</param>
        /// <param name="strWhere">查询条件</param>
        /// <param name="filedOrder">排序条件</param>
        /// <returns></returns>
        public DataSet GetList(int top, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (top > 0)
            {
                strSql.Append(" top "+top);
            }
            strSql.Append(" OrganizationId,OrganizationName,OrganizationState,Remarks ");
            strSql.Append(" from sys_Organizations ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where "+strWhere);
            }
            if (filedOrder.Trim() != "")
            {
                strSql.Append(" order by " + filedOrder);
            }
            else
            {
                strSql.Append(" order by OrganizationId asc ");
            }
            return DbHelperSQL.Query(strSql.ToString());

        }

        /// <summary>
        /// 获取查询分页数据
        /// </summary>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="pageIndex">当前页数</param>
        /// <param name="strWhere">查询条件</param>
        /// <param name="filedOrder">排序条件</param>
        /// <param name="recordCount">总记录数</param>
        /// <returns></returns>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from sys_Organizations ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where "+ strWhere );
            }
            recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
            return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount,pageSize,pageIndex,strSql.ToString(),filedOrder));
        }

        #endregion
    }
}
