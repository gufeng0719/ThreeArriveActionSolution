using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using ThreeArriveAction.Model;
using ThreeArriveAction.DBUtility;
using ThreeArriveAction.Common;
namespace ThreeArriveAction.DAL
{
    /// <summary>
    /// 数据访问操作类:互动留言
    /// </summary>
    public partial class sys_LeaveMessageDAL
    {
        public sys_LeaveMessageDAL() { }

        #region 添加
        public int AddLeave(sys_LeaveMessageModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into sys_LeaveMessage (LeaveContent,LeaveImages,UserId,");
            strSql.Append("VillageId,LeaveDateTime,LeavePraiseNumber,LeavePraiseUserIds,LeaveState) ");
            strSql.Append("values(@LeaveContent,@LeaveImages,@UserId,@VillageId,@LeaveDateTime,@LeavePraiseNumber,");
            strSql.Append("@LeavePraiseUserIds,@LeaveState)");
            SqlParameter[] parameters =
            {
                new SqlParameter("@LeaveContent",SqlDbType.NVarChar,500),
                new SqlParameter("@LeaveImages",SqlDbType.NVarChar,500),
                new SqlParameter("@UserId",SqlDbType.Int,4),
                new SqlParameter("@VillageId",SqlDbType.Int,4),
                new SqlParameter("@LeaveDateTime",SqlDbType.DateTime),
                new SqlParameter("@LeavePraiseNumber",SqlDbType.Int),
                new SqlParameter("@LeavePraiseUserIds",SqlDbType.NVarChar,500),
                new SqlParameter("@LeaveState",SqlDbType.Int)
            };
            parameters[0].Value = model.LeaveContent;
            parameters[1].Value = model.LeaveImages;
            parameters[2].Value = model.UserId;
            parameters[3].Value = model.VillageId;
            parameters[4].Value = model.LeaveDateTime;
            parameters[5].Value = model.LeavePraiseNumber;
            parameters[6].Value = model.LeavePraiseUserIds;
            parameters[7].Value = model.LeaveState;
            int number = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            return number;
        }
        #endregion

        #region 修改
        public int UpdateLeave(sys_LeaveMessageModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Update sys_LeaveMessage set ");
            strSql.Append("LeaveContent=@LeaveContent,LeaveImages=@LeaveImages,UserId=@UserId,");
            strSql.Append("VillageId=@VillageId,LeaveDateTime=@LeaveDateTime,LeavePraiseNumber=@LeavePraiseNumber,");
            strSql.Append("LeavePraiseUserIds=@LeavePraiseUserIds,LeaveState=@LeaveState  ");
            strSql.Append(" where LeaveId=@LeaveId ");
            SqlParameter[] parameters =
            {
                new SqlParameter("@LeaveContent",SqlDbType.NVarChar,500),
                new SqlParameter("@LeaveImages",SqlDbType.NVarChar,500),
                new SqlParameter("@UserId",SqlDbType.Int,4),
                new SqlParameter("@VillageId",SqlDbType.Int,4),
                new SqlParameter("@LeaveDateTime",SqlDbType.DateTime),
                new SqlParameter("@LeavePraiseNumber",SqlDbType.Int),
                new SqlParameter("@LeavePraiseUserIds",SqlDbType.NVarChar,500),
                new SqlParameter("@LeaveState",SqlDbType.Int),
                new SqlParameter("@LeaveId",SqlDbType.Int)
            };
            parameters[0].Value = model.LeaveContent;
            parameters[1].Value = model.LeaveImages;
            parameters[2].Value = model.UserId;
            parameters[3].Value = model.VillageId;
            parameters[4].Value = model.LeaveDateTime;
            parameters[5].Value = model.LeavePraiseNumber;
            parameters[6].Value = model.LeavePraiseUserIds;
            parameters[7].Value = model.LeaveState;
            parameters[8].Value = model.LeaveId;
            int number = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            return number;
        }

        public int UpdateLeaveState(int id, int state)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Update sys_LeaveMessage set LeaveState=@LeaveState ");
            strSql.Append("where LeaveId=@LeaveId ");
            SqlParameter[] parameters =
            {
                new SqlParameter("@LeaveState",SqlDbType.Int,4),
                new SqlParameter("@LeaveId",SqlDbType.Int,4)
            };
            parameters[0].Value = state;
            parameters[1].Value = id;
            int number = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            return number;
        }

        public int PublishLeaveMessage(int id, int score)
        {
            sys_LeaveMessageModel model = GetModel(id);
            if (model != null)
            {
                List<CommandInfo> cmdList = new List<CommandInfo>();
                CommandInfo cmd1;
                //修改该留言状态为已审核
                StringBuilder strSql1 = new StringBuilder();
                strSql1.Append("update sys_LeaveMessage set LeaveState=1 where LeaveId=" + id);
                cmd1 = new CommandInfo(strSql1.ToString(), null);
                cmdList.Add(cmd1);
                //修改该用户积分
                //先查询出积分编号
                string strSql2 = "select top 1 IntegralId from sys_UserIntegrals where UserId=" + model.UserId
                        + " and IntegralYear=" + DateTime.Now.Year + " and IntegralMonth= " + DateTime.Now.Month;
                int integralId = Convert.ToInt32(DbHelperSQL.GetSingle(strSql2));
                if (integralId != 0)
                {
                    StringBuilder strSql3 = new StringBuilder();
                    strSql3.Append("update sys_UserIntegrals set IntegralScore=IntegralScore+ " + score);
                    strSql3.Append(" where IntegralId =" + integralId);
                    cmd1 = new CommandInfo(strSql3.ToString(), null);
                    cmdList.Add(cmd1);
                }
                else
                {
                    StringBuilder strSql5 = new StringBuilder();
                    strSql5.Append("insert into sys_UserIntegrals (UserId,IntegralScore,IntegralYear,IntegralMonth)");
                    strSql5.Append("values(@UserId,@IntegralScore,@IntegralYear,@IntegralMonth);set @ReturnValue=@@IDENTITY");
                    SqlParameter[] parameters5 = {
                            new SqlParameter("@UserId",SqlDbType.Int,4),
                            new SqlParameter("@IntegralScore",SqlDbType.Int,4),
                            new SqlParameter("@IntegralYear",SqlDbType.Int,4),
                            new SqlParameter("@IntegralMonth",SqlDbType.Int,4),
                            new SqlParameter("@ReturnValue",SqlDbType.Int,4)

                    };
                    parameters5[0].Value = model.UserId;
                    parameters5[1].Value = score;
                    parameters5[2].Value = DateTime.Now.Year;
                    parameters5[3].Value = DateTime.Now.Month;
                    parameters5[4].Direction = ParameterDirection.Output;
                    cmd1 = new CommandInfo(strSql5.ToString(), parameters5);
                    cmdList.Add(cmd1);
                }

                //添加积分详细信息
                StringBuilder strSql4 = new StringBuilder();
                strSql4.Append("insert into sys_IntegralInfo (IntegralId,UserId,IntegralType,Score,IntegralDate,");
                strSql4.Append("IntegralEventId) values(@IntegralId,@UserId,@IntegralType,@Score,@IntegralDate,@IntegralEventId )");
                SqlParameter[] parameter4 =
                {
                    new SqlParameter("@IntegralId",SqlDbType.Int,4),
                    new SqlParameter("@UserId",SqlDbType.Int,4),
                    new SqlParameter("@IntegralType",SqlDbType.Int,4),
                    new SqlParameter("@Score",SqlDbType.Int,4),
                    new SqlParameter("@IntegralDate",SqlDbType.DateTime),
                    new SqlParameter("@IntegralEventId",SqlDbType.Int,4)
                };
                if (integralId == 0)
                {
                    parameter4[0].Value = ParameterDirection.Output;
                }
                else
                {
                    parameter4[0].Value = integralId;
                }
                parameter4[1].Value = model.UserId;
                parameter4[2].Value = 4;
                parameter4[3].Value = score;
                parameter4[4].Value = DateTime.Now;
                parameter4[5].Value = model.LeaveId;
                cmd1 = new CommandInfo(strSql4.ToString(), parameter4);
                cmdList.Add(cmd1);
                int number = DbHelperSQL.ExecuteSqlTran(cmdList);
                return number;
            }
            else
            {
                return 0;
            }
        }

        public int SetPraiseNumber(int id, int userid)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update sys_LeaveMessage set LeavePraiseNumber=LeavePraiseNumber+1,");
            strSql.Append("LeavePraiseUserIds=LeavePraiseUserIds +'," + userid + "' where LeaveId=" + id);
            int number = DbHelperSQL.ExecuteSql(strSql.ToString());
            return number;
        }
        #endregion

        #region 删除
        public int DeleteLeave(int id)
        {
            List<string> sqlList = new List<string>();
            //删除留言
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Delete from sys_LeaveMessage where LeaveId=" + id);
            sqlList.Add(strSql.ToString());
            //删除回复表
            StringBuilder strSql2 = new StringBuilder();
            strSql2.Append("Delete from sys_Replays where InteractionId=" + id);
            sqlList.Add(strSql2.ToString());
            int number = DbHelperSQL.ExecuteSqlTran(sqlList);
            return number;
        }
        public int DeleteLeaves(string ids)
        {
            List<string> sqlList = new List<string>();
            //删除留言
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Delete from sys_LeaveMessage where LeaveId in (" + ids + ")");
            sqlList.Add(strSql.ToString());
            //删除留言的全部回复
            StringBuilder strSql2 = new StringBuilder();
            strSql2.Append("Delete from sys_Replays where InteractionId in (" + ids + ")");
            sqlList.Add(strSql2.ToString());
            int number = DbHelperSQL.ExecuteSqlTran(sqlList);
            return number;
        }
        #endregion

        #region 查询

        public sys_LeaveMessageModel GetModel(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 LeaveId,LeaveContent,LeaveImages,UserId,VillageId,");
            strSql.Append("LeaveDateTime,LeavePraiseNumber,LeavePraiseUserIds,LeaveState ");
            strSql.Append(" from sys_LeaveMessage where LeaveId=" + id);
            DataSet ds = DbHelperSQL.Query(strSql.ToString());
            sys_LeaveMessageModel model = new sys_LeaveMessageModel();
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["LeaveId"] != null && ds.Tables[0].Rows[0]["LeaveId"].ToString() != "")
                {
                    model.LeaveId = int.Parse(ds.Tables[0].Rows[0]["LeaveId"].ToString());
                }
                if (ds.Tables[0].Rows[0]["LeaveContent"] != null)
                {
                    model.LeaveContent = ds.Tables[0].Rows[0]["LeaveContent"].ToString();
                }
                if (ds.Tables[0].Rows[0]["LeaveImages"] != null)
                {
                    model.LeaveImages = ds.Tables[0].Rows[0]["LeaveImages"].ToString();
                }
                if (ds.Tables[0].Rows[0]["UserId"] != null && ds.Tables[0].Rows[0]["UserId"].ToString() != "")
                {
                    model.UserId = int.Parse(ds.Tables[0].Rows[0]["UserId"].ToString());
                }
                if (ds.Tables[0].Rows[0]["VillageId"] != null && ds.Tables[0].Rows[0]["VillageId"].ToString() != "")
                {
                    model.VillageId = int.Parse(ds.Tables[0].Rows[0]["VillageId"].ToString());
                }
                if (ds.Tables[0].Rows[0]["LeaveDateTime"] != null && ds.Tables[0].Rows[0]["LeaveDateTime"].ToString() != "")
                {
                    model.LeaveDateTime = DateTime.Parse(ds.Tables[0].Rows[0]["LeaveDateTime"].ToString());
                }
                if (ds.Tables[0].Rows[0]["LeavePraiseNumber"] != null && ds.Tables[0].Rows[0]["LeavePraiseNumber"].ToString() != "")
                {
                    model.LeavePraiseNumber = int.Parse(ds.Tables[0].Rows[0]["LeavePraiseNumber"].ToString());
                }
                if (ds.Tables[0].Rows[0]["LeavePraiseUserIds"] != null)
                {
                    model.LeavePraiseUserIds = ds.Tables[0].Rows[0]["LeavePraiseUserIds"].ToString();
                }
                if (ds.Tables[0].Rows[0]["LeaveState"] != null && ds.Tables[0].Rows[0]["LeaveState"].ToString() != "")
                {
                    model.LeaveState = int.Parse(ds.Tables[0].Rows[0]["LeaveState"].ToString());
                }
                return model;
            }
            else
            {
                return null;
            }

        }

        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string fieldOrder, out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select LeaveId,LeaveContent,LeaveImages,a.UserId,c.VillageId,Convert(varchar(100),LeaveDateTime,23) as LeaveDateTime,");
            strSql.Append("LeavePraiseNumber,LeavePraiseUserIds,LeaveState,b.UserName,c.VillageName,c.VillageParId from sys_LeaveMessage a ");
            strSql.Append("inner join sys_Users b on a.UserId=b.UserId ");
            strSql.Append("inner join sys_Villages c on a.VillageId = c.VillageId ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
            return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), fieldOrder));
        }
        #endregion
    }
}
