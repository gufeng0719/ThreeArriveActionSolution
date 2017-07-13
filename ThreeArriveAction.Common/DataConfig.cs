using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThreeArriveAction.Common;
using ThreeArriveAction.Model;

namespace ThreeArriveAction.Common
{
    public class DataConfig
    {
        public static List<sys_VillagesModel> GetVillages()
        {
            var list = CacheHelper.Get<List<sys_VillagesModel>>("GetListVillagesModel");
            if (list == null)
            {
                var sh = new SqlHelper<sys_VillagesModel>(new sys_VillagesModel());
                list = sh.Select().ToList();
            }
            return list;
        }
    }
}
