using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreeArriveAction.Model
{
    #region 互动学习===========================
    /// <summary>
    /// 互动学习
    /// </summary>
    [Serializable]
   public partial class sys_InteractionLearnsModel
    {
        public sys_InteractionLearnsModel() { }
        private int learnId;
        /// <summary>
        /// 学习编号
        /// </summary>
        public int LearnId
        {
            get { return learnId; }
            set { learnId = value; }
        }
        private int learnType;
        /// <summary>
        /// 学习类型
        /// </summary>
        public int LearnType
        {
            get { return learnType; }
            set { learnType = value; }
        }
        private string learnTitle;
        /// <summary>
        /// 学习表头
        /// </summary>
        public string LearnTitle
        {
            get { return learnTitle; }
            set { learnTitle = value; }
        }
        private string learnContent;
        /// <summary>
        /// 学习内容
        /// </summary>
        public string LearnContent
        {
            get { return learnContent; }
            set { learnContent = value; }
        }
        private int publisher;
        /// <summary>
        /// 发布者
        /// </summary>
        public int Publisher
        {
            get { return publisher; }
            set { publisher = value; }
        }
        private DateTime publishDate;
        /// <summary>
        /// 发布日期
        /// </summary>
        public DateTime PublishDate
        {
            get { return publishDate; }
            set { publishDate = value; }
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
