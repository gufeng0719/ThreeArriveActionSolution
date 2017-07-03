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
    /// 数据访问类:用户额外信息
    /// </summary>
   public class sys_UsersInfoDAL
    {
       public sys_UsersInfoModel GetUsersInfoByUserId(int UserId)
       {
           StringBuilder strSql = new StringBuilder();
           strSql.Append("select top 1 InfoId,UserId,UserPhoto,UserUrl,PersonalIntroduction,PersonalHonor, ");
           strSql.Append("UserEducation,JoinPartyDate,UserRemarks ");
           strSql.Append(" where UserId=@UserId ");
           SqlParameter[] parameters ={
                                         new SqlParameter("@UserId",SqlDbType.Int,4)
                                     };
           parameters[0].Value = UserId;
           sys_UsersInfoModel usersInfoModel = new sys_UsersInfoModel();
           SqlDataReader reader = DbHelperSQL.ExecuteReader(strSql.ToString(), parameters);
           if (reader.HasRows)
           {
               while (reader.Read())
               {
                   usersInfoModel.InfoId = reader.GetInt32(0);
                   usersInfoModel.UserId = reader.GetInt32(1);
                   try
                   {
                       usersInfoModel.UserPhoto = reader.GetString(2);
                   }
                   catch
                   {
                       usersInfoModel.UserPhoto = null;
                   }

                   try
                   {
                       usersInfoModel.UserUrl = reader.GetString(3);
                   }
                   catch
                   {
                       usersInfoModel.UserUrl = null;
                   }
                   try
                   {
                       usersInfoModel.PersonalIntroduction = reader.GetString(4);
                   }
                   catch
                   {
                       usersInfoModel.PersonalIntroduction = null;
                   }
                   try
                   {
                       usersInfoModel.PersonalHonor = reader.GetString(5);
                   }
                   catch
                   {
                       usersInfoModel.PersonalHonor = null;
                   }
                   try
                   {
                       usersInfoModel.UserEducation = reader.GetString(6);
                   }
                   catch
                   {
                       usersInfoModel.UserEducation = null;
                   }
                   try
                   {
                       usersInfoModel.JoinPartyDate = reader.GetString(7);
                   }
                   catch
                   {
                       usersInfoModel.JoinPartyDate = null;
                   }
                   try
                   {
                       usersInfoModel.UserRemarks = reader.GetString(8);
                   }
                   catch
                   {
                       usersInfoModel.UserRemarks = null;
                   }
               }
               reader.Close();
               reader.Dispose();
               return usersInfoModel;
           }
           else
           {
               return null;
           }
       }
    }
}
