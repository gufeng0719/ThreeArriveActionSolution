using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThreeArriveAction.DAL;
using ThreeArriveAction.Model;
using ThreeArriveAction.Common;

namespace ThreeArriveAction.BLL
{
    public partial class sys_configBLL
    {
        private readonly sys_configDAL dal = new sys_configDAL();

        /// <summary>
        ///  读取配置文件
        /// </summary>
        public siteconfig loadConfig()
        {
            siteconfig model = CacheHelper.Get<siteconfig>(MXKeys.CACHE_SITE_CONFIG);
            if (model == null)
            {
                CacheHelper.Insert(MXKeys.CACHE_SITE_CONFIG, dal.loadConfig(Utils.GetXmlMapPath(MXKeys.FILE_SITE_XML_CONFING)),
                    Utils.GetXmlMapPath(MXKeys.FILE_SITE_XML_CONFING));
                model = CacheHelper.Get<siteconfig>(MXKeys.CACHE_SITE_CONFIG);
            }
            return model;
        }

        /// <summary>
        ///  保存配置文件
        /// </summary>
        public siteconfig saveConifg(siteconfig model)
        {
            return dal.saveConifg(model, Utils.GetXmlMapPath(MXKeys.FILE_SITE_XML_CONFING));
        }

    }
}
