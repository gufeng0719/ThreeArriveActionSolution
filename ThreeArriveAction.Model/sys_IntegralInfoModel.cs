using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreeArriveAction.Model
{
    #region 积分详细信息=====================
    /// <summary>
    /// 积分详细信息
    /// </summary>
    [Serializable]
   public partial class sys_IntegralInfoModel
    {
        public sys_IntegralInfoModel() { }
        private int integralInfoId;
        /// <summary>
        /// 积分详细编号
        /// </summary>
        public int IntegralInfoId
        {
            get { return integralInfoId; }
            set { integralInfoId = value; }
        }
        private int integralId;
        /// <summary>
        /// 用户积分编号
        /// </summary>
        public int IntegralId
        {
            get { return integralId; }
            set { integralId = value; }
        }

        private int integralType;
        /// <summary>
        /// 积分类型
        /// </summary>
        public int IntegralType
        {
            get { return integralType; }
            set { integralType = value; }
        }

        private int userId;
        /// <summary>
        /// 积分用户编号
        /// </summary>
        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }
        private int score;
        /// <summary>
        /// 所得积分
        /// </summary>
        public int Score
        {
            get { return score; }
            set { score = value; }
        }
        private DateTime integralDate = DateTime.Now;
        /// <summary>
        /// 积分得取时间
        /// </summary>
        public DateTime IntegralDate
        {
            get { return integralDate; }
            set { integralDate = value; }
        }
        private int? integralEventId;
        /// <summary>
        /// 积分事件编号
        /// </summary>
        public int? IntegralEventId
        {
            get { return integralEventId; }
            set { integralEventId = value; }
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
