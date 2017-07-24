using System;

namespace ThreeArriveAction.Model
{
    public class sys_SignInfoModel
    {
        public int SignInfoId { get; set; }
        public int SignId { get; set; }
        public string Path1 { get; set; } = string.Empty;
        public string Path2 { get; set; } = string.Empty;
        public string Path3 { get; set; } = string.Empty;
        public string Path4 { get; set; } = string.Empty;
        public string Msg { get; set; } = string.Empty;
        public DateTime SignInfoDate { get; set; } = DateTime.Now;
    }
}
