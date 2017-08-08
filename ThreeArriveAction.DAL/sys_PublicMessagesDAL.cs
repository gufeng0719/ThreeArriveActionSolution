using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using ThreeArriveAction.Common;
using ThreeArriveAction.DBUtility;
using ThreeArriveAction.Model;

namespace ThreeArriveAction.DAL
{
    public class sys_PublicMessagesDAL
    {

        #region 添加公开信息
        public int Add(sys_PublicMessagesModel model)
        {
            var sql =
                "INSERT INTO ThreeArriveAction.dbo.sys_PublicMessages (VillageId,PublishDate,PublicType,ThumbnailUrl,ImageUrl,UserId,Remarks) " +
                "VALUES (@VillageId,@PublishDate,@PublicType,@ThumbnailUrl,@ImageUrl,@UserId,@Remarks)";
            SqlParameter[] parameters ={
                new SqlParameter("@VillageId",SqlDbType.Int),
                new SqlParameter("@PublishDate",SqlDbType.DateTime),
                new SqlParameter("@PublicType",SqlDbType.Int),
                new SqlParameter("@ThumbnailUrl",SqlDbType.VarChar,100),
                new SqlParameter("@ImageUrl",SqlDbType.VarChar,5000),
                new SqlParameter("@UserId",SqlDbType.Int),
                new SqlParameter("@Remarks",SqlDbType.VarChar,500),
            };
            parameters[0].Value = model.VillageId;
            parameters[1].Value = model.PublishDate;
            parameters[2].Value = model.PublicType;
            parameters[3].Value = model.ThumbnailUrl;
            parameters[4].Value = model.ImageUrl;
            parameters[5].Value = model.UserId;
            parameters[6].Value = model.Remarks;
            return DbHelperSQL.ExecuteSql(sql, parameters);

        }
        #endregion

        #region 删除公开信息
        public int DeletePublicMessage(int publicId)
        {
            string strSql = "Delete from sys_PublicMessages where PublicId="+publicId;
            int number = DbHelperSQL.ExecuteSql(strSql);
            return number;
        }

        public int DeletePublicMessage(string ids)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Delete from sys_PublicMessages where PublicId in ("+ids+")");
            int number = DbHelperSQL.ExecuteSql(strSql.ToString());
            return number;
        }
        #endregion

        #region 查询

        public sys_PublicMessagesModel GetModel(int publicId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 * from sys_PublicMessages ");
            strSql.Append(" where PublicId ="+publicId);
            sys_PublicMessagesModel model = new sys_PublicMessagesModel();
            DataTable dt = DbHelperSQL.Query(strSql.ToString()).Tables[0];
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["PublicId"] != null && dt.Rows[0]["PublicId"].ToString() != "")
                {
                    model.PublicId = int.Parse(dt.Rows[0]["PublicId"].ToString());
                }
                if (dt.Rows[0]["VillageId"] != null && dt.Rows[0]["VillageId"].ToString() != "")
                {
                    model.VillageId = int.Parse(dt.Rows[0]["VillageId"].ToString());
                }
                if (dt.Rows[0]["PublishDate"] != null && dt.Rows[0]["PublishDate"].ToString() != "")
                {
                    model.PublishDate = DateTime.Parse(dt.Rows[0]["PublishDate"].ToString());
                }
                if (dt.Rows[0]["PublicType"] != null && dt.Rows[0]["PublicType"].ToString() != "")
                {
                    model.PublicType = int.Parse(dt.Rows[0]["PublicType"].ToString());
                }
                model.ThumbnailUrl = dt.Rows[0]["ThumbnailUrl"].ToString();
                model.ImageUrl = dt.Rows[0]["ImageUrl"].ToString();
                if (dt.Rows[0]["UserId"] != null && dt.Rows[0]["UserId"].ToString() != "")
                {
                    model.UserId = int.Parse(dt.Rows[0]["UserId"].ToString());
                }
                model.Remarks = dt.Rows[0]["Remarks"].ToString();
                return model;
            }
            else
            {
                return null;
            }
            
        }

        /// <summary>
        /// 分页查询公开信息
        /// </summary>
        /// <param name="pageSize">每页条数</param>
        /// <param name="pageIndex">当前页码数</param>
        /// <param name="strWhere">查询条件</param>
        /// <param name="filedOrder">排序条件</param>
        /// <param name="recordCount">总记录数</param>
        /// <returns></returns>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select a.*,b.VillageName,c.VillageName as TownName from sys_PublicMessages a inner join sys_Villages b");
            strSql.Append(" on a.VillageId=b.VillageId inner join sys_Villages c on b.VillageParId=c.VillageId ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
            return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }
        #endregion
    }
}
