using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ThreeArriveAction.DAL;
using ThreeArriveAction.Model;

namespace ThreeArriveAction.BLL
{
   public partial class sys_LeaveMessageBLL
    {
        private readonly sys_LeaveMessageDAL lmDAL = new sys_LeaveMessageDAL();
        public sys_LeaveMessageBLL() { }
        #region 增加
        public string AddLeave(sys_LeaveMessageModel model)
        {
            int number = lmDAL.AddLeave(model);
            if (number > 0)
            {
                return "{\"info\":\"留言添加成功\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"留言添加失败\",\"status\":\"n\"}";
            }
        }
        #endregion

        #region 修改
        public string UpdateLeave(sys_LeaveMessageModel model)
        {
            int number = lmDAL.UpdateLeave(model);
            if (number > 0)
            {
                return "{\"info\":\"留言修改成功\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"留言修改失败\",\"status\":\"n\"}";
            }
        }

        public string UpdateLeaveState(int id,int state)
        {
            int number = lmDAL.UpdateLeaveState(id, state);
            if (number > 0)
            {
                return "{\"info\":\"留言操作成功\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"留言操作失败\",\"status\":\"n\"}";
            }
        }

        /// <summary>
        /// 审核留言信息
        /// </summary>
        /// <param name="id">留言编号</param>
        /// <param name="score">积分</param>
        /// <returns></returns>
        public string PublishLeaveMessage(int id,int score)
        {
            int number = lmDAL.PublishLeaveMessage(id, score);
            if (number > 0)
            {
                return "{\"info\":\"留言审核通过\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"留言审核失败\",\"status\":\"n\"}";
            }
        }

        public string SetPraiseNumber(int id,int userid)
        {
            int number = lmDAL.SetPraiseNumber(id, userid);
            if (number > 0)
            {
                return "{\"info\":\"留言点赞成功\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"留言点赞失败\",\"status\":\"n\"}";
            }
        }
        #endregion

        #region 删除
        public string DeleteLeave(int id)
        {
            int number = lmDAL.DeleteLeave(id);
            if (number > 0)
            {
                return "{\"info\":\"留言删除成功\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"留言删除失败\",\"status\":\"n\"}";
            }
        }

        public string DeleteLeaves(string ids)
        {
            int number = lmDAL.DeleteLeaves(ids);
            if (number > 0)
            {
                return "{\"info\":\"留言批量操作成功\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"留言批量操作失败\",\"status\":\"n\"}";
            }
        }
        #endregion
        #region 查询
        public DataSet GetList(int pageSize,int pageIndex,string strWhere,string fieldOrder,out int recordCount)
        {
            DataSet ds = lmDAL.GetList(pageSize, pageIndex, strWhere, fieldOrder, out recordCount);
            return ds;
        }
        #endregion
    }
}
