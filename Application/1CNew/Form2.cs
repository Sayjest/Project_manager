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
using System.Threading;
using System.IO;

namespace _1CNew
{
    public partial class Form2 : Form
    {
        public Form2(string login)
        {
            InitializeComponent();
            Login = login;
            

        }
        public int mm
        {
            get { return chForMm; }
        }
        ListBox lb, lb2, lb_Z;
        string Login, current, help, nom, connStr, firstValueSms, filePathForSend, fileForZ;
        dynamic result;
        bool buttonClick, buttonClick2, buttonClick2Vn, secondClick, potokBe;
        int tPosl, chForMm;
        TabPage tabNew;
        ComboBox combobox;
        Label labelFS, labelFS2;
        TextBox textboxFS, phaseText, phaseTextBox;
        List<string> listSotr;
        MySqlConnection conn, conn12;
        Form3 form3;
        Form4 form4;
        Thread potok, potok2;

        private void Form2_Load(object sender, EventArgs e)
        {
            chForMm = 0;
            potokBe = false;
            label1.Text = "";
            filePathForSend = "";
            ToolTip tip = new ToolTip();
            tip.SetToolTip(button6, "Обновить главную страницу");         
            tip.SetToolTip(button7, "Обновить открытую страницу проекта");
            tip.SetToolTip(button3, "Добавить файл в чат");
            tip.SetToolTip(button4, "Вывести указанное количество прошедших сообщений");
            tip.SetToolTip(button2, "Создать/Обновить чат для открытой страницы проекта");
            tip.SetToolTip(button5, "Показать все файлы текущего чата");
            tip.SetToolTip(button1, "Отправить сообщение в чат");
            tip.SetToolTip(button8, "Сменить пользователя");
            lb = new ListBox();
            lb.Width = tabPage1.Width;
            lb.Height = tabPage1.Height;
            lb.BorderStyle = BorderStyle.Fixed3D;
            lb_Z = new ListBox();
            lb_Z.Width = tabPage2.Width;
            lb_Z.Height = tabPage2.Height;
            lb_Z.BorderStyle = BorderStyle.Fixed3D;
            connStr = "server=****;user=root;password=1234;persistsecurityinfo=True;port=3306;database=basec;SslMode=none";
            conn = new MySqlConnection(connStr);
            conn.Open();
            potok = new Thread(new ThreadStart(function1));                                 
            potok.Start();
            Load_MainPage();
            this.Text = "Приложение (" + current.Substring(0,10) + "...)";
        }

        private void Load_MainPage()
        {
            
            string str = "";
            try
            {
                string user = "admin";
                string pas = "1234";
                string file = "****";
                dynamic refer;
                V83.COMConnector com1s = new V83.COMConnector();
                com1s.PoolCapacity = 10;
                com1s.PoolTimeout = 60;
                com1s.MaxConnections = 2;
                result = com1s.Connect("File='" + file + "';Usr='" + user + "';pwd='" + pas + "';");
                potok2 = new Thread(new ThreadStart(function2));                               
                potok2.Start();


                refer = result.Справочники.ДокументыПроектыП.Выбрать();
                while (refer.Следующий)
                {
                    if (Login.Equals(refer.Логин))
                    {
                        current = refer.Физлицо;
                    }
                }


                refer = result.Документы.ДокументыПроекты.Выбрать();

                if (refer == null)
                {
                    lb.Items.Insert(0, "Активных проектов нет");
                    tabPage1.Controls.Add(lb);
                    lb_Z.Items.Insert(0, "Завершенных проектов нет");
                    tabPage2.Controls.Add(lb_Z);
                }
                else
                {
                    int k = 0;
                    string ruk = "";
                    int i = -1, ii = -1;
                    while (refer.Следующий == true)
                    {
                        int a = 0;
                        foreach (dynamic line in refer.Сотрудники)
                        {

                            if (((line.ФизлицоСтрока).Equals(current) == true) && ((refer.ЗавершенСтрока).Equals("Завершено") == false))
                            {
                                k = 1;
                            }
                            else if (((line.ФизлицоСтрока).Equals(current) == true) && ((refer.ЗавершенСтрока).Equals("Завершено") == true))
                            {
                                k = 2;
                            }
                            if ((line.РуководительСтрока).Equals("РуководительП"))
                            {
                                ruk = line.ФизлицоСтрока;
                            }
                        }
                        if (k == 1)
                        {
                            if (ruk.Equals(current) == true)
                            {
                                ruk = "Вы ";
                            }
                            string Text = refer.ОписаниеПроекта;
                            if (Text.Length > 100)
                            {
                                Text = Text.Substring(0, 100) + "...";
                            }
                            string dateN = (refer.ДатаНач).ToString();
                            string dateK = (refer.ДатаКон).ToString();
                            i++;
                            lb.Items.Insert(i, refer.НаименованиеПроекта + "\n     Дата начала: " + dateN.Substring(0, 10) + "\tДата окончания: " + dateK.Substring(0, 10) + "\n     Руководитель проекта: " + ruk + "\n\tОписание:\n" + Text);

                            lb.SelectedIndexChanged += DoubleClick_Active;                        
                        }
                        else if (k == 2)
                        {
                            if (ruk.Equals(current) == true)
                            {
                                ruk = "Вы ";
                            }
                            string Text = refer.ОписаниеПроекта;
                            if (Text.Length > 100)
                            {
                                Text = Text.Substring(0, 100) + "...";
                            }

                            string dateN = (refer.ДатаНач).ToString();
                            string dateK = (refer.ДатаКон).ToString();
                            ii++;
                            lb_Z.Items.Insert(ii, refer.НаименованиеПроекта + "\n     Дата начала: " + dateN.Substring(0, 10) + "\tДата окончания: " + dateK.Substring(0, 10) + "\n     Руководитель проекта: " + ruk + "\n\tОписание:\n" + Text);
                            lb_Z.SelectedIndexChanged += DoubleClick_Conf;
                        }
                        k = 0;
                    }
                    if (lb.Items.Count > 0)
                    {
                        lb.DrawMode = DrawMode.OwnerDrawVariable;
                        lb.MeasureItem += lb_MeasureItem;
                        lb.DrawItem += lb_DrawItem;
                        tabPage1.Controls.Add(lb);
                    }
                    else
                    {
                        lb.Items.Insert(0, "Активных проектов нет");
                        tabPage1.Controls.Add(lb);
                    }
                    if (lb_Z.Items.Count > 0)
                    {
                        lb_Z.DrawMode = DrawMode.OwnerDrawVariable;
                        lb_Z.MeasureItem += lb_Z_MeasureItem;
                        lb_Z.DrawItem += lb_Z_DrawItem;
                        tabPage2.Controls.Add(lb_Z);
                    }
                    else
                    {
                        lb_Z.Items.Insert(0, "Активных проектов нет");
                        tabPage2.Controls.Add(lb_Z);
                    }
                }
            }
            catch (Exception ex)
            {
                lb.Items.Insert(0, ex.Message.ToString());
                lb.DrawMode = DrawMode.OwnerDrawVariable;
                lb.MeasureItem += lb_MeasureItem;
                lb.DrawItem += lb_DrawItem;
                tabPage1.Controls.Add(lb);
            }
        }

        private void function2()
        {
            potokBe = true;
            dynamic refer = result.Документы.ДокументыПроекты.Выбрать();
            conn12 = new MySqlConnection(connStr);
            conn12.Open();
            while (refer.Следующий() == true)
            {
                if (refer.Проверен.Equals("Нет"))
                {                   
                    try                                 // чат
                    {
                        try
                        {
                            string sql12 = "";
                            if ((refer.ЗавершенСтрока).Equals("Завершено") == false)
                            {
                                sql12 = "CREATE TABLE " + (refer.НаименованиеПроекта).Replace(' ', '_') + " (sms_id SMALLINT NOT NULL AUTO_INCREMENT PRIMARY KEY, time DATETIME, name VARCHAR(100), text VARCHAR(500), fileName VARCHAR(100), filePath VARCHAR (120))";
                            }
                            else if ((refer.ЗавершенСтрока).Equals("Завершено") == true)
                            {
                                sql12 = "DROP TABLE " + (refer.НаименованиеПроекта).Replace(' ', '_');
                            }
                            MySqlCommand cmd12 = new MySqlCommand(sql12, conn12);
                            cmd12.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            // ничего не делать
                        }
                        try
                        {
                            string sql12 = "";
                            if ((refer.ЗавершенСтрока).Equals("Завершено") == false)
                            {
                                sql12 = "CREATE TABLE " + (refer.НаименованиеПроекта).Replace(' ', '_') + "З (sms_id SMALLINT NOT NULL AUTO_INCREMENT PRIMARY KEY, name VARCHAR(100), text VARCHAR(500), fileName VARCHAR(100), filePath VARCHAR (120))";
                            }
                            else if ((refer.ЗавершенСтрока).Equals("Завершено") == true)
                            {
                                sql12 = "DROP TABLE " + (refer.НаименованиеПроекта).Replace(' ', '_') + "З";
                            }
                            MySqlCommand cmd12 = new MySqlCommand(sql12, conn12);
                            cmd12.ExecuteNonQuery();
                        }
                        catch
                        {
                            // ничего не делать
                        }
                    }
                    catch
                    {
                        //
                    }
                    dynamic rp = refer.ПолучитьОбъект();
                    rp.Проверен = "Да";
                    rp.Записать();
                }
            }          
            conn12.Close();
            potokBe = false;
            potok2.Abort();         
            potok2.Join(50);
        }

        private void DoubleClick_Conf(object sender, EventArgs e)
        {
            tPosl = 0;
            ListBox listBox = (ListBox)sender;
            string[] text = (listBox.Items[listBox.SelectedIndex].ToString()).Split(new char[] { '\n' });
            if (text[0].Equals(help) == false)
            {
                tabNew = new TabPage();
                Conf_Ex(text);
                tabControl1.Controls.Add(tabNew);
            }

        }

        private void DoubleClick_Active(object sender, EventArgs e)
        {
            tPosl = 0;
            ListBox listBox = (ListBox)sender;
            string[] text = (listBox.Items[listBox.SelectedIndex].ToString()).Split(new char[] { '\n' });
            if (text[0].Equals(help) == false)
            {
                tabNew = new TabPage();
                Active_Ex(text);
                tabControl1.Controls.Add(tabNew);
            }
        }

        private void Active_Ex (string [] text)
        {
            tabNew.AutoScroll = true;
            tabNew.BackColor = Color.White;
            help = text[0];
            tabControl1.SelectedIndex = tabControl1.TabPages.IndexOf(tabNew);
            tabNew.Text = text[0];
            tabNew.MouseDoubleClick += Delete_Page;
            tabNew.ToolTipText = text[0];
            Label label1 = Create_Label(5, 5, 16, text[0]);
            Label label2 = Create_Label(5, label1.Bottom + 5, 14, text[2].Substring(5));
            Label label3;
            tabNew.Controls.Add(label1);
            tabNew.Controls.Add(label2);
            dynamic refer = result.Документы.ДокументыПроекты.Выбрать();
            while (refer.Следующий == true)
            {
                if ((refer.НаименованиеПроекта).Equals(text[0]))
                {
                    Label label4 = Create_Label(5, label2.Bottom + 30, 14, "Другие участники проекта:");
                    tabNew.Controls.Add(label4);
                    int i = 1, iPosl = label4.Bottom;
                    foreach (dynamic line in refer.Сотрудники)
                    {
                        if ((line.ФизлицоСтрока).Equals(current))
                        {
                            label3 = Create_Label(5, label2.Bottom + 5, 14, "Ваша должность: " + line.ДолжностьСтрока);
                            tabNew.Controls.Add(label3);
                        }
                        if ((line.ФизлицоСтрока).Equals(current) == false && (line.ФизлицоСтрока).Equals(text[2]) == false)
                        {
                            Label label5 = Create_Label(5, label4.Top + 25 * i, 14, line.ФизлицоСтрока);
                            tabNew.Controls.Add(label5);
                            iPosl = label5.Bottom;
                            i++;
                        }

                    }
                    if (iPosl == label4.Bottom)
                    {
                        Label label5 = Create_Label(5, iPosl + 4, 14, "Нет");
                        tabNew.Controls.Add(label5);
                        iPosl = label5.Bottom;
                    }

                    Label label6 = Create_Label(5, iPosl + 5, 16, "Описание:");
                    TextBox textbox6 = Create_TextBox(5, label6.Bottom + 5, 640, refer.ОписаниеПроекта);
                    textbox6.ScrollBars = ScrollBars.Vertical;
                    textbox6.ReadOnly = true;
                    tabNew.Controls.Add(label6);
                    tabNew.Controls.Add(textbox6);

                    string phase = "";                                                                   // Фаза
                    foreach (dynamic line in refer.Фазы)
                    {
                        phase = line.НаименованиеФазы;
                    }
                    Label label7 = Create_Label(5, textbox6.Bottom + 5, 14, "Текущая фаза: " + phase);
                    tabNew.Controls.Add(label7);

                    if (text[2].Substring(27).Equals("Вы "))                             // ветка руководителя        начало
                    {
                        Label labelForSotr = Create_Label(5, label7.Bottom + 25, 16, "Новое задание для сотрудника");
                        tabNew.Controls.Add(labelForSotr);
                        combobox = new ComboBox();
                        combobox.Width = 250;
                        listSotr = new List<string>();
                        foreach (dynamic line in refer.Сотрудники)
                        {
                            if (current.Equals(line.ФизлицоСтрока) != true)
                            {
                                combobox.Items.Add(line.ФизлицоСтрока);
                                listSotr.Add(line.ФизлицоСтрока);
                            }
                        }
                        combobox.Location = new Point(5, labelForSotr.Bottom + 10);
                        combobox.Font = new Font("Times New Roman", 12, FontStyle.Regular);
                        ToolTip tip = new ToolTip();
                        tip.SetToolTip(combobox, "Выберите сотрудника, которому будете отправлять задание");
                        tabNew.Controls.Add(combobox);
                        Button buttonAllZ = Create_Button(combobox.Right + 5, combobox.Location.Y, "Задания");
                        buttonAllZ.Click += buttonAllZ_Click;
                        ToolTip tipForBAZ = new ToolTip();
                        tipForBAZ.SetToolTip(buttonAllZ, "Показать все задания, отправленные сотрудникам ранее");
                        tabNew.Controls.Add(buttonAllZ);
                        labelFS = Create_Label(buttonAllZ.Right + 5, combobox.Location.Y + 3, 15, "");   // √
                        Button buttonFileSend = Create_Button(buttonAllZ.Right + 30, combobox.Location.Y, "Файл");
                        buttonFileSend.Click += buttonFileSend_Click;
                        ToolTip tipForBFS = new ToolTip();
                        tipForBFS.SetToolTip(buttonFileSend, "Прикрепить файл");
                        Button buttonSend = Create_Button(buttonFileSend.Right + 5, combobox.Location.Y, "Отправить");
                        buttonSend.Click += buttonSend_Click;
                        ToolTip tipForBS = new ToolTip();
                        tipForBFS.SetToolTip(buttonSend, "Отправить задание и файл (если есть)");
                        labelFS2 = Create_Label(100, combobox.Bottom + 5, 10, filePathForSend);
                        labelFS2.ForeColor = Color.Gray;
                        tabNew.Controls.Add(labelFS);
                        tabNew.Controls.Add(buttonFileSend);
                        tabNew.Controls.Add(buttonSend);
                        tabNew.Controls.Add(labelFS2);
                        textboxFS = Create_TextBox(5, combobox.Bottom + 25, 640, "");
                        tip.SetToolTip(textboxFS, "Введите текст задания");
                        textboxFS.ScrollBars = ScrollBars.Vertical;
                        tabNew.Controls.Add(textboxFS);
                        Label label8 = Create_Label(5, textboxFS.Bottom + 15, 16, "Файлы");
                        tabNew.Controls.Add(label8);
                        int k = 1, kPosl = label8.Bottom;

                        foreach (dynamic line in refer.Файлы)
                        {
                            Label label9 = Create_Label(15, textboxFS.Bottom + 20 + k * 25, 14, k + ". " + line.ИмяФайла);
                            tabNew.Controls.Add(label9);
                            Label label9_2 = Create_Label(label9.Right + 5, label9.Location.Y, 14, line.Владелец);
                            Image image1 = Image.FromFile("C:\\Файлы1с\\save.png");
                            label9_2.Image = image1;
                            label9_2.ForeColor = Color.Transparent;
                            label9_2.ImageAlign = ContentAlignment.TopLeft;
                            label9_2.TextAlign = ContentAlignment.BottomRight;                               //сдела
                            label9_2.DoubleClick += label9_2_DoubleClick;
                            tabNew.Controls.Add(label9_2);
                            kPosl = label9.Bottom;
                            k++;
                        }
                        if (kPosl == label8.Bottom)
                        {
                            Label label9 = Create_Label(15, textboxFS.Bottom + 20 + k * 25, 14, "Нет");
                            tabNew.Controls.Add(label9);
                            kPosl = label9.Bottom;
                        }
                        Button buttonAddFile = Create_Button(15, kPosl + 15, "Добавить");
                        ToolTip toolTipAddF = new ToolTip();
                        toolTipAddF.SetToolTip(buttonAddFile, "Добавить файл в базу 1С. После этого обновите страницу");
                        buttonAddFile.Click += buttonAddF_Click;
                        tabNew.Controls.Add(buttonAddFile);
                        Label label10 = Create_Label(5, buttonAddFile.Bottom + 25, 16, "Новая фаза:");
                        tabNew.Controls.Add(label10);
                        phaseText = Create_TextBox(5, label10.Bottom + 15, 350, phase);
                        phaseText.Height = 30;
                        toolTipAddF.SetToolTip(phaseText, "Введите наименование новой фазы (при необходимости)");
                        tabNew.Controls.Add(phaseText);
                        phaseTextBox = Create_TextBox(5, phaseText.Bottom + 15, 640, "");
                        phaseTextBox.ScrollBars = ScrollBars.Vertical;
                        toolTipAddF.SetToolTip(phaseTextBox, "Введите описание фазы");
                        tabNew.Controls.Add(phaseTextBox);
                        Button buttonAddPhase = Create_Button(400, phaseTextBox.Bottom + 15, "Добавить");
                        ToolTip toolTipAddPh = new ToolTip();
                        toolTipAddPh.SetToolTip(buttonAddPhase, "Добавить фазу в базу 1С. После этого обновите страницу");
                        buttonAddPhase.Click += buttonAddPh_Click;
                        tabNew.Controls.Add(buttonAddPhase);
                        Label label11 = Create_Label(5, buttonAddPhase.Bottom + 25, 14, "Предыдущие фазы:");
                        tabNew.Controls.Add(label11);
                        int s = 1, sPosl = label11.Bottom - 90;
                        List<string> listPhase1 = new List<string>();
                        List<string> listPhase2 = new List<string>();
                        foreach (dynamic line in refer.Фазы)
                        {
                            listPhase1.Add(line.НаименованиеФазы);
                            listPhase2.Add(line.Описание);
                        }
                        int lPosl = 0;
                        if (listPhase1.Count != 0)
                        {
                            for (int ss = listPhase1.Count - 1; ss > -1; ss--)
                            {
                                Label label12 = Create_Label(5, sPosl + s * 90, 14, listPhase1.ElementAt(ss));
                                tabNew.Controls.Add(label12);
                                TextBox text12 = Create_TextBox(25, label12.Bottom + 5, 620, listPhase2.ElementAt(ss));
                                text12.Height = 50;
                                text12.ScrollBars = ScrollBars.Vertical;
                                text12.ReadOnly = true;
                                tabNew.Controls.Add(text12);
                                lPosl = text12.Bottom;
                                s++;
                            }
                        }
                        else
                        {
                            Label label12 = Create_Label(25, label11.Bottom + 15, 14, "Нет");
                            tabNew.Controls.Add(label12);
                            lPosl = label12.Bottom;
                        }
                        listPhase1.Clear();
                        listPhase2.Clear();
                        Label label13 = Create_Label(5, lPosl + 20, 2, "");
                        tabNew.Controls.Add(label13);                                          // ветка руководителя        конец

                    }
                    else
                    {
                        Label label8 = Create_Label(5, label7.Bottom + 25, 16, "Ваше задание:");
                        tabNew.Controls.Add(label8);
                        MySqlConnection conn10 = new MySqlConnection(connStr);
                        conn10.Open();
                        string sql10 = "SELECT t.text, t.fileName, t.filePath FROM (SELECT * FROM " + help.Replace(' ', '_') + "З WHERE name LIKE '" + current + "' ORDER BY sms_id DESC LIMIT 1) as t ";
                        MySqlCommand cmd10 = new MySqlCommand(sql10, conn10);
                        MySqlDataReader rdr10 = cmd10.ExecuteReader();
                        int mPosl = 0, mm = 0;
                        while (rdr10.Read())
                        {
                            mm = 1;
                            string te = (string)Convert.ChangeType(rdr10[0], typeof(String));
                            if (te.Equals(""))
                            {
                                te = "Ваше задание находится в прикрепленном файле";
                            }
                            TextBox textBox8 = Create_TextBox(5, label8.Bottom + 15, 640, te);
                            textBox8.ScrollBars = ScrollBars.Vertical;
                            textBox8.ReadOnly = true;
                            tabNew.Controls.Add(textBox8);
                            mPosl = textBox8.Bottom;
                            if (Convert.ChangeType(rdr10[1], typeof(String)).Equals("") == false)
                            {
                                Label label9 = Create_Label(15, label8.Bottom + 220, 14, (string)Convert.ChangeType(rdr10[1], typeof(String)));
                                tabNew.Controls.Add(label9);
                                Label label9_2 = Create_Label(label9.Right + 5, label9.Location.Y, 14, (string)Convert.ChangeType(rdr10[2], typeof(String)));
                                fileForZ = (string)Convert.ChangeType(rdr10[2], typeof(String));
                                Image image1 = Image.FromFile("C:\\Файлы1с\\save.png");
                                label9_2.Image = image1;
                                label9_2.ForeColor = Color.Transparent;
                                label9_2.ImageAlign = ContentAlignment.TopLeft;
                                label9_2.TextAlign = ContentAlignment.BottomRight;                     

                                label9_2.DoubleClick += label9_2Z_DoubleClick;
                                tabNew.Controls.Add(label9_2);
                                mPosl = label9_2.Bottom;
                            }
                        }
                        if (mm == 0)
                        {

                            TextBox textBox8 = Create_TextBox(5, label8.Bottom + 15, 640, "Текущего задания нет");
                            textBox8.ScrollBars = ScrollBars.Vertical;
                            textBox8.ReadOnly = true;
                            tabNew.Controls.Add(textBox8);
                            mPosl = textBox8.Bottom;
                        }
                        rdr10.Close();
                        conn10.Close();

                        Label label10 = Create_Label(5, mPosl + 15, 16, "Файлы");
                        tabNew.Controls.Add(label10);
                        int k = 1, kPosl = label10.Bottom;
                        foreach (dynamic line in refer.Файлы)
                        {
                            Label label9 = Create_Label(15, mPosl + 20 + k * 25, 14, k + ". " + line.ИмяФайла);
                            tabNew.Controls.Add(label9);
                            Label label9_2 = Create_Label(label9.Right + 5, label9.Location.Y, 14, line.Владелец);
                            Image image1 = Image.FromFile("C:\\Файлы1с\\save.png");
                            label9_2.Image = image1;
                            label9_2.ForeColor = Color.Transparent;
                            label9_2.ImageAlign = ContentAlignment.TopLeft;
                            label9_2.TextAlign = ContentAlignment.BottomRight;                               //

                            label9_2.DoubleClick += label9_2_DoubleClick;
                            tabNew.Controls.Add(label9_2);
                            kPosl = label9.Bottom;
                            k++;
                        }
                        if (kPosl == label10.Bottom)
                        {
                            Label label9 = Create_Label(15, mPosl + 20 + k * 25, 14, "Нет");
                            tabNew.Controls.Add(label9);
                            kPosl = label9.Bottom;
                        }
                        Label label11 = Create_Label(5, kPosl + 25, 16, "Добавить комментарий");
                        ToolTip toolTipLabel11 = new ToolTip();
                        toolTipLabel11.SetToolTip(label11, "Добавить комментарий к текущей фазе (" + phase + ")");
                        tabNew.Controls.Add(label11);
                        phaseTextBox = Create_TextBox(5, label11.Bottom + 15, 640, "");
                        phaseTextBox.ScrollBars = ScrollBars.Vertical;
                        tabNew.Controls.Add(phaseTextBox);
                        Button buttonAddPhase = Create_Button(400, phaseTextBox.Bottom + 15, "Добавить");
                        ToolTip toolTipAddPh = new ToolTip();
                        toolTipAddPh.SetToolTip(buttonAddPhase, "Добавить комментарий к фазе в базу 1С");
                        buttonAddPhase.Click += buttonAddCom_Click;
                        tabNew.Controls.Add(buttonAddPhase);
                        Label label12 = Create_Label(5, buttonAddPhase.Bottom + 20, 2, "");
                        tabNew.Controls.Add(label12);
                    }

                    break;       
                }
            }           
        }

        private void Conf_Ex(string [] text)
        {
            tabNew.AutoScroll = true;
            tabNew.BackColor = Color.White;
            help = text[0];
            tabControl1.SelectedIndex = tabControl1.TabPages.IndexOf(tabNew);
            tabNew.Text = text[0];
            tabNew.MouseDoubleClick += Delete_Page;
            tabNew.ToolTipText = text[0];
            Label label1 = Create_Label(5, 5, 16, text[0]);
            Label label2 = Create_Label(5, label1.Bottom + 5, 14, text[2].Substring(5));
            Label label3;
            tabNew.Controls.Add(label1);
            tabNew.Controls.Add(label2);
            dynamic refer = result.Документы.ДокументыПроекты.Выбрать();
            while (refer.Следующий == true)
            {
                if ((refer.НаименованиеПроекта).Equals(text[0]))
                {
                    Label label4 = Create_Label(5, label2.Bottom + 30, 14, "Другие участники проекта:");
                    tabNew.Controls.Add(label4);
                    int i = 1, iPosl = label4.Bottom;
                    foreach (dynamic line in refer.Сотрудники)
                    {
                        if ((line.ФизлицоСтрока).Equals(current))
                        {
                            label3 = Create_Label(5, label2.Bottom + 5, 14, "Ваша должность: " + line.ДолжностьСтрока);
                            tabNew.Controls.Add(label3);
                        }
                        if ((line.ФизлицоСтрока).Equals(current) == false && (line.ФизлицоСтрока).Equals(text[2]) == false)
                        {
                            Label label5 = Create_Label(5, label4.Top + 25 * i, 14, line.ФизлицоСтрока);
                            tabNew.Controls.Add(label5);
                            iPosl = label5.Bottom;
                            i++;
                        }
                    }
                    if (iPosl == label4.Bottom)
                    {
                        Label label5 = Create_Label(5, iPosl + 4, 14, "Нет");
                        tabNew.Controls.Add(label5);
                        iPosl = label5.Bottom;
                    }

                    Label label6 = Create_Label(5, iPosl + 5, 16, "Описание:");
                    TextBox textbox6 = Create_TextBox(5, label6.Bottom + 5, 640, refer.ОписаниеПроекта);
                    textbox6.ScrollBars = ScrollBars.Vertical;
                    textbox6.ReadOnly = true;
                    tabNew.Controls.Add(label6);
                    tabNew.Controls.Add(textbox6);

                    if (text[2].Substring(27).Equals("Вы "))                             // ветка руководителя        начало
                    {
                        Label label8 = Create_Label(5, textbox6.Bottom + 15, 16, "Файлы");
                        tabNew.Controls.Add(label8);
                        int k = 1, kPosl = label8.Bottom;

                        foreach (dynamic line in refer.Файлы)
                        {
                            Label label9 = Create_Label(15, textbox6.Bottom + 20 + k * 25, 14, k + ". " + line.ИмяФайла);
                            tabNew.Controls.Add(label9);
                            Label label9_2 = Create_Label(label9.Right + 5, label9.Location.Y, 14, line.Владелец);
                            Image image1 = Image.FromFile("C:\\Файлы1с\\save.png");
                            label9_2.Image = image1;
                            label9_2.ForeColor = Color.Transparent;
                            label9_2.ImageAlign = ContentAlignment.TopLeft;
                            label9_2.TextAlign = ContentAlignment.BottomRight;                       

                            label9_2.DoubleClick += label9_2_DoubleClick;
                            tabNew.Controls.Add(label9_2);
                            kPosl = label9.Bottom;
                            k++;
                        }
                        if (kPosl == label8.Bottom)
                        {
                            Label label9 = Create_Label(15, textboxFS.Bottom + 20 + k * 25, 14, "Нет");
                            tabNew.Controls.Add(label9);
                            kPosl = label9.Bottom;
                        }

                        Label label11 = Create_Label(5, kPosl + 25, 14, "Предыдущие фазы:");
                        tabNew.Controls.Add(label11);
                        int s = 1, sPosl = label11.Bottom - 90;
                        List<string> listPhase1 = new List<string>();
                        List<string> listPhase2 = new List<string>();
                        foreach (dynamic line in refer.Фазы)
                        {
                            listPhase1.Add(line.НаименованиеФазы);
                            listPhase2.Add(line.Описание);

                        }
                        int lPosl = 0;
                        if (listPhase1.Count != 0)
                        {
                            for (int ss = listPhase1.Count - 1; ss > -1; ss--)
                            {
                                Label label12 = Create_Label(5, sPosl + s * 90, 14, listPhase1.ElementAt(ss));
                                tabNew.Controls.Add(label12);
                                TextBox text12 = Create_TextBox(25, label12.Bottom + 5, 620, listPhase2.ElementAt(ss));
                                text12.Height = 50;
                                text12.ScrollBars = ScrollBars.Vertical;
                                text12.ReadOnly = true;
                                tabNew.Controls.Add(text12);
                                lPosl = text12.Bottom;
                                s++;
                            }
                        }
                        else
                        {
                            Label label12 = Create_Label(25, label11.Bottom + 15, 14, "Нет");
                            tabNew.Controls.Add(label12);
                            lPosl = label12.Bottom;
                        }
                        listPhase1.Clear();
                        listPhase2.Clear();
                        Label label13 = Create_Label(5, lPosl + 20, 2, "");
                        tabNew.Controls.Add(label13);                                          // ветка руководителя        конец

                    }
                    else
                    {
                        Label label10 = Create_Label(5, textbox6.Bottom + 15, 16, "Файлы");
                        tabNew.Controls.Add(label10);
                        int k = 1, kPosl = label10.Bottom;
                        foreach (dynamic line in refer.Файлы)
                        {
                            Label label9 = Create_Label(15, textbox6.Bottom + 20 + k * 25, 14, k + ". " + line.ИмяФайла);
                            tabNew.Controls.Add(label9);
                            Label label9_2 = Create_Label(label9.Right + 5, label9.Location.Y, 14, line.Владелец);
                            Image image1 = Image.FromFile("C:\\Файлы1с\\save.png");
                            label9_2.Image = image1;
                            label9_2.ForeColor = Color.Transparent;
                            label9_2.ImageAlign = ContentAlignment.TopLeft;
                            label9_2.TextAlign = ContentAlignment.BottomRight;                    

                            label9_2.DoubleClick += label9_2_DoubleClick;
                            tabNew.Controls.Add(label9_2);
                            kPosl = label9.Bottom;
                            k++;
                        }
                        if (kPosl == label10.Bottom)
                        {
                            Label label9 = Create_Label(15, textbox6.Bottom + 20 + k * 25, 14, "Нет");
                            tabNew.Controls.Add(label9);
                            kPosl = label9.Bottom;
                        }

                        Label label12 = Create_Label(5, kPosl + 20, 2, "");
                        tabNew.Controls.Add(label12);
                    }
                    break;          
                }
            }
        }

        private void buttonAddCom_Click(object sender, EventArgs e)
        {
            if (phaseTextBox.Text.Equals("") == false)
            {
                dynamic refer = result.Документы.ДокументыПроекты.Выбрать();
                dynamic rp, rr;
                while (refer.Следующий == true)
                {
                    if (help.Equals(refer.НаименованиеПроекта))
                    {
                        string temp = "";
                        foreach (dynamic line in refer.Фазы)
                        {
                            temp = line.НаименованиеФазы;
                        }
                        if (temp.Equals("") == false)
                        {
                            rp = refer.ПолучитьОбъект();
                            rr = rp.Фазы.Добавить();
                            rr.НаименованиеФазы = temp;
                            rr.Описание = "[" + current + "] - " + phaseTextBox.Text;
                            rp.Записать();
                            phaseTextBox.Clear();
                        }
                        else
                        {
                            MessageBox.Show("Поле комментария к фазе не может быть пустым", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                }
            }
        }

        private void label9_2Z_DoubleClick(object sender, EventArgs e)
        {
            FolderBrowserDialog dirDialog = new FolderBrowserDialog();

            if (dirDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string filePath = fileForZ.Replace('╕', '\\') ;
                    File.Copy(filePath, dirDialog.SelectedPath + "\\" + Path.GetFileName(filePath));
                }
                catch
                {
                    MessageBox.Show("Файл поврежден или уже существует в текущей папке", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
        }

        private void buttonAllZ_Click(object sender, EventArgs e)
        {
            form4 = new Form4(listSotr, tabControl1.SelectedTab.Text, connStr);
            form4.Show();
        }

        private void buttonAddPh_Click(object sender, EventArgs e)
        {
            if (phaseText.Text.Equals("") == false && phaseTextBox.Text.Equals("") == false)
            {
                dynamic refer = result.Документы.ДокументыПроекты.Выбрать();
                dynamic rp, rr;
                while (refer.Следующий == true)
                {
                    if (help.Equals(refer.НаименованиеПроекта))
                    {
                        rp = refer.ПолучитьОбъект();
                        rr = rp.Фазы.Добавить();
                        rr.НаименованиеФазы = phaseText.Text;
                        rr.Описание = phaseTextBox.Text;
                        rp.Записать();
                        phaseTextBox.Clear();
                    }
                }
            }
            else
            {
                MessageBox.Show("Поле описания фазы не может быть пустым", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void buttonAddF_Click(object sender, EventArgs e)
        {
            dynamic refer = result.Документы.ДокументыПроекты.Выбрать();
            dynamic rp, rr;
            while (refer.Следующий == true)
            {
                if (help.Equals(refer.НаименованиеПроекта))
                {
                    OpenFileDialog openFileDialog3 = new OpenFileDialog();
                    openFileDialog3.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                    openFileDialog3.RestoreDirectory = true;
                    bool create = false;
                    string newFileName = "";
                    Random rand = new Random();
                    if (openFileDialog3.ShowDialog() == DialogResult.OK)
                    {
                        while (create != true)
                        {
                            newFileName = "C:\\Файлы1с\\" + tabControl1.SelectedTab.Text + "О\\" + rand.Next(0, 200) + "_" + Path.GetFileName(openFileDialog3.FileName);
                            if (File.Exists(newFileName) == false)
                            {
                                create = true;
                                File.Copy(openFileDialog3.FileName, newFileName);
                            }
                        }
                        rp = refer.ПолучитьОбъект();
                        rr = rp.Файлы.Добавить();
                        rr.ИмяФайла = Path.GetFileName(openFileDialog3.FileName);
                        rr.Владелец = newFileName;
                        rp.Записать();
                    }
                }
            } 
        }

        private void label9_2_DoubleClick(object sender, EventArgs e)
        {
            Label label = (Label)sender;
            FolderBrowserDialog dirDialog = new FolderBrowserDialog();
            
            if (dirDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string filePath = label.Text;

                    File.Copy(filePath, dirDialog.SelectedPath + "\\" + Path.GetFileName(filePath));
                }
                catch
                {
                    MessageBox.Show("Файл поврежден или уже существует в текущей папке", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (combobox.SelectedItem != null)
            {
                Random rand = new Random();
                string sql7 = "";
                string newFileName = "";
                bool create = false;
                if (filePathForSend.Equals("") != true)
                {
                    while (create != true)
                    {
                        newFileName = "C:\\Файлы1с\\" + help + "\\" + rand.Next(0, 200) + "_" + Path.GetFileName(filePathForSend);
                        if (File.Exists(newFileName) == false)
                        {
                            create = true;
                        }
                    }
                    sql7 = "INSERT INTO " + help.Replace(' ', '_') + "З (name, text, fileName, filePath) VALUES ('" + combobox.SelectedItem.ToString() + "', '" + textboxFS.Text + "', '" + Path.GetFileName(filePathForSend) + "', '" + newFileName.Replace('\\', '╕') + "')";  // alt + 184
                    File.Copy(filePathForSend, newFileName);
                }
                else
                {
                    sql7 = "INSERT INTO " + help.Replace(' ', '_') + "З (name, text) VALUES ('" + combobox.SelectedItem.ToString() + "', '" + textboxFS.Text + "')";
                }
                MySqlConnection conn7 = new MySqlConnection(connStr);
                conn7.Open();
                
                MySqlCommand cmd7 = new MySqlCommand(sql7, conn7);
                cmd7.ExecuteNonQuery();
                conn7.Close();
                filePathForSend = "";
                labelFS.Text = "";
                labelFS2.Text = "";
                textboxFS.Clear();
            }
            else
            {
                MessageBox.Show("Поле сотрудника не может быть пустым!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void lb_Z_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (lb_Z.Items.Count > 0)
            {
                e.DrawBackground();
                SolidBrush myBrush = new SolidBrush(Color.Red);
                string textLi = lb_Z.Items[e.Index].ToString();
                if (e.Index % 2 == 0)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.LightGray), e.Bounds);
                }
                else
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.White), e.Bounds);
                }
                e.Graphics.DrawString(lb_Z.Items[e.Index].ToString(), new Font("Times New Roman", 14, FontStyle.Regular), Brushes.Black, e.Bounds);
                e.DrawFocusRectangle();
            }
        }

        private void lb_Z_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = (int)e.Graphics.MeasureString(lb_Z.Items[e.Index].ToString(), new Font("Times New Roman", 14, FontStyle.Regular), lb_Z.Width).Height;
        }

        private void buttonFileSend_Click(object sender, EventArgs e)
        {          
            OpenFileDialog openFileDialog2 = new OpenFileDialog();
            openFileDialog2.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog2.RestoreDirectory = true;
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                filePathForSend = openFileDialog2.FileName;
                labelFS2.Text = filePathForSend;
                labelFS.Text = "√";            
            }          
        }

        private void function1()
        {
            int kol = 0;
            while (true)
            {
                if (buttonClick2 == true)
                {
                    secondClick = true;
                    buttonClick2 = false;
                    nom = "0";
                    tPosl = 0;
                    bool first = true;
                    
                    try
                    {
                        string pageName = "";
                        lb2 = new ListBox();                                            
                        lb2.Width = tabPage3.Width;
                        lb2.Height = tabPage3.Height;
                        lb2.BorderStyle = BorderStyle.Fixed3D;
                        lb2.DoubleClick += lb2_DoubleClick;
                        int i = 0;              
                        tabControl1.Invoke(new Action(() => { pageName = tabControl1.SelectedTab.Text ; }));
                        if ((pageName.Equals("") == false) && (pageName.Equals("Текущие") == false) && (pageName.Equals("Завершенные") == false))
                        {
                            button2.Invoke(new Action(() => { button2.Enabled = false; }));
                            tabPage3.Invoke(new Action(() => { tabPage3.Controls.Clear(); tabPage3.Text = pageName; tabPage3.ToolTipText = pageName; }));
                            string pageNameInv = pageName.Replace(' ', '_');
                            string sql1 = "SELECT t.sms_id, t.time, t.name, t.text, t.fileName, t.filePath FROM (SELECT * FROM " + pageNameInv + " ORDER BY sms_id DESC LIMIT 50)as t ORDER BY t.sms_id";
                            MySqlCommand cmd = new MySqlCommand(sql1, conn);
                            string Text1 = "";
                            int tNach = 0;
                            MySqlDataReader rdr = cmd.ExecuteReader();
                            
                            while (rdr.Read()) 
                            {
                                string name = (string)Convert.ChangeType(rdr[2], typeof(String));
                                if (name.Equals(current))
                                {
                                    name = "Вы";
                                }

                                if (Convert.ChangeType(rdr[3], typeof(String)).Equals("") != true && Convert.ChangeType(rdr[4], typeof(String)).Equals("") == true)
                                {
                                    Text1 = (string)Convert.ChangeType(rdr[0], typeof(String)) + ". " + name + "\n" + (string)Convert.ChangeType(rdr[1], typeof(String)) + "\n" + (string)Convert.ChangeType(rdr[3], typeof(String)) + "\n\n";
                                }
                                else if (Convert.ChangeType(rdr[3], typeof(String)).Equals("") == true && Convert.ChangeType(rdr[4], typeof(String)).Equals("") != true)
                                {
                                    Text1 = (string)Convert.ChangeType(rdr[0], typeof(String)) + ". " + name + "\n" + (string)Convert.ChangeType(rdr[1], typeof(String)) + "\nФайл: " + (string)Convert.ChangeType(rdr[4], typeof(String)) + "\n\n";                   
                                }
                                nom = (string)Convert.ChangeType(rdr[0], typeof(String));
                                if (first == true)
                                {
                                    firstValueSms = nom;
                                    first = false;
                                }
                                lb2.Items.Insert(i, Text1);
                                i++;
                            }
                            rdr.Close();

                            lb2.DrawMode = DrawMode.OwnerDrawVariable;
                            lb2.MeasureItem += lb2_MeasureItem;
                            lb2.DrawItem += lb2_DrawItem;
                            tabPage3.Invoke(new Action(() => { tabPage3.Controls.Add(lb2); }));                            
                            tabPage3.Invoke(new Action(() => { lb2.SelectedIndex = lb2.Items.Count - 1; }));
                            button2.Invoke(new Action(() => { button2.Enabled = true; }));                       
                            string sql2;
                            MySqlCommand cmd2;
                            MySqlDataReader rdr2;
                            string Text2 = "";

                            while (true)
                            {
                                if (buttonClick2Vn == true)
                                {
                                    buttonClick2Vn = false;
                                    break;
                                }
                                sql2 = "SELECT sms_id, time, name, text, fileName, filePath FROM " + pageNameInv + " WHERE sms_id > " + nom;
                                cmd2 = new MySqlCommand(sql2, conn);
                                rdr2 = cmd2.ExecuteReader();
                                while (rdr2.Read())
                                {
                                    string name = (string)Convert.ChangeType(rdr2[2], typeof(String));
                                    if (name.Equals(current))
                                    {
                                        name = "Вы";
                                    }
                                    if (Convert.ChangeType(rdr2[3], typeof(String)).Equals("") != true && Convert.ChangeType(rdr2[4], typeof(String)).Equals("") == true)
                                    {
                                        Text2 = (string)Convert.ChangeType(rdr2[0], typeof(String)) + ". " + name + "\n" + (string)Convert.ChangeType(rdr2[1], typeof(String)) + "\n" + (string)Convert.ChangeType(rdr2[3], typeof(String)) + "\n\n";
                                    }
                                    else if (Convert.ChangeType(rdr2[3], typeof(String)).Equals("") == true && Convert.ChangeType(rdr2[4], typeof(String)).Equals("") != true)
                                    {
                                        Text2 = (string)Convert.ChangeType(rdr2[0], typeof(String)) + ". " + name + "\n" + (string)Convert.ChangeType(rdr2[1], typeof(String)) + "\nФайл: " + (string)Convert.ChangeType(rdr2[4], typeof(String)) + "\n\n";                                                    
                                    }
                                    tabPage3.Invoke(new Action(() => { lb2.Items.Insert(i, Text2); lb2.SelectedIndex = lb2.Items.Count - 1; }));
                                    nom = (string)Convert.ChangeType(rdr2[0], typeof(String));
                                    if (first == true)
                                    {
                                        firstValueSms = nom;
                                        first = false;
                                    }
                                    i++;
                                }
                                rdr2.Close();

                                Thread.Sleep(2000);
                                kol += 2000;
                                if (kol == 1800000)
                                {
                                    kol = 0;
                                    potok2 = new Thread(new ThreadStart(function2));                               
                                    potok2.Start();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            button2.Invoke(new Action(() => { button2.Enabled = true; }));
                        }
                        catch { }                       
                    }                 
                }
            }
        }

        private void lb2_DoubleClick(object sender, EventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            string textLiClick = listBox.Items[listBox.SelectedIndex].ToString();
            if (textLiClick.IndexOf("\nФайл: ") != -1)
            {
                string text = textLiClick.Substring(0, textLiClick.IndexOf(". "));
                MySqlConnection conn4 = new MySqlConnection(connStr);
                conn4.Open();
                string sql4 = "SELECT sms_id, filePath FROM " + tabPage3.Text.Replace(' ', '_') + " WHERE sms_id = " + text;
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
            else
            {
                string[] textMas = textLiClick.Split(new char[] { '\n' });
                string textCopy = "";
                for (int i = 0; i<textMas.Length - 2; i++)
                {
                    if (i > 1)
                    {
                        textCopy += textMas[i] + "\n";
                    }                    
                }
                try
                {
                    Clipboard.SetText(textCopy);       
                    MessageBox.Show("Текст сообщения успешно скопирован!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    MessageBox.Show("Скопировать текст сообщения в буфер обмена не удалось!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
        }

        private void lb2_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (lb2.Items.Count > 0)
            {
                e.DrawBackground();
                SolidBrush myBrush = new SolidBrush(Color.Red);
                string textLi = lb2.Items[e.Index].ToString();
                if (textLi.IndexOf("\nФайл: ") != -1)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.Cornsilk), e.Bounds);
                }
                else
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.White), e.Bounds);
                }
                e.Graphics.DrawString(lb2.Items[e.Index].ToString(), new Font("Times New Roman", 14, FontStyle.Regular), Brushes.Black, e.Bounds);
                e.DrawFocusRectangle();
            }
        }

        private void lb2_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = (int)e.Graphics.MeasureString(lb2.Items[e.Index].ToString(), new Font("Times New Roman", 14, FontStyle.Regular), lb2.Width).Height;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string Name = tabControl1.SelectedTab.Text;
            if (Name.Equals("Текущие") == false && Name.Equals("Завершенные") == false)
            {
                tabControl1.TabPages[tabControl1.SelectedIndex].Controls.Clear();
                string[] text;
                int zn = 0;
                for (int i = 0; i < lb.Items.Count; i++)
                {
                    text = (lb.Items[i].ToString()).Split(new char[] { '\n' });
                    if (Name.Equals(text[0]) == true)
                    {
                        tPosl = 0;
                        button7.Enabled = false;
                        Active_Ex(text);
                        button7.Enabled = true;
                        zn = 1;
                        break;
                    }
                }
                if (zn == 0)
                {
                    for (int i = 0; i < lb_Z.Items.Count; i++)
                    {
                        text = (lb_Z.Items[i].ToString()).Split(new char[] { '\n' });
                        if (Name.Equals(text[0]) == true)
                        {
                            tPosl = 0;
                            button7.Enabled = false;
                            Conf_Ex(text);
                            button7.Enabled = true;
                            break;
                        }
                    }
                }
            }
            if (Name.Equals("Текущие") == true || Name.Equals("Завершенные") == true)
            {
                button7.Enabled = false;
                lb.Items.Clear();
                lb_Z.Items.Clear();
                Load_MainPage();
                button7.Enabled = true;
            }
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (mm == 0)
            {
                conn.Close();

                if (potok != null || potok.IsAlive == true)
                {
                    if (potok2 != null || potok2.IsAlive == true)
                    {
                        potok2.Abort();
                        potok2.Join(200);
                    }
                    potok.Abort();
                    potok.Join(200);
                    Application.Exit();
                }
                else
                {
                    System.Environment.Exit(1);
                }
            }
            if (mm == 1)
            {
                conn.Close();
                if (potok != null || potok.IsAlive == true)
                {
                    if (potok2 != null || potok2.IsAlive == true)
                    {
                        potok2.Abort();
                        potok2.Join(200);
                    }
                    potok.Abort();
                    potok.Join(200);
                  
                }
            }
        
    }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                if (form3 != null)
                {
                    form3.Close();
                }
                if (form4 != null)
                {
                    form4.Close();
                }
                chForMm = 1;
                 this.Close();
            }
            catch (AccessViolationException ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            button6.Enabled = false;
            lb.Items.Clear();
            lb_Z.Items.Clear();
            conn12.Close();
            if (potokBe)
            {
                potokBe = false;
                potok2.Abort();
                potok2.Join(50);
            }
            Load_MainPage();
            button6.Enabled = true;
        }

        private void Delete_Page(object sender, MouseEventArgs e)
        {
            TabPage page = (TabPage)sender;
            tabControl1.Controls.Remove(page);
            if (listSotr != null)
            {
                listSotr.Clear();
            }
            help = "";
        }

        private void lb_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (lb.Items.Count > 0)
            {
                e.DrawBackground();
                SolidBrush myBrush = new SolidBrush(Color.Red);
                if (e.Index%2 == 0)
                {
                   e.Graphics.FillRectangle(new SolidBrush(Color.LightCyan), e.Bounds);
                }
                else
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.White), e.Bounds);
                }            
                e.Graphics.DrawString(lb.Items[e.Index].ToString(), new Font("Times New Roman", 14, FontStyle.Regular), Brushes.Black, e.Bounds);
                e.DrawFocusRectangle();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (tabPage3.Text.Equals("") == false)
            {              
                form3 = new Form3(current, tabPage3.Text);
                form3.Show();
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8)
            {
                e.Handled = true;
            }
        }

        private void Form2_Resize(object sender, EventArgs e)
        {
            if (lb2 != null)
            {
                lb2.Width = tabPage3.Width;
                lb2.Height = tabPage3.Height;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if ((tabPage3.Text).Equals("") == false)
            {
                button3.Enabled = false;
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog1.RestoreDirectory = true;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Random rand = new Random();
                    string newFileName = "";
                    bool create = false;
                    while (create != true)
                    {
                        newFileName = "C:\\Файлы1с\\" + tabPage3.Text + "\\" + rand.Next(0, 200) + "_"+ Path.GetFileName(openFileDialog1.FileName);
                        if (File.Exists(newFileName) == false)
                        {
                            create = true;
                        }
                    }
                    MySqlConnection conn3 = new MySqlConnection(connStr);
                    conn3.Open();
                    string sql3 = "INSERT INTO " + tabPage3.Text.Replace(' ', '_') + " (time, name, fileName, filePath) VALUES ('" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "', '" + current + "', '" + Path.GetFileName(openFileDialog1.FileName) + "', '" + newFileName.Replace('\\', '╕') + "')";  // alt + 184
                    MySqlCommand cmd3 = new MySqlCommand(sql3, conn3);
                    cmd3.ExecuteNonQuery();
                    conn3.Close();

                    File.Copy(openFileDialog1.FileName, newFileName);
                }
                button3.Enabled = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)                     // загрузить последние
        {
            string pageName = tabPage3.Text;
            if (pageName.Equals("") == false && textBox2.Text.Equals("") == false)
            {
                button4.Enabled = false;
                string pageNameInv = pageName.Replace(' ', '_');
               
                var conn5 = new MySqlConnection(connStr);
                conn5.Open();
                string sql5 = "SELECT sms_id, time, name, text, fileName, filePath FROM " + pageNameInv + " WHERE sms_id < " + firstValueSms + " ORDER BY sms_id DESC LIMIT " + textBox2.Text;
                MySqlCommand cmd5 = new MySqlCommand(sql5, conn5);
                MySqlDataReader rdr5 = cmd5.ExecuteReader();
                string Text5 = "";
                while (rdr5.Read())
                {
                    if (Convert.ChangeType(rdr5[3], typeof(String)).Equals("") != true && Convert.ChangeType(rdr5[4], typeof(String)).Equals("") == true)
                    {
                        Text5 = (string)Convert.ChangeType(rdr5[0], typeof(String)) + ". " + (string)Convert.ChangeType(rdr5[2], typeof(String)) + "\n" + (string)Convert.ChangeType(rdr5[1], typeof(String)) + "\n" + (string)Convert.ChangeType(rdr5[3], typeof(String)) + "\n\n";
                    }
                    else if (Convert.ChangeType(rdr5[3], typeof(String)).Equals("") == true && Convert.ChangeType(rdr5[4], typeof(String)).Equals("") != true)
                    {
                        Text5 = (string)Convert.ChangeType(rdr5[0], typeof(String)) + ". " + (string)Convert.ChangeType(rdr5[2], typeof(String)) + "\n" + (string)Convert.ChangeType(rdr5[1], typeof(String)) + "\nФайл: " + (string)Convert.ChangeType(rdr5[4], typeof(String)) + "\n\n";
                    }
                    tabPage3.Invoke(new Action(() => { lb2.Items.Insert(0, Text5); lb2.SelectedIndex = 0; }));
                    firstValueSms = (string)Convert.ChangeType(rdr5[0], typeof(String)); ;
                }
                conn5.Close();
                button4.Enabled = true;
            }
        }



        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
            if (textBox1.TextLength > (textBox1.MaxLength - 70))
            {
                label1.Text = ":" + (textBox1.MaxLength - textBox1.TextLength);
            }
            else
            {
                label1.Text = "";
            }
        }

        private void button2_Click(object sender, EventArgs e)                // кнопка обновления сообщений
        {
            buttonClick2 = true;
            if (secondClick == true)
            {
                buttonClick2Vn = true;
            }
        }

        private void lb_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = (int)e.Graphics.MeasureString(lb.Items[e.Index].ToString(), new Font("Times New Roman", 14, FontStyle.Regular), lb.Width).Height;
        }
        

        private Label Create_Label(int X, int Y, int SizeSh, string Text)
        {
            Label label = new Label();
            label.AutoSize = true;
            label.Font = new Font("Times New Roman", SizeSh, FontStyle.Regular);
            label.Location = new Point(X, Y);
            label.Text = Text;
            return label;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ((tabPage3.Text).Equals("") == false)
            {
                if ((textBox1.Text).Equals("") == false)
                {
                    try
                    {
                        label1.Text = "";
                        var conn2 = new MySqlConnection(connStr);
                        conn2.Open();
                        string sql = "INSERT INTO " + tabPage3.Text.Replace(' ', '_') + " (time, name, text) VALUES ('" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "', '" + current + "', '" + textBox1.Text + "')";  // '19.05.2018 17:53:01'
                        MySqlCommand cmd = new MySqlCommand(sql, conn2);
                        cmd.ExecuteNonQuery();
                        conn2.Close();
                        textBox1.Clear();
                        button1.Enabled = false;
                    }
                    catch
                    {
                        MessageBox.Show("Обновите чат! Возможно, проект уже завершен", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private TextBox Create_TextBox(int X, int Y, int Width, string Text)
        {
            TextBox textBox = new TextBox();
            textBox.Location = new Point(X, Y);
            textBox.Font = new Font("Times New Roman", 14, FontStyle.Regular);
            textBox.Multiline = true;
            textBox.Width = Width;
            textBox.Height = 200;
            textBox.Text = Text;
            if (textBox.Text != null)
            {
                if (textBox.Lines.Length > 4)
                {
                    textBox.ScrollBars = ScrollBars.Vertical;
                }
            }
            return textBox;
        }

        private Button Create_Button (int X, int Y, string Text)
        {
            Button button = new Button();
            button.Location = new Point(X, Y);
            button.Size = new Size(110, 29);
            button.Font = new Font("Times New Roman", 14, FontStyle.Regular);
            button.Text = Text;
            return button;
        }
    }


}
