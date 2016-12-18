using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommentPrefixVSP
{
    public partial class SettingsDialog : Form
    {
        BindingSource mBindingSource = new BindingSource();

        
        public Settings Settings { get; set; }
        public SettingsDialog(Settings settings)
        {
            InitializeComponent();

            Settings = settings;

            InitBindings();

            btnOK.Click += BtnOK_Click;

            btnCancel.Click += BtnCancel_Click;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void InitBindings()
        {
            dgvCommentTokens.DataSource = Settings.CommentTokens;

            txtPrefix.DataBindings.Add(new Binding("Text", Settings, "Prefix"));

            txtDateFormat.DataBindings.Add(new Binding("Text", Settings, "DateFormat"));
        }
    }
}
