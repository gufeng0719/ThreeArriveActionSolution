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
   public partial class sys_MajorInfoDAL
    {
        public sys_MajorInfoDAL() { }

        #region 添加
        public int AddMajor(sys_MajorInfoModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into sys_MajorInfo (MajorTitle,MajorContent,MajorFromUserId,MajorDate) ");
            strSql.Append(" Values (@MajorTitle,@MajorContent,@MajorFromUserId,@MajorDate )");
            SqlParameter[] parameters =
            {
                new SqlParameter("@MajorTitle",SqlDbType.NVarChar,200),
                new SqlParameter("@MajorContent",SqlDbType.Text,5000),
                new SqlParameter("@MajorFromUserId",SqlDbType.Int,4),
                new SqlParameter("@MajorDate",SqlDbType.DateTime)
            };
            parameters[0].Value = model.MajorTitle;
            parameters[1].Value = model.MajorContent;
            parameters[2].Value = model.MajorFromUserId;
            parameters[3].Value = model.MajorDate;
            int number = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            return number;
        }
        #endregion

        #region 修改
        public int UpdateMajor(sys_MajorInfoModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update sys_MajorInfo set MajorTitle=@MajorTitle,");
            strSql.Append("MajorContent=@MajorContent ");
            strSql.Append(" where MajorId=@MajorId ");
            SqlParameter[] parameters =
            {
                new SqlParameter("@MajorTitle",SqlDbType.NVarChar,200),
                new SqlParameter("@MajorContent",SqlDbType.Text,5000),
                new SqlParameter("@MajorId",SqlDbType.Int,4)
            };
            parameters[0].Value = model.MajorTitle;
            parameters[1].Value = model.MajorContent;
            parameters[2].Value = model.MajorId;
            int number = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            return number;
        }
        #endregion

        #region 删除
        public int DeleteMajor(int majorId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Delete from sys_MajorInfo where MajorId="+majorId);
            int number = DbHelperSQL.ExecuteSql(strSql.ToString());
            return number;
        }

        public int DeleteMajors(string ids)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Delete from sys_MajorInfo where MajorId in (" + ids + ")");
            int number = DbHelperSQL.ExecuteSql(strSql.ToString());
            return number;
        }
        #endregion

        #region 查询
        public sys_MajorInfoModel GetModel(int majorId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 MajorId,MajorTitle,MajorContent,MajorFromUserId,MajorDate ");
            strSql.Append(" from sys_MajorInfo where MajorId="+majorId);
            DataTable dt = DbHelperSQL.Query(strSql.ToString()).Tables[0];
            sys_MajorInfoModel model = new sys_MajorInfoModel();
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["MajorId"] != null && dt.Rows[0]["MajorId"].ToString() != "")
                {
                    model.MajorId = int.Parse(dt.Rows[0]["MajorId"].ToString());
                }
                if (dt.Rows[0]["MajorTitle"] != null)
                {
                    model.MajorTitle = dt.Rows[0]["MajorTitle"].ToString();
                }
                if (dt.Rows[0]["MajorContent"] != null)
                {
                    model.MajorContent = dt.Rows[0]["MajorContent"].ToString();
                }
                if (dt.Rows[0]["MajorDate"] != null && dt.Rows[0]["MajorDate"].ToString() != "")
                {
                    model.MajorDate = DateTime.Parse(dt.Rows[0]["MajorDate"].ToString());
                }
            }
            return model;
            
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="strWhere">查询条件</param>
        /// <param name="fieldOrder">排序条件</param>
        /// <param name="recordCount">返回总记录数</param>
        /// <returns></returns>
        public DataTable GetList(int pageSize,int pageIndex,string strWhere,string fieldOrder,out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select MajorId,MajorTitle,MajorContent,MajorFromUserId,MajorDate from sys_MajorInfo ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where "+strWhere);
            }
            recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
            DataTable dt = DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), fieldOrder)).Tables[0];
            return dt;
        }
        #endregion
    }
}
