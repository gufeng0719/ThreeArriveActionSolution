using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreeArriveAction.Model
{
    /// <summary>
    /// 开启签到实体类
    /// </summary>
    [Serializable]
    public class sys_OnSignsModel
    {
        public sys_OnSignsModel(){}
        private int onSignId;
        /// <summary>
        /// 编号
        /// </summary>
        public int OnSignId
        {
            get { return onSignId; }
            set { onSignId = value; }
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

        private int userId;
        /// <summary>
        /// 用户编号
        /// </summary>
        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }
        private DateTime onTime;
        /// <summary>
        /// 开启事件
        /// </summary>
        public DateTime OnTime
        {
            get { return onTime; }
            set { onTime = value; }
        }
    }
}
