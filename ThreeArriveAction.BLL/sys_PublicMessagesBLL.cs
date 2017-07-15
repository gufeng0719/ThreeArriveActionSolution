using ThreeArriveAction.DAL;
using ThreeArriveAction.Model;

namespace ThreeArriveAction.BLL
{
    public class sys_PublicMessagesBLL
    {
        private sys_PublicMessagesDAL dal = new sys_PublicMessagesDAL();

        public int Add(sys_PublicMessagesModel model)
        {
            return dal.Add(model);
        }
    }
}
