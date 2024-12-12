using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace Farjana_1281481
{
    public partial class AddForm : Form
    {
        List<QualifyData> QualifyDataList = new List<QualifyData>();
        string currentFile = "";
        public AddForm()
        {
            InitializeComponent();
        }
        public MainForm TheForm { get; set; }


        private void AddForm_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            QualifyDataList.Add(new QualifyData { degree= textBox3.Text, institute= textBox4.Text,passingyear= (int)numericUpDown2.Value, result= textBox5.Text });
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = QualifyDataList;
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                currentFile = openFileDialog1.FileName;
                pictureBox1.Image = Image.FromFile(currentFile);
            }
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
                        string ext = Path.GetExtension(currentFile);
                        string f = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                        string savePath = @"..\..\Pictures\" + f;
                        MemoryStream ms = new MemoryStream(File.ReadAllBytes(currentFile));
                        byte[] bytes = ms.ToArray();
                        FileStream fs = new FileStream(savePath, FileMode.Create);
                        fs.Write(bytes, 0, bytes.Length);
                        fs.Close();
                        cmd.CommandText = "INSERT INTO employees(name, address, salary, joiningdate, picture, isacurrentemployee) VALUES (@n, @add, @s, @jd, @pic, @ce);SELECT SCOPE_IDENTITY();";
                        cmd.Parameters.AddWithValue("@n", textBox1.Text);
                        cmd.Parameters.AddWithValue("@add", textBox2.Text);
                        cmd.Parameters.AddWithValue("@s", numericUpDown1.Value);
                        cmd.Parameters.AddWithValue("@jd", dateTimePicker1.Value);
                        cmd.Parameters.AddWithValue("@pic", f);
                        cmd.Parameters.AddWithValue("@ce", checkBox1.Checked);
                        try
                        {
                            var eid = cmd.ExecuteScalar();
                            foreach (var q in QualifyDataList)
                            {
                                cmd.CommandText = "INSERT INTO qualification (degree, institute, passingyear, result, employeeid) VALUES (@d, @it, @py, @r, @i)";
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@d", q.degree);
                                cmd.Parameters.AddWithValue("@it", q.institute);
                                cmd.Parameters.AddWithValue("@py", q.passingyear);
                                cmd.Parameters.AddWithValue("@r", q.result);
                                cmd.Parameters.AddWithValue("@i", eid);
                                cmd.ExecuteNonQuery();
                            }
                            trx.Commit();

                            TheForm.LoadDataBindingSources();
                            MessageBox.Show("Data Saved", "Success");
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
    }
    public class QualifyData
    {
        public string degree { get; set; }
        public string institute { get; set; }
        public int passingyear { get; set; }
        public string result { get; set; }
    }
}
