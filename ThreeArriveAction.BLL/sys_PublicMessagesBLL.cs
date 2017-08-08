using System.Data;
using System.Text;
using ThreeArriveAction.Common;
using ThreeArriveAction.DAL;
using ThreeArriveAction.Model;

namespace ThreeArriveAction.BLL
{
    public class sys_PublicMessagesBLL
    {
        private sys_PublicMessagesDAL dal = new sys_PublicMessagesDAL();

        #region 添加
        public int Add(sys_PublicMessagesModel model)
        {
            return dal.Add(model);
        }
        #endregion

        #region 删除
        public string DeletePublicMessage(int publicId)
        {
            int number = dal.DeletePublicMessage(publicId);
            if (number > 0)
            {
                return "{\"info\":\"三务公开消息删除成功\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"三务公开消息删除失败\",\"status\":\"n\"}";
            }
        }

        public string DeletePublicMessage(string ids)
        {
            int number = dal.DeletePublicMessage(ids);
            if (number > 0)
            {
                return "{\"info\":\"三务公开消息删除成功\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"三务公开消息删除失败\",\"status\":\"n\"}";
            }
        }
        #endregion 

        #region 查询

        public sys_PublicMessagesModel GetModel(int publicId)
        {
            sys_PublicMessagesModel model = dal.GetModel(publicId);
            return model;
        }

        public string GetModelJson(int publicId)
        {
            sys_PublicMessagesModel model = GetModel(publicId);
            if (model == null)
            {
                return "";
            }
            else
            {
                return model.ToJson();
            }
        }

        /// <summary>
        /// 分页查询公开信息
        /// </summary>
        /// <param name="pageSize">每页条数</param>
        /// <param name="pageIndex">当前页码数</param>
        /// <param name="strWhere">查询条件</param>
        /// <param name="filedOrder">排序条件</param>
        /// <param name="recordCount">总记录数</param>
        /// <returns></returns>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            DataSet ds = dal.GetList(pageSize,pageIndex,strWhere,filedOrder,out recordCount);
            return ds;
        }

        public string GetListJson(int pageSize, int pageIndex, string strWhere, string fieldOrder)
        {
            StringBuilder strJson = new StringBuilder();
            int recordCount = 0;
            DataTable dt = GetList(pageSize, pageIndex, strWhere, fieldOrder, out recordCount).Tables[0];
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
