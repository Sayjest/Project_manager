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

namespace _1CNew
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            enter_acc();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                enter_acc();
            }
        }

        private void enter_acc()
        {
            bool enter = false;
            using (StreamReader fs = new StreamReader(@"C:\Файлы1с\u_189.txt"))
            {
                while (true)
                {
                    // Читаем строку из файла во временную переменную.
                    string temp = fs.ReadLine();

                    // Если достигнут конец файла, прерываем считывание.
                    if (temp == null) break;
                    string[] temp2 = temp.Split(new char[] { ' ' });
                    // Пишем считанную строку в итоговую переменную.
                    if (textBox1.Text.Equals(temp2[0]) == true && textBox2.Text.Equals(temp2[1]) == true)
                    {
                        Form2 form2 = new Form2(textBox1.Text);
                        form2.FormClosed += (object s, FormClosedEventArgs ev) => { if (form2.mm == 1) { this.Show(); } };
                        enter = true;
                        this.Hide();
                        form2.Show();
                        break;
                    }
                }
            }
            if (enter == false)
            {
                MessageBox.Show("Введен неправильный логин или пароль!", "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                enter_acc();
            }
        }
    }
}
