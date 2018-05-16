using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        static string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\Database1.mdf;Integrated Security=True";
        SqlCommand sCommand;
        SqlDataAdapter sAdapter;
        SqlCommandBuilder sBuilder;
        DataSet sDs;
        DataTable sTable;

        private void MonkeyRepeat(string sql)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            sCommand = new SqlCommand(sql, connection); // se utilizeaza comanda sql si connection

            sAdapter = new SqlDataAdapter(sCommand);
            sBuilder = new SqlCommandBuilder(sAdapter); // adapter
            connection.Open();
            sDs = new DataSet(); //crearea DataSet
            sAdapter.Fill(sDs, "masini"); //Folosirea adapterului asupra tabelului elev
            sTable = sDs.Tables["masini"]; // folosirea table asupra tabelului elev
            connection.Close();

        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'database1DataSet.masini' table. You can move, or remove it, as needed.
            MonkeyRepeat("SELECT * FROM masini");
            dataGridView1.DataSource = sDs.Tables["masini"];
            this.masiniTableAdapter.Fill(this.database1DataSet.masini);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!((textBox1.Text == null)
                  && (textBox2.Text == null)
                  && (textBox3.Text == null)
                  && (textBox4.Text == null)
                  && (textBox5.Text == null)
                  && (textBox6.Text == null)
                  && (textBox7.Text == null)
                  ))
            {

                string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\Database1.mdf;Integrated Security=True";
                string sql = "SELECT * FROM masini";
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                foreach (DataGridViewColumn col in this.dataGridView1.Columns)
                {
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = String.Format("INSERT dbo.masini (Id, firmaproducatoare,model,tara,culoare,nrdeexemplare,pretul,anul)VALUES({0}, N'{1}', N'{2}', {3}, N'{4}', {5}, {6}, {7});",                                                                                         
                    Int32.Parse(textBox1.Text), textBox2.Text, textBox3.Text, Int32.Parse(textBox4.Text), textBox5.Text, Int32.Parse(textBox6.Text), Int32.Parse(textBox7.Text), Int32.Parse(textBox8.Text));
                cmd.Connection = connection;
                cmd.ExecuteNonQuery();
                this.sCommand = new SqlCommand(sql, connection);
                this.sAdapter = new SqlDataAdapter(sCommand);
                this.sBuilder = new SqlCommandBuilder(sAdapter);
                this.sDs = new DataSet();
                this.sAdapter.Fill(sDs, "masini");
                this.sTable = sDs.Tables["masini"];
                this.sAdapter.Update(sTable);
                connection.Close();
                dataGridView1.DataSource = sDs.Tables["masini"];
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullColumnSelect;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!(textBox9.Text == null))
            {
                dataGridView1.Rows.RemoveAt(Int32.Parse(textBox9.Text));
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string sql = "Select * FROM masini ORDER BY firmaproducatoare"; //Comanda sql de select
            MonkeyRepeat(sql);
            dataGridView1.DataSource = sDs.Tables["masini"];

            this.masiniTableAdapter.Fill(this.database1DataSet.masini);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!(textBox10.Text == null))
            {
                int avr = 0;
                int cnt = 0;
                foreach (DataRow row in this.sTable.Rows)
                {
                    if (Int32.Parse(row["tara"].ToString()) == Int32.Parse(textBox10.Text))
                    {
                        avr += Int32.Parse(row["pretul"].ToString());
                        cnt++;
                    }
                }
                double res= avr / cnt;
                textBox10.Text = res.ToString();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string sql = "Select * FROM masini WHERE pretul=(SELECT MAX(pretul) FROM masini WHERE culoare ='verde');" +
                "Select * FROM masini WHERE pretul=(SELECT MIN(pretul) FROM masini  WHERE culoare ='verde');"; //Comanda sql de select //Comanda sql de select
            MonkeyRepeat(sql);
            dataGridView1.DataSource = sDs.Tables["masini"];

            this.masiniTableAdapter.Fill(this.database1DataSet.masini);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string sql = "Select firmaproducatoare FROM masini WHERE nrdeexemplare=(SELECT MAX(nrdeexemplare) FROM masini)"; //Comanda sql de select //Comanda sql de select
            MonkeyRepeat(sql);
            dataGridView1.DataSource = sDs.Tables["masini"];

            this.masiniTableAdapter.Fill(this.database1DataSet.masini);
        }

 

        private void button8_Click(object sender, EventArgs e)
        {
            string sql = "Select * From masini WHERE firmaproducatoare='Audi' OR firmaproducatoare='BMW'";
            MonkeyRepeat(sql);
            dataGridView1.DataSource = sDs.Tables["masini"];

            this.masiniTableAdapter.Fill(this.database1DataSet.masini);
        }

        private void button9_Click(object sender, EventArgs e)
        {

            string sql = "Select * From masini WHERE pretul<50000";
            Console.WriteLine(sql);
            MonkeyRepeat(sql);

            var lines = new List<string>();
            string[] columnNames = sTable.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName).
                                              ToArray();
            var header = string.Join(",", columnNames);
            lines.Add(header);
            var valueLines = sTable.AsEnumerable()
                               .Select(row => string.Join(",", row.ItemArray));
            lines.AddRange(valueLines);
            File.WriteAllLines("excel.csv", lines);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            string sql = "Select anul, tara From masini ";
            MonkeyRepeat(sql);
            dataGridView1.DataSource = sDs.Tables["masini"];

            this.masiniTableAdapter.Fill(this.database1DataSet.masini);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            string sql = "Select * From masini WHERE anul=2018";
            MonkeyRepeat(sql);
            dataGridView1.DataSource = sDs.Tables["masini"];

            this.masiniTableAdapter.Fill(this.database1DataSet.masini);
        }
    }
}
