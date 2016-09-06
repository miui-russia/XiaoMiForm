#region using region

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Innocellence.GSK.WeChat.HM.Models;
using Innocellence.GSK.WeChat.Module.Common;
using Innocellence.Xiaomi;

#endregion

namespace XiaomiWinForm
{
    public class MetadataService
    {
        public void UpdateMetaData(Setting personSetting, DataResponse xiaoMiData)
        {
            var conn = XiaoMiData.GetConnectstr();
            var selectString = "select * from Innocellence_GSK_WeChat_HM_MetaData where WechatId='{0}'and CreatedDate='{1}' ";
            var updateString = "update Innocellence_GSK_WeChat_HM_MetaData set Steps={0},Score={1} where WechatId='{2}'and CreatedDate='{3}'";
            var createString = "insert into Innocellence_GSK_WeChat_HM_MetaData(WechatId,CreatedDate,Steps,Score) values ('{0}','{1}',{2},{3})";
            var data = SqlHelper.ExecuteDataset(conn, CommandType.Text, string.Format(selectString, personSetting.WechatId, xiaoMiData.date)).Tables[0];
            var metaDataResult = SqlHelper.ConvertTo<MetaData>(data).ToList();
            if (metaDataResult.Count > 0)
            {
                SqlHelper.ExecuteNonQuery(conn, CommandType.Text, string.Format(updateString, xiaoMiData.step, (int.Parse(xiaoMiData.step) > 7999 ? 1 : 0), personSetting.WechatId, xiaoMiData.date));
            }
            else
            {
                SqlHelper.ExecuteNonQuery(conn, CommandType.Text, string.Format(createString, personSetting.WechatId, xiaoMiData.date, xiaoMiData.step, (int.Parse(xiaoMiData.step) > 8000 ? 1 : 0)));
            }
        }

        public List<MetaData> GetAllMetaData()
        {
            var conn = XiaoMiData.GetConnectstr();
            var sql = @"SELECT * From Innocellence_GSK_WeChat_HM_MetaData";
            var data = SqlHelper.ExecuteDataset(conn, CommandType.Text, sql).Tables[0];
            var result = SqlHelper.ConvertTo<MetaData>(data).ToList();
            return result;
        }

        public List<MetaData> GetAllMetaData(DateTime fromDate,DateTime toDate)
        {
            var conn = XiaoMiData.GetConnectstr();
            var sql = @"SELECT * From Innocellence_GSK_WeChat_HM_MetaData where CreatedDate>='"+fromDate.Date+"' and CreatedDate<='"+toDate.Date+"'";
            var data = SqlHelper.ExecuteDataset(conn, CommandType.Text, sql).Tables[0];
            var result = SqlHelper.ConvertTo<MetaData>(data).ToList();
            return result;
        }

        public void dealRewardMetadata(List<MetaData> allMetadata)
        {
            var sql = @"select * from Innocellence_GSK_WeChat_HM_Score";
            var conn = XiaoMiData.GetConnectstr();
            var data = SqlHelper.ExecuteDataset(conn, CommandType.Text, sql).Tables[0];
            var result = SqlHelper.ConvertTo<Score>(data).ToList();
            foreach (var score in result)
            {
                var traget=allMetadata.Find(string.Compare(m => m.WechatId.ToLower() , score.WechatId.ToLower(),true)==0&&DateTime.Compare(m.CreatedDate.Date,score.CreatedDate.Date)==0);
                if (traget != null)
                {
                    traget.Score += score.score;                    
                }
            }
        }
    }
}