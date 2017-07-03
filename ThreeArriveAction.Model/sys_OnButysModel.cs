using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreeArriveAction.Model
{
    #region 值班表====================
    /// <summary>
    /// 值班表实体类
    /// </summary>
    public partial class sys_OnButysModel
    {
        private int onbutyId;
        /// <summary>
        /// 值班编号
        /// </summary>
        public int OnbutyId
        {
            get { return onbutyId; }
            set { onbutyId = value; }
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
        /// 值班用户
        /// </summary>
        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        private DateTime butyDate;
        /// <summary>
        /// 值班日期
        /// </summary>
        public DateTime ButyDate
        {
            get { return butyDate; }
            set { butyDate = value; }
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
