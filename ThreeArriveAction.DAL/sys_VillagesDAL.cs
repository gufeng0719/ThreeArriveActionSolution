using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using ThreeArriveAction.DBUtility;
using ThreeArriveAction.Model;
using System.Collections;

namespace ThreeArriveAction.DAL
{
    /// <summary>
    /// 数据访问操作类:村居表
    /// </summary>
    public partial class sys_VillagesDAL
    {
        #region 添加
        #endregion

        #region 修改
        #endregion

        #region 删除
        #endregion

        #region 查询
        /// <summary>
        /// 根据村居编号查询该村居信息
        /// </summary>
        /// <param name="villageId">村居编号</param>
        /// <returns></returns>
        public sys_VillagesModel GetVillage(int villageId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 VillageId,VillageName,VillageParId,VillageGrage,Remarks ");
            strSql.Append("from sys_Villages where VillageId=@VillageId ");
            SqlParameter[] parameters ={
                                          new SqlParameter("@VillageId",SqlDbType.Int,4)
                                      };
            parameters[0].Value = villageId;
            DataSet ds = DbHelperSQL.Query(strSql.ToString(), parameters);
            sys_VillagesModel model = new sys_VillagesModel();
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["VillageId"] != null && ds.Tables[0].Rows[0]["VillageId"].ToString() != "")
                {
                    model.VillageId = int.Parse(ds.Tables[0].Rows[0]["VillageId"].ToString());
                }
                if (ds.Tables[0].Rows[0]["VillageName"].ToString() != "")
                {
                    model.VillageName = ds.Tables[0].Rows[0]["VillageName"].ToString();
                }
                if (ds.Tables[0].Rows[0]["VillageParId"] != null && ds.Tables[0].Rows[0]["VillageParId"].ToString() != "")
                {
                    model.VillageParId = int.Parse(ds.Tables[0].Rows[0]["VillageParId"].ToString());
                }
                if (ds.Tables[0].Rows[0]["VillageGrage"] != null && ds.Tables[0].Rows[0]["VillageGrage"].ToString() != "")
                {
                    model.VillageGrage = int.Parse(ds.Tables[0].Rows[0]["VillageGrage"].ToString());
                }
                if (ds.Tables[0].Rows[0]["Remarks"].ToString() != "")
                {
                    model.Remarks = ds.Tables[0].Rows[0]["Remarks"].ToString();
                }
                return model;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// 根据父级编号，查询所有子集村居
        /// </summary>
        /// <param name="parId">父级编号</param>
        /// <returns></returns>
        public DataTable GetVillageListByParId(int parId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select VillageId,VillageName,VillageParId,VillageGrage,Remarks ");
            strSql.Append("from sys_Villages ");
            strSql.Append("where VillageParId=@VillageParId ");
            SqlParameter[] parameter ={
                                          new SqlParameter("@VillageParId",SqlDbType.Int,4)
                                     };
            parameter[0].Value = parId;
            DataSet ds = DbHelperSQL.Query(strSql.ToString(), parameter);
            return ds.Tables[0];

        }

        /// <summary>
        /// 取得所有村居列表(已经排序好)
        /// </summary>
        /// <param name="parId"></param>
        /// <returns></returns>
        public DataTable GetList(int parId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" select VillageId,VillageName,VillageParId,VillageGrage,Remarks ");
            strSql.Append(" from sys_Villages order by VillageId asc ");
            DataSet ds = DbHelperSQL.Query(strSql.ToString());
            DataTable oldData = ds.Tables[0] as DataTable;
            if (oldData == null)
            {
                return null;
            }
            //复制结构
            DataTable newData = oldData.Clone();
            //调用
            GetChilds(oldData, newData, parId);
            return newData;
        }
        #endregion
        #region 私有方法==============
        /// <summary>
        /// 从内存中取得所有下级村居列表(自身迭代)
        /// </summary>
        /// <param name="oldData"></param>
        /// <param name="newData"></param>
        /// <param name="parId"></param>
        private void GetChilds(DataTable oldData,DataTable newData,int parId)
        {
            DataRow[] dr = oldData.Select("VillageParId="+parId);
            for (int i = 0; i < dr.Length; i++)
            {
                //添加一行数据
                DataRow row = newData.NewRow();
                row["VillageId"] = int.Parse(dr[i]["VillageId"].ToString());
                row["VillageName"] = dr[i]["VillageName"].ToString();
                row["VillageParId"] = int.Parse(dr[i]["VillageParId"].ToString());
                row["VillageGrage"] = int.Parse(dr[i]["VillageGrage"].ToString());
                row["Remarks"] = dr[i]["Remarks"].ToString();
                newData.Rows.Add(row);
                //调用自身迭代
                this.GetChilds(oldData,newData,int.Parse(dr[i]["VillageId"].ToString()));
            }
        }
        #endregion
    }
}
