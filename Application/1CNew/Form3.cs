using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using MySql.Data.MySqlClient;

namespace _1CNew
{
    public partial class Form3 : Form
    {
        public Form3(string Current, string page)
        {
            InitializeComponent();
            current = Current;
            pageName = page;
        }
        string current, pageName, connStr;

        private void Form3_Load(object sender, EventArgs e)
        {
            this.Text = "Файлы (" + pageName + ")";
            listBox1.DrawMode = DrawMode.OwnerDrawVariable;
            listBox1.MeasureItem += lb_MeasureItem;
            listBox1.DrawItem += lb_DrawItem;
            listBox1.DoubleClick += lb_DoubleClick;
            connStr = "server=****;user=root;password=1234;persistsecurityinfo=True;port=3306;database=basec;SslMode=none";
            MySqlConnection conn6 = new MySqlConnection(connStr);
            conn6.Open();
            string sql6 = "SELECT * FROM " + pageName.Replace(' ', '_') + " WHERE fileName != ''";
            MySqlCommand cmd6 = new MySqlCommand(sql6, conn6);
            MySqlDataReader rdr6 = cmd6.ExecuteReader();
            string Text6;
            int i = 0;
            while (rdr6.Read())
            {
                Text6 = (string)Convert.ChangeType(rdr6[0], typeof(String)) + ". " + (string)Convert.ChangeType(rdr6[2], typeof(String)) + "\n" + (string)Convert.ChangeType(rdr6[1], typeof(String)) + "\nФайл: " + (string)Convert.ChangeType(rdr6[4], typeof(String)) + "\n\n";
                listBox1.Items.Insert(i, Text6);
                i++;
            }
            rdr6.Close();
            conn6.Close();
        }

        private void lb_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (listBox1.Items.Count > 0)
            {
                e.DrawBackground();
                SolidBrush myBrush = new SolidBrush(Color.Red);
                string textLi = listBox1.Items[e.Index].ToString();
                if (e.Index % 2 == 0)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.Cornsilk), e.Bounds);
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
        private void lb_DoubleClick(object sender, EventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            string textLiClick = listBox.Items[listBox.SelectedIndex].ToString();
            string text = textLiClick.Substring(0, textLiClick.IndexOf(". "));
            MySqlConnection conn4 = new MySqlConnection(connStr);
            conn4.Open();
            string sql4 = "SELECT sms_id, filePath FROM " + pageName.Replace(' ', '_') + " WHERE sms_id = " + text;  
            MySqlCommand cmd4 = new MySqlCommand(sql4, conn4);
            MySqlDataReader rdr4 = cmd4.ExecuteReader();
            while (rdr4.Read())
            {
                FolderBrowserDialog dirDialog = new FolderBrowserDialog();
                if (dirDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string filePath = ((string)Convert.ChangeType(rdr4[1], typeof(String))).Replace('╕', '\\');
                        File.Copy(filePath, dirDialog.SelectedPath + "\\" + Path.GetFileName(filePath));
                    }
                    catch
                    {
                        MessageBox.Show("Файл поврежден или уже существует в текущей папке", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
            }
            rdr4.Close();
            conn4.Close();
        }
    }
}
