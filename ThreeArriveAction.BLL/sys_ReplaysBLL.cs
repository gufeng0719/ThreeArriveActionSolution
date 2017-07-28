using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThreeArriveAction.DAL;
using ThreeArriveAction.Common;
using ThreeArriveAction.Model;
namespace ThreeArriveAction.BLL
{
    public partial class sys_ReplaysBLL
    {
        private readonly sys_ReplaysDAL rDAL = new sys_ReplaysDAL();
        public sys_ReplaysBLL() { }

        #region 添加回复
        public string AddReplay(sys_ReplaysModel model)
        {
            int number = rDAL.AddReplay(model);
            if (number > 0)
            {
                return "{\"info\":\"留言回复成功\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"留言回复失败\",\"status\":\"n\"}";
            }
        }
        #endregion
    }
}
