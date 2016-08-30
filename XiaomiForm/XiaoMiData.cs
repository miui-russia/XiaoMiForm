#region using region

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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

        public void UpdateSettingMacAndToken(string accessToken,string macKey,string refreshToken,string wechatId)
        {
            var conn = GetConnectstr();
            var sql = @"update Innocellence_GSK_WeChat_HM_Setting set AccessToken='{0}',MacKey='{1}',RefreshToken='{2}' where wechatId='{3}'";
            LogHelper.Info(typeof (XiaoMiData), string.Format(sql, accessToken, macKey, refreshToken, wechatId));
            SqlHelper.ExecuteNonQuery(conn, CommandType.Text, string.Format(sql, accessToken, macKey, refreshToken, wechatId));
        }
        public string GetXiaoMiDataUrl(Setting settingInfo)
        {
            var macKey = settingInfo.MacKey;
            var accToken = settingInfo.AccessToken;
            var appid = ConfigurationManager.AppSettings["appid"];
            var third_appid = ConfigurationManager.AppSettings["third_appid"];
            var third_appsecret = ConfigurationManager.AppSettings["third_appsecret"];
            var call_id = DateTime.Now.Second;
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

        public string RefreshTokenURL(string oldToken)
        {

            var appid = ConfigurationManager.AppSettings["appid"];
            var redirect_uri = ConfigurationManager.AppSettings["redirect_uri"];
            var client_secret = ConfigurationManager.AppSettings["client_secret"];
            var url = "https://account.xiaomi.com/oauth2/token?client_id={0}&redirect_uri={1}&client_secret={2}&grant_type=refresh_token&refresh_token={3}";
            return string.Format(url, appid, redirect_uri, client_secret, oldToken);
        }

        public static string GetConnectstr()
        {
            var conn = ConfigurationManager.AppSettings["ConnectionString"];
            //Form1.myRichText.Text += "get connect string: "+conn+"\r\n";
            return conn;
        }

        public static Dictionary<string, string> GetWeChatNameById()
        {
            var conn = XiaoMiData.GetConnectstr();
            var sql = @"SELECT WechatId,WechatName FROM Innocellence_GSK_WeChat_HM_Setting";
            using (SqlConnection connection = new SqlConnection(conn))
            {
                connection.Open();
                var data = SqlHelper.ExecuteReader(connection, CommandType.Text, sql);
                var result = new Dictionary<string, string>();
                while (data.Read())
                {
                    result[data.GetString(0)] = data.GetString(1);
                }
                return result;
            }
        }



        public GetDataResponse GetDateResponse(string getTokenUrl)
        {
            var returnText = RequestUtility.HttpGet(getTokenUrl, null);
            var js = new JavaScriptSerializer();
            var returnObj = js.Deserialize<GetDataResponse>(returnText);
            if (returnObj.result != "success")
            {
                LogHelper.Error(typeof (XiaoMiData), returnText);
            }
            return returnObj;
        }
    }
}