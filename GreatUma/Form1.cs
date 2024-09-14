using GreatUma.Domain;
using GreatUma.Model;
using GreatUma.Models;
using OpenQA.Selenium.DevTools.V124.WebAudio;
using System.ComponentModel;

namespace GreatUma
{
    public partial class Form1 : Form
    {
        private BindingList<HorseAndOddsCondition> BindingList { get; } = new BindingList<HorseAndOddsCondition>();
        private BindingSource BindingSource { get; } = new BindingSource();
        private List<HorseAndOddsCondition> HorseAndOddsConditionList { get; } = new List<HorseAndOddsCondition>();
        private AutoPurchaserMainTask AutoPurchaserMainTask { get; } = new AutoPurchaserMainTask();
        private TargetManagementTask TargetManagementTask { get; } = new TargetManagementTask();
        private TimeSpan CurrentOddsCheckSpan { get; } = new TimeSpan(0, 10, 0);
        private DateTime LastCheckTime { get; set; } = DateTime.MinValue;

        private bool IsAutoUpdating { get; set; }
        private bool IsAutoPurchasing { get; set; }

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
                if (column.HeaderText == "3•ª‘Ow“üðŒ")
                {
                    continue;
                }
                column.ReadOnly = true;
            }

            BindingList.Add(new HorseAndOddsCondition()
            {
                StartTime = DateTime.Now,
                Region = "•Ÿ“‡",
                Title = "–¢Ÿ—˜",
                Course = "ŽÅ 1500",
                Jocky = "•–L",
                HorseNum = "3",
                MidnightOdds = "1.4 1.1-1.2",
                CurrentOdds = "1.3 1.1-1.1",
                PurchaseCondition = 100
            });
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            TargetManagementTask.Run();
            Update.Enabled = false;
            IsAutoUpdating = true;
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (IsAutoUpdating)
            {
                Update.Enabled = false;
                if (!TargetManagementTask.Running &&
                    DateTime.Now - LastCheckTime > CurrentOddsCheckSpan)
                {
                    var currentConditionList = TargetManagementTask.GetHorseAndOddsCondition();
                    BindingList.Clear();
                    foreach (var currentCondition in currentConditionList)
                    {
                        BindingList.Add(currentCondition);
                    }
                    TargetManagementTask.Run();
                }
            }
            else
            {
                if (TargetManagementTask.Running)
                {
                    Update.Enabled = false;
                }
                else
                {
                    Update.Enabled = true;
                }
            }
            if (IsAutoPurchasing)
            {
                button6.Enabled = false;
                if (!AutoPurchaserMainTask.Running)
                {
                    AutoPurchaserMainTask.Run();
                }
            }
            else
            {
                if (AutoPurchaserMainTask.Running)
                {
                    button6.Enabled = false;
                }
                else
                {
                    button6.Enabled = true;
                }
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            TargetManagementTask.Stop();
            IsAutoUpdating = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            AutoPurchaserMainTask.Run();
            button6.Enabled = false;
            IsAutoPurchasing = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            AutoPurchaserMainTask.Stop();
            IsAutoPurchasing = false;
        }
    }
}
