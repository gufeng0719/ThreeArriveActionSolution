using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ThreeArriveAction.DAL;
using ThreeArriveAction.Common;
using ThreeArriveAction.Model;
namespace ThreeArriveAction.BLL
{
    public class sys_SubscriberFamilyBLL
    {
        private readonly sys_SubscriberFamilyDAL subDAL = new sys_SubscriberFamilyDAL();
        #region 添加
        /// <summary>
        /// 添加七户信息
        /// </summary>
        /// <param name="subModel">七户实体信息</param>
        /// <returns></returns>
        public string AddSubscriberFamily(sys_SubscriberFamilyModel subModel)
        {
            int numbers = subDAL.AddSubscriberFamily(subModel);
            if (numbers > 0)
            {
                return "{\"info\":\"七户添加成功\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"七户添加失败\",\"status\":\"n\"}";
            }
        }
        #endregion

        #region 修改
        /// <summary>
        /// 根据七户编号，修改该七户信息
        /// </summary>
        /// <param name="subModel">七户实体信息</param>
        /// <returns></returns>
        public string UpdateSubscriberFamily(sys_SubscriberFamilyModel subModel)
        {
            int numbers = subDAL.UpdateSubscriberFamily(subModel);
            if (numbers > 0)
            {
                return "{\"info\":\"七户修改成功\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"七户修改失败\",\"status\":\"n\"}";
            }
        }

        /// <summary>
        /// 根据户号编号，修改该户的坐标
        /// </summary>
        /// <param name="subscriberId">户号编号</param>
        /// <returns></returns>
        public int UpdateCoordinate(int subscriberId)
        {
            return subDAL.UpdateCoordinate(subscriberId);
        }
        #endregion

        #region 删除
        /// <summary>
        /// 根据编号字符串，删除七户信息
        /// </summary>
        /// <param name="subids">编号</param>
        /// <returns></returns>
        public string DeleteSubscriberFamily(string subids)
        {
            int numbers = subDAL.DeleteSubscriberFamily(subids);
            if (numbers > 0)
            {
                return "{\"info\":\"七户信息删除成功\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"七户信息删除失败\",\"status\":\"n\"}";
            }
        }

        #endregion

        #region 查询
        /// <summary>
        /// 根据客户编号,查询该客户信息
        /// </summary>
        /// <param name="subId">客户编号</param>
        /// <returns></returns>
        public sys_SubscriberFamilyModel GetSubscriberFamilyBySubId(int subId)
        {
            return subDAL.GetSubscriberFamilyBySubId(subId);
        }

        public string GetSubscriberFamilyJsonBySubId(int subId)
        {
            sys_SubscriberFamilyModel model = GetSubscriberFamilyBySubId(subId);
            if (model != null)
            {
                return JsonHelper.ToJson(model);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 根据用户编号，查询该用户所责任区七户信息
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <returns></returns>
        public List<sys_SubscriberFamilyModel> GetSubscriberFamilyByUserId(int userId)
        {
            return subDAL.GetSubscriberFamilyByUserId(userId);
        }

        /// <summary>
        /// 根据村居编号，查询该村七户信息
        /// </summary>
        /// <param name="villageId">村居编号</param>
        /// <returns></returns>
        public List<sys_SubscriberFamilyModel> GetSubscriberFamilyByVillageId(int villageId)
        {
            return subDAL.GetSubscriberFamilyByVillageId(villageId);
        }

        public string GetSubscriberFamilyStringByUserId(int userId)
        {
            List<sys_SubscriberFamilyModel> subList = GetSubscriberFamilyByUserId(userId);
            if (subList.Count > 0)
            {
                return JsonHelper.ListToJson(subList);
            }
            else
            {
                return "";
            }
        }

        public DataSet GetList(int top,string strWhere,string filedOrder) 
        {
            return subDAL.GetList(top, strWhere, filedOrder);
        }

        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int totalCount)
        {
            DataSet ds = subDAL.GetList(pageSize, pageIndex, strWhere, filedOrder, out totalCount);
            return ds;
        }
        #endregion
    }
}
