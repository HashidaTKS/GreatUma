using GreatUma.Domain;
using GreatUma.Infrastructure;
using GreatUma.Infrastructures;
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
        private TargetStatusRepository TargetStatusRepository { get; } = new TargetStatusRepository();
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
            TargetManagementTask.TargetStatusRepository = TargetStatusRepository;
            AutoPurchaserMainTask.TargetStatusRepository = TargetStatusRepository;

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                if (column.HeaderText == "3分前購入条件")
                {
                    continue;
                }
                column.ReadOnly = true;
            }
            var targetStatus = TargetStatusRepository.ReadAll();
            if (targetStatus != null)
            {
                if (targetStatus.HorseAndOddsConditionList != null)
                {
                    foreach (var condition in targetStatus.HorseAndOddsConditionList)
                    {
                        BindingList.Add(condition);
                    }
                }
                numericUpDownPurchasePrice.Value = targetStatus.PurchasePrice;
            }
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            TargetManagementTask.Run();
            buttonStartUpdate.Enabled = false;
            IsAutoUpdating = true;
        }

        private void NumericUpDownPurchasePrice_ValueChanged(object sender, EventArgs e)
        {
            decimal value = numericUpDownPurchasePrice.Value;
            decimal roundedValue = Math.Floor(value / 100) * 100;
            numericUpDownPurchasePrice.Value = roundedValue;
        }

        private void ButtonLoginConfig_Click(object sender, EventArgs e)
        {
            var loginConfigForm = new LoginConfigForm();
            loginConfigForm.ShowDialog();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (IsAutoUpdating)
            {
                buttonStartUpdate.Enabled = false;
                if (!TargetManagementTask.Running)
                {
                    IsAutoUpdating = false;
                    BindingList.Clear();
                    var currentConditionList = TargetManagementTask.GetHorseAndOddsCondition();
                    foreach (var currentCondition in currentConditionList)
                    {
                        BindingList.Add(currentCondition);
                    }
                }
            }
            if (!IsAutoUpdating)
            {
                if (TargetManagementTask.Running)
                {
                    buttonStartUpdate.Enabled = false;
                }
                else
                {
                    buttonStartUpdate.Enabled = true;
                }
            }
            if (IsAutoPurchasing)
            {
                buttonStartAutoPurchase.Enabled = false;
                if (!AutoPurchaserMainTask.Running)
                {
                    AutoPurchaserMainTask.Run();
                }
            }
            else
            {
                if (AutoPurchaserMainTask.Running)
                {
                    buttonStartAutoPurchase.Enabled = false;
                }
                else
                {
                    buttonStartAutoPurchase.Enabled = true;
                }
            }

        }

        private void ButtonUpdateStatus_Click(object sender, EventArgs e)
        {
            TargetManagementTask.Stop();
            IsAutoUpdating = false;
        }

        private void ButtonAutoPurchase_Click(object sender, EventArgs e)
        {
            AutoPurchaserMainTask.Run();
            buttonStartAutoPurchase.Enabled = false;
            IsAutoPurchasing = true;
        }

        private void ButtonStopAutoPurchase_Click(object sender, EventArgs e)
        {
            AutoPurchaserMainTask.Stop();
            IsAutoPurchasing = false;
        }

        private void ButtonGetPrice_Click(object sender, EventArgs e)
        {
            var targetCondition = BindingList.LastOrDefault();
            if (targetCondition == null)
            {
                MessageBox.Show("対象のレースがないため残高の取得をスキップします。");
                return;
            }
            var loginRepo = new LoginConfigRepository();
            var loginData = loginRepo.ReadAll();
            if (loginData == null)
            {
                MessageBox.Show("ログイン情報がないため残高を取得できません。");
                return;
            }
            var autoPurchaser = new AutoPurchaser(loginData);
            int currentPrice = 0;
            try
            {
                currentPrice = autoPurchaser.GetCurrentPrice(targetCondition.RaceData);
            }
            catch (Exception ex)
            {
                MessageBox.Show("残高取得中にエラーが発生しました。");
            }
            textBoxCurrentPrice.Text = currentPrice.ToString();
            //textBoxCurrentPrice.Text = "5000";
        }

        private void ButtonSetPurchasePrice_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBoxPriceAppliedRatio.Text, out int price))
            {
                numericUpDownPurchasePrice.Value = price / 100 * 100;
            }
        }

        private void NumericUpDownRatio_ValueChanged(object sender, EventArgs e)
        {
            SetPriceRatio();
        }

        private void TextBoxCurrentPrice_TextChanged(object sender, EventArgs e)
        {
            SetPriceRatio();
        }

        private void SetPriceRatio()
        {
            if (double.TryParse(textBoxCurrentPrice.Text, out double price))
            {
                var ratio = (double)numericUpDownRatio.Value;
                var value = price * ratio / 100.0;
                textBoxPriceAppliedRatio.Text = ((int)Math.Ceiling(value)).ToString();
            }
        }

        private void buttonSaveConfition_Click(object sender, EventArgs e)
        {
            var targetStatus = TargetStatusRepository.ReadAll(true);
            targetStatus.PurchasePrice = (int)this.numericUpDownPurchasePrice.Value;
            targetStatus.HorseAndOddsConditionList = BindingList?.Select(_ => _)?.ToList() ?? new List<HorseAndOddsCondition>();
            TargetStatusRepository.Store(targetStatus);
        }
    }
}
