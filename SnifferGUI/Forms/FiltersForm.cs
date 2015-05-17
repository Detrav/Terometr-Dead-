using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Detrav.SnifferGUI.Forms
{
    public partial class FiltersForm : Form
    {
        public FiltersForm()
        {
            InitializeComponent();
        }

        private void checkBoxWhiteList_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBoxWhiteList.Checked)
                checkBoxBlackList.Checked = false;
            panelWhite.Enabled = checkBoxWhiteList.Checked;
        }

        private void checkBoxBlackList_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxBlackList.Checked)
                checkBoxWhiteList.Checked = false;
            panelBlack.Enabled = checkBoxBlackList.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void buttonAddBlack_Click(object sender, EventArgs e)
        {
            if(listBoxPacketsNameForBlack.SelectedItem!=null)
            {
                string str = listBoxPacketsNameForBlack.SelectedItem as string;
                listBoxPacketsNameForBlack.Items.RemoveAt(listBoxPacketsNameForBlack.SelectedIndex);
                listBoxBlack.Items.Add(str);
                sortListBox(ref listBoxBlack);
            }
        }

        private void buttonRemoveBlack_Click(object sender, EventArgs e)
        {
            if(listBoxBlack.SelectedItem!=null)
            {
                string str = listBoxBlack.SelectedItem as string;
                listBoxBlack.Items.RemoveAt(listBoxBlack.SelectedIndex);
                listBoxPacketsNameForBlack.Items.Add(str);
                sortListBox(ref listBoxPacketsNameForBlack);
            }
        }


        public void sortListBox(ref ListBox pList)
        {
            System.Collections.ArrayList arrayList = new System.Collections.ArrayList();
            foreach (var item in pList.Items)
                arrayList.Add(item);
            arrayList.Sort();
            pList.Items.Clear();
            foreach (var item in arrayList)
            {
                pList.Items.Add(item);
            }
        }

        private void buttonAddWhite_Click(object sender, EventArgs e)
        {
            if (listBoxPacketsNameForWhite.SelectedItem != null)
            {
                string str = listBoxPacketsNameForWhite.SelectedItem as string;
                listBoxPacketsNameForWhite.Items.RemoveAt(listBoxPacketsNameForWhite.SelectedIndex);
                listBoxWhite.Items.Add(str);
                sortListBox(ref listBoxWhite);
            }
        }

        private void buttonRemoveWhite_Click(object sender, EventArgs e)
        {
            if (listBoxWhite.SelectedItem != null)
            {
                string str = listBoxWhite.SelectedItem as string;
                listBoxWhite.Items.RemoveAt(listBoxWhite.SelectedIndex);
                listBoxPacketsNameForWhite.Items.Add(str);
                sortListBox(ref listBoxPacketsNameForWhite);
            }
        }
    }
}
