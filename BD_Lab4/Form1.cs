using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Data.Sqlite;

namespace BD_Lab4
{
    public partial class Form1 : Form
    {
        private DataGridView myGrid;
        private TextBox tSearch, tPos;
        private string dbFile = "inventar.db";
        private string connectionString;

        public Form1()
        {
            InitializeComponent();
            connectionString = $"Data Source={dbFile};";
            InitializeDatabase();
            BuildAdaptiveModernUI();
        }

        private void InitializeDatabase()
        {
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = @"CREATE TABLE IF NOT EXISTS person (
                                id_person INTEGER PRIMARY KEY,
                                name TEXT,
                                position TEXT,
                                code INTEGER
                              );";
                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void BuildAdaptiveModernUI()
        {
            this.Text = "Лабораторна робота №4";
            this.Width = 1120;
            this.Height = 670;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 250, 252);

            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.ColumnCount = 2;
            mainLayout.RowCount = 1;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
            mainLayout.Padding = new Padding(20);
            this.Controls.Add(mainLayout);

            myGrid = new DataGridView();
            myGrid.Dock = DockStyle.Fill;
            myGrid.BackgroundColor = Color.White;
            myGrid.BorderStyle = BorderStyle.None;
            myGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            myGrid.RowHeadersVisible = false;
            myGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            myGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
            myGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(226, 232, 240);
            myGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            myGrid.EnableHeadersVisualStyles = false;

            Panel gridPanel = new Panel();
            gridPanel.Dock = DockStyle.Fill;
            gridPanel.Padding = new Padding(0, 0, 15, 0);
            gridPanel.Controls.Add(myGrid);
            mainLayout.Controls.Add(gridPanel, 0, 0);

            FlowLayoutPanel controlPanel = new FlowLayoutPanel();
            controlPanel.Dock = DockStyle.Fill;
            controlPanel.FlowDirection = FlowDirection.TopDown;
            controlPanel.WrapContents = false;
            controlPanel.AutoScroll = true;
            mainLayout.Controls.Add(controlPanel, 1, 0);

            Label lblHeader = new Label();
            lblHeader.Text = "Панель керування XML та SQLite";
            lblHeader.Font = new Font("Segoe UI", 11.5f, FontStyle.Bold);
            lblHeader.ForeColor = Color.FromArgb(30, 41, 59);
            lblHeader.Size = new Size(420, 35);
            controlPanel.Controls.Add(lblHeader);

            Button btn1 = CreateFullWidthButton("1. Створити XML з таблиці (Client.xml)", Color.FromArgb(37, 99, 235));
            btn1.Click += but_xml_from_table_Click;
            controlPanel.Controls.Add(btn1);

            Button btn2 = CreateFullWidthButton("2. Створити XML на основі БД SQLite (Person.xml)", Color.FromArgb(37, 99, 235));
            btn2.Click += but_create_xml_Click;
            controlPanel.Controls.Add(btn2);

            Button btn3 = CreateFullWidthButton("3. Вибірка за кодом у Result.xml (code > 5)", Color.FromArgb(37, 99, 235));
            btn3.Click += but_select_other_xml_Click;
            controlPanel.Controls.Add(btn3);

            controlPanel.Controls.Add(CreateSpacer(10));

            Label lblSearch = new Label();
            lblSearch.Text = "4. Пошук особи у XML за ім'ям:";
            lblSearch.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblSearch.ForeColor = Color.FromArgb(71, 85, 105);
            lblSearch.AutoSize = true;
            controlPanel.Controls.Add(lblSearch);

            FlowOptionsPanel searchSubPanel = new FlowOptionsPanel();
            tSearch = new TextBox();
            tSearch.Size = new Size(275, 30);
            tSearch.Font = new Font("Segoe UI", 10);
            tSearch.Text = "Максим";
            searchSubPanel.Controls.Add(tSearch);

            Button btn4 = CreateActionSmallButton("Знайти", Color.FromArgb(16, 185, 129));
            btn4.Click += but_search_person_Click;
            searchSubPanel.Controls.Add(btn4);
            controlPanel.Controls.Add(searchSubPanel);

            controlPanel.Controls.Add(CreateSpacer(10));

            Label lblPos = new Label();
            lblPos.Text = "5. Фільтрація за посадою у Result2.xml:";
            lblPos.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblPos.ForeColor = Color.FromArgb(71, 85, 105);
            lblPos.AutoSize = true;
            controlPanel.Controls.Add(lblPos);

            FlowOptionsPanel posSubPanel = new FlowOptionsPanel();
            tPos = new TextBox();
            tPos.Size = new Size(275, 30);
            tPos.Font = new Font("Segoe UI", 10);
            tPos.Text = "Менеджер";
            posSubPanel.Controls.Add(tPos);

            Button btn5 = CreateActionSmallButton("Фільтр", Color.FromArgb(245, 158, 11));
            btn5.Click += but_select_position_Click;
            posSubPanel.Controls.Add(btn5);
            controlPanel.Controls.Add(posSubPanel);

            controlPanel.Controls.Add(CreateSpacer(12));

            Button btn6 = CreateFullWidthButton("6. Записати з XML у БД SQLite (INSERT)", Color.FromArgb(139, 92, 246));
            btn6.Click += xml_to_db_Click;
            controlPanel.Controls.Add(btn6);
        }

        private class FlowOptionsPanel : FlowLayoutPanel
        {
            public FlowOptionsPanel()
            {
                this.FlowDirection = FlowDirection.LeftToRight;
                this.Size = new Size(420, 42);
                this.Margin = new Padding(0, 3, 0, 3);
            }
        }

        private Button CreateFullWidthButton(string text, Color color)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Size = new Size(415, 42);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            btn.Margin = new Padding(0, 5, 0, 5);
            return btn;
        }

        private Button CreateActionSmallButton(string text, Color color)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Size = new Size(130, 32);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            btn.Margin = new Padding(8, 0, 0, 0);
            return btn;
        }

        private Panel CreateSpacer(int height)
        {
            Panel p = new Panel();
            p.Size = new Size(420, height);
            return p;
        }

        private void but_xml_from_table_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable("client");
            DataSet ds = new DataSet();
            dt.Columns.Add(new DataColumn("ID", Type.GetType("System.Int32")));
            dt.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("Age", Type.GetType("System.Int32")));
            dt.Columns.Add(new DataColumn("Phone", Type.GetType("System.String")));

            dt.Rows.Add(1, "Максим", 27, "+380982328654");
            dt.Rows.Add(2, "Софія", 45, "+380684290517");
            dt.Rows.Add(3, "Артем", 25, "+380964175896");
            dt.Rows.Add(4, "Вікторія", 26, "+380979797573");

            ds.Tables.Add(dt);
            ds.WriteXml("Client.xml");

            myGrid.DataSource = dt;
            MessageBox.Show("Файл Client.xml успішно створено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void but_create_xml_Click(object sender, EventArgs e)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    DataTable dt = new DataTable("person");
                    using (SqliteCommand cmd = new SqliteCommand("SELECT * FROM person", connection))
                    {
                        using (SqliteDataReader reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    ds.WriteXml("Person.xml");

                    myGrid.DataSource = dt;
                    MessageBox.Show("Файл Person.xml успішно створено на основі SQLite БД!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка БД:\n" + ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void but_select_other_xml_Click(object sender, EventArgs e)
        {
            if (File.Exists("Person.xml"))
            {
                DataSet ds = new DataSet();
                ds.ReadXml("Person.xml");

                DataView dv = new DataView(ds.Tables[0], "code > 5", "name", DataViewRowState.CurrentRows);
                DataTable filteredTable = dv.ToTable();
                filteredTable.TableName = "ResultTable";
                filteredTable.WriteXml("Result.xml");

                myGrid.DataSource = filteredTable;
                MessageBox.Show("Вибірка за умовою (code > 5) успішна! Файл Result.xml створено.", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Спершу створіть файл Person.xml (натисніть кнопку 2)!", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void but_search_person_Click(object sender, EventArgs e)
        {
            if (File.Exists("Person.xml"))
            {
                DataSet ds = new DataSet();
                ds.ReadXml("Person.xml");

                DataView dv = new DataView(ds.Tables[0]);
                dv.Sort = "name";

                string searchName = tSearch.Text.Trim();
                int index = dv.Find(searchName);

                if (index == -1)
                {
                    MessageBox.Show("Даний запис відсутній у XML файлі !!!", "Результат пошуку", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    string resName = dv[index]["name"].ToString();
                    string resPos = dv[index]["position"].ToString();
                    MessageBox.Show($"{resName} — посада: {resPos}", "Результат пошуку", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Файл Person.xml відсутній! Спершу виконайте Крок 2.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void but_select_position_Click(object sender, EventArgs e)
        {
            if (File.Exists("Result.xml"))
            {
                DataSet ds = new DataSet();
                ds.ReadXml("Result.xml");

                string posText = tPos.Text.Trim();
                string searchFilter = "position = '" + posText + "'";
                DataView dv = new DataView(ds.Tables[0], searchFilter, "name", DataViewRowState.CurrentRows);
                DataTable resTable = dv.ToTable();
                resTable.TableName = "Result2Table";
                resTable.WriteXml("Result2.xml");

                myGrid.DataSource = resTable;
                MessageBox.Show("Фільтрацію за посадою завершено! Файл Result2.xml створено.", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Файл Result.xml відсутній! Виконайте Крок 3.", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void xml_to_db_Click(object sender, EventArgs e)
        {
            if (!File.Exists("NewPerson.xml"))
            {
                File.WriteAllText("NewPerson.xml",
                    "<?xml version=\"1.0\" standalone=\"yes\"?>\n" +
                    "<NewDataSet>\n" +
                    "  <Table><id_person>7</id_person><name>Денис</name><position>Аналітик</position><code>47</code></Table>\n" +
                    "  <Table><id_person>8</id_person><name>Катерина</name><position>Дизайнер</position><code>25</code></Table>\n" +
                    "</NewDataSet>");
            }

            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    DataSet ds = new DataSet();
                    ds.ReadXml("NewPerson.xml");

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        var row = ds.Tables[0].Rows[i];
                        string sql = $"INSERT OR REPLACE INTO person (id_person, name, position, code) VALUES ({row["id_person"]}, '{row["name"]}', '{row["position"]}', {row["code"]});";
                        using (SqliteCommand command = new SqliteCommand(sql, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }

                    DataTable dt = new DataTable("person");
                    using (SqliteCommand cmd = new SqliteCommand("SELECT * FROM person", connection))
                    {
                        using (SqliteDataReader reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                    myGrid.DataSource = dt;

                    MessageBox.Show("Успішно! Дані з XML-файлу записано в базу даних SQLite через INSERT!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка запису в БД SQLite:\n" + ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}