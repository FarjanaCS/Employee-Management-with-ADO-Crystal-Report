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
    public partial class MainForm : Form
    {
        BindingSource bsE = new BindingSource();
        BindingSource bsQ = new BindingSource();
        DataSet ds;
        public MainForm()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AddForm { TheForm = this }.ShowDialog();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            dataGridView2.AutoGenerateColumns=false;
            LoadDataBindingSources();
        }

        public void LoadDataBindingSources()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["data"].ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM employees ", con))
                {
                    ds = new DataSet();
                    da.Fill(ds, "employees");
                    da.SelectCommand.CommandText = @"SELECT  * FROM   qualification  ";
                    da.Fill(ds, "qualification");
                    
                    ds.Tables["employees"].Columns.Add(new DataColumn("image", typeof(byte[])));
                    for (var i = 0; i < ds.Tables["employees"].Rows.Count; i++)
                    {
                        ds.Tables["employees"].Rows[i]["image"] = File.ReadAllBytes($@"..\..\Pictures\{ds.Tables["employees"].Rows[i]["picture"]}");
                    }
                    DataRelation rel = new DataRelation("FK_E_Q", ds.Tables["employees"].Columns["employeeid"], ds.Tables["qualification"].Columns["employeeid"]);
                    ds.Relations.Add(rel);
                    bsE.DataSource = ds;
                    bsE.DataMember = "employees";

                    bsQ.DataSource = bsE;
                    bsQ.DataMember = "FK_E_Q";
                    
                    dataGridView2.DataSource = bsQ;
                    AddDataBindings();
                }
            }
        }

        private void AddDataBindings()
        {
            labelid.DataBindings.Clear();
            labelid.DataBindings.Add(new Binding("Text", bsE, "employeeid"));
            labelname.DataBindings.Clear();
            labelname.DataBindings.Add(new Binding("Text", bsE, "name"));
            labeladd.DataBindings.Clear();
            labeladd.DataBindings.Add(new Binding("Text", bsE, "address"));
            labelprc.DataBindings.Clear();
            Binding bp = new Binding("Text", bsE, "salary", true);
            bp.Format += Bp_Format;
            labelprc.DataBindings.Add(bp);
            pictureBox1.DataBindings.Clear();
            pictureBox1.DataBindings.Add(new Binding("Image", bsE, "image", true));
            Binding bm = new Binding("Text", bsE, "joiningdate", true);
            bm.Format += Bm_Format;
            labeldate.DataBindings.Clear();
            labeldate.DataBindings.Add(bm);
            checkBox1.DataBindings.Clear();
            checkBox1.DataBindings.Add("Checked", bsE, "isacurrentemployee", true);
        }

        private void Bm_Format(object sender, ConvertEventArgs e)
        {
            DateTime d = (DateTime)e.Value;
            e.Value = d.ToString("yyyy-MM-dd");
        }

        private void Bp_Format(object sender, ConvertEventArgs e)
        {
            decimal d = (decimal)e.Value;
            e.Value = d.ToString("0.00");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (bsE.Position < bsE.Count - 1)
            {
                bsE.MoveNext();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (bsE.Position > 0)
            {
                bsE.MovePrevious();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bsE.MoveFirst();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bsE.MoveLast();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int v = int.Parse((bsE.Current as DataRowView).Row[0].ToString());
            new EditForm { TheForm = this, IdToEdit = v }.ShowDialog();
        }

        private void editDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new EditForm { TheForm = this }.ShowDialog();
        }

        private void report1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ReportForm1().Show();
        }

        private void subReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ReportForm2().Show();
        }
    }
}
