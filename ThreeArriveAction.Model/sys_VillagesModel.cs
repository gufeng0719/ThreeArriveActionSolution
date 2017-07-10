using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreeArriveAction.Model
{
    #region  村居表============
    /// <summary>
    /// 村居类
    /// </summary>
    [Serializable]
    public partial class sys_VillagesModel
    {
        public sys_VillagesModel() { }
        private int villageId;
        /// <summary>
        /// 村居编号
        /// </summary>
        public int VillageId
        {
            get { return villageId; }
            set { villageId = value; }
        }
        private string villageName;
        /// <summary>
        /// 村居名称
        /// </summary>
        public string VillageName
        {
            get { return villageName; }
            set { villageName = value; }
        }
        private int villageParId;
        /// <summary>
        /// 父级村居编号
        /// </summary>
        public int VillageParId
        {
            get { return villageParId; }
            set { villageParId = value; }
        }
        private int villageGrade;
        /// <summary>
        /// 村居登记
        /// </summary>
        public int VillageGrade
        {
            get { return villageGrade; }
            set { villageGrade = value; }
        }

        private int villageState;
        /// <summary>
        /// 村居状态
        /// </summary>
        public int VillageState
        {
            get { return villageState; }
            set { villageState = value; }
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
