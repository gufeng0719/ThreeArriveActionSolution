using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ThreeArriveAction.Model;
using ThreeArriveAction.Common;
using ThreeArriveAction.DAL;

namespace ThreeArriveAction.BLL
{
    public partial class sys_OnButysBLL
    {
        private readonly sys_OnButysDAL butyDAL = new sys_OnButysDAL();

        #region 基本操作
        /// <summary>
        /// 检索在某天某村是否已有值班信息
        /// </summary>
        /// <param name="villageid">村居编号</param>
        /// <param name="seachTime">查询日期</param>
        /// <returns></returns>
        public bool ExisByVillageId(int villageid, DateTime seachTime)
        {
            return butyDAL.ExisByVillageId(villageid, seachTime);
        }
        #endregion

        #region 添加
        public string AddOnButys(sys_OnButysModel butyModel)
        {
            int number = butyDAL.AddOnButys(butyModel);
            if (number > 0)
            {
                return "{\"info\":\"值班设置成功\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"值班设置失败\",\"status\":\"n\"}";
            }
        }
        #endregion

        #region 查询
        /// <summary>
        /// 检索在某天某村值班信息
        /// </summary>
        /// <param name="villageid">村居编号</param>
        /// <param name="butyDate">查询日期</param>
        /// <returns></returns>
        public sys_OnButysModel GetButyByVillageAndButyDate(int villageid, DateTime butyDate)
        {
            return butyDAL.GetButyByVillageAndButyDate(villageid, butyDate);
        }

        /// <summary>
        /// 根据日期查询，该天值班信息
        /// </summary>
        /// <param name="seachTime">查询日期</param>
        /// <returns></returns>
        public DataSet GetButyList(DateTime seachTime)
        {
            return butyDAL.GetButyList(seachTime);
        }

        public DataTable GetList(int pageSize, int pageIndex, string strWhere, string fieldOrder, out int recordCount)
        {
            DataSet ds = butyDAL.GetList(pageSize, pageIndex, strWhere, fieldOrder, out recordCount);
            return ds.Tables[0];
        }

        public string GetListJson(int pageSize, int pageIndex, string strWhere, string fieldOrder)
        {
            StringBuilder strJson = new StringBuilder();
            int recordCount = 0;
            DataTable  dt = GetList(pageSize, pageIndex, strWhere, fieldOrder, out recordCount);
            strJson.Append("{\"total\":" + recordCount);
            strJson.Append(",\"rows\":");
            if (recordCount > 0)
            {
                strJson.Append(JsonHelper.ToJson(dt));
            }
            else
            {
                strJson.Append("[]");
            }
            string pageContent = Utils.OutPageList(pageSize, pageIndex, recordCount, "Load(__id__)", 8);
            if (pageContent == "")
            {
                strJson.Append(",\"pageContent\":\"\"");
            }
            else
            {
                strJson.Append(",\"pageContent\":" + pageContent);
            }

            strJson.Append("}");
            return strJson.ToString();
        }
        #endregion
    }
}
