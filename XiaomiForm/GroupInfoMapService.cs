using System;
using System.Collections.Generic;
using System.Data;
using Innocellence.GSK.WeChat.HM.Models;
using Innocellence.GSK.WeChat.Module.Common;

namespace XiaomiWinForm
{
    public class GroupInfoMapService
    {
        public Dictionary<string, string> getGroupAndUserMap()
        {
            var result = new Dictionary<string, string>();
            var conn = XiaoMiData.GetConnectstr();
            var sql = @"select WechatId,GroupName from Innocellence_GSK_WeChat_IPP_GroupInfo";
            var data = SqlHelper.ExecuteReader(conn, CommandType.Text, sql);
            while (data.Read())
            {
                result[data.GetString(0)] = data.GetString(1);
            }
            return result;
        }

        public List<GroupScoreRank> getGroupScoreRank()
        {
            var result = new List<GroupScoreRank>();
            var metaDataService = new MetadataService();
            var groupMap = getGroupAndUserMap();
            var eightDayMetaData = metaDataService.GetAllMetaData(DateTime.Now.Date.AddDays(-8), DateTime.Now.Date);
            foreach (var metaData in eightDayMetaData)
            {
                var groupInfo = new GroupScoreRank();
                if (result.Exists(m => m.GroupName == (groupMap.ContainsKey(metaData.WechatId) ? groupMap[metaData.WechatId] : "Other")))
                {}
                else
                {
                    groupInfo.GroupName = groupMap.ContainsKey(metaData.WechatId) ? groupMap[metaData.WechatId] : "Other";
                    groupInfo.Score = metaData.Score;
                }
            }
            return result;
        }
    }
}