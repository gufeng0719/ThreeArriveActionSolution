using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThreeArriveAction.Model;
using System.Data.SqlClient;
using System.Data;
using ThreeArriveAction.DBUtility;

namespace ThreeArriveAction.DAL
{
    /// <summary>
    /// 系统菜单数据访问类
    /// </summary>
   public  class sys_NavigationsDAL
    {
       public sys_NavigationsDAL() { }

        #region 添加
        #endregion
        #region 修改
        #endregion
        #region 删除
        #endregion
        #region 查询
       /// <summary>
       /// 根据导航编号查询导航信息
       /// </summary>
       /// <param name="navigationId">导航编号</param>
       /// <returns></returns>
       public sys_NavigationsModel GetNavigationsByNavId(int navigationId)
       {
           StringBuilder strSql = new StringBuilder();
           strSql.Append("select top 1 NavigationId,NavigationName,ParentId,NavIcon,NavUrl,NvaState,Remarks ");
           strSql.Append(" from sys_Navigations where NavigationId=@NavigationId ");
           SqlParameter[] parameters ={
                                          new SqlParameter("@NavigationId",SqlDbType.Int,4)
                                      };
           parameters[0].Value = navigationId;
           sys_NavigationsModel navigationsModel = new sys_NavigationsModel();
           DataSet ds = DbHelperSQL.Query(strSql.ToString(), parameters);
           if (ds.Tables[0].Rows.Count > 0)
           {
               if (ds.Tables[0].Rows[0]["NavigationId"].ToString() != "")
               {
                   navigationsModel.NavigationId = int.Parse(ds.Tables[0].Rows[0]["NavigationId"].ToString());
               }
               navigationsModel.NavigationName = ds.Tables[0].Rows[0]["NavigationName"].ToString();
               if (ds.Tables[0].Rows[0]["ParentId"].ToString() != "")
               {
                   navigationsModel.ParentId = int.Parse(ds.Tables[0].Rows[0]["ParentId"].ToString());
               }
               if (ds.Tables[0].Rows[0]["NavIcon"] != null)
               {
                   navigationsModel.NavIcon = ds.Tables[0].Rows[0]["NavIcon"].ToString();
               }
               if (ds.Tables[0].Rows[0]["NavUrl"] != null)
               {
                   navigationsModel.NavUrl = ds.Tables[0].Rows[0]["NavUrl"].ToString();
               }
               if (ds.Tables[0].Rows[0]["NavState"].ToString() != "")
               {
                   navigationsModel.NavState = int.Parse(ds.Tables[0].Rows[0]["NavState"].ToString());
               }
               navigationsModel.Reamrks = ds.Tables[0].Rows[0]["Remarks"].ToString();
               return navigationsModel;
           }
           else
           {
               return null;
           }

       }
       /// <summary>
       /// 根据父级编号查询所有子级导航
       /// </summary>
       /// <param name="parnetId"></param>
       /// <returns></returns>
       public List<sys_NavigationsModel> GetNavigationsByParentId(int parnetId)
       {
           List<sys_NavigationsModel> modelList = new List<sys_NavigationsModel>();
           StringBuilder strSql = new StringBuilder();
           strSql.Append("select NavigationId,NavigationName,ParentId,Navicon,NavUrl,NavState,Remarks ");
           strSql.Append(" from sys_Navigations ");
           strSql.Append(" where ParentId="+parnetId);
           DataTable dt = DbHelperSQL.Query(strSql.ToString()).Tables[0];
           int rowsCount = dt.Rows.Count;
           if (rowsCount > 0)
           {
               sys_NavigationsModel model;
               for (int n = 0; n < rowsCount; n++)
               {
                   model = new sys_NavigationsModel();
                   if (dt.Rows[n]["NavigationId"] != null && dt.Rows[n]["NavigationId"].ToString() != "")
                   {
                       model.NavigationId = int.Parse(dt.Rows[n]["NavigationId"].ToString());
                   }
                   if (dt.Rows[n]["NavigationName"] != null)
                   {
                       model.NavigationName = dt.Rows[n]["NavigationName"].ToString();
                   }
                   if (dt.Rows[n]["ParentId"] != null && dt.Rows[n]["ParentId"].ToString() != "")
                   {
                       model.ParentId = int.Parse(dt.Rows[n]["ParentId"].ToString());
                   }
                   if (dt.Rows[n]["NavIcon"] != null)
                   {
                       model.NavIcon = dt.Rows[n]["NavIcon"].ToString();
                   }
                   if (dt.Rows[n]["NavUrl"] != null)
                   {
                       model.NavUrl = dt.Rows[n]["NavUrl"].ToString();
                   }
                   if (dt.Rows[n]["NavState"] != null && dt.Rows[n]["NavState"].ToString() != "")
                   {
                       model.NavState = int.Parse(dt.Rows[n]["NavState"].ToString());
                   }
                   if (dt.Rows[n]["Remarks"] != null)
                   {
                       model.Reamrks = dt.Rows[n]["Remarks"].ToString();
                   }
                   modelList.Add(model);
               }
           }
           return modelList;
       }

       /// <summary>
       /// 取得所有类别列表
       /// </summary>
       /// <param name="parentId">父级编号</param>
       /// <returns></returns>
       public DataTable GetDataList(int parentId)
       {
           StringBuilder strSql = new StringBuilder();
           strSql.Append("select NavigationId,NavigationName,ParentId,NavIcon,NavUrl,NavState ");
           strSql.Append(" from sys_Navigations ");
           strSql.Append(" where NavState=1 ");
           DataSet ds = DbHelperSQL.Query(strSql.ToString());
           DataTable oldData = ds.Tables[0] as DataTable;
           if (oldData == null)
           {
               return null;
           }
           //复制结构
           DataTable newData = oldData.Clone();
           //调用
           GetChilds(oldData, newData, parentId);
           return newData;
       }

       /// <summary>
       /// 根据组织等级编号获取该等级所拥有的权限
       /// </summary>
       /// <param name="organizationId">组织等级</param>
       /// <returns></returns>
       public List<sys_NavigationsModel> GetNavigationsByOrganization(int organizationId)
       {
           List<sys_NavigationsModel> modelList = new List<sys_NavigationsModel>();
           StringBuilder strSql = new StringBuilder();
           strSql.Append("select NavigationId,NavigationName,ParentId,NavIcon,NavState,Remarks,OrganizationId ");
           strSql.Append(" from  v_OrgAndNavigations ");
           strSql.Append(" where OrganizationId=" + organizationId);
           DataTable dt = DbHelperSQL.Query(strSql.ToString()).Tables[0];
           int rowsCount = dt.Rows.Count;
           if (rowsCount > 0)
           {
               sys_NavigationsModel model;
               for (int n = 0; n < rowsCount; n++)
               {
                   model = new sys_NavigationsModel();
                   if (dt.Rows[n]["NavigationId"] != null && dt.Rows[n]["NavigationId"].ToString() != "")
                   {
                       model.NavigationId = int.Parse(dt.Rows[n]["NavigationId"].ToString());
                   }
                   if (dt.Rows[n]["NavigationName"] != null)
                   {
                       model.NavigationName = dt.Rows[n]["NavigationName"].ToString();
                   }
                   if (dt.Rows[n]["ParentId"] != null && dt.Rows[n]["ParentId"].ToString() != "")
                   {
                       model.ParentId = int.Parse(dt.Rows[n]["ParentId"].ToString());
                   }
                   if (dt.Rows[n]["NavIcon"] != null)
                   {
                       model.NavIcon = dt.Rows[n]["NavIcon"].ToString();
                   }
                   if (dt.Rows[n]["NavUrl"] != null)
                   {
                       model.NavUrl = dt.Rows[n]["NavUrl"].ToString();
                   }
                   if (dt.Rows[n]["NavState"] != null && dt.Rows[n]["NavState"].ToString() != "")
                   {
                       model.NavState = int.Parse(dt.Rows[n]["NavState"].ToString());
                   }
                   if (dt.Rows[n]["Remarks"] != null)
                   {
                       model.Reamrks = dt.Rows[n]["Remarks"].ToString();
                   }
                   modelList.Add(model);
               }
           }
           return modelList;
       }

        #endregion
        #region 私有方法=====
       /// <summary>
       /// 从内存中取得所有下级导航列表(自身迭代)
       /// </summary>
       /// <param name="oldData"></param>
       /// <param name="newData"></param>
       /// <param name="parId"></param>
       private void GetChilds(DataTable oldData, DataTable newData, int parId)
       {
           DataRow[] dr = oldData.Select("VillageParId=" + parId);
           for (int i = 0; i < dr.Length; i++)
           {
               //添加一行数据
               DataRow row = newData.NewRow();
               row["NavgationId"] = int.Parse(dr[i]["NavgationId"].ToString());
               row["NavgationName"] = dr[i]["NavgationName"].ToString();
               row["ParentId"] = int.Parse(dr[i]["ParentId"].ToString());
               row["Navicon"] = dr[i]["Navicon"].ToString();
               row["NavUrl"] = dr[i]["NavUrl"].ToString();
               row["NavState"] = dr[i]["NavState"];
               row["Remarks"] = dr[i]["Remarks"];
               newData.Rows.Add(row);
               //调用自身迭代
               this.GetChilds(oldData, newData, int.Parse(dr[i]["ParentId"].ToString()));
           }
       }
        #endregion
    }
}
