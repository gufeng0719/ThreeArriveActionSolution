using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThreeArriveAction.Model;
using ThreeArriveAction.DAL;

namespace ThreeArriveAction.BLL
{
    public class sys_OrganizationAndNavigationsBLL
    {
        private readonly sys_OrganizationAndNavigationsDAL organNavDAL = new sys_OrganizationAndNavigationsDAL();
        public sys_OrganizationAndNavigationsBLL() { }
        #region 查询
        /// <summary>
        /// 根据组织等级编号，查询所拥有的菜单
        /// </summary>
        /// <param name="organizationId">组织等级编号</param>
        /// <returns></returns>
        public List<sys_OrganizationANDNavigationsModel> GetOrganizationNavigation(int organizationId)
        {
            return organNavDAL.GetOrganizationNavigation(organizationId);
        }
        #endregion
    }
}
