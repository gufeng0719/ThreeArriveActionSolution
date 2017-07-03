﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThreeArriveAction.DAL;
using System.Data;
using ThreeArriveAction.Model;

namespace ThreeArriveAction.BLL
{
    /// <summary>
	/// 微信请求回复的内容
	/// </summary>
    public class wx_requestRuleContentBLL
    {
        private readonly wx_requestRuleContentDAL dal=new wx_requestRuleContentDAL();
        public wx_requestRuleContentBLL()
		{}
		#region  BasicMethod

		/// <summary>
		/// 得到最大ID
		/// </summary>
		public int GetMaxId()
		{
			return dal.GetMaxId();
		}

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int id)
		{
			return dal.Exists(id);
		}

		/// <summary>
		/// 增加一条数据
		/// </summary>
		public int  Add(wx_requestRuleContentModel model)
		{
			return dal.Add(model);
		}

		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(wx_requestRuleContentModel model)
		{
			return dal.Update(model);
		}

		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool Delete(int id)
		{
			
			return dal.Delete(id);
		}
		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool DeleteList(string idlist )
		{
			return dal.DeleteList(idlist );
		}

		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public wx_requestRuleContentModel GetModel(int id)
		{
			
			return dal.GetModel(id);
		}

		 

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public DataSet GetList(string strWhere)
		{
			return dal.GetList(strWhere);
		}
		/// <summary>
		/// 获得前几行数据
		/// </summary>
		public DataSet GetList(int Top,string strWhere,string filedOrder)
		{
			return dal.GetList(Top,strWhere,filedOrder);
		}
		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<wx_requestRuleContentModel> GetModelList(string strWhere)
		{
			DataSet ds = dal.GetList(strWhere);
			return DataTableToList(ds.Tables[0]);
		}
		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<wx_requestRuleContentModel> DataTableToList(DataTable dt)
		{
            List<wx_requestRuleContentModel> modelList = new List<wx_requestRuleContentModel>();
			int rowsCount = dt.Rows.Count;
			if (rowsCount > 0)
			{
				wx_requestRuleContentModel model;
				for (int n = 0; n < rowsCount; n++)
				{
					model = dal.DataRowToModel(dt.Rows[n]);
					if (model != null)
					{
						modelList.Add(model);
					}
				}
			}
			return modelList;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public DataSet GetAllList()
		{
			return GetList("");
		}

		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		public int GetRecordCount(string strWhere)
		{
			return dal.GetRecordCount(strWhere);
		}
		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		public DataSet GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
		{
			return dal.GetListByPage( strWhere,  orderby,  startIndex,  endIndex);
		}
		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		//public DataSet GetList(int PageSize,int PageIndex,string strWhere)
		//{
			//return dal.GetList(PageSize,PageIndex,strWhere);
		//}

		#endregion  BasicMethod

        #region 微信端获取数据，需要提升效率

        /// <summary>
        /// 得到回复规则的纯文本信息
        /// </summary>
        ///<param name="rid">规则主键Id</param>
        /// <returns></returns>
        public string GetTxtContent(int rid)
        {
            return dal.GetTxtContent(rid);
        }
        /// <summary>
        /// 2014-9-18新增抽奖功能
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public string GetTxtContent(string  openid)
        {
            return dal.GetTxtContent(openid);
        }

        /// <summary>
        /// 得到回复规则的语音信息
        /// </summary>
        /// <param name="rid">规则主键Id</param>
        /// <returns></returns>
        public wx_requestRuleContentModel GetMusicContent(int rid)
        {
            return dal.GetMusicContent(rid);
        }

        /// <summary>
        /// 得到回复规则的【图文】信息
        /// </summary>
        /// <param name="rid">规则主键Id</param>
        /// <returns></returns>
        public IList<wx_requestRuleContentModel> GetTuWenContent(int rid)
        {

            IList<wx_requestRuleContentModel> twList = new List<wx_requestRuleContentModel>();
            twList = dal.GetTuWenContent(rid);

            return twList;
        }


        #endregion

		#region  ExtensionMethod

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public List<wx_requestRuleContentModel> GetModelList(int Top, string strWhere, string filedOrder)
        {
            DataSet ds= dal.GetList(Top, strWhere, filedOrder);
            return DataTableToList(ds.Tables[0]);
        }

        /// <summary>
        /// 如果有该openid已经注册过会员卡信息，则拼接cardno=卡号
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        //public string cardnoStr(int wid,string openid)
        //{
        //    string ret = "";
        //    if (openid == null || openid.Trim() == "")
        //    {
        //        return "";
        //    }
        //    BLL.users ubll = new BLL.users();
        //    string cardno = ubll.getCardnoByOpenId(wid,openid);
        //    if (cardno == "")
        //    {
        //        ret = "";
        //    }
        //    else
        //    {
        //        ret = "&cardno=" + cardno;
        //    }
        //    return ret;

        //}

		#endregion  ExtensionMethod
    }
}