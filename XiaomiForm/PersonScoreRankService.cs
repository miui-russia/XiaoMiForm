#region using region

using System.Collections.Generic;
using System.Data;
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
                    onePerson.WechatName = XiaoMiData.GetWeChatNameById(metadata.WechatId);
                    personRank.Add(onePerson);
                }
            }
            var personRankResult = personRank.OrderByDescending(p => p.Score).ToList();
            for (var i = 0; i < personRankResult.Count(); i++)
            {
                personRankResult[i].Rank = i + 1;
            }
            return personRankResult;
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