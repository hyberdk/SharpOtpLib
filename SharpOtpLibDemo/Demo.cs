using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SharpOtpLib;
using Timer = System.Windows.Forms.Timer;

namespace SharpOtpLibDemo
{
    public partial class Demo : Form
    {

        TimebasedOneTimePassword _totp = new TimebasedOneTimePassword();
        HashbasedOneTimePassword _hotp = new HashbasedOneTimePassword();
        Timer _timer = new Timer();

        public Demo()
        {
            InitializeComponent();

        }

        private void Demo_Load(object sender, EventArgs e)
        {
            _timer.Interval = 100;
            _timer.Tick += TimerTick;
            _timer.Enabled = true;
            Setup();
        }

        void TimerTick(object sender, EventArgs e)
        {
            if (!rdoTotp.Checked) return;
            byte[] key = Base32.Convert(txtSecret.Text);
            long counter = _totp.CurrentInterval;
            if (rdoTotp.Checked)
                txtCurCode.Text =
                    HashbasedOneTimePassword.Generate(key, counter, OneTimePassword.CodeLength.Six).ToString();
        }



        private void txtIdent_TextChanged(object sender, EventArgs e)
        {

            Setup();

        }

        private void Setup()
        {
            if (rdoHotp.Checked)
            {
                SetupHotp();
            }
            else
            {
                SetupTotp();   
            }

            chkLocked.Checked = false;


        }

        private void SetupTotp()
        {
            byte[] key = Base32.Convert(txtSecret.Text);
            string ident = txtIdent.Text;

            _totp.Devices.Flush();
            _totp.Devices.Add(ident, key);
            txtCounter.Text = _totp.CurrentInterval.ToString();
            txtOffset.Text = _totp.Devices.GetOffset(ident).ToString();
            txtThrottling.Text = (_totp.ThrottlingDelay * _totp.Devices.GetFailures(ident)).ToString();
            _totp.Window = (int) numWindow.Value;
            txtResult.Text = "";

            using (MemoryStream ms = new MemoryStream(_totp.RenderQrCode(ident)))
            {
                picQrCode.Image = Image.FromStream(ms);
            }

        }


        private void SetupHotp()
        {
            byte[] key = Base32.Convert(txtSecret.Text);
            string ident = txtIdent.Text;
            _hotp.Devices.Flush();
            _hotp.Devices.Add(ident, key);
            txtCounter.Text = _hotp.Devices.GetCounter(ident).ToString();
            txtCurCode.Text = HashbasedOneTimePassword.Generate(key, _hotp.Devices.GetCounter(ident), OneTimePassword.CodeLength.Six).ToString();
            txtOffset.Text = "N/A";
            txtThrottling.Text = (_hotp.ThrottlingDelay * _hotp.Devices.GetFailures(ident)).ToString();
            txtResult.Text = "";
            _hotp.Window = (int)numWindow.Value;
            using (MemoryStream ms = new MemoryStream(_hotp.RenderQrCode(txtIdent.Text)))
            {
                picQrCode.Image = Image.FromStream(ms);
            }
            
        }


        private void GenerateQr()
        {
            
        }

        private void CheckCode()
        {
            if (rdoHotp.Checked)
            {
                CheckHotpCode();
            }
            else
            {
                CheckTotpCode();
            }
        }

        private void cmdCheckCode_Click(object sender, EventArgs e)
        {
            CheckCode();
        }

        private void CheckTotpCode()
        {
            string ident = txtIdent.Text;
            txtResult.Text = _totp.Verify(ident, txtCode.Text).ToString(); 
            txtThrottling.Text = (_totp.ThrottlingDelay * _totp.Devices.GetFailures(ident)).ToString();
            txtOffset.Text = _totp.Devices.GetOffset(ident).ToString();
            chkLocked.Checked = _totp.Devices.IsLocked(ident);

        }

        private void CheckHotpCode()
        {
            string ident = txtIdent.Text;
            byte[] key = Base32.Convert(txtSecret.Text);
            txtResult.Text = _hotp.Verify(ident, txtCode.Text).ToString();
            txtThrottling.Text = (_hotp.ThrottlingDelay * _hotp.Devices.GetFailures(ident)).ToString();
            txtCounter.Text = _hotp.Devices.GetCounter(ident).ToString();
            txtCurCode.Text = HashbasedOneTimePassword.Generate(key, _hotp.Devices.GetCounter(ident), OneTimePassword.CodeLength.Six).ToString();
            chkLocked.Checked = _hotp.Devices.IsLocked(ident);
        }


        private void txtSecret_TextChanged(object sender, EventArgs e)
        {
            Setup();
        }

        private void numWindow_ValueChanged(object sender, EventArgs e)
        {
            _hotp.Window = (int) numWindow.Value;
            _totp.Window = (int) numWindow.Value;
        }

        private void rdoHotp_CheckedChanged(object sender, EventArgs e)
        {
            Setup();
        }

        private void cmdGenerateSecret_Click(object sender, EventArgs e)
        {
            txtSecret.Text = Base32.Convert(Utils.GenerateSecret());
        }

        private void rdoTotp_CheckedChanged(object sender, EventArgs e)
        {
            Setup();
        }

        private void cmdSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void Save()
        {
            if (rdoHotp.Checked)
            {
                _hotp.Devices.Save("hotp.xml");
                MessageBox.Show("Saved files as: hotp.xml");

            }
            else
            {
                _totp.Devices.Save("totp.xml");
                MessageBox.Show("Saved files as: totp.xml");   
            }
       }

        private void LoadXml()
        {
            if (rdoHotp.Checked)
            {
                if (File.Exists("hotp.xml"))
                {
                    _hotp.Devices.Load("hotp.xml");
                    MessageBox.Show("loaded hotp.xml");
                }
                else
                {
                    MessageBox.Show("hotp.xml not found!");
                }
            }
            else
            {
                if (File.Exists("totp.xml"))
                {
                    _hotp.Devices.Load("totp.xml");
                    MessageBox.Show("loaded totp.xml");
                }
                else
                {
                    MessageBox.Show("totp.xml not found!");
                }
            }
        }

        private void cmdLoad_Click(object sender, EventArgs e)
            {
                LoadXml();
            }


    }
}
