using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreeArriveAction.Model
{
    #region 系统菜单列表=========================
    /// <summary>
    /// 菜单列表
    /// </summary>
    [Serializable]
    public partial class sys_NavigationsModel
    {
        public sys_NavigationsModel() { }
        private int navigationId;
        /// <summary>
        /// 菜单导航编号
        /// </summary>
        public int NavigationId
        {
            get { return navigationId; }
            set { navigationId = value; }
        }
        private string navigationName;
        /// <summary>
        /// 菜单导航名称
        /// </summary>
        public string NavigationName
        {
            get { return navigationName; }
            set { navigationName = value; }
        }
        private int parentId;
        /// <summary>
        /// 父级菜单编号
        /// </summary>
        public int ParentId
        {
            get { return parentId; }
            set { parentId = value; }
        }

        private string navIcon;
        /// <summary>
        /// 菜单图标
        /// </summary>
        public string NavIcon
        {
            get { return navIcon; }
            set { navIcon = value; }
        }
        private string navUrl;
        /// <summary>
        /// 菜单导航路径
        /// </summary>
        public string NavUrl
        {
            get { return navUrl; }
            set { navUrl = value; }
        }
        private int navState;
        /// <summary>
        /// 菜单状态
        /// </summary>
        public int NavState
        {
            get { return navState; }
            set { navState = value; }
        }
        private string reamrks;
        /// <summary>
        /// 备注
        /// </summary>
        public string Reamrks
        {
            get { return reamrks; }
            set { reamrks = value; }
        }

    }
    #endregion
}
