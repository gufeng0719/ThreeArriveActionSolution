using System;

namespace ThreeArriveAction.Model
{
    #region 在线学习========================
    /// <summary>
    /// 在线学习实体类
    /// </summary>
   public  class sys_OnlineInteractionsModel
    {
        private int interactionId;
       /// <summary>
       /// 在线互动编号
       /// </summary>
        public int InteractionId
        {
            get { return interactionId; }
            set { interactionId = value; }
        }
       private string interactionTitle;
       /// <summary>
       /// 互动主题
       /// </summary>
       private string interactionContent;
       private int publisher;
       /// <summary>
       /// 发布者
       /// </summary>
       private DateTime publishDate;
       /// <summary>
       /// 发布日期
       /// </summary>
       public DateTime PublishDate
       {
           get { return publishDate; }
           set { publishDate = value; }
       }
       private int interactionType;
       /// <summary>
       /// 互动类型
       /// </summary>
       public int InteractionType
       {
           get { return interactionType; }
           set { interactionType = value; }
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
