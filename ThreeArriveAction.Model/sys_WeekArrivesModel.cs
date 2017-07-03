using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreeArriveAction.Model
{
    #region 每周到表==============
    /// <summary>
    /// 每周到类
    /// </summary>
    [Serializable]
    public partial class sys_WeekArrivesModel
    {
        public sys_WeekArrivesModel() { }
        private int weekArriveId;
        /// <summary>
        /// 每周到编号
        /// </summary>
        public int WeekArriveId
        {
            get { return weekArriveId; }
            set { weekArriveId = value; }
        }
        private DateTime weekArriveDate;
        /// <summary>
        /// 每周到日期
        /// </summary>
        public DateTime WeekArriveDate
        {
            get { return weekArriveDate; }
            set { weekArriveDate = value; }
        }
        private int subcriberId;
        /// <summary>
        /// 受访家庭编号
        /// </summary>
        public int SubcriberId
        {
            get { return subcriberId; }
            set { subcriberId = value; }
        }
        private string thingMessage;
        /// <summary>
        /// 事件信息
        /// </summary>
        public string ThingMessage
        {
            get { return thingMessage; }
            set { thingMessage = value; }
        }
        private string thingResult;
        /// <summary>
        /// 事件结果
        /// </summary>
        public string ThingResult
        {
            get { return thingResult; }
            set { thingResult = value; }
        }
        private string thingImgUrl;
        /// <summary>
        /// 事件图片
        /// </summary>
        public string ThingImgUrl
        {
            get { return thingImgUrl; }
            set { thingImgUrl = value; }
        }
        private int userId;
        /// <summary>
        /// 出访者
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
        private int weekArriveState;
        /// <summary>
        /// 每周到状态
        /// </summary>
        public int WeekArriveState
        {
            get { return weekArriveState; }
            set { weekArriveState = value; }
        }
    }
    #endregion
}
