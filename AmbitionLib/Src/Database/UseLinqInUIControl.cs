using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AmbitonLib.Database
{
    /// <summary>
    /// LINQ在WINFORM中的使用方式
    /// </summary>
    public class UseLinqInUIControl
    {
        /// <summary>
        /// 获取与sender同级别的所有选中CheckBox, 依TabIndex排序, LINQ
        /// </summary>
        /// <param name="sender">检查的CheckBox</param>
        /// <returns>相同容器中所有选中CheckBox</returns>
        public static List<CheckBox> GetCheckedCheckBoxs(CheckBox sender)
        {
            return sender.Parent.Controls.OfType<CheckBox>().Where(c => c.Checked).OrderBy(c => c.TabIndex).ToList();
        }
        /// <summary>
        /// 获取与sender同级别的选中RadioButton, LINQ
        /// </summary>
        /// <param name="sender">检查的RadioButton</param>
        /// <returns>选中RadioButton, 全部未选中返回NULL</returns>
        public static RadioButton GetCheckedRadio(RadioButton sender)
        {
            return sender.Parent.Controls.OfType<RadioButton>().Where(c => c.Checked).FirstOrDefault();
        }
        /// <summary>
        /// 清除容器中所有指定前缀名称的Label文本, LINQ
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="labelNamePrefix">Label名称前缀</param>
        public static void ClearLabelTextByNamePrefix(Control container, string labelNamePrefix)
        {
            container.Controls.OfType<Label>().Where(c => c.Name.StartsWith(labelNamePrefix)).ToList().ForEach(c => c.Text = "");
        }
    }
}
