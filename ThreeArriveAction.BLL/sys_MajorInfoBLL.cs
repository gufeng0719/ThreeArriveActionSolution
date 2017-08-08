using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ThreeArriveAction.Common;
using ThreeArriveAction.DAL;
using ThreeArriveAction.Model;
namespace ThreeArriveAction.BLL
{
    public partial class sys_MajorInfoBLL
    {
        public sys_MajorInfoBLL() { }
        private readonly sys_MajorInfoDAL mDAL = new sys_MajorInfoDAL();
        #region 添加
        public string AddMajor(sys_MajorInfoModel model)
        {
            int number = mDAL.AddMajor(model);
            if (number > 0)
            {
                return "{\"info\":\"重要通知添加成功\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"重要通知添加失败\",\"status\":\"n\"}";
            }
        }
        #endregion

        #region 修改
        public string UpdateMajor(sys_MajorInfoModel model)
        {
            int number = mDAL.UpdateMajor(model);
            if (number > 0)
            {
                return "{\"info\":\"重要通知修改成功\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"重要通知修改失败\",\"status\":\"n\"}";
            }
        }
        #endregion

        #region 删除
        public string DeleteMajor(int majorId)
        {
            int number = mDAL.DeleteMajor(majorId);
            if (number > 0)
            {
                return "{\"info\":\"重要通知删除成功\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"重要通知删除失败\",\"status\":\"n\"}";
            }
        }

        public string DeleteMajors(string ids)
        {
            int number = mDAL.DeleteMajors(ids);
            if (number > 0)
            {
                return "{\"info\":\"重要通知删除成功\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"重要通知删除失败\",\"status\":\"n\"}";
            }
        }
        #endregion

        #region 查询
        public sys_MajorInfoModel GetModel(int majorId)
        {
            sys_MajorInfoModel model = mDAL.GetModel(majorId);
            return model;
        }

        public DataTable GetList(int pageSize,int pageIndex,string strWhere,string fieldOrder,out int recordCount)
        {
            DataTable dt = mDAL.GetList(pageSize, pageIndex, strWhere, fieldOrder, out recordCount);
            return dt;
        }
        #endregion
    }
}
