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
    /// <summary>
    /// “组织等级对应系统菜单”数据访问类
    /// </summary>
    public class sys_OrganizationAndNavigationsDAL
    {
        public sys_OrganizationAndNavigationsDAL() { }

        #region 查询
        public List<sys_OrganizationANDNavigationsModel> GetOrganizationNavigation(int organizationId)
        {
            List<sys_OrganizationANDNavigationsModel> modelList = new List<sys_OrganizationANDNavigationsModel>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select Id,OrganizationId,NavigationId ");
            strSql.Append(" from sys_OrganizationANDNavigations ");
            strSql.Append(" where OrganizationId=@OrganizationId ");
            SqlParameter[] parameters ={
                                         new SqlParameter("@OrganizationId",SqlDbType.Int,4)
                                     };
            parameters[0].Value = organizationId;
            DataTable dt = DbHelperSQL.Query(strSql.ToString(), parameters).Tables[0];
            sys_OrganizationANDNavigationsModel model = new sys_OrganizationANDNavigationsModel();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    model = new sys_OrganizationANDNavigationsModel();
                    if (dr["Id"].ToString() != "") { model.Id = int.Parse(dr["Id"].ToString()); }
                    if (dr["OrganizationId"].ToString() != "")
                    {
                        model.OrganizationId = int.Parse(dr["OrganizationId"].ToString());
                    }
                    if (dr["NavigationId"].ToString() != "")
                    {
                        model.NavigationId = int.Parse(dr["NavigationId"].ToString());
                    }
                    modelList.Add(model);
                }
            }
            return modelList;

        }
        #endregion
    }
}
