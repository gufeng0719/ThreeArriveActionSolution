using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreeArriveAction.Model
{
    #region 签到表=================
    /// <summary>
    /// 签到表类
    /// </summary>
    public partial class sys_SignsModel
    {
        public sys_SignsModel() { }
        private int signId;
        /// <summary>
        /// 签到编号
        /// </summary>
        public int SignId
        {
            get { return signId; }
            set { signId = value; }
        }
        private DateTime signDate;
        /// <summary>
        /// 签到日期
        /// </summary>
        public DateTime SignDate
        {
            get { return signDate; }
            set { signDate = value; }
        }
        private int signUserId;
        /// <summary>
        /// 签到用户编号
        /// </summary>
        public int SignUserId
        {
            get { return signUserId; }
            set { signUserId = value; }
        }
    }
    #endregion
}
