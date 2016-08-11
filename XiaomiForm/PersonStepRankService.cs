#region using region

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Innocellence.GSK.WeChat.HM.Models;
using Innocellence.GSK.WeChat.Module.Common;

#endregion

namespace XiaomiWinForm
{
    public class PersonStepRankService
    {
        public List<PersonStepRank> GetPersonStepRankList(List<MetaData> allMetadata)
        {
            var personStepRankList = new List<PersonStepRank>();
            foreach (var metadata in allMetadata)
            {
                var personStepRank = new PersonStepRank();
                if (personStepRankList.Exists(p => p.WechatId == metadata.WechatId))
                {
                    var target = personStepRankList.FirstOrDefault(p => p.WechatId == metadata.WechatId);
                    if (target != null)
                    {
                        target.Steps += metadata.Steps;
                        target.StepsByDay = DateTime.Compare(metadata.CreatedDate, DateTime.Now.Date) == 0 ? metadata.Steps : 0;
                    }
                }
                else
                {
                    personStepRank.StepsByDay = DateTime.Compare(metadata.CreatedDate, DateTime.Now.Date) == 0 ? metadata.Steps : 0;
                    personStepRank.RankByDay = GetPersonStepRankByWeChatId(metadata.WechatId);
                    personStepRank.Steps = metadata.Steps;
                    personStepRank.WechatId = metadata.WechatId;
                    personStepRank.WechatName = XiaoMiData.GetWeChatNameById(metadata.WechatId);
                    personStepRankList.Add(personStepRank);
                }
            }
            var personStepRankListResult = personStepRankList.OrderByDescending(m => m.Steps).ToList();
            for (var i = 0; i < personStepRankListResult.Count(); i++)
            {
                personStepRankListResult[i].Rank = i + 1;
            }
            return personStepRankListResult;
        }

        public int GetPersonStepRankByWeChatId(string WechatId)
        {
            var result = 0;
            var conn = XiaoMiData.GetConnectstr();
            var sqlString = "select stepRank from (select WechatId,rank() over (order by steps desc)AS stepRank from Innocellence_GSK_WeChat_HM_MetaData where CreatedDate='{0}') as rankTable where WechatId='{1}'";
            var data = SqlHelper.ExecuteReader(conn, CommandType.Text, string.Format(sqlString, DateTime.Now.Date, WechatId));
            while (data.Read())
            {
                result = (int)data.GetInt64(0);
            }
            return result;
        }

        public void UpdatePersonStepRank(List<PersonStepRank> allPersonRank)
        {
            var conn = XiaoMiData.GetConnectstr();
            var updateString = "begin transaction delete from Innocellence_GSK_WeChat_HM_PersonStepRank {0} commit transaction";
            var insertSql = allPersonRank.Aggregate("", (current, stepRank) => current + createInsertPersonStepRankStr(stepRank.WechatId, stepRank.WechatName, stepRank.RankByDay, stepRank.StepsByDay, stepRank.Rank, stepRank.Steps));
            SqlHelper.ExecuteNonQuery(conn, CommandType.Text, string.Format(updateString, insertSql));
        }

        private string createInsertPersonStepRankStr(string wechatId, string wechatName, int rankByDay, int stepsByDay, int rank, int steps)
        {
            return string.Format("INSERT INTO Innocellence_GSK_WeChat_HM_PersonStepRank values ('{0}',N'{1}','{2}','{3}','{4}','{5}') ", wechatId, wechatName, rankByDay, stepsByDay, rank, steps);
        }
    }
}