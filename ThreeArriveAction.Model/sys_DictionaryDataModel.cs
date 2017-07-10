using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreeArriveAction.Model
{
    #region 系统字典类======================
    /// <summary>
    /// DictionaryData:系统字典值类
    /// </summary>
    [Serializable]
   public partial class sys_DictionaryDataModel
    {
        public sys_DictionaryDataModel() { }

        private int dictId;
        /// <summary>
        /// 字典值编号
        /// </summary>
        public int DictId
        {
            get { return dictId; }
            set { dictId = value; }
        }
        private int dict_value;
        /// <summary>
        /// 字典类型值
        /// </summary>
        public int Dict_value
        {
            get { return dict_value; }
            set { dict_value = value; }
        }
        private string dictdata_Name;
        /// <summary>
        /// 字典值描述
        /// </summary>
        public string Dictdata_Name
        {
            get { return dictdata_Name; }
            set { dictdata_Name = value; }
        }
        
        private string dictdata_Value;
        /// <summary>
        /// 字典数值
        /// </summary>
        public string Dictdata_Value
        {
            get { return dictdata_Value; }
            set { dictdata_Value = value; }
        }
        private int dictdata_State;
        /// <summary>
        /// 字典值状态
        /// </summary>
        public int Dictdata_State
        {
            get { return dictdata_State; }
            set { dictdata_State = value; }
        }
        private int dictdata_isfixed;
        /// <summary>
        /// 字典值是否固定
        /// </summary>
        public int Dictdata_isfixed
        {
            get { return dictdata_isfixed; }
            set { dictdata_isfixed = value; }
        }
    }
    #endregion
}
