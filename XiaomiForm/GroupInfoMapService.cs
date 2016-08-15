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
            var sql = @"select Account,GroupName from Innocellence_GSK_WeChat_IPP_GroupInfo";
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
                    var currentDate = result.Find(g => g.ScoreData == metaData.CreatedDate && g.GroupName == groupMap[metaData.WechatId]);
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
            return DealSumScore(result);
        }

        public List<GroupScoreRank> DealSumScore(List<GroupScoreRank> groupList)
        {
            var allGroupName = new List<string> { "Medical China", "NS", "PTS", "Combo"};
            var allDateTime = new List<DateTime>();
            for (var i = 0; i<=(DateTime.Now.Date-new DateTime(2016,8,1).Date).Days ; i++)
            {
                allDateTime.Add(DateTime.Now.AddDays(-i).Date);
            }
            foreach (var newGroupInfo in from currentGroup in allGroupName from currentDate in allDateTime let groupInfo = groupList.FirstOrDefault(g => g.GroupName == currentGroup && g.ScoreData == currentDate) where groupInfo == null select new GroupScoreRank()
            {
                GroupName = currentGroup,
                ScoreData = currentDate,
                Score = 0
            })
            {
                groupList.Add(newGroupInfo);
            }
            foreach (var groupName in allGroupName)
            {
                foreach (var createDate in allDateTime)
                {
                    var currentGroup = groupList.Find(g => g.GroupName == groupName && (DateTime.Compare(g.ScoreData, createDate) == 0));
                    foreach (var tempGroup in groupList.FindAll(g => g.GroupName == currentGroup.GroupName && (DateTime.Compare(g.ScoreData, createDate) != 1)))
                    {
                        currentGroup.TotalScore += tempGroup.Score;
                    }
                }
            }
            return groupList.OrderBy(g=>g.GroupName).ThenBy(g=>g.ScoreData).ToList();;
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
            return string.Format("INSERT INTO Innocellence_GSK_WeChat_HM_GroupScoreRank values ('{0}',N'{1}','{2}','{3}','{4}','{5}') ",null,group.GroupName, group.Rank, group.Score, group.ScoreData,group.TotalScore);
        }
    }
}