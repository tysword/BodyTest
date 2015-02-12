using Microsoft.Kinect;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
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
        public Window3(int examId)
        {
            this.examId = examId;
            InitializeComponent();
            _reportViewer.Load += ReportViewer_Load;
            _reportViewer.LocalReport.SubreportProcessing +=LocalReport_SubreportProcessing;
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
            apt1.Fill(ds.AnalysisType,type_id,examId); 
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

                
                ReportDataSource rs1 = new ReportDataSource();
                rs1.Name = "DataSet2";
                rs1.Value = ds.tab_exam_type;
                this._reportViewer.LocalReport.DataSources.Add(rs1);
                

                this._reportViewer.LocalReport.EnableExternalImages = true;
                this._reportViewer.LocalReport.ReportPath = "Report3.rdlc";

                ds.EndInit();

                jointexamDataSetTableAdapters.ExamMasterTableAdapter apt = new jointexamDataSetTableAdapters.ExamMasterTableAdapter();
                apt.ClearBeforeFill = true;
                apt.Fill(ds.ExamMaster,examId);

                jointexamDataSetTableAdapters.tab_exam_typeTableAdapter apt1 = new jointexamDataSetTableAdapters.tab_exam_typeTableAdapter();
                apt1.ClearBeforeFill = true;
                apt1.Fill(ds.tab_exam_type);

                _reportViewer.RefreshReport();
                _isReportViewerLoaded = true;

            }
        }

        private void WindowsFormsHost_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {

        }
    }
}
