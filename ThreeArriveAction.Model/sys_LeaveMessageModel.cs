using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreeArriveAction.Model
{
    public class sys_LeaveMessageModel
    {
        public int LeaveId { get; set; }
        public string LeaveContent { get; set; } = string.Empty;
        public string LeaveImages { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int VillageId { get; set; }
        public DateTime LeaveDateTime { get; set; } = DateTime.Now;
        public int LeavePraiseNumber { get; set; }
        public  string LeavePraiseUserIds { get; set; } = string.Empty;
    }
}
