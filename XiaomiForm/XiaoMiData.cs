#region using region

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.Script.Serialization;
using Innocellence.GSK.WeChat.HM.Models;
using Innocellence.GSK.WeChat.Module.Common;
using Innocellence.Xiaomi;
using Innocellence.Xiaomi.HttpUtility;

#endregion

namespace XiaomiWinForm
{
    public class XiaoMiData
    {
        public List<Setting> GetSetting()
        {
            var conn = GetConnectstr();
            var sql = @"SELECT * FROM Innocellence_GSK_WeChat_HM_Setting";
            var data = SqlHelper.ExecuteDataset(conn, CommandType.Text, sql).Tables[0];
            var result = SqlHelper.ConvertTo<Setting>(data).ToList();
            return result;
        }

        public string GetXiaoMiDataUrl(Setting settingInfo)
        {
            var macKey = settingInfo.MacKey;
            var accToken = settingInfo.AccessToken;
            var appid = ConfigurationManager.AppSettings["appid"];
            var third_appid = ConfigurationManager.AppSettings["third_appid"];
            var third_appsecret = ConfigurationManager.AppSettings["third_appsecret"];
            var call_id = ConfigurationManager.AppSettings["call_id"];
            var v = ConfigurationManager.AppSettings["v"];
            var l = ConfigurationManager.AppSettings["l"];
            var url = "https://hmservice.mi-ae.com.cn/user/summary/getData?appid={0}&third_appid={1}&third_appsecret={2}&mac_key={3}&call_id={4}&access_token={5}&fromdate={6}&todate={7}&v={8}&l={9}";
            return string.Format(url, appid, third_appid, third_appsecret, macKey, call_id, accToken, DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd"), v, l);
        }

        public string GetXiaoMiDataUrl(Setting settingInfo, DateTime fromDate, DateTime toDate)
        {
            var macKey = settingInfo.MacKey;
            var accToken = settingInfo.AccessToken;
            var appid = ConfigurationManager.AppSettings["appid"];
            var third_appid = ConfigurationManager.AppSettings["third_appid"];
            var third_appsecret = ConfigurationManager.AppSettings["third_appsecret"];
            var call_id = ConfigurationManager.AppSettings["call_id"];
            var v = ConfigurationManager.AppSettings["v"];
            var l = ConfigurationManager.AppSettings["l"];
            var url = "https://hmservice.mi-ae.com.cn/user/summary/getData?appid={0}&third_appid={1}&third_appsecret={2}&mac_key={3}&call_id={4}&access_token={5}&fromdate={6}&todate={7}&v={8}&l={9}";
            return string.Format(url, appid, third_appid, third_appsecret, macKey, call_id, accToken, fromDate.ToString("yyyy-MM-dd"), toDate.ToString("yyyy-MM-dd"), v, l);
        }

        public static string GetConnectstr()
        {
            var conn = ConfigurationManager.AppSettings["ConnectionString"];
            //Form1.myRichText.Text += "get connect string: "+conn+"\r\n";
            return conn;
        }

        public static string GetWeChatNameById(string weChatId)
        {
            var conn = XiaoMiData.GetConnectstr();
            var sql = @"SELECT WechatName FROM Innocellence_GSK_WeChat_HM_Setting where WechatId='{0}'";
            var data = SqlHelper.ExecuteReader(conn, CommandType.Text, string.Format(sql, weChatId));
            var result = "";
            while (data.Read())
            {
                result = data[0] != null ? data.GetString(0) : "";
            }
            return result;
        }



        public GetDataResponse GetDateResponse(string getTokenUrl)
        {
            var returnText = RequestUtility.HttpGet(getTokenUrl, null);
            var js = new JavaScriptSerializer();
            var returnObj = js.Deserialize<GetDataResponse>(returnText);
            return returnObj;
        }
    }
}