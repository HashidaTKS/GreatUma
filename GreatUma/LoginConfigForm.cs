using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GreatUma.Infrastructures;
using GreatUma.Models;


namespace GreatUma
{
    public partial class LoginConfigForm : Form
    {
        public LoginConfigForm()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            var repository = new LoginConfigRepository();
            var config = repository.ReadAll();
            if(config == null)
            {
                return;
            }
            textBox_InetId.Text = config.JRA_INET_ID;
            textBox_JraPassword.Text = config.JRA_LoginPassword;
            textBox_P_Ars.Text = config.JRA_P_ARS;
            textBox_IpaSubscriberNumber.Text = config.JRA_SubscriberNumber;
            textBox_NetkeibaLoginId.Text = config.NetkeibaId;
            textBox_NetkeibaPassword.Text = config.NetkeibaPassword;
            textBox_RakutenLoginId.Text = config.RakutenId;
            textBox_RakutenPassword.Text = config.RakutenPassword;

        }

        private void Button_Save_Click(object sender, EventArgs e)
        {
            var config = new LoginConfig();
            config.JRA_INET_ID = textBox_InetId.Text;
            config.JRA_LoginPassword = textBox_JraPassword.Text;
            config.JRA_P_ARS = textBox_P_Ars.Text;
            config.JRA_SubscriberNumber = textBox_IpaSubscriberNumber.Text;
            config.NetkeibaId = textBox_NetkeibaLoginId.Text;
            config.NetkeibaPassword = textBox_NetkeibaPassword.Text;
            config.RakutenId = textBox_RakutenLoginId.Text;
            config.RakutenPassword = textBox_RakutenPassword.Text;

            var repository = new LoginConfigRepository();
            repository.Store(config);
            Close();
        }

        private void Button_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
