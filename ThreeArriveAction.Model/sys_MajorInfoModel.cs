using System;

namespace ThreeArriveAction.Model
{
    public class sys_MajorInfoModel
    {
        public int MajorId { get; set; }
        public string MajorTitle { get; set; } = string.Empty;
        public string MajorContent { get; set; } = string.Empty;
        public int MajorFromUserId { get; set; }
        public DateTime MajorDate { get; set; } = DateTime.Now;
    }
}
