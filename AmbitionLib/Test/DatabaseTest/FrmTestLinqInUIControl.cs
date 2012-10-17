using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AmbitonLib.Database
{
    public partial class FrmTestLinqInUIControl : Form
    {
        public FrmTestLinqInUIControl()
        {
            InitializeComponent();
            this.Controls.OfType<GroupBox>().ToList().ForEach(grp => UseLinqInUIControl.ClearLabelTextByNamePrefix(grp,"lbl"));
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton ctl = UseLinqInUIControl.GetCheckedRadio((RadioButton)sender);
            ((RadioButton)sender).Parent.Controls.OfType<Label>().Where(c => c.Name.EndsWith("RadioButtonSelected")).First().Text = ctl == null ? "" : ctl.Text;
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            List<CheckBox> ctls = UseLinqInUIControl.GetCheckedCheckBoxs((CheckBox)sender);
            Label lbl = ((CheckBox)sender).Parent.Controls.OfType<Label>().Where(c => c.Name.EndsWith("CheckboxSelected")).FirstOrDefault();
            lbl.Text = string.Join(",",ctls.Select(c => c.Text));
            //lbl.Text = "";
            //ctls.ForEach(c =>
            //{
            //    lbl.Text += c.Text + ",";
            //});
        }
    }
}
