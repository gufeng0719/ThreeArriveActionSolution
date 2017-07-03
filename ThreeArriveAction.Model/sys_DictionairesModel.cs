using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreeArriveAction.Model
{
    #region 系统字典类======================
    /// <summary>
    /// Dictionaires:系统字典类
    /// </summary>
    [Serializable]
   public partial class sys_DictionairesModel
    {
        public sys_DictionairesModel() { }

        private int dictionariesId;
        /// <summary>
        /// 字典编号
        /// </summary>
        public int DictionariesId
        {
            get { return dictionariesId; }
            set { dictionariesId = value; }
        }
        private int dictionariesType = 1;
        /// <summary>
        /// 字典类型编号
        /// </summary>
        public int DictionariesType
        {
            get { return dictionariesType; }
            set { dictionariesType = value; }
        }
        private string dictionariesValue = "";
        /// <summary>
        /// 字典值
        /// </summary>
        public string DictionariesValue
        {
            get { return dictionariesValue; }
            set { dictionariesValue = value; }
        }
        private string dictionariesDescribe;
        /// <summary>
        /// 字典值描述
        /// </summary>
        public string DictionariesDescribe
        {
            get { return dictionariesDescribe; }
            set { dictionariesDescribe = value; }
        }
        private int dictionaireState = 1;
        /// <summary>
        /// 字典值状态
        /// </summary>
        public int DictionaireState
        {
            get { return dictionaireState; }
            set { dictionaireState = value; }
        }
    }
    #endregion
}
