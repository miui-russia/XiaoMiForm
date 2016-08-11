using System;
using System.Windows.Forms;

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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += "click button\r\n";
            SyncXiaoMiData();
        }

        private void myTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            richTextBox1.Text += "timer started\r\n";
            SyncXiaoMiData();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            richTextBox1.Text += "begin timer\r\n";
            myTimer = new System.Timers.Timer(1000*60*60);//定时周期1小时
            myTimer.Elapsed += myTimer_Elapsed;//到1小时做的事件
            myTimer.AutoReset = true; //是否不断重复定时器操作
            myTimer.Enabled = true;
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void SyncXiaoMiData()
        {
            var xiaomiDate = new XiaoMiData();
            var MetadataService = new MetadataService();
            var personScoreRankService = new PersonScoreRankService();
            var personStepRankService = new PersonStepRankService();
            richTextBox1.Text += "begin get setting from DB\r\n";
            var settingList = xiaomiDate.GetSetting();
            richTextBox1.Text += string.Format("getted {0} settings,they are:\r\n",settingList.Count);
            foreach (var setting in settingList)
            {
                richTextBox1.Text += "wechatId: " + setting.WechatId + " wechatName: " + setting.WechatName+"\r\n";
                var url = xiaomiDate.GetXiaoMiDataUrl(setting);
                var sevenDaysData = xiaomiDate.GetDateResponse(url);
                if (sevenDaysData.result == "success")
                {
                    richTextBox1.Text += "Get Setting metadata  success！\r\n";
                    foreach (var data in sevenDaysData.data)
                    {
                        MetadataService.UpdateMetaData(setting, data);
                        richTextBox1.Text += "update Metadata date:" + data.date + " steps: " + data.step+"\r\n";
                    }
                }
            }
            var allMetadata = MetadataService.GetAllMetaData();
            var personRank = personScoreRankService.GetNewPersonStepRank(allMetadata);
            personScoreRankService.UpdatePersonRank(personRank);
            richTextBox1.Text += "Update Person score Rank success!\r\n";
            var personStepRank = personStepRankService.GetPersonStepRankList(allMetadata);
            personStepRankService.UpdatePersonStepRank(personStepRank);
            richTextBox1.Text += "Update Person step Rank success!\r\n";
        }
    }
}
