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
    public partial class sys_ThingRecordsBLL
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

        #region 查询

        public DataTable GetThingModelByThingId(int thingId)
        {
            return thingDAL.GetThingModelByThingId(thingId);
        }

        public DataSet SearchThingRecord(int pageSize,int pageIndex,string strWhere,string fieldOrder,out int recordCount)
        {
            DataSet ds = thingDAL.SearchThingRecord(pageSize, pageIndex, strWhere, fieldOrder, out recordCount);
            return ds;
        }

        public DataTable StatisThingRecord(int town,string strWhere)
        {
            DataTable dt = thingDAL.StatisThingRecord(town,strWhere);
            return dt;
        }
        #endregion
    }
}