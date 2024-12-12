using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Farjana_1281481
{
    public partial class EditForm : Form
    {
        List<QualifyData> QualifyDataList = new List<QualifyData>();
        string currentFile = "";
        string oldFile = "";
        public EditForm()
        {
            InitializeComponent();
        }
        public MainForm TheForm { get; set; }
        public int IdToEdit { get; set; }
        private void EditForm_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            LoadDataToForm();
        }

        private void LoadDataToForm()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["data"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM employees WHERE employeeid=@i", con))
                {
                    cmd.Parameters.AddWithValue("@i", IdToEdit);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        textBox1.Text = dr.GetString(1);
                        textBox2.Text = dr.GetString(2);
                        numericUpDown1.Value = dr.GetDecimal(3);
                        dateTimePicker1.Value = dr.GetDateTime(4).Date;
                        pictureBox1.Image = Image.FromFile(@"..\..\Pictures\" +dr.GetString(5));
                        checkBox1.Checked = dr.GetBoolean(6);
                        
                        oldFile = dr.GetString(5);
                    }
                    dr.Close();
                    cmd.CommandText = @"SELECT * FROM qualification WHERE employeeid = @i";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@i", IdToEdit);
                    SqlDataReader dr2 = cmd.ExecuteReader();
                    while (dr2.Read())
                    {
                        QualifyDataList.Add(new QualifyData { degree = dr2.GetString(1), institute = dr2.GetString(2), passingyear = dr2.GetInt32(3), result = dr2.GetString(4) });
                    }
                    SetDataSources();
                    con.Close();
                }
            }
        }
        private void SetDataSources()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = QualifyDataList;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                currentFile = openFileDialog1.FileName;
                pictureBox1.Image = Image.FromFile(currentFile);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            QualifyDataList.Add(new QualifyData { degree = textBox3.Text, institute = textBox4.Text, passingyear = (int)numericUpDown2.Value, result = textBox5.Text });
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = QualifyDataList;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["data"].ConnectionString))
            {
                con.Open();
                using (SqlTransaction trx = con.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.Transaction = trx;
                        string f = oldFile;
                        if (currentFile != "")
                        {
                            string ext = Path.GetExtension(currentFile);
                            f = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                            string savePath = @"..\..\Pictures\" + f;
                            MemoryStream ms = new MemoryStream(File.ReadAllBytes(currentFile));
                            byte[] bytes = ms.ToArray();
                            FileStream fs = new FileStream(savePath, FileMode.Create);
                            fs.Write(bytes, 0, bytes.Length);
                            fs.Close();
                        }
                        cmd.CommandText = "UPDATE employees SET name = @n, address = @add, salary = @s, joiningdate = @jd, picture = @pic, isacurrentemployee = @ce WHERE employeeid=@id ";
                        cmd.Parameters.AddWithValue("@id", IdToEdit);
                        cmd.Parameters.AddWithValue("@n", textBox1.Text);
                        cmd.Parameters.AddWithValue("@add", textBox2.Text);
                        cmd.Parameters.AddWithValue("@s", numericUpDown1.Value);
                        cmd.Parameters.AddWithValue("@jd", dateTimePicker1.Value);
                        cmd.Parameters.AddWithValue("@pic", f);
                        cmd.Parameters.AddWithValue("@ce", checkBox1.Checked);
                        
                        try
                        {
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = "DELETE FROM qualification WHERE employeeid = @id";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@id", IdToEdit);
                            cmd.ExecuteNonQuery();
                            foreach (var q in QualifyDataList)
                            {
                                cmd.CommandText = "INSERT INTO qualification  (degree, institute, passingyear, result, employeeid) VALUES (@d, @it, @py, @r, @i)";
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@d", q.degree);
                                cmd.Parameters.AddWithValue("@it", q.institute);
                                cmd.Parameters.AddWithValue("@py", q.passingyear);
                                cmd.Parameters.AddWithValue("@r", q.result);
                                cmd.Parameters.AddWithValue("@i", IdToEdit);
                                cmd.ExecuteNonQuery();
                            }
                            trx.Commit();
                            TheForm.LoadDataBindingSources();
                            MessageBox.Show("Data Updated", "Success");
                            QualifyDataList.Clear();
                            dataGridView1.DataSource = null;
                            dataGridView1.DataSource = QualifyDataList;
                            textBox1.Clear();
                            textBox2.Clear();
                            numericUpDown1.Value = 0;
                            dateTimePicker1.Value = DateTime.Now;
                            pictureBox1.Image = Image.FromFile(@"..\..\Pictures\1.png");
                            checkBox1.Checked = false;
                            textBox3.Clear();
                            textBox4.Clear();
                            numericUpDown2.Value = 0;
                            textBox5.Clear();
                        }
                        catch(Exception ex)
                        {
                            trx.Rollback();
                            MessageBox.Show("ERR: " + ex.Message, "Error");
                        }
                    }
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {
                QualifyDataList.RemoveAt(e.RowIndex);
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = QualifyDataList;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["data"].ConnectionString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {
                    string sql = @"DELETE employees 
                                        WHERE employeeid=@id";
                   
                    using (SqlCommand cmd = new SqlCommand(sql, con, tran))
                    {
                        cmd.Parameters.AddWithValue("@id", IdToEdit);
                        
                        try
                        {
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            cmd.CommandText = "DELETE FROM qualification where employeeid=@id";
                            cmd.Parameters.AddWithValue("@id", IdToEdit);
                            cmd.ExecuteNonQuery();
                            tran.Commit();
                            MessageBox.Show("Data Deleted", "Success");
                            TheForm.LoadDataBindingSources();
                            this.Close();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            MessageBox.Show("ERR: " + ex.Message, "Error");
                        }
                    }
                }
            }
            
        }
    }
}
