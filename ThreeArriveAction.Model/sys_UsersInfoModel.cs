using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreeArriveAction.Model
{
    #region 用户详细表
    /// <summary>
    /// 用户详细类
    /// </summary>
    [Serializable]
    public class sys_UsersInfoModel
    {
        public sys_UsersInfoModel() { }
        private int infoId;
        /// <summary>
        /// 用户信息编号
        /// </summary>
        public int InfoId
        {
            get { return infoId; }
            set { infoId = value; }
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
        private string userPhoto;
        /// <summary>
        /// 用户照片
        /// </summary>
        public string UserPhoto
        {
            get { return userPhoto; }
            set { userPhoto = value; }
        }
        private string userUrl;
        /// <summary>
        /// 用户联系地址
        /// </summary>
        public string UserUrl
        {
            get { return userUrl; }
            set { userUrl = value; }
        }
        private string personalIntroduction;
        /// <summary>
        /// 用户简介
        /// </summary>
        public string PersonalIntroduction
        {
            get { return personalIntroduction; }
            set { personalIntroduction = value; }
        }
        private string personalHonor;
        /// <summary>
        /// 用户荣誉
        /// </summary>
        public string PersonalHonor
        {
            get { return personalHonor; }
            set { personalHonor = value; }
        }
        private string userEducation;
        /// <summary>
        /// 用户学历
        /// </summary>
        public string UserEducation
        {
            get { return userEducation; }
            set { userEducation = value; }
        }
        private string joinPartyDate;
        /// <summary>
        /// 入党时间
        /// </summary>
        public string JoinPartyDate
        {
            get { return joinPartyDate; }
            set { joinPartyDate = value; }
        }
        private string userRemarks;
        /// <summary>
        /// 备注
        /// </summary>
        public string UserRemarks
        {
            get { return userRemarks; }
            set { userRemarks = value; }
        }
    }
    #endregion
}
