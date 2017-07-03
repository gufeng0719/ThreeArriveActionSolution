using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreeArriveAction.Model
{
    #region 用户积分表=======================
    /// <summary>
    /// 用户积分表
    /// </summary>
    [Serializable]
    public partial class sys_UserIntegralsModel
    {
        public sys_UserIntegralsModel() { }
        private int integralId;
        /// <summary>
        /// 积分编号
        /// </summary>
        public int IntegralId
        {
            get { return integralId; }
            set { integralId = value; }
        }
        private int userId;
        /// <summary>
        /// 用户编号
        /// </summary>
        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }
        private int integralScore;
        /// <summary>
        /// 积分总分
        /// </summary>
        public int IntegralScore
        {
            get { return integralScore; }
            set { integralScore = value; }
        }
        private int integralYear;
        /// <summary>
        /// 积分年
        /// </summary>
        public int IntegralYear
        {
            get { return integralYear; }
            set { integralYear = value; }
        }
        private int integralMonth;
        /// <summary>
        /// 积分月份
        /// </summary>
        public int IntegralMonth
        {
            get { return integralMonth; }
            set { integralMonth = value; }
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
    }
    #endregion
}
