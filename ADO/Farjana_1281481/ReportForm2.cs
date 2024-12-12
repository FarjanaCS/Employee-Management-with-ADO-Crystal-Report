using Farjana_1281481.Reports;
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
    public partial class ReportForm2 : Form
    {
        public ReportForm2()
        {
            InitializeComponent();
        }

        private void ReportForm2_Load(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["data"].ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM employees ", con))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds, "employees1");
                    da.SelectCommand.CommandText = @"SELECT  * FROM   qualification  ";
                    da.Fill(ds, "qualification");

                    ds.Tables["employees1"].Columns.Add(new DataColumn("image", typeof(byte[])));
                    for (var i = 0; i < ds.Tables["employees1"].Rows.Count; i++)
                    {
                        ds.Tables["employees1"].Rows[i]["image"] = File.ReadAllBytes($@"..\..\Pictures\{ds.Tables["employees1"].Rows[i]["picture"]}");
                    }
                    Report2 rpt = new Report2();
                    rpt.SetDataSource(ds);
                    crystalReportViewer1.ReportSource = rpt;
                    rpt.Refresh();
                    crystalReportViewer1.Refresh();
                }
            }
        }
    }
}
