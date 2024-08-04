using GreatUma.Domain;
using GreatUma.Models;
using System.ComponentModel;

namespace GreatUma
{
    public class LowInfo
    {
        [DisplayName("��������")]
        public DateTime StartTime { get; set; }
        [DisplayName("�N���X")]
        public string RaceClass { get; set; }
        [DisplayName("�R�[�X")]
        public string Course { get; set; }
        [DisplayName("�R��")]
        public string Jocky { get; set; }
        [DisplayName("0���I�b�Y")]
        public string MidnightOdds { get; set; }
        [DisplayName("���݃I�b�Y")]
        public string CurrentOdds { get; set; }
        [DisplayName("3���O�w������")]
        public double PurchaseCondition { get; set; }
    }


    public partial class Form1 : Form
    {
        private BindingList<LowInfo> BindingList = new BindingList<LowInfo>();


        private BindingSource BindingSource = new BindingSource();

        public List<LowInfo> LowInfoList = new List<LowInfo>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BindingSource.DataSource = BindingList;
            dataGridView1.DataSource = BindingSource;
            //BindingList.Add(new LowInfo()
            //{
            //    StartTime = DateTime.Now,
            //    RaceClass = "������",
            //    Course = "�� 1500",
            //    Jocky = "���L",
            //    MidnightOdds = "1.4 1.1-1.2",
            //    CurrentOdds = "1.3 1.1-1.1",
            //    PurchaseCondition = 100
            //});
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            var scraper = new Scraper();
            var date = new DateTime(2024, 7, 6);
            var acrual = scraper.GetHoldingInformation(date, Utils.RegionType.Central);
            var raceData = new RaceData(acrual.HoldingData[0], 1);
            var winList = scraper.GetRealTimeOdds(raceData, Utils.TicketType.Win);
            var placeList = scraper.GetRealTimeOdds(raceData, Utils.TicketType.Place);

        }
    }
}
