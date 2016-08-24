#region using region

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Innocellence.GSK.WeChat.HM.Models;
using Innocellence.GSK.WeChat.Module.Common;

#endregion

namespace XiaomiWinForm
{
    public class PersonScoreRankService
    {
        public List<PersonScoreRank> GetNewPersonStepRank(List<MetaData> allMetaData)
        {
            var personRank = new List<PersonScoreRank>();
            var wechatMap = XiaoMiData.GetWeChatNameById();
            foreach (var metadata in allMetaData)
            {
                var onePerson = new PersonScoreRank();
                if (personRank.Exists(p => p.WechatId == metadata.WechatId))
                {
                    var personScoreRank = personRank.FirstOrDefault(p => p.WechatId == metadata.WechatId);
                    if (personScoreRank != null)
                    {
                        personScoreRank.Score += metadata.Score;
                    }
                }
                else
                {
                    onePerson.Score = metadata.Score;
                    onePerson.WechatId = metadata.WechatId;
                    onePerson.WechatName = wechatMap.ContainsKey(metadata.WechatId)?wechatMap[metadata.WechatId]:"";
                    personRank.Add(onePerson);
                }
            }
            dealRewardsScore(personRank);
            var personRankResult = personRank.OrderByDescending(p => p.Score).ToList();
            for (var i = 0; i < personRankResult.Count(); i++)
            {
                personRankResult[i].Rank = i + 1;
            }
            return personRankResult;
        }

        public void dealRewardsScore(List<PersonScoreRank> personScoreRank)
        {
            var sql = @"SELECT WechatId,sum([score]) as score FROM Innocellence_GSK_WeChat_HM_Score group by WechatId";
            var conn = XiaoMiData.GetConnectstr();
            var result = new Dictionary<string, int>();
            using (SqlConnection connection = new SqlConnection(conn))
            {
                connection.Open();
                var data = SqlHelper.ExecuteReader(connection, CommandType.Text, sql);
                while (data.Read())
                {
                    result[data.GetString(0)] = data.GetInt32(1);
                }
            }
            foreach (var rewardsScore in result)
            {
                var targetPerson = personScoreRank.Find(p => p.WechatId == rewardsScore.Key);
                if (targetPerson != null)
                {
                    targetPerson.Score += rewardsScore.Value;
                }
            }
        }

        public void UpdatePersonRank(List<PersonScoreRank> personScoreRank)
        {
            var conn = XiaoMiData.GetConnectstr();
            var updateString = "begin transaction delete from Innocellence_GSK_WeChat_HM_PersonScoreRank {0} commit transaction";
            var insertSql = personScoreRank.Aggregate("", (current, scoreRank) => current + createInsertPersonRankStr(scoreRank.WechatId, scoreRank.WechatName, scoreRank.Rank, scoreRank.Score));
            SqlHelper.ExecuteNonQuery(conn, CommandType.Text, string.Format(updateString, insertSql));
        }


        private string createInsertPersonRankStr(string wechatId, string wechatName, int rank, int score)
        {
            return string.Format("INSERT INTO Innocellence_GSK_WeChat_HM_PersonScoreRank values ('{0}',N'{1}','{2}','{3}') ", wechatId, wechatName, rank, score);
        }


    }
}