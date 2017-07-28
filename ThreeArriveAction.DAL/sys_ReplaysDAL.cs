using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using ThreeArriveAction.Model;
using ThreeArriveAction.DBUtility;


namespace ThreeArriveAction.DAL
{
   public partial class sys_ReplaysDAL
    {
        public sys_ReplaysDAL() { }

        #region 添加回复
        public int AddReplay(sys_ReplaysModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into sys_Replays (InteractionId,ReplayContent,ReplayerId,RepalyDate,");
            strSql.Append("ReplayType,ReToplayerId ) values (@InteractionId,@ReplayContent,@ReplayerId,");
            strSql.Append("@ReplayDate,@ReplayType,@ReToplayerId )");
            SqlParameter[] parameter =
            {
                new SqlParameter("@InteractionId",SqlDbType.Int,4),
                new SqlParameter("@ReplayContent",SqlDbType.NVarChar,5000),
                new SqlParameter("@ReplayerId",SqlDbType.Int,4),
                new SqlParameter("@ReplayDate",SqlDbType.DateTime),
                new SqlParameter("@ReplayType",SqlDbType.Int,4),
                new SqlParameter("@ReToPlayerId",SqlDbType.Int,4)
            };
            parameter[0].Value = model.InteractionId;
            parameter[1].Value = model.ReplayContent;
            parameter[2].Value = model.ReplayerId;
            parameter[3].Value = model.RepalyDate;
            parameter[4].Value = 0;
            parameter[5].Value = model.ReToplayerId;
            int number = DbHelperSQL.ExecuteSql(strSql.ToString(), parameter);
            return number;
        }
        #endregion
    }
}
