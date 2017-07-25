using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreeArriveAction.Model
{
    #region 有事马上到记录===========================
    /// <summary>
    /// 有事马上到记录表类
    /// </summary>
    [Serializable]
    public partial class sys_ThingRecordsModel
    {
        public sys_ThingRecordsModel() { }
        private int thingId;
        /// <summary>
        /// 有事到编号
        /// </summary>
        public int ThingId
        {
            get { return thingId; }
            set { thingId = value; }
        }
        private DateTime thingDate;
        /// <summary>
        /// 有事到日期
        /// </summary>
        public DateTime ThingDate
        {
            get { return thingDate; }
            set { thingDate = value; }
        }
        private string thingName;
        /// <summary>
        /// 有事到名称
        /// </summary>
        public string ThingName
        {
            get { return thingName; }
            set { thingName = value; }
        }
        private string thingReason;
        /// <summary>
        /// 事情原由
        /// </summary>
        public string ThingReason
        {
            get { return thingReason; }
            set { thingReason = value; }
        }
        private string thingSolution;
        /// <summary>
        /// 事情解决方案
        /// </summary>
        public string ThingSolution
        {
            get { return thingSolution; }
            set { thingSolution = value; }
        }
        private string thingHaving;
        /// <summary>
        /// 事情是否解决
        /// </summary>
        public string ThingHaving
        {
            get { return thingHaving; }
            set { thingHaving = value; }
        }
        private string thingImgUrl;
        /// <summary>
        /// 事情图片路径
        /// </summary>
        public string ThingImgUrl
        {
            get { return thingImgUrl; }
            set { thingImgUrl = value; }
        }
        private int subcriberId;
        /// <summary>
        /// 有事家庭编号
        /// </summary>
        public int SubcriberId
        {
            get { return subcriberId; }
            set { subcriberId = value; }
        }
        private int userId;
        /// <summary>
        /// 处理者编号
        /// </summary>
        public int UserId
        {
            get { return userId; }
            set { userId = value; }
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


        private string extraName;
        /// <summary>
        /// 额外的拜访户的姓名(不在七户表数据中的用户)
        /// </summary>
        public string ExtraName
        {
            get { return extraName; }
            set { extraName = value; }
        }

        private string extraPhone;
        /// <summary>
        /// 额外的拜访户的联系方式(不在七户表数据中的用户)
        /// </summary>
        public string ExtraPhone
        {
            get { return extraPhone; }
            set { extraPhone = value; }
        }
    }
    #endregion
}
