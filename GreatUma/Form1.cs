using GreatUma.Domain;
using GreatUma.Models;
using System.ComponentModel;

namespace GreatUma
{
    public partial class Form1 : Form
    {
        private BindingList<HorseAndOddsCondition> BindingList = new BindingList<HorseAndOddsCondition>();
        private BindingSource BindingSource = new BindingSource();
        public List<HorseAndOddsCondition> HorseAndOddsConditionList = new List<HorseAndOddsCondition>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BindingSource.DataSource = BindingList;
            dataGridView1.DataSource = BindingSource;

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                if (column.HeaderText == "3���O�w������")
                {
                    continue;
                }
                column.ReadOnly = true;
            }

            BindingList.Add(new HorseAndOddsCondition()
            {
                StartTime = DateTime.Now,
                RaceClass = "������",
                Course = "�� 1500",
                Jocky = "���L",
                MidnightOdds = "1.4 1.1-1.2",
                CurrentOdds = "1.3 1.1-1.1",
                PurchaseCondition = 100
            });
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            var scraper = new Scraper();
            var date = DateTime.Today;
            var acrual = scraper.GetHoldingInformation(date, Utils.RegionType.Central);
            var raceData = new RaceData(acrual.HoldingData[0], 1);
            var winList = scraper.GetRealTimeOdds(raceData, Utils.TicketType.Win);
            var placeList = scraper.GetRealTimeOdds(raceData, Utils.TicketType.Place);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            decimal value = numericUpDown2.Value;
            decimal roundedValue = Math.Floor(value / 100) * 100;
            numericUpDown2.Value = roundedValue;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var loginConfigForm = new LoginConfigForm();
            loginConfigForm.ShowDialog();
        }
    }

    public class HorseAndOddsCondition
    {
        [DisplayName("��������")]
        public DateTime StartTime { get; set; }
        [DisplayName("�N���X")]
        public string RaceClass { get; set; }
        [DisplayName("�R�[�X")]
        public string Course { get; set; }
        [DisplayName("�n��")]
        public string HorseNum { get; set; }
        [DisplayName("�R��")]
        public string Jocky { get; set; }
        [DisplayName("0���I�b�Y")]
        public string MidnightOdds { get; set; }
        [DisplayName("���݃I�b�Y")]
        public string CurrentOdds { get; set; }
        [DisplayName("3���O�w������")]
        public double PurchaseCondition { get; set; }
    }

}
