using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreeArriveAction.Model
{
    #region 用户主表================
    /// <summary>
    /// 用户主表
    /// </summary>
    [Serializable]
    public partial class sys_UsersModel
    {
        public sys_UsersModel() { }
        private int userId;
        /// <summary>
        /// 用户编号
        /// </summary>
        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }
        private string userPhone;
        /// <summary>
        /// 用户联系方式
        /// </summary>
        public string UserPhone
        {
            get { return userPhone; }
            set { userPhone = value; }
        }
        private string userName;
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }
        private string userDuties;
        /// <summary>
        /// 用户职务
        /// </summary>
        public string UserDuties
        {
            get { return userDuties; }
            set { userDuties = value; }
        }
        private int organizationId;
        /// <summary>
        /// 组织等级编号
        /// </summary>
        public int OrganizationId
        {
            get { return organizationId; }
            set { organizationId = value; }
        }
        private int villageId;
        /// <summary>
        /// 村居编号
        /// </summary>
        public int VillageId
        {
            get { return villageId; }
            set { villageId = value; }
        }
        private string userBirthday;
        /// <summary>
        /// 出生日期
        /// </summary>
        public string UserBirthday
        {
            get { return userBirthday; }
            set { userBirthday = value; }
        }
        private string userPassword;
        /// <summary>
        /// 用户密码
        /// </summary>
        public string UserPassword
        {
            get { return userPassword; }
            set { userPassword = value; }
        }
        private string userRemark;
        /// <summary>
        /// 备注
        /// </summary>
        public string UserRemark
        {
            get { return userRemark; }
            set { userRemark = value; }
        }

        private sys_UsersInfoModel userInfoModel;
        /// <summary>
        /// 用户附加信息
        /// </summary>
        public sys_UsersInfoModel UserInfoModel
        {
            get { return userInfoModel; }
            set { userInfoModel = value; }
        }
    }
    #endregion
}
