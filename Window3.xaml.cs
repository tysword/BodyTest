using Microsoft.Kinect;
using Microsoft.Reporting.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    /// <summary>
    /// Window3.xaml 的交互逻辑
    /// </summary>
    public partial class Window3 : Window
    {
        private int examId;
        private String pname;
        public Window3(int examId)
        {
            this.examId = examId;
            InitializeComponent();
            _reportViewer.Load += ReportViewer_Load;
            _reportViewer.LocalReport.SubreportProcessing += LocalReport_SubreportProcessing;
        }

        private void LocalReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            int type_id = Convert.ToInt32(e.Parameters["type_id"].Values[0]);
            jointexamDataSet ds = new jointexamDataSet();
            ds.EnforceConstraints = false;
            ds.BeginInit();

            ReportDataSource rs1 = new ReportDataSource();
            rs1.Name = "DataSet1";
            rs1.Value = ds.AnalysisType;
            e.DataSources.Add(rs1);

            jointexamDataSetTableAdapters.AnalysisTypeTableAdapter apt1 = new jointexamDataSetTableAdapters.AnalysisTypeTableAdapter();
            apt1.ClearBeforeFill = true;
            apt1.Fill(ds.AnalysisType, type_id, examId);
        }

        private bool _isReportViewerLoaded;
        private void ReportViewer_Load(object sender, EventArgs e)
        {
            if (!_isReportViewerLoaded)
            {
                jointexamDataSet ds = new jointexamDataSet();
                ds.EnforceConstraints = false;
                ds.BeginInit();

                ReportDataSource rs = new ReportDataSource();
                rs.Name = "DataSet1";
                rs.Value = ds.ExamMaster;
                this._reportViewer.LocalReport.DataSources.Add(rs);
                jointexamDataSetTableAdapters.ExamMasterTableAdapter apt = new jointexamDataSetTableAdapters.ExamMasterTableAdapter();
                apt.ClearBeforeFill = true;
                apt.Fill(ds.ExamMaster, examId);

                this._reportViewer.LocalReport.EnableExternalImages = true;
                this._reportViewer.LocalReport.ReportPath = "Report3.rdlc";
                pname = ds.ExamMaster.First().name;
                ds.EndInit();


                DataTable dt = fillExamResult(apt.GetData(examId).First().id);
                this._reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet2", dt));
                this._reportViewer.ProcessingMode = ProcessingMode.Local;
                _reportViewer.RefreshReport();
                _isReportViewerLoaded = true;
            }
        }



        private DataTable fillExamResult(int cid)
        {
            MySqlConnection _connection = new MySqlConnection();
            _connection.ConnectionString = Properties.Settings.Default.jointexamConnectionString;
            _connection.Open();

            string rtn = "presult1";
            MySqlCommand cmd = new MySqlCommand(rtn, _connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("CCID", cid);


            DataTable dt = new DataTable();
            MySqlDataReader rdr = cmd.ExecuteReader();


            for (int i = 0; i < rdr.FieldCount; i++)
            {
                String fname = rdr.GetName(i);
                Console.WriteLine(fname);
                dt.Columns.Add(fname);
            }
            while (rdr.Read())
            {
                DataRow dr = dt.NewRow();
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    dr[i] = rdr[i];
                }
                dt.Rows.Add(dr);
            }

            rdr.Close();

            return dt;
        }

        private void WindowsFormsHost_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            saveRptAs("PDF");
        }

        private void saveRptAs(String s_rptType)
        {

            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;

            byte[] bytes = this._reportViewer.LocalReport.Render(
            s_rptType, null, out mimeType, out encoding, out extension,
            out streamids, out warnings);


            FileStream stream = File.OpenWrite(@"d:\report\"+pname+".pdf");
            stream.Write(bytes, 0, bytes.Length);
            stream.Close();

            MessageBox.Show("导出完成！");
        }
    }
}




