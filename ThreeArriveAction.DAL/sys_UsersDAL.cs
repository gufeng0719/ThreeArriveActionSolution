using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThreeArriveAction.DBUtility;
using ThreeArriveAction.Model;
using System.Data.SqlClient;
using System.Data;
using ThreeArriveAction.Common;

namespace ThreeArriveAction.DAL
{
    /// <summary>
    /// 数据访问类:系统用户
    /// </summary>
    public partial class sys_UsersDAL
    {

        #region 基本方法
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int userId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from sys_Users");
            strSql.Append(" where UserId=@UserId ");
            SqlParameter[] parameters = {
					new SqlParameter("@UserId", SqlDbType.Int,4)};
            parameters[0].Value = userId;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 查询用户名是否存在
        /// </summary>
        public bool Exists(string userName)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from sys_Users");
            strSql.Append(" where UserName=@UserName ");
            SqlParameter[] parameters = {
					new SqlParameter("@UserName", SqlDbType.NVarChar,50)};
            parameters[0].Value = userName;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }
        /// <summary>
        /// 查询用户名是否存在
        /// </summary>
        public bool ExistsByPhone(string userPhone)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from sys_Users");
            strSql.Append(" where userPhone=@userPhone ");
            SqlParameter[] parameters = {
					new SqlParameter("@userPhone", SqlDbType.NVarChar,15)
                                        };
            parameters[0].Value = userPhone;
            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        #endregion  Method

        #region 添加用户
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int AddUser(sys_UsersModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into sys_Users(");
            strSql.Append("UserPhone,UserName,UserDuties,OrganizationId,VillageId,UserBirthday,UserPassword,UserState)");
            strSql.Append(" values (");
            strSql.Append("@UserPhone,@UserName,@UserDuties,@OrganizationId,@VillageId,@UserBirthday,@UserPassword,@UserState)");
            strSql.Append(";set @ReturnValue= @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@UserPhone", SqlDbType.VarChar,11),
					new SqlParameter("@UserName", SqlDbType.VarChar,10),
                    new SqlParameter("@UserDuties", SqlDbType.NVarChar,50),
					new SqlParameter("@OrganizationId", SqlDbType.Int,4),
					new SqlParameter("@VillageId", SqlDbType.Int,4),
					new SqlParameter("@UserBirthday", SqlDbType.NVarChar,50),
					new SqlParameter("@UserPassword", SqlDbType.NVarChar,20),
                    new SqlParameter("@UserState",SqlDbType.Int,4),
                    new SqlParameter("@ReturnValue",SqlDbType.Int)};
            parameters[0].Value = model.UserPhone;
            parameters[1].Value = model.UserName;
            parameters[2].Value = model.UserDuties;
            parameters[3].Value = model.OrganizationId;
            parameters[4].Value = model.VillageId;
            parameters[5].Value = model.UserBirthday;
            parameters[6].Value = model.UserPassword;
            parameters[7].Value = model.UserState;
            parameters[8].Direction = ParameterDirection.Output;

            List<CommandInfo> sqllist = new List<CommandInfo>();
            CommandInfo cmd = new CommandInfo(strSql.ToString(), parameters);
            sqllist.Add(cmd);

            //用户扩展信息
            if (model.UserInfoModel != null)
            {
                StringBuilder strSql2 = new StringBuilder();
                strSql2.Append("insert into sys_UsersInfo(");
                strSql2.Append("UserId,UserPhoto,UserUrl,PersonalIntroduction,PersonalHonor,");
                strSql2.Append("UserEducation,JoinPartyDate,UserRemarks)");
                strSql2.Append(" values (");
                strSql2.Append("@UserId,@UserPhoto,@UserUrl,@PersonalIntroduction,");
                strSql2.Append("@PersonalHonor,@UserEducation,@JoinPartyDate,@UserRemarks)");
                SqlParameter[] parameters2 = {
					    new SqlParameter("@UserId", SqlDbType.Int,4),
					    new SqlParameter("@UserPhoto", SqlDbType.NVarChar,50),
					    new SqlParameter("@UserUrl", SqlDbType.NVarChar,500),
                        new SqlParameter("@PersonalIntroduction",SqlDbType.NVarChar,5000),
                        new SqlParameter("@PersonalHonor",SqlDbType.NVarChar,5000),
                        new SqlParameter("@UserEducation",SqlDbType.NVarChar,100),
                        new SqlParameter("@JoinPartyDate",SqlDbType.VarChar,50),
					    new SqlParameter("@UserRemarks", SqlDbType.NVarChar,5000)
                                                 };
                parameters2[0].Direction = ParameterDirection.InputOutput;
                parameters2[1].Value = model.UserInfoModel.UserPhoto;
                parameters2[2].Value = model.UserInfoModel.UserUrl;
                parameters2[3].Value = model.UserInfoModel.PersonalIntroduction;
                parameters2[4].Value = model.UserInfoModel.PersonalHonor;
                parameters2[5].Value = model.UserInfoModel.UserEducation;
                parameters2[6].Value = model.UserInfoModel.JoinPartyDate;
                parameters2[7].Value = model.UserInfoModel.UserRemarks;
                cmd = new CommandInfo(strSql2.ToString(), parameters2);
                sqllist.Add(cmd);
            }
            DbHelperSQL.ExecuteSqlTranWithIndentity(sqllist);
            return (int)parameters[8].Value;

        }
        #endregion

        #region 修改用户
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(sys_UsersModel model)
        {
            List<CommandInfo> cmdList = new List<CommandInfo>();
            CommandInfo cmdInfo1 = new CommandInfo();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update sys_Users set ");
            strSql.Append("UserPhone=@UserPhone,");
            strSql.Append("UserName=@UserName,");
            strSql.Append("UserDuties=@UserDuties,");
            strSql.Append("OrganizationId=@OrganizationId,");
            strSql.Append("VillageId=@VillageId,");
            strSql.Append("UserBirthday=@UserBirthday,");
            strSql.Append("UserPassword=@UserPassword ");
            strSql.Append(" where UserId=@UserId");
            SqlParameter[] parameters = {
					new SqlParameter("@UserId", SqlDbType.Int,4),
					new SqlParameter("@UserPhone", SqlDbType.VarChar,11),
					new SqlParameter("@UserName", SqlDbType.VarChar,10),
					new SqlParameter("@UserDuties", SqlDbType.VarChar,50),
					new SqlParameter("@OrganizationId", SqlDbType.Int,4),
					new SqlParameter("@VillageId", SqlDbType.Int,4),
					new SqlParameter("@UserBirthday", SqlDbType.VarChar,50),
					new SqlParameter("@UserPassword", SqlDbType.VarChar,20)};
            parameters[0].Value = model.UserId;
            parameters[1].Value = model.UserPhone;
            parameters[2].Value = model.UserName;
            parameters[3].Value = model.UserDuties;
            parameters[4].Value = model.OrganizationId;
            parameters[5].Value = model.VillageId;
            parameters[6].Value = model.UserBirthday;
            parameters[7].Value = model.UserPassword;
            cmdInfo1.CommandText = strSql.ToString();
            cmdInfo1.Parameters = parameters;
            cmdList.Add(cmdInfo1);
            sys_UsersInfoDAL infoDAL = new sys_UsersInfoDAL();
            //已存在子表信息
            if (infoDAL.GetUsersInfoByUserId(model.UserId) != null)
            {
                CommandInfo cmdInfo2 = new CommandInfo();
                StringBuilder strSql2 = new StringBuilder();
                strSql2.Append("update sys_UsersInfo set ");
                strSql2.Append("UserPhoto =@UserPhoto,");
                strSql2.Append("UserUrl=@UserUrl,");
                strSql2.Append("PersonalIntroduction=@PersonalIntroduction,");
                strSql2.Append("PersonalHonor=@PersonalHonor,");
                strSql2.Append("UserEducation=@UserEducation,");
                strSql2.Append("JoinPartyDate=@JoinPartyDate,");
                strSql2.Append("UserRemarks=@UserRemarks");
                strSql2.Append(" where UserId=@UserId ");
                SqlParameter[] parameters2 ={
                                            new SqlParameter("@UserId",SqlDbType.Int,4),
                                            new SqlParameter("@UserPhoto",SqlDbType.VarChar,50),
                                            new SqlParameter("@UserUrl",SqlDbType.VarChar,500),
                                            new SqlParameter("@PersonalIntroduction",SqlDbType.VarChar,5000),
                                            new SqlParameter("@PersonalHonor",SqlDbType.VarChar,5000),
                                            new SqlParameter("@UserEducation",SqlDbType.VarChar,100),
                                            new SqlParameter("@JoinPartyDate",SqlDbType.VarChar,50),
                                            new SqlParameter("@UserRemarks",SqlDbType.VarChar,5000)
                                       };
                parameters2[0].Value = model.UserId;
                parameters2[1].Value = model.UserInfoModel.UserPhoto;
                parameters2[2].Value = model.UserInfoModel.UserUrl;
                parameters2[3].Value = model.UserInfoModel.PersonalIntroduction;
                parameters2[4].Value = model.UserInfoModel.PersonalHonor;
                parameters2[5].Value = model.UserInfoModel.UserEducation;
                parameters2[6].Value = model.UserInfoModel.JoinPartyDate;
                parameters2[7].Value = model.UserInfoModel.UserRemarks;
                cmdInfo2.CommandText = strSql2.ToString();
                cmdInfo2.Parameters = parameters2;
                cmdList.Add(cmdInfo2);
            }
            else
            {
                //不存在子表信息
                if (model.UserInfoModel != null)
                {
                    StringBuilder strSql3 = new StringBuilder();
                    strSql3.Append("insert into sys_UsersInfo(");
                    strSql3.Append("UserId,UserPhoto,UserUrl,PersonalIntroduction,PersonalHonor,");
                    strSql3.Append("UserEducation,JoinPartyDate,UserRemarks)");
                    strSql3.Append(" values (");
                    strSql3.Append("@UserId,@UserPhoto,@UserUrl,@PersonalIntroduction,");
                    strSql3.Append("@PersonalHonor,@UserEducation,@JoinPartyDate,@UserRemarks)");
                    SqlParameter[] parameters3 = {
					    new SqlParameter("@UserId", SqlDbType.Int,4),
					    new SqlParameter("@UserPhoto", SqlDbType.NVarChar,50),
					    new SqlParameter("@UserUrl", SqlDbType.NVarChar,500),
                        new SqlParameter("@PersonalIntroduction",SqlDbType.NVarChar,5000),
                        new SqlParameter("@PersonalHonor",SqlDbType.NVarChar,5000),
                        new SqlParameter("@UserEducation",SqlDbType.NVarChar,100),
                        new SqlParameter("@JoinPartyDate",SqlDbType.VarChar,50),
					    new SqlParameter("@UserRemarks", SqlDbType.NVarChar,5000)
                                                 };
                    parameters3[0].Value = model.UserId;
                    parameters3[1].Value = model.UserInfoModel.UserPhoto;
                    parameters3[2].Value = model.UserInfoModel.UserUrl;
                    parameters3[3].Value = model.UserInfoModel.PersonalIntroduction;
                    parameters3[4].Value = model.UserInfoModel.PersonalHonor;
                    parameters3[5].Value = model.UserInfoModel.UserEducation;
                    parameters3[6].Value = model.UserInfoModel.JoinPartyDate;
                    parameters3[7].Value = model.UserInfoModel.UserRemarks;
                   CommandInfo cmd3 = new CommandInfo(strSql3.ToString(), parameters3);
                   cmdList.Add(cmd3);
                }
            }
            int rows = DbHelperSQL.ExecuteSqlTran(cmdList);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 根据用户编号修改用户密码
        /// </summary>
        /// <param name="uid">用户编号</param>
        /// <param name="password">用户新密码</param>
        /// <returns></returns>
        public bool ChangePassword(int uid, string password)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update sys_Users set ");
            strSql.Append("UserPassword=@UserPassword ");
            strSql.Append(" where UserId=@UserId");
            SqlParameter[] parameters =
            {
                new SqlParameter("@UserPassword", SqlDbType.VarChar, 50),
                new SqlParameter("@UserId", SqlDbType.Int, 4)
            };
            parameters[0].Value = password;
            parameters[1].Value = uid;
            int rows = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion

        #region 删除用户
        /// <summary>
        /// 删除一条数据
        /// </summary>
        public int Delete(string userIds)
        {
            //删除附属数据
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update  sys_Users  set UserState=2");
            strSql.Append(" where UserId in ("+userIds+") ");
            int rows = DbHelperSQL.ExecuteSql(strSql.ToString());
            return rows;
        }
        #endregion

        #region 查询用户

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public sys_UsersModel GetModel(int userId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 UserId,UserPhone,UserName,UserDuties,OrganizationId,");
            strSql.Append("VillageId,UserBirthday,UserPassword,UserState,UserRemark from sys_Users ");
            strSql.Append(" where UserId=@UserId");
            SqlParameter[] parameters = {
					new SqlParameter("@UserId", SqlDbType.Int,4)};
            parameters[0].Value = userId;

            sys_UsersModel model = new sys_UsersModel();
            DataSet ds = DbHelperSQL.Query(strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                #region 用户主表数据
                if (ds.Tables[0].Rows[0]["UserId"].ToString() != "")
                {
                    model.UserId = int.Parse(ds.Tables[0].Rows[0]["UserId"].ToString());
                }
                model.UserPhone = ds.Tables[0].Rows[0]["UserPhone"].ToString();
                model.UserName = ds.Tables[0].Rows[0]["UserName"].ToString();
                model.UserDuties = ds.Tables[0].Rows[0]["UserDuties"].ToString();
                if (ds.Tables[0].Rows[0]["OrganizationId"].ToString() != "")
                {
                    model.OrganizationId = int.Parse(ds.Tables[0].Rows[0]["OrganizationId"].ToString());
                }
                if (ds.Tables[0].Rows[0]["VillageId"].ToString() != "")
                {
                    model.VillageId = int.Parse(ds.Tables[0].Rows[0]["VillageId"].ToString());
                }
                model.UserBirthday = ds.Tables[0].Rows[0]["UserBirthday"].ToString();
                model.UserPassword = ds.Tables[0].Rows[0]["UserPassword"].ToString();
                if (ds.Tables[0].Rows[0]["UserState"] != null && ds.Tables[0].Rows[0]["UserState"].ToString() != "")
                {
                    model.UserState = int.Parse(ds.Tables[0].Rows[0]["UserState"].ToString());
                }
                model.UserRemark = ds.Tables[0].Rows[0]["UserRemark"].ToString();
                #endregion

                return model;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据用户号码返回一个实体
        /// </summary>
        public sys_UsersModel GetModelByUserPhone(string userPhone)
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append("select UserId,UserPhone,UserName,UserDuties,OrganizationId,");
            strSql.Append(" VillageId,UserBirthday,UserPassword,UserState,UserRemark from sys_Users");
            strSql.Append(" where UserPhone=@UserPhone ");
            SqlParameter[] parameters = {
                    new SqlParameter("@UserPhone", SqlDbType.NVarChar,11)};
            parameters[0].Value = userPhone;
            SqlDataReader reader = DbHelperSQL.ExecuteReader(strSql.ToString(), parameters);
            sys_UsersModel userModel = new sys_UsersModel();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    userModel.UserId = reader.GetInt32(0);
                    userModel.UserPhone = reader.GetString(1);
                    userModel.UserName = reader.GetString(2);
                    userModel.UserDuties = reader.GetString(3);
                    userModel.OrganizationId = reader.GetInt32(4);
                    userModel.VillageId = reader.GetInt32(5);
                    try
                    {
                        userModel.UserBirthday = reader.GetString(6);
                    }
                    catch (Exception)
                    {
                        userModel.UserBirthday = "不详";
                    }
                    userModel.UserPassword = reader.GetString(7);
                    userModel.UserState = reader.GetInt32(8);
                    try
                    {
                        userModel.UserRemark = reader.GetString(9);
                    }
                    catch
                    {
                        userModel.UserRemark = null;
                    }
                }
                reader.Close();
                reader.Dispose();
                return userModel;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetList(int Top, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (Top > 0)
            {
                strSql.Append(" top " + Top.ToString());
            }
            strSql.Append(" UserId,UserPhone,UserName,UserDuties,OrganizationId,VillageId,UserBirthday,UserPassword,UserState,UserRemark ");
            strSql.Append(" FROM sys_Users ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where UserState =1 and " + strWhere);
            }
            if (filedOrder.Trim() != "")
            {
                strSql.Append(" order by " + filedOrder);
            }
            return DbHelperSQL.Query(strSql.ToString());
        }

        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from v_UsersInfo ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where UserState=1 and  " + strWhere);
            }
            recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
            return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }
        #endregion
    }
}
