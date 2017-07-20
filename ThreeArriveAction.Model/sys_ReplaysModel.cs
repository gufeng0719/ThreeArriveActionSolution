using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreeArriveAction.Model
{
    #region 互动回复========================
    /// <summary>
    /// 互动回复类
    /// </summary>
    public partial class sys_ReplaysModel
    {
        public sys_ReplaysModel() { }
        private int replayId;
        /// <summary>
        /// 回复编号
        /// </summary>
        public int ReplayId
        {
            get { return replayId; }
            set { replayId = value; }
        }
        private int interactionId;
        /// <summary>
        /// 互动编号
        /// </summary>
        public int InteractionId
        {
            get { return interactionId; }
            set { interactionId = value; }
        }
        private string replayContent;
        /// <summary>
        /// 回复内容
        /// </summary>
        public string ReplayContent
        {
            get { return replayContent; }
            set { replayContent = value; }
        }
        private int replayerId;

        public int ReplayerId
        {
            get { return replayerId; }
            set { replayerId = value; }
        }

        private DateTime repalyDate;
        /// <summary>
        /// 回复日期
        /// </summary>
        public DateTime RepalyDate
        {
            get { return repalyDate; }
            set { repalyDate = value; }
        }
        private int replayType;
        /// <summary>
        /// 回复类型
        /// </summary>
        public int ReplayType
        {
            get { return replayType; }
            set { replayType = value; }
        }
        private string remarks;

        public string Remarks
        {
            get { return remarks; }
            set { remarks = value; }
        }

        public int reToplayerId;

        public int ReToplayerId
        {
            get { return reToplayerId; }
            set { reToplayerId = value; }
        }
    }
    #endregion
}
