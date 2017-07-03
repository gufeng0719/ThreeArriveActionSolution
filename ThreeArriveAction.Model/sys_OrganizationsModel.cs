using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreeArriveAction.Model
{
    #region 组织机构等级=======================
    /// <summary>
    /// 组织机构等级类
    /// </summary>
    public partial class sys_OrganizationsModel
    {
        public sys_OrganizationsModel() { }
        private int organizationId;
        /// <summary>
        /// 组织机构等级编号
        /// </summary>
        public int OrganizationId
        {
            get { return organizationId; }
            set { organizationId = value; }
        }
        private string organizationName;
        /// <summary>
        /// 组织机构等级名称
        /// </summary>
        public string OrganizationName
        {
            get { return organizationName; }
            set { organizationName = value; }
        }
        private int organizationState;
        /// <summary>
        /// 组织机构状态
        /// </summary>
        public int OrganizationState
        {
            get { return organizationState; }
            set { organizationState = value; }
        }
        private string remarks;
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks
        {
            get { return remarks; }
            set { remarks = value; }
        }

        private List<sys_OrganizationANDNavigationsModel> orgAndNavs;

        /// <summary>
        /// 所拥有菜单列表
        /// </summary>
        public List<sys_OrganizationANDNavigationsModel> OrgAndNavs
        {
            get { return orgAndNavs; }
            set { orgAndNavs = value; }
        }
    }
    #endregion
}
