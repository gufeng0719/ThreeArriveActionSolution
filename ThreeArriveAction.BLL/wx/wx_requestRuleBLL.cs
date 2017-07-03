using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThreeArriveAction.DAL;
using ThreeArriveAction.Model;
using System.Data;
namespace ThreeArriveAction.BLL
{
    /// <summary>
    /// 微信请求回复规则表
    /// </summary>
    public class wx_requestRuleBLL
    {
        private readonly wx_requestRuleDAL dal = new  wx_requestRuleDAL();
        public wx_requestRuleBLL()
        { }
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
        public int Add(wx_requestRuleModel model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(wx_requestRuleModel model)
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
        public bool DeleteList(string idlist)
        {
            return dal.DeleteList(idlist);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public wx_requestRuleModel GetModel(int id)
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
        public DataSet GetList(int Top, string strWhere, string filedOrder)
        {
            return dal.GetList(Top, strWhere, filedOrder);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<wx_requestRuleModel> GetModelList(string strWhere)
        {
            DataSet ds = dal.GetList(strWhere);
            return DataTableToList(ds.Tables[0]);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<wx_requestRuleModel> DataTableToList(DataTable dt)
        {
            List<wx_requestRuleModel> modelList = new List<wx_requestRuleModel>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                wx_requestRuleModel model;
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
            return dal.GetListByPage(strWhere, orderby, startIndex, endIndex);
        }
        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        //public DataSet GetList(int PageSize,int PageIndex,string strWhere)
        //{
        //return dal.GetList(PageSize,PageIndex,strWhere);
        //}

        #endregion  BasicMethod
        #region  ExtensionMethod



        /// <summary>
        /// 获得查询分页数据(文本和语音)
        /// </summary>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }


        /// <summary>
        /// 获得查询分页数据(图文的关键词列表)
        /// </summary>
        public DataSet GetTWKeyWordList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return dal.GetTWKeyWordList(pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }



        /// <summary>
        ///关注时，非图文内容，则删除该条微信回复的规则，并且将其对应的内容删除
        /// </summary>
        public bool DeleteRule(int id)
        {
            return dal.DeleteRule(id);
        }

        /// <summary>
        /// 通过试图获得回复规则以及规则对应的内容
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public DataSet GetRuleContent(string strWhere)
        {
            return dal.GetRuleContent(strWhere);
        }

        /// <summary>
        /// 通过试图获得回复规则以及规则对应的内容
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public DataSet GetRuleContent(int top, string strWhere)
        {
            return dal.GetRuleContent(top, strWhere);
        }


        /// <summary>
        /// 功能模块添加图文回复规则【关键词匹配为精确查询】
        /// </summary>
        /// <param name="wid">微帐号主键</param>
        /// <param name="modelName">模块名称</param>
        /// <param name="modelId">模块的主键</param>
        /// <param name="keyword">关键词</param>
        public  void AddModeltxtPicRule(int wid, string modelName, int modelId, string keyword)
        {
            wx_requestRuleModel rule = new  wx_requestRuleModel();
            rule.wId = wid;
            rule.reqKeywords = keyword;
            rule.modelFunctionName = modelName;
            rule.ruleName = "功能模块图片回复";
            rule.modelFunctionId = modelId;
            rule.createDate = DateTime.Now;
            rule.reqestType = 1;
            rule.responseType = 2;
            rule.isLikeSearch = false;
            this.Add(rule);
        }


        #endregion  ExtensionMethod

        #region 微信前端使用，需要高效率
        /// <summary>
        /// 得到回复规则的主键Id
        /// </summary>
        /// <param name="wid">微帐号主键Id</param>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public int GetRuleId(int wid, string strWhere)
        {
            return dal.GetRuleId(wid, strWhere);
        }

        public int GetRuleIdAndResponseType(int wid, string strWhere, out int responseType)
        {
            return dal.GetRuleIdAndResponseType(wid, strWhere, out  responseType);
        }

        /// <summary>
        /// 关键词查询，找到该规则对应的主键ID,使用存储过程
        /// </summary>
        /// <param name="wid"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public int GetRuleIdByKeyWords(int wid, string keywords, out int responseType, out string modelFunctionName, out int modelFunctionId)
        {
            return dal.GetRuleIdByKeyWords(wid, keywords, out responseType, out modelFunctionName, out modelFunctionId);
        }


        #endregion
    }
}
