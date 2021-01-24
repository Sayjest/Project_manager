using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace _1CNew
{
    public partial class Form4 : Form
    {
        public Form4(List<string> list, string Help, string connstr)
        {
            InitializeComponent();
            listSotr = list;
            help = Help;
            connStr = connstr;
        }
        List<string> listSotr;
        string help, connStr;

        private void Form4_Load(object sender, EventArgs e)
        {
            ToolTip tip = new ToolTip();
            tip.SetToolTip(comboBox1, "Выберите сотрудника из списка, чтобы увидеть его предыдущие задания");
            this.Text = "Задания (" + help + ")";
            listBox1.DrawMode = DrawMode.OwnerDrawVariable;
            listBox1.MeasureItem += lb_MeasureItem;
            listBox1.DrawItem += lb_DrawItem;
            for (int i = 0; i < listSotr.Count; i++)
            {
                comboBox1.Items.Add(listSotr.ElementAt(i));
            }
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            MySqlConnection conn9 = new MySqlConnection(connStr);
            conn9.Open();
            string sql9 = "SELECT t.text, t.fileName FROM (SELECT * FROM " + help.Replace(' ', '_') + "З WHERE name LIKE '" + comboBox1.SelectedItem.ToString() + "' ORDER BY sms_id DESC) as t ";
            MySqlCommand cmd9 = new MySqlCommand(sql9, conn9);
            MySqlDataReader rdr9 = cmd9.ExecuteReader();
            int i = 0;
            while (rdr9.Read())
            {
                if (Convert.ChangeType(rdr9[1], typeof(String)).Equals("") == false)
                {

                    listBox1.Items.Insert(i, "Задание:\n" + (string)Convert.ChangeType(rdr9[0], typeof(String)) + "\nФайл:\n" + (string)Convert.ChangeType(rdr9[1], typeof(String)));
                }
                else
                {
                    listBox1.Items.Insert(i, "Задание:\n" + (string)Convert.ChangeType(rdr9[0], typeof(String)));
                }
                i++;
            }
            rdr9.Close();
            conn9.Close();
        }

        private void lb_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (listBox1.Items.Count > 0)
            {
                e.DrawBackground();
                SolidBrush myBrush = new SolidBrush(Color.Red);
                if (e.Index % 2 == 0)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.Honeydew), e.Bounds);
                }
                else
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.White), e.Bounds);
                }

                e.Graphics.DrawString(listBox1.Items[e.Index].ToString(), new Font("Times New Roman", 14, FontStyle.Regular), Brushes.Black, e.Bounds);
                e.DrawFocusRectangle();

            }
        }

        private void lb_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = (int)e.Graphics.MeasureString(listBox1.Items[e.Index].ToString(), new Font("Times New Roman", 14, FontStyle.Regular), listBox1.Width).Height;
        }
    }
}
