using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Innocellence.GSK.WeChat.HM.Models;
using Innocellence.GSK.WeChat.Module.Common;

namespace XiaomiWinForm
{
    public class GroupInfoMapService
    {
        private Dictionary<string, string> getGroupAndUserMap()
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
            var eightDayMetaData = metaDataService.GetAllMetaData();
            foreach (var metaData in eightDayMetaData)
            {
                var groupInfo = new GroupScoreRank();
                if (result.Exists(m => m.GroupName == (groupMap.ContainsKey(metaData.WechatId) ? groupMap[metaData.WechatId] : "Other")))
                {
                    var currentDate=result.Find(g => g.ScoreData == metaData.CreatedDate);
                    if (currentDate != null)
                    {
                        currentDate.Score += metaData.Score;
                    }
                    else
                    {
                        groupInfo.GroupName = groupMap.ContainsKey(metaData.WechatId) ? groupMap[metaData.WechatId] : "Other";
                        groupInfo.Score = metaData.Score;
                        groupInfo.ScoreData = metaData.CreatedDate;
                        result.Add(groupInfo);
                    }
                }
                else
                {
                    groupInfo.GroupName = groupMap.ContainsKey(metaData.WechatId) ? groupMap[metaData.WechatId] : "Other";
                    groupInfo.Score = metaData.Score;
                    groupInfo.ScoreData = metaData.CreatedDate;
                    result.Add(groupInfo);
                }
            }
            return result.OrderBy(r=>r.ScoreData).ToList();
        }

        public void UpdateGroupRank(List<GroupScoreRank> group)
        {
            var conn = XiaoMiData.GetConnectstr();
            var sql = "begin transaction delete from Innocellence_GSK_WeChat_HM_GroupScoreRank {0} commit transaction";
            var insertSql = string.Format(sql, group.Aggregate("", (current, currentGroup) => current + createInsertGroupRankStr(currentGroup)));
            SqlHelper.ExecuteNonQuery(conn, CommandType.Text, string.Format(sql, insertSql));
        }

        private string createInsertGroupRankStr(GroupScoreRank group)
        {
            return string.Format("INSERT INTO Innocellence_GSK_WeChat_HM_GroupScoreRank values ('{0}',N'{1}','{2}','{3}','{4}') ",null,group.GroupName, group.Rank, group.Score, group.ScoreData);
        }
    }
}