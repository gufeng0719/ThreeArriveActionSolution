using Dapper;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Reflection;

namespace ThreeArriveAction.Common
{
    public class SqlHelper<T> where T : class // todo 约束特定的class 而不是泛指全部class 需要代码生产框架的支持
    {
        private readonly T _model;
        private readonly string _tableName;
        private readonly Dictionary<string, object> _updateList = new Dictionary<string, object>();
        private readonly List<WhereDictionary> _whereList = new List<WhereDictionary>();
        private readonly List<string> _updateStr = new List<string>();
        private readonly List<string> _whereStr = new List<string>();
        private readonly List<string> _showStr = new List<string>();
        private readonly List<string> _joinStr = new List<string>();
        private readonly List<JoinDictionary> _joinList = new List<JoinDictionary>();
        private readonly List<string> _sortStr = new List<string>();
        private readonly Dictionary<string, SortEnum> _sortList = new Dictionary<string, SortEnum>();
        private readonly List<string> _groupStr = new List<string>();

        public SqlHelper(T t)
        {
            _model = t;
            _tableName =
                $"{ConfigurationManager.ConnectionStrings["DATABASE"].ConnectionString}.dbo.{RemoveStrModel(_model.GetType().UnderlyingSystemType.Name)}";
            PrimaryKey = GetPrimaryKey();
            IdentityKey = GetIdentityKey();

        }

        public SqlHelper(T t, string primaryKey)
        {
            _model = t;
            _tableName =
                $"{ConfigurationManager.ConnectionStrings["DATABASE"].ConnectionString}.dbo.{RemoveStrModel(_model.GetType().UnderlyingSystemType.Name)}";
            PrimaryKey = primaryKey;
            IdentityKey = GetIdentityKey();
        }

        public SqlHelper(T t, string primaryKey, string identityKey)
        {
            _model = t;
            _tableName =
                $"{ConfigurationManager.ConnectionStrings["DATABASE"].ConnectionString}.dbo.{RemoveStrModel(_model.GetType().UnderlyingSystemType.Name)}";
            PrimaryKey = primaryKey;
            IdentityKey = identityKey;
        }

        public SqlHelper(T t, string primaryKey, string identityKey, string tableName)
        {
            _model = t;
            PrimaryKey = primaryKey;
            IdentityKey = identityKey;
            _tableName = $"{ConfigurationManager.ConnectionStrings["DATABASE"].ConnectionString}.dbo.{tableName}";
        }

        public string IdentityKey { get; }

        public string PrimaryKey { get; }

        public int Top { get; set; } = 0;

        public PageConfig PageConfig { get; set; } = new PageConfig();

        /// <summary>
        /// 方便与排查异常
        /// </summary>
        public StringBuilder SqlString { get; private set; } = new StringBuilder();

        /// <summary>
        /// 主表的别名 使用到AddJoin时,必选设置此属性
        /// </summary>
        public string Alia { get; set; } = string.Empty;


        #region AddWhere
        public void AddWhere(string where)
        {
            _whereStr.Add(where);
        }

        public void AddWhere(string field, object value)
        {
            _whereList.Add(new WhereDictionary
            {
                Field = field,
                Value = value,
                Relation = RelationEnum.Equal,
                Coexist = CoexistEnum.And
            });
        }

        public void AddWhere(string field, object value, RelationEnum relation)
        {
            _whereList.Add(new WhereDictionary
            {
                Field = field,
                Value = value,
                Relation = relation,
                Coexist = CoexistEnum.And
            });
        }

        public void AddWhere(string field, object value, RelationEnum relation, CoexistEnum coexist)
        {
            _whereList.Add(new WhereDictionary
            {
                Field = field,
                Value = value,
                Relation = relation,
                Coexist = coexist
            });
        }
        #endregion

        #region Insert
        public int Insert()
        {
            SqlString = new StringBuilder();
            var properties = _model.GetType().GetProperties();
            var fields = string.Empty;
            var values = string.Empty;
            var para = new DynamicParameters();
            foreach (PropertyInfo t in properties)
            {
                if (IsNullOrEmpty(IdentityKey) || IdentityKey != t.Name)
                {
                    fields += $"{t.Name},";
                    values += $"@{t.Name},";
                    para.Add("@" + t.Name, t.GetValue(_model, null));
                }
            }
            SqlString.Append($" INSERT INTO {_tableName} ({fields.TrimEnd(',')}) VALUES ({values.TrimEnd(',')});");
            if (!IsNullOrEmpty(IdentityKey))
            {
                SqlString.Append(" SELECT @@IDENTITY; ");
                var dt = DbClient.Query<DataTable>(SqlString.ToString(), para).FirstOrDefault();
                if (dt != null)
                {
                    var identity = dt.Rows[0][0];
                    return Convert.ToInt32(identity);
                }
            }
            return DbClient.Excute(SqlString.ToString(), para);
        }
        #endregion

        #region UPDATE
        public int Update(T model)
        {
            if (IsNullOrEmpty(IdentityKey)) throw new Exception(GetDescription(ErrorEnum.E1000));
            SqlString = new StringBuilder();
            SqlString.Append($" UPDATE {_tableName} SET ");
            var properties = model.GetType().GetProperties();
            var para = new DynamicParameters();
            var keyValue = new object();
            foreach (PropertyInfo t in properties)
            {
                if (IdentityKey != t.Name && PrimaryKey != t.Name)
                {
                    SqlString.Append($" {t.Name} = @{t.Name},");
                    para.Add("@" + t.Name, t.GetValue(model, null));
                }
                else
                {
                    if (t.Name == PrimaryKey)
                    {
                        keyValue = t.GetValue(model, null);
                    }
                }
            }
            SqlString = RemoveEndNumber(SqlString, 1);
            SqlString.Append($" WHERE {PrimaryKey} = @{PrimaryKey}_Key ");
            para.Add($"@{PrimaryKey}_Key", keyValue);
            return DbClient.Excute(SqlString.ToString(), para);
        }

        public void AddUpdate(string field, object value)
        {
            _updateList.Add(field, value);
        }

        public void AddUpdate(string update)
        {
            _updateStr.Add(update);
        }

        public int Update()
        {
            SqlString = new StringBuilder();
            var top = Top > 0 ? $"TOP({Top})" : "";
            SqlString.Append($"UPDATE {top} {_tableName} SET ");
            var para = new DynamicParameters();
            var sp = GetUpdateString();
            if (IsNullOrEmpty(sp.SqlStr))
            {
                throw new Exception(GetDescription(ErrorEnum.E1002));
            }
            SqlString.Append($"{sp.SqlStr}");
            para.AddDynamicParams(sp.Parameter);
            sp = GetWhereString();
            if (IsNullOrEmpty(sp.SqlStr))
            {
                throw new Exception(GetDescription(ErrorEnum.E1001));
            }
            SqlString.Append(" WHERE 1=1 " + sp.SqlStr);
            para.AddDynamicParams(sp.Parameter);
            return DbClient.Excute(SqlString.ToString(), para);
        }
        #endregion

        #region DELETE
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="where">where条件 以AND开头</param>
        /// <returns></returns>
        public int Delete(string where)
        {
            if (IsNullOrEmpty(where)) throw new Exception(GetDescription(ErrorEnum.E1001));
            SqlString = new StringBuilder();
            SqlString.Append($"DELECT {_tableName} WHERE 1=1 {where} ");
            return DbClient.Excute(SqlString.ToString());
        }

        public int DeleteByPrimaryKey(object id)
        {
            if (IsNullOrEmpty(PrimaryKey)) throw new Exception(GetDescription(ErrorEnum.E1000));
            SqlString = new StringBuilder();
            SqlString.Append($"DELECT {_tableName} WHERE {PrimaryKey} = @key ");
            return DbClient.Excute(SqlString.ToString(), new { @key = id });
        }

        public int Delete()
        {
            if (!_whereList.Any() && !_whereStr.Any()) throw new Exception(GetDescription(ErrorEnum.E1001));

            SqlString = new StringBuilder();
            var sq = GetWhereString();
            SqlString.Append($"DELETE {_tableName} WHERE 1=1 {sq.SqlStr}");
            var para = new DynamicParameters();
            para.AddDynamicParams(sq.Parameter);
            return DbClient.Excute(SqlString.ToString(), para);

        }
        #endregion

        #region SELECT
        public void AddShow(string shows)
        {
            _showStr.Add(shows);
        }

        public void AddShow(Enum show)
        {
            _showStr.Add(show.ToString());
        }

        public void AddJoin(string join)
        {
            _joinStr.Add(join);
        }

        /// <summary>
        /// 添加表连接关系
        /// </summary>
        /// <param name="relationJoin">连接关系</param>
        /// <param name="thatTable">另一个表</param>
        /// <param name="thatAlia">另一个别名</param>
        /// <param name="relationField">主关联字段</param>
        /// <param name="thatRelationField">另一个关联字段</param>
        /// <param name="where">链接之前的筛选(以AND开头)</param>
        public void AddJoin(JoinEnum relationJoin, string thatTable, string thatAlia, string relationField, string thatRelationField, string where = "")
        {
            _joinList.Add(new JoinDictionary
            {
                RelationJoin = relationJoin,
                RelationField = relationField,
                ThatAlia = thatAlia,
                ThatRelationField = thatRelationField,
                ThatTable = thatTable,
                Where = where
            });
        }

        public void AddOrder(string order)
        {
            _sortStr.Add(order);
        }

        public void AddOrder(string field, SortEnum sort)
        {
            _sortList.Add(field, sort);
        }

        public void AddGroup(string field)
        {
            _groupStr.Add(field);
        }

        public IEnumerable<T> Select()
        {
            var isPage = PageConfig.PageIndex > 0 && PageConfig.PageSize > 0;
            SqlString = new StringBuilder();
            if (isPage)
            {
                string pageSortField;
                if (IsNullOrEmpty(PageConfig.PageSortField.Trim()))
                {
                    pageSortField = IsNullOrEmpty(IdentityKey.Trim()) ? PrimaryKey : IdentityKey;
                    pageSortField = Alia + "." + pageSortField;
                }
                else
                {
                    pageSortField = PageConfig.PageSortField;
                }
                SqlString.Append($"SELECT * FROM (SELECT TOP({PageConfig.PageSize}) ROW_NUMBER() OVER ( ORDER BY " +
                                 $"{pageSortField} {GetDescription(PageConfig.SortEnum)}" +
                                 $" ) AS ROWNUMBER ,{GetShowString()} FROM {_tableName} ");
            }
            else
            {
                var top = Top > 0 ? $"TOP({Top})" : "";
                SqlString.Append($"SELECT {top} {GetShowString()} FROM {_tableName} ");
            }
            var join = GetJoinString();
            if (!IsNullOrEmpty(join.Trim()))
            {
                if (IsNullOrEmpty(Alia.Trim()))
                {
                    throw new Exception(GetDescription(ErrorEnum.E1003));
                }
                SqlString.Append(join);
            }
            var para = new DynamicParameters();
            var sp = GetWhereString();
            para.AddDynamicParams(sp.Parameter);
            SqlString.Append(" WHERE 1=1 " + sp.SqlStr);

            var group = GetGroupString();
            if (!IsNullOrEmpty(group.Trim()))
            {
                SqlString.Append(" GROUP BY " + group);
            }
            var sort = GetSortString();
            if (!IsNullOrEmpty(sort.Trim()))
            {
                SqlString.Append(" ORDER BY " + sort);
            }
            if (isPage)
            {
                SqlString.Append($") A WHERE ROWNUMBER BETWEEN {(PageConfig.PageIndex - 1) * PageConfig.PageSize + 1} AND {PageConfig.PageIndex * PageConfig.PageSize} ");
            }
            return DbClient.Query<T>(SqlString.ToString(), para);
        }

        #endregion

        #region 简单查询
        public IEnumerable<T> GetModelList(string field, object value, string show = "")
        {
            GetSelectSql(field, value, show);
            return DbClient.Query<T>(SqlString.ToString());
        }

        public T GetModel(string key, object value, string show = "")
        {
            GetSelectSql(key, value, show);
            return DbClient.Query<T>(SqlString.ToString()).FirstOrDefault();
        }
        #endregion

        #region 针对一些不太好生成的 sql 可选用此中的方法, 自定义性质比较强
        public IEnumerable<T> GetModelListBySql(string sql)
            => DbClient.Query<T>(sql);

        public IEnumerable<T> GetModelList(string where, string show = "")
        {
            GetSelectSql(where, show);
            return DbClient.ExecuteScalar<IEnumerable<T>>(SqlString.ToString());
        }

        public IEnumerable<dynamic> GetDynamic(string where, string show = "")
        {
            GetSelectSql(where, show);
            return DbClient.Query<dynamic>(SqlString.ToString());
        }
        #endregion


        private void GetSelectSql(string where, string show = "")
        {
            var top = Top > 0 ? $"TOP ({Top})" : "";
            SqlString = new StringBuilder();
            SqlString.Append(IsNullOrEmpty(show)
                ? $" SELECT {top} * FROM {_tableName} WHERE 1=1 {where}"
                : $" SELECT {top} {show} FROM {_tableName} WHERE 1=1 {where}");
        }

        private void GetSelectSql(string field, object value, string show = "")
        {
            SqlString = new StringBuilder();
            var top = Top > 0 ? $"TOP ({Top})" : "";
            SqlString.Append(IsNaN(value)
                ? (IsNullOrEmpty(show)
                    ? $" SELECT {top} * FROM {_tableName} WHERE {field} = '{value}' "
                    : $" SELECT {top} {show} FROM {_tableName} WHERE {field} = '{value}' ")
                : (IsNullOrEmpty(show)
                    ? $" SELECT {top} * FROM {_tableName} WHERE {field} = {value} "
                    : $" SELECT {top} {show} FROM {_tableName} WHERE {field} = {value} "));
        }

        private string GetIdentityKey()
        {
            var field = string.Empty;
            var strSql =
                $@"USE {ConfigurationManager.ConnectionStrings["DATABASE"].ConnectionString};
                   SELECT ( CASE WHEN COLUMNPROPERTY(id, name, 'IsIdentity') = 1
                                               THEN '1'
                                               ELSE '0'
                                          END ) as identityKey,
                                        name
                              FROM      syscolumns
                              WHERE     id = OBJECT_ID ('{_tableName}') ";
            var list = DbClient.Query<dynamic>(strSql);
            var model = list.FirstOrDefault(x => x.identityKey.ToString() == "1");
            if (model != null) field = (model.name ?? "").ToString();
            return field;
        }

        public string GetPrimaryKey()
        {
            var strSql = $"USE {ConfigurationManager.ConnectionStrings["DATABASE"].ConnectionString};EXEC sp_pkeys @table_name='{RemoveStrModel(_model.GetType().UnderlyingSystemType.Name)}'";
            var dr = DbClient.Query<dynamic>(strSql).FirstOrDefault();

            // ReSharper disable once PossibleNullReferenceException
            return dr.COLUMN_NAME.ToString();
        }

        private bool IsNaN(object value)
            => !(value is int || value is long || value is double || value is float || value is byte || value is decimal || value is short || value is ushort || value is sbyte);

        public StringBuilder RemoveEndNumber(StringBuilder sbStr, int number)
        {
            if (sbStr.Length < number)
            {
                return sbStr;
            }
            return sbStr.Remove(sbStr.Length - number, number);
        }

        private string GetDescription(Enum enumItemName)
        {
            var fi = enumItemName.GetType().GetField(enumItemName.ToString());

            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            return enumItemName.ToString();
        }

        private SqlAndParameter GetUpdateString()
        {
            var re = new SqlAndParameter();
            var count = 0;
            foreach (var update in _updateList)
            {
                re.SqlStr += $"{update.Key}=@{update.Key}{count},";
                re.Parameter.Add($"@{update.Key}{count}", update.Value);
                count++;
            }
            foreach (var update in _updateStr)
            {
                re.SqlStr += update + ",";
            }
            re.SqlStr = re.SqlStr.TrimEnd(',');
            return re;
        }

        private SqlAndParameter GetWhereString()
        {
            var re = new SqlAndParameter();
            var count = 0;
            foreach (var where in _whereList)
            {
                if (where.Relation == RelationEnum.In)
                {
                    re.SqlStr += $"{GetDescription(where.Coexist)} {where.Field} IN (@{where.Field.Replace(".", "_")}{count}) ";
                }
                else if (where.Relation == RelationEnum.NotIn)
                {
                    re.SqlStr += $"{GetDescription(where.Coexist)} {where.Field} NOT IN (@{where.Field.Replace(".", "_")}{count}) ";
                }
                else if (where.Relation == RelationEnum.IsNotNull || where.Relation == RelationEnum.IsNull)
                {
                    re.SqlStr += $"{GetDescription(where.Coexist)} {where.Field} {GetDescription(where.Relation)} ";
                }
                else if (where.Relation == RelationEnum.Like)
                {
                    re.SqlStr += $"{GetDescription(where.Coexist)} {where.Field} LIKE '%'+@{where.Field.Replace(".", "_")}{count}+'%' ";
                }
                else if (where.Relation == RelationEnum.LeftLike)
                {
                    re.SqlStr += $"{GetDescription(where.Coexist)} {where.Field} LIKE '%'+@{where.Field.Replace(".", "_")}{count} ";
                }
                else if (where.Relation == RelationEnum.RightLike)
                {
                    re.SqlStr += $"{GetDescription(where.Coexist)} {where.Field} LIKE @{where.Field.Replace(".", "_")}{count}+'%' ";
                }
                else
                {
                    re.SqlStr += $"{GetDescription(where.Coexist)} {where.Field} {GetDescription(where.Relation)} @{where.Field.Replace(".", "_")}{count} ";
                }
                re.Parameter.Add($"@{where.Field.Replace(".", "_")}{count}", where.Value);
                count++;
            }
            foreach (var where in _whereStr)
            {
                re.SqlStr += where;
            }

            return re;
        }

        private string GetShowString()
        {
            var str = string.Join(",", _showStr).TrimEnd(',');
            return IsNullOrEmpty(str) ? " * " : str;
        }

        private string GetSortString()
        {
            var strSql = _sortList
                .Aggregate("",
                    (current, sort) =>
                        current + $"{sort.Key} {GetDescription(sort.Value)},");
            return _sortStr.Aggregate(strSql, (current, sort) => current + $"{sort},").TrimEnd(',');
        }

        private string GetGroupString()
            => _groupStr.Aggregate("", (current, @group) => current + group + ",").TrimEnd(',');

        private string GetJoinString()
        {
            var strSql = $" {Alia} ";
            strSql = _joinList
                .Aggregate(strSql,
                (current, @join)
                => current +
                $@" {GetDescription(join.RelationJoin)} {ConfigurationManager.ConnectionStrings["DATABASE"].ConnectionString}.dbo.{join.ThatTable} 
                    {join.ThatAlia} ON {Alia}.{join.RelationField} = {join.ThatAlia}.{join.ThatRelationField} {join.Where}");
            if (_joinStr.Any())
            {
                strSql += _joinStr.Aggregate(strSql, (current, join) => current + $" {join} ");
            }
            return strSql;
        }

        /// <summary>
        /// 移除 T 后面可能出现的个别字符串
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string RemoveStrModel(string name)
        {
            var re = name.ToLower();
            if (re.IndexOf("model") == re.Length - 5)
            {
                re = name.Remove(name.Length - 5);
            }
            if (re.Remove(0, re.Length - 1) == "_" || re.Remove(0, re.Length - 1) == ".")
            {
                re = name.Remove(name.Length - 1);
            }
            return re;
        }

        private bool IsNullOrEmpty(string str)
        {
            return string.IsNullOrEmpty(str.Trim());
        }

    }

    public class PageConfig
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; } = 0;

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; } = 0;

        /// <summary>
        /// 分页关键排序
        /// </summary>
        public string PageSortField { get; set; } = string.Empty;

        /// <summary>
        /// 排序类型
        /// </summary>
        public SortEnum SortEnum { get; set; }
    }


    class WhereDictionary
    {
        public string Field { get; set; } = string.Empty;
        public object Value { get; set; } = new object();
        public RelationEnum Relation { get; set; }
        public CoexistEnum Coexist { get; set; }
    }

    class SqlAndParameter
    {
        public string SqlStr { get; set; } = string.Empty;
        public DynamicParameters Parameter { get; set; } = new DynamicParameters();
    }

    class JoinDictionary
    {
        public JoinEnum RelationJoin { get; set; }
        public string ThatTable { get; set; } = string.Empty;
        public string ThatAlia { get; set; } = string.Empty;
        public string RelationField { get; set; } = string.Empty;
        public string ThatRelationField { get; set; } = string.Empty;
        public string Where { get; set; } = string.Empty;
    }

    /// <summary>
    /// 键值关系
    /// </summary>
    public enum RelationEnum
    {
        /// <summary>
        /// 等于
        /// </summary>
        [Description("=")]
        Equal,
        /// <summary>
        /// 不等于
        /// </summary>
        [Description("<>")]
        NotEqual,
        /// <summary>
        /// in
        /// </summary>
        [Description("IN")]
        In,
        /// <summary>
        /// NotIn
        /// </summary>
        [Description("NOT IN")]
        NotIn,
        /// <summary>
        /// 大于
        /// </summary>
        [Description(">")]
        Greater,
        /// <summary>
        /// 大于等于
        /// </summary>
        [Description(">=")]
        GreaterEqual,
        /// <summary>
        /// 小于
        /// </summary>
        [Description("<")]
        Less,
        /// <summary>
        /// 小于等于
        /// </summary>
        [Description("<=")]
        LessEqual,
        /// <summary>
        /// 匹配
        /// </summary>
        [Description("LIKE")]
        Like,
        /// <summary>
        /// 右匹配
        /// </summary>
        [Description("LIKE")]
        RightLike,
        /// <summary>
        /// 左匹配
        /// </summary>
        [Description("LIKE")]
        LeftLike,
        /// <summary>
        /// 是
        /// </summary>
        [Description("IS")]
        IsNull,
        /// <summary>
        /// 不是
        /// </summary>
        [Description("IS NOT NULL")]
        IsNotNull
    }

    /// <summary>
    /// 向前条件的并存关系
    /// </summary>
    public enum CoexistEnum
    {
        /// <summary>
        /// AND 关系
        /// </summary>
        [Description("AND")]
        And,
        /// <summary>
        /// OR 关系
        /// </summary>
        [Description("OR")]
        Or
    }

    /// <summary>
    /// 排序关系
    /// </summary>
    public enum SortEnum
    {
        /// <summary>
        /// 正序
        /// </summary>
        [Description("ASC")]
        Asc,
        /// <summary>
        /// 倒序
        /// </summary>
        [Description("DESC")]
        Desc,
    }

    /// <summary>
    /// 表链接 关系
    /// </summary>
    public enum JoinEnum
    {
        /// <summary>
        /// 链接
        /// </summary>
        [Description("JOIN")]
        Join,
        /// <summary>
        /// 链接
        /// </summary>
        [Description("INNER JOIN")]
        InnerJoin,
        /// <summary>
        /// 左链接
        /// </summary>
        [Description("LEFT JOIN")]
        LeftJoin,
        /// <summary>
        /// 有链接
        /// </summary>
        [Description("RIGHT JOIN")]
        RightJoin
    }

    /// <summary>
    /// 错误枚举
    /// </summary>
    public enum ErrorEnum
    {
        /// <summary>
        /// 当前表结构缺少PrimaryKey
        /// </summary>
        [Description("当前表结构缺少PrimaryKey")]
        E1000,
        /// <summary>
        /// 当前操作必须传入条件限制
        /// </summary>
        [Description("当前操作必须传入条件限制")]
        E1001,
        /// <summary>
        /// 当前操作必须传入UPDATE字段和值
        /// </summary>
        [Description("当前操作必须传入UPDATE字段和值")]
        E1002,
        /// <summary>
        /// 当您尝试 JOIN 时,请先设置 Alia 值
        /// </summary>
        [Description("当您尝试 JOIN 时,请先设置 Alia 值")]
        E1003
    }

    public class DbClient
    {
        public static IEnumerable<T> Query<T>(string sql, object param = null)
        {
            if (string.IsNullOrEmpty(sql))
            {
                throw new ArgumentNullException(nameof(sql));
            }
            using (IDbConnection con = DataSource.GetConnection())
            {
                IEnumerable<T> tList;
                try
                {
                    tList = con.Query<T>(sql, param);
                    con.Close();
                    return tList;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + "------------ SQL:" + sql);
                }
            }
        }

        public static int Excute(string sql, object param = null, IDbTransaction transaction = null)
        {
            if (string.IsNullOrEmpty(sql))
            {
                throw new ArgumentNullException(nameof(sql));
            }
            using (IDbConnection con = DataSource.GetConnection())
            {
                var line = 0;
                try
                {
                    line = con.Execute(sql, param, transaction);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + "-------------- SQL:" + sql);
                }
                return line;
            }
        }

        public static T ExecuteScalar<T>(string sql, object param = null)
        {
            if (string.IsNullOrEmpty(sql))
            {
                throw new ArgumentNullException(nameof(sql));
            }
            using (IDbConnection con = DataSource.GetConnection())
            {
                return con.ExecuteScalar<T>(sql, param);
            }
        }

        public static T ExecuteScalarProc<T>(string strProcName, object param = null)
        {
            using (IDbConnection con = DataSource.GetConnection())
            {
                return (T)con.ExecuteScalar(strProcName, param, commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// 执行带参数的存储过程(查询)
        /// </summary>
        /// <param name="strProcName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static IEnumerable<T> ExecuteQueryProc<T>(string strProcName, object param = null)
        {
            using (IDbConnection con = DataSource.GetConnection())
            {
                IEnumerable<T> tList = con.Query<T>(strProcName, param, commandType: CommandType.StoredProcedure);
                con.Close();
                return tList;
            }
        }

        /// <summary>
        /// 执行带参数的存储过程
        /// </summary>
        /// <param name="strProcName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static int ExecuteProc(string strProcName, object param = null)
        {
            try
            {
                using (IDbConnection con = DataSource.GetConnection())
                {
                    return con.Execute(strProcName, param, commandType: CommandType.StoredProcedure);
                }
            }
            catch
            {
                return 0;
            }
        }
    }

    public class DataSource
    {
        public static string ConnString = ConfigurationManager.ConnectionStrings["DBString"].ConnectionString;
        public static IDbConnection GetConnection()
        {
            if (string.IsNullOrEmpty(ConnString))
                throw new NoNullAllowedException(nameof(ConnString));
            return new SqlConnection(ConnString);
        }
    }
}
