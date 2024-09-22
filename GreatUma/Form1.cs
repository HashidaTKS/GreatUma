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
        private BindingList<TargetCondition> BindingList { get; } = new BindingList<TargetCondition>();
        private BindingSource BindingSource { get; } = new BindingSource();
        private List<TargetCondition> TargetConditionList { get; } = new List<TargetCondition>();
        private AutoPurchaserMainTask AutoPurchaserMainTask { get; } = new AutoPurchaserMainTask();
        private TargetManagementTask TargetManagementTask { get; } = new TargetManagementTask();
        private TargetConfigRepository TargetConfigRepository { get; } = new TargetConfigRepository();
        private TimeSpan CurrentOddsCheckSpan { get; } = new TimeSpan(0, 10, 0);
        private DateTime LastCheckTime { get; set; } = DateTime.MinValue;

        private bool IsAutoUpdating { get; set; }
        private bool IsAutoPurchasing { get; set; }

        public Form1()
        {
            InitializeComponent();
            BindingSource.ListChanged += ListChangedEventHandler;
        }

        private void ListChangedEventHandler(object? sender, ListChangedEventArgs e)
        {
            buttonSaveConfition.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BindingSource.DataSource = BindingList;
            dataGridView1.DataSource = BindingSource;
            TargetManagementTask.TargetConfigRepository = TargetConfigRepository;
            AutoPurchaserMainTask.TargetConfigRepository = TargetConfigRepository;

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                if (column.HeaderText == "購入オッズ（変更可）")
                {
                    continue;
                }
                column.ReadOnly = true;
            }
            var targetConfig = TargetConfigRepository.ReadAll();
            if (targetConfig != null)
            {
                if (targetConfig.TargetConditionList != null)
                {
                    foreach (var condition in targetConfig.TargetConditionList)
                    {
                        BindingList.Add(condition);
                    }
                }
                numericUpDownPurchasePrice.Value = targetConfig.PurchasePrice;
                numericUpDownTargetPlaceOdds.Value = (decimal)targetConfig.TargetPlaceOdds;
            }
            buttonSaveConfition.Enabled = false;
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
            buttonSaveConfition.Enabled = true;
        }

        private void ButtonLoginConfig_Click(object sender, EventArgs e)
        {
            var loginConfigForm = new LoginConfigForm();
            loginConfigForm.ShowDialog();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (LastCheckTime.Date < DateTime.Today && 
                DateTime.Now >= DateTime.Today.AddMinutes(30) &&
                DateTime.Now < DateTime.Today.AddHours(2))
            {
                // 0時30以降かつ2時未満だったら、データを自動で取得する。
                // 夜間自動取得処理
                LastCheckTime = DateTime.Now;
                TargetManagementTask.Run();
                buttonStartUpdate.Enabled = false;
                IsAutoUpdating = true;
            } 

            if (IsAutoUpdating)
            {
                buttonStartUpdate.Enabled = false;
                if (!TargetManagementTask.Running)
                {
                    IsAutoUpdating = false;
                    BindingList.Clear();
                    var currentStatus = TargetConfigRepository.ReadAll(true);
                    var currentConditionList = currentStatus.TargetConditionList ?? new List<TargetCondition>();
                    foreach (var currentCondition in currentConditionList)
                    {
                        BindingList.Add(currentCondition);
                    }
                    oddsDateTimeLabel.Text = DateTime.Now.ToString();
                    LastCheckTime = DateTime.Now;
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
            using var autoPurchaser = new AutoPurchaser(loginData);
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
            var targetConfig = TargetConfigRepository.ReadAll(true);
            if (targetConfig.TargetPlaceOdds != (double)this.numericUpDownTargetPlaceOdds.Value)
            {
                //取得条件が変わっているので、最初から再取得する
                TargetManagementTask.SetInitialized(false);
            }
            targetConfig.PurchasePrice = (int)this.numericUpDownPurchasePrice.Value;
            targetConfig.TargetPlaceOdds = (double)this.numericUpDownTargetPlaceOdds.Value;
            targetConfig.TargetConditionList = BindingList?.Select(_ => _)?.ToList() ?? new List<TargetCondition>();
            TargetConfigRepository.Store(targetConfig);
            buttonSaveConfition.Enabled = false;
        }

        private void buttonRemoveInfo_Click(object sender, EventArgs e)
        {
            BindingList.Clear();
        }
    }
}
