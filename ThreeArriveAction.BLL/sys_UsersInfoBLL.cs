using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ThreeArriveAction.Common;
using ThreeArriveAction.Model;
using ThreeArriveAction.DAL;

namespace ThreeArriveAction.BLL
{
   public partial class sys_UsersInfoBLL
    {
       private readonly sys_UsersInfoDAL infoDAL = new sys_UsersInfoDAL();
       public sys_UsersInfoModel GetUserInfoModel(int userId)
       {
           sys_UsersInfoModel model = infoDAL.GetUsersInfoByUserId(userId);
           return model;
       }
    }
}
