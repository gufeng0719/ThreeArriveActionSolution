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
    public partial class sys_InteractionLearnsBLL
    {
        private readonly sys_InteractionLearnsDAL iiDAL = new sys_InteractionLearnsDAL();
        public sys_InteractionLearnsBLL() { }

        #region 添加
        public string AddLearns(sys_InteractionLearnsModel model)
        {
            int number = iiDAL.AddLearns(model);
            if (number > 0)
            {
                return "{\"info\":\"在线学习添加成功\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"在线学习添加失败\",\"status\":\"n\"}";
            }
        }
        #endregion

        #region 修改
        public string UpdateLearns(sys_InteractionLearnsModel model)
        {
            int number = iiDAL.UpdateLearn(model);
            if (number > 0)
            {
                return "{\"info\":\"在线学习修改成功\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"在线学习修改失败\",\"status\":\"n\"}";
            }
        }
        #endregion

        #region 删除
        public string DeleteLearns(string ids)
        {
            int number = iiDAL.DeleteLearns(ids);
            if (number > 0)
            {
                return "{\"info\":\"在线学习删除成功\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"在线学习删除成功\",\"status\":\"n\"}";
            }
        }
        #endregion

        #region  查询
        public DataSet GetList(int pageSize,int pageIndex,string strWhere,string fieldOrder,out int recordCount)
        {
            DataSet ds = iiDAL.GetList(pageSize,pageIndex,strWhere,fieldOrder,out recordCount);
            return ds;
        }
        #endregion
    }
}
