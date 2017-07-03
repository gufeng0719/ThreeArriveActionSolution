using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ThreeArriveAction.DAL;
using ThreeArriveAction.Model;
using ThreeArriveAction.Common;
namespace ThreeArriveAction.BLL
{
    /// <summary>
    /// 业务逻辑类:有事马上到
    /// </summary>
  public partial  class sys_ThingRecordsBLL
    {
      private readonly sys_ThingRecordsDAL thingDAL = new sys_ThingRecordsDAL();
        public string AddThingRecord(sys_ThingRecordsModel thingModel)
        {
            int number = thingDAL.AddThingRecord(thingModel);
            if (number > 0)
            {
                return "{\"info\":\"本次拜访提交成功\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"本次拜访提交失败\",\"status\":\"n\"}";
            }
        }
    }
}
