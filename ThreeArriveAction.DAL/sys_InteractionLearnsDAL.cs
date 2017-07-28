using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ThreeArriveAction.DBUtility;
using ThreeArriveAction.Common;
using ThreeArriveAction.Model;
using System.Data.SqlClient;

namespace ThreeArriveAction.DAL
{
   public partial class sys_InteractionLearnsDAL
    {
        public sys_InteractionLearnsDAL() { }

        #region 添加
        public int AddLearns(sys_InteractionLearnsModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into sys_InteractionLearns (LearnType,LearnTitle,LearnContent,");
            strSql.Append("Publisher,PublishDate,Remarks) values (@LearnType,@LearnTitle,");
            strSql.Append("@LearnContent,@Publisher,@publishDate,@Remarks)");
            SqlParameter[] parameters =
            {
                new SqlParameter("@LearnType",SqlDbType.Int,4),
                new SqlParameter("@LearnTitle",SqlDbType.VarChar,500),
                new SqlParameter("@LearnContent",SqlDbType.NVarChar,5000),
                new SqlParameter("@Publisher",SqlDbType.Int,4),
                new SqlParameter("@PublishDate",SqlDbType.DateTime),
                new SqlParameter("@Remarks",SqlDbType.VarChar,500)
            };
            parameters[0].Value = model.LearnType;
            parameters[1].Value = model.LearnTitle;
            parameters[2].Value = model.LearnContent;
            parameters[3].Value = model.Publisher;
            parameters[4].Value = model.PublishDate;
            parameters[5].Value = model.Remarks;
            int number = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            return number;
        }
        #endregion

        #region 修改
        public int UpdateLearn(sys_InteractionLearnsModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update sys_InteractionLearns set ");
            strSql.Append("LearnType=@LearnType,");
            strSql.Append("LearnTitle=@LearnTitle,");
            strSql.Append("LearnContent=@LearnContent,");
            strSql.Append("Publisher=@Publisher,");
            strSql.Append("PublishDate=@PublishDate,");
            strSql.Append("Remarks=@Remarks ");
            strSql.Append(" where LearnId=@LearnId "); SqlParameter[] parameters =
             {
                new SqlParameter("@LearnType",SqlDbType.Int,4),
                new SqlParameter("@LearnTitle",SqlDbType.VarChar,500),
                new SqlParameter("@LearnContent",SqlDbType.NVarChar,5000),
                new SqlParameter("@Publisher",SqlDbType.Int,4),
                new SqlParameter("@PublishDate",SqlDbType.DateTime),
                new SqlParameter("@Remarks",SqlDbType.VarChar,500),
                new  SqlParameter("@LearnId",SqlDbType.Int,4)
            };
            parameters[0].Value = model.LearnType;
            parameters[1].Value = model.LearnTitle;
            parameters[2].Value = model.LearnContent;
            parameters[3].Value = model.Publisher;
            parameters[4].Value = model.PublishDate;
            parameters[5].Value = model.Remarks;
            parameters[6].Value = model.LearnId;
            int number = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            return number;

        }
        #endregion

        #region
        public int DeleteLearns(string ids)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Delete sys_InteractionLearns where LearnId in ("+ids+")");
            int number = DbHelperSQL.ExecuteSql(strSql.ToString());
            return number;

        }
        #endregion

        #region 查询
        /// <summary>
        /// 分页查询在线学习
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="strWhere"></param>
        /// <param name="fieldOrder"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataSet GetList(int pageSize,int pageIndex,string strWhere,string fieldOrder,out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select LearnId,LearnType,LearnTitle,LearnContent,Publisher,Convert(varchar(100),PublishDate,23) as PublishDate,");
            strSql.Append("Remarks,UserName from sys_InteractionLearns a inner join sys_Users b on a.Publisher =b.UserId ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where "+strWhere);
            }
            recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
            return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount,pageSize,pageIndex,strSql.ToString(),fieldOrder));
        }
        #endregion
    }
}
