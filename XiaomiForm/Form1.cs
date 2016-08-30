using System;
using System.Collections.Generic;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Innocellence.GSK.WeChat.HM.Models;
using Innocellence.Xiaomi.HttpUtility;

namespace XiaomiWinForm
{
    public partial class Form1 : Form
    {
        System.Timers.Timer myTimer;
        public static RichTextBox myRichText;
        public Form1()
        {
            InitializeComponent();
            myRichText = richTextBox1;
            richTextBox1.Text += "load form\r\n";
            LogHelper.Info(typeof(Form1), "load form");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.Text += "click button\r\n";
                var date = dateTimePicker1.Value;
                Thread myTread = new Thread(new ParameterizedThreadStart(SyncXiaoMiData));
                myTread.Start(new DateTime(2016, 8, 22));
                LogHelper.Info(typeof(Form1), "begin sync tread");
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof (Form1), ex);
            }

            //ThreadPool.QueueUserWorkItem(SyncXiaoMiData,date.Date);
            //SyncXiaoMiData(date.Date);
        }

        private void myTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            richTextBox1.Text += "timer started\r\n";
            try
            {
                Thread myTread = new Thread(new ParameterizedThreadStart(SyncXiaoMiData));
                myTread.Start(new DateTime(2016, 8, 22));
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof (Form1), ex);
                richTextBox1.Text += "some thing go wrong";
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            richTextBox1.Text += "timer inited\r\n";
            myTimer = new System.Timers.Timer(1000 * 60 * 15);//定时周期1小时
            myTimer.Elapsed += myTimer_Elapsed;//到1小时做的事件
            myTimer.AutoReset = true; //是否不断重复定时器操作
            dateTimePicker1.Value = new DateTime(2016, 8, 22);
            button1.Enabled = true;
            button2.Enabled = false;
        }

        private void SyncXiaoMiData()
        {
            SyncXiaoMiData(new DateTime(2016, 8, 22));
        }
        private void SyncXiaoMiData(object date)
        {
            try
            {
                Control.CheckForIllegalCrossThreadCalls = false;
                var fromDate = date as DateTime? ?? new DateTime();
                var xiaomiDate = new XiaoMiData();
                richTextBox1.Text = "" + DateTime.Now;
                richTextBox1.Text += "begin get setting from DB\r\n";
                var settingList = xiaomiDate.GetSetting();
                richTextBox1.Text += string.Format("getted {0} settings,they are:\r\n", settingList.Count);
                UpdateMetadata(settingList);
                richTextBox1.Text += "Update Person metadata success!\r\n";
                UpdateScoreRank(fromDate);
                richTextBox1.Text += "Update Person score Rank success!\r\n";
                UpdateStepRank(fromDate);
                richTextBox1.Text += "Update Person step Rank success!\r\n";
                UpdateGroupRank(fromDate);
                richTextBox1.Text += "Update Person group Rank success!\r\n";
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof (Form1), ex);
            }
        }

        private void UpdateMetadata(List<Setting> allSetting)
        {
            var xiaomiDate = new XiaoMiData();
            var MetadataService = new MetadataService();
            foreach (var setting in allSetting)
            {
                richTextBox1.Text += "wechatId: " + setting.WechatId + " wechatName: " + setting.WechatName + "\r\n";
                var url = xiaomiDate.GetXiaoMiDataUrl(setting);
                var sevenDaysData = xiaomiDate.GetDateResponse(url);
                if (sevenDaysData.result == "success")
                {
                    richTextBox1.Text += "Get Setting metadata  success！\r\n";
                    foreach (var data in sevenDaysData.data)
                    {
                        MetadataService.UpdateMetaData(setting, data);
                        richTextBox1.Text += "update Metadata date:" + data.date + " steps: " + data.step + "\r\n";
                    }
                }
                else
                {
                    LogHelper.Error(typeof (Form1), "get setting date error: wechatId:" + setting.WechatId);
                }
            }
        }

        private void UpdateScoreRank(DateTime fromdate)
        {
            var MetadataService = new MetadataService();
            var personScoreRankService = new PersonScoreRankService();
            var allMetadata = MetadataService.GetAllMetaData(fromdate, DateTime.Now);
            var personRank = personScoreRankService.GetNewPersonStepRank(allMetadata);
            personScoreRankService.UpdatePersonRank(personRank);
        }

        private void UpdateStepRank(DateTime fromdate)
        {
            var personStepRankService = new PersonStepRankService();
            var MetadataService = new MetadataService();
            var allMetadata = MetadataService.GetAllMetaData(fromdate, DateTime.Now);
            var personStepRank = personStepRankService.GetPersonStepRankList(allMetadata);
            personStepRankService.UpdatePersonStepRank(personStepRank);
        }

        private void UpdateGroupRank(DateTime fromdate)
        {
            var groupService = new GroupInfoMapService();
            var result = groupService.getGroupScoreRank(fromdate, DateTime.Now);
            groupService.UpdateGroupRank(result);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            richTextBox1.Text += "begin timer\r\n";
            myTimer.Start();
            Control.CheckForIllegalCrossThreadCalls = false;
            button1.Enabled = false;
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += "stop timer\r\n";
            myTimer.Stop();
            button1.Enabled = true;
            button2.Enabled = false;
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            try
            {
                var xiaomiDate = new XiaoMiData();
                var allSetting = xiaomiDate.GetSetting();
                richTextBox1.Text = "get all setting,count:" + allSetting.Count;
                LogHelper.Info(typeof(Form1), "get all setting,count:" + allSetting.Count);
                foreach (var setting in allSetting)
                {
                    //if (setting.WechatId != "13354280516")
                    //{
                    //    continue;
                    //}
                    richTextBox1.Text += "get old access token:wechatId:" + setting.WechatId;
                    LogHelper.Info(typeof(Form1), "get old access token:wechatId:" + setting.WechatId);
                    if (setting.RefreshToken != null)
                    {
                        var url = xiaomiDate.RefreshTokenURL(setting.RefreshToken);
                        LogHelper.Info(typeof (Form1), "get old access token url:" + url);
                        var returnText = RequestUtility.HttpGet(url, null);
                        LogHelper.Info(typeof (Form1), "get new access token::" + returnText);
                        var js = new JavaScriptSerializer();
                        var returnObj = (Dictionary<string, string>) js.Deserialize(returnText.Replace("&&&START&&&", ""), typeof (Dictionary<string, string>));
                        if (!returnObj.ContainsKey("error") && returnObj.ContainsKey("access_token") && returnObj.ContainsKey("mac_key") && returnObj.ContainsKey("refresh_token"))
                        {
                            richTextBox1.Text += "update access token:" + returnObj["access_token"];
                            xiaomiDate.UpdateSettingMacAndToken(returnObj["access_token"], returnObj["mac_key"], returnObj["refresh_token"], setting.WechatId);
                        }
                        else
                        {
                            LogHelper.Error(typeof (Form1), "get new access token error:" + returnText);
                        }
                    }
                    else
                    {
                        LogHelper.Info(typeof(Form1), "current user have not refresh token:");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(Form1), ex);
            }
        }
    }
}
