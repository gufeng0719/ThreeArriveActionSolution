using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreeArriveAction.Model
{
    #region 微信菜单表
    /// <summary>
    /// 微信菜单类
    /// </summary>
    [Serializable]
    public partial class wx_MenusModel
    {
        public wx_MenusModel() { }
        private int menuId;
        /// <summary>
        /// 菜单编号
        /// </summary>
        public int MenuId
        {
            get { return menuId; }
            set { menuId = value; }
        }
        private string menuName;
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName
        {
            get { return menuName; }
            set { menuName = value; }
        }
        private string menuKey;
        /// <summary>
        /// 菜单关键字
        /// </summary>
        public string MenuKey
        {
            get { return menuKey; }
            set { menuKey = value; }
        }
        private string menuUrl;
        /// <summary>
        /// 菜单路径
        /// </summary>
        public string MenuUrl
        {
            get { return menuUrl; }
            set { menuUrl = value; }
        }
        private int menuPId;
        /// <summary>
        /// 父级菜单编号
        /// </summary>
        public int MenuPId
        {
            get { return menuPId; }
            set { menuPId = value; }
        }
        private string menuType;
        /// <summary>
        /// 菜单类型
        /// </summary>
        public string MenuType
        {
            get { return menuType; }
            set { menuType = value; }
        }
        private int menuState;
        /// <summary>
        /// 菜单状态
        /// </summary>
        public int MenuState
        {
            get { return menuState; }
            set { menuState = value; }
        }
        private string menuRemark;
        /// <summary>
        /// 菜单备注
        /// </summary>
        public string MenuRemark
        {
            get { return menuRemark; }
            set { menuRemark = value; }
        }
    }
    #endregion
}
