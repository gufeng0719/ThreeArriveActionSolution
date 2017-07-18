using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreeArriveAction.Model
{
    #region 七户家庭表
    public partial class sys_SubscriberFamilyModel
    {
        public sys_SubscriberFamilyModel() { }
        private int subscriberId;
        /// <summary>
        /// 七户家庭编号
        /// </summary>
        public int SubscriberId
        {
            get { return subscriberId; }
            set { subscriberId = value; }
        }
        private string subscriberName;
        /// <summary>
        /// 七户户主姓名
        /// </summary>
        public string SubscriberName
        {
            get { return subscriberName; }
            set { subscriberName = value; }
        }
        private string subscriberPhone;
        /// <summary>
        /// 户主联系方式
        /// </summary>
        public string SubscriberPhone
        {
            get { return subscriberPhone; }
            set { subscriberPhone = value; }
        }
        private int subscriberType;
        /// <summary>
        /// 七户类型
        /// </summary>
        public int SubscriberType
        {
            get { return subscriberType; }
            set { subscriberType = value; }
        }
        private string familyCoordinate = string.Empty;
        /// <summary>
        /// 七户家庭坐标
        /// </summary>
        public string FamilyCoordinate
        {
            get { return familyCoordinate; }
            set { familyCoordinate = value; }
        }
        private string familyAddress;
        /// <summary>
        /// 七户家庭住址
        /// </summary>
        public string FamilyAddress
        {
            get { return familyAddress; }
            set { familyAddress = value; }
        }
        private int familyNumber;
        /// <summary>
        /// 家庭人口
        /// </summary>
        public int FamilyNumber
        {
            get { return familyNumber; }
            set { familyNumber = value; }
        }
        private int villageId;
        /// <summary>
        /// 所属村居
        /// </summary>
        public int VillageId
        {
            get { return villageId; }
            set { villageId = value; }
        }

        private int userId;
        /// <summary>
        /// 所属用户
        /// </summary>
        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }
        private string reamarks;
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks
        {
            get { return reamarks; }
            set { reamarks = value; }
        }
    }
    #endregion
}
