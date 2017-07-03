using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreeArriveAction.Model
{
    #region
    public partial class sys_OrganizationANDNavigationsModel
    {
        public sys_OrganizationANDNavigationsModel() { }
        private int id;
        /// <summary>
        /// 编号
        /// </summary>
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        private int organizationId;
        /// <summary>
        /// 组织结构编号
        /// </summary>
        public int OrganizationId
        {
            get { return organizationId; }
            set { organizationId = value; }
        }
        private int navigationId;
        /// <summary>
        /// 菜单编号
        /// </summary>
        public int NavigationId
        {
            get { return navigationId; }
            set { navigationId = value; }
        }
    }
    #endregion
}
