using System;
namespace ThreeArriveAction.Model
{
    public class v_OnButyUsers
    {
        public int OnbutyId { get; set; }
        public int VillageId { get; set; }
        public int VillageParId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserPhone { get; set; } = string.Empty;
        public string UserPhoto { get; set; } = string.Empty;
        public DateTime ButyDate { get; set; } = DateTime.Now;
    }
}
