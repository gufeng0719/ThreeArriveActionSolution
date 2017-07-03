﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThreeArriveAction.Common;
using ThreeArriveAction.DAL;
using ThreeArriveAction.Model;
namespace ThreeArriveAction.BLL
{
   public partial class sys_SignsBLL
    {
       private readonly sys_SignsDAL signsDAL = new sys_SignsDAL();
       private readonly sys_OnSignsBLL onSignBLL = new sys_OnSignsBLL();
        #region 添加
       /// <summary>
       /// 根据用户编号添加签到信息
       /// </summary>
       /// <param name="userId">用户编号</param>
       /// <param name="villageId">村居编号</param>
       /// <returns></returns>
       public string AddSign(int userId,int villageId,int organizationId)
       {
           JsonMessage json = new JsonMessage();
           
           //获取该村是否开启签到
           if (!onSignBLL.ExistsByVillageId(villageId))
           {
               //不存在，则开启签到
               if (organizationId == 4)//普通村居干部，不具备开启签到
               {
                   json.success = false;
                   json.msg = "还未开启签到";
               }
               else
               {
                   //该村负责人开启签到
                   sys_OnSignsModel onSignsModel = new sys_OnSignsModel();
                   onSignsModel.VillageId = villageId;
                   onSignsModel.UserId = userId;
                   onSignsModel.OnTime = DateTime.Now;
                   int number = onSignBLL.AddOnSign(onSignsModel);
                   if (number == 0)
                   {
                       json.success = false;
                       json.msg = "出错";
                       return JsonHelper.ToJson(json);
                   }
               }
           }
           int sid = signsDAL.AddSign(userId);
           if (sid > 0)
           {
               json.success = true;
               json.msg = "签到成功";
           }
           else
           {
               json.success = false;
               json.msg = "签到失败";
           }
           return JsonHelper.ToJson(json);
       }

        #endregion
        #region 查询
       /// <summary>
       /// 根据用户编号与签到日期查询是否存在签到信息
       /// </summary>
       /// <param name="userId">用户编号</param>
       /// <param name="signDate">签到日期</param>
       /// <returns></returns>
       public bool Exists(int userId, DateTime signDate)
       {
           return signsDAL.Exists(userId, signDate);

       }
        #endregion
    }
}
