using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThreeArriveAction.DAL;
using ThreeArriveAction.Common;
using ThreeArriveAction.Model;
using System.Data;

namespace ThreeArriveAction.BLL
{
   public partial class sys_NavigationsBLL
    {
       private readonly sys_NavigationsDAL navDAL = new sys_NavigationsDAL();
        #region 增加
        #endregion
        #region 修改
        #endregion
        #region 删除
        #endregion
        #region 查询
       /// <summary>
       /// 根据菜单编号，查询该菜单信息
       /// </summary>
       /// <param name="navigationId">菜单编号</param>
       /// <returns></returns>
       public sys_NavigationsModel GetNavigationsByNavId(int navigationId)
       {
           sys_NavigationsModel model = navDAL.GetNavigationsByNavId(navigationId);
           return model;
       }
       /// <summary>
       /// 根据父级编号，查询所有子级菜单
       /// </summary>
       /// <param name="parnetId">父级编号</param>
       /// <returns></returns>
       public List<sys_NavigationsModel> GetNavigationsByParentId(int parnetId)
       {
           List<sys_NavigationsModel> modelList = navDAL.GetNavigationsByParentId(parnetId);
           return modelList;
       }
       /// <summary>
       /// 根据组织编号，查询该等级所拥有的菜单
       /// </summary>
       /// <param name="organizationId">组织等级编号</param>
       /// <returns></returns>
       public List<sys_NavigationsModel> GetNavigationsByOrganization(int organizationId)
       {
           List<sys_NavigationsModel> modelList = navDAL.GetNavigationsByOrganization(organizationId);
           return modelList;
       }

       public DataTable GetDataList(int parentId)
       {
           return navDAL.GetDataList(parentId);
       }
        #endregion
    }
}
