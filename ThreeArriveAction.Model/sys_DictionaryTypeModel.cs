using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreeArriveAction.Model
{
    /// <summary>
    /// 数据实体类:数据字典类型
    /// </summary>
    [Serializable]
   public  class sys_DictionaryTypeModel
    {
        private int dict_value;
        /// <summary>
        /// 数据字典区域值
        /// </summary>
        public int Dict_value
        {
            get { return dict_value; }
            set { dict_value = value; }
        }
        private string dict_name;
        /// <summary>
        /// 字段区域名称
        /// </summary>
        public string Dict_name
        {
            get { return dict_name; }
            set { dict_name = value; }
        }
    }
}
