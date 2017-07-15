using System.Data;
using System.Data.SqlClient;
using ThreeArriveAction.DBUtility;
using ThreeArriveAction.Model;

namespace ThreeArriveAction.DAL
{
    public class sys_PublicMessagesDAL
    {

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
                new SqlParameter("@ImageUrl",SqlDbType.VarChar,200),
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
    }
}
