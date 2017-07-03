using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreeArriveAction.Model
{
    #region 公开信息==========================
    /// <summary>
    /// 公开信息类
    /// </summary>
   public partial class sys_PublicMessagesModel
    {
       public sys_PublicMessagesModel() { }
       private int publicId;
       /// <summary>
       /// 公开信息编号
       /// </summary>
       public int PublicId
       {
           get { return publicId; }
           set { publicId = value; }
       }
       private int villageId;
       /// <summary>
       /// 所属村居编号
       /// </summary>
       public int VillageId
       {
           get { return villageId; }
           set { villageId = value; }
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
       private int publicType;
       /// <summary>
       /// 公开类型
       /// </summary>
       public int PublicType
       {
           get { return publicType; }
           set { publicType = value; }
       }
       private string thumbnailUrl;
       /// <summary>
       /// 缩略图地址
       /// </summary>
       public string ThumbnailUrl
       {
           get { return thumbnailUrl; }
           set { thumbnailUrl = value; }
       }
       private string imageUrl;
       /// <summary>
       /// 原始图片地址
       /// </summary>
       public string ImageUrl
       {
           get { return imageUrl; }
           set { imageUrl = value; }
       }
       private int userId;
       /// <summary>
       /// 发布者编号
       /// </summary>
       public int UserId
       {
           get { return userId; }
           set { userId = value; }
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
