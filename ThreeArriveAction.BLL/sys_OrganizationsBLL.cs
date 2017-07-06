using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ThreeArriveAction.Common;
using ThreeArriveAction.DAL;
using ThreeArriveAction.Model;
namespace ThreeArriveAction.BLL
{
    /// <summary>
    /// 业务逻辑类:组织等级操作
    /// </summary>
    public class sys_OrganizationsBLL
    {
        private readonly sys_OrganizationsDAL organDAL = new sys_OrganizationsDAL();
        #region 添加
        /// <summary>
        /// 添加组织等级
        /// </summary>
        /// <param name="organModel">组织等级实体</param>
        /// <returns></returns>
        public string AddOrganization(sys_OrganizationsModel organModel)
        {
            int number = organDAL.AddOrganization(organModel);
            if (number > 0)
            {
                return "{\"info\":\"组织角色添加成功\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"组织角色添加失败\",\"status\":\"n\"}";
            }
        }
        #endregion

        #region 修改
        /// <summary>
        /// 根据编号修改等级信息
        /// </summary>
        /// <param name="organModel">组织等级信息</param>
        /// <returns></returns>
        public string UpdateOrganization(sys_OrganizationsModel organModel)
        {
            int number = organDAL.UpdateOrganization(organModel);
            if (number > 0)
            {
                return "{\"info\":\"组织角色修改成功\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"组织角色修改失败\",\"status\":\"n\"}";
            }
        }
        #endregion

        #region 查询
        /// <summary>
        /// 根据编号，查询该组织的信息
        /// </summary>
        /// <param name="organId">组织等级编号</param>
        /// <returns></returns>
        public string GetOrganization(int organId)
        {
            sys_OrganizationsModel model= organDAL.GetOrganizationsModel(organId);
            if (model != null)
            {
                sys_OrganizationAndNavigationsBLL organNavBLL = new sys_OrganizationAndNavigationsBLL();
                string navArr = organNavBLL.GetNavigationIdsByOrganizationId(model.OrganizationId);
                StringBuilder strJson = new StringBuilder();
                strJson.Append("{\"orgModel\":[");
                strJson.Append(JsonHelper.ToJson(model));
                strJson.Append("],\"navArr\":["+navArr+"]");
                strJson.Append("}");
                return strJson.ToString();
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="strWhere">查询条件(不需要where关键字)</param>
        /// <returns></returns>
        public DataSet GetList(string strWhere)
        {
            return organDAL.GetList(strWhere);
        }

        public string GetListJson(string strWhere)
        {
            StringBuilder strJson = new StringBuilder();
            DataTable dt = GetList(strWhere).Tables[0];
            strJson.Append("{\"total\":");
            if (dt.Rows.Count > 0)
            {
                
                strJson.Append(dt.Rows.Count + ",\"rows\":");
                strJson.Append(JsonHelper.ToJson(dt));
            }
            else
            {
                strJson.Append("0");
            }
            strJson.Append("}");
            return strJson.ToString();
        }
        /// <summary>
        /// 查询前几行数据
        /// </summary>
        /// <param name="top">条数</param>
        /// <param name="strWhere">查询条件</param>
        /// <param name="filedOrder">排序条件</param>
        /// <returns></returns>
        public DataSet GetList(int top, string strWhere, string filedOrder)
        {
            return organDAL.GetList(top, strWhere, filedOrder);
        }

        /// <summary>
        /// 获取查询分页数据
        /// </summary>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="pageIndex">当前页数</param>
        /// <param name="strWhere">查询条件</param>
        /// <param name="filedOrder">排序条件</param>
        /// <param name="recordCount">总记录数</param>
        /// <returns></returns>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return organDAL.GetList(pageSize,pageIndex,strWhere,filedOrder,out recordCount);
        }

        #endregion
    }
}
