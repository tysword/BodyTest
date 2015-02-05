using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {

        
    /// <summary>
    /// Current status text to display
    /// </summary>
        private string statusText = "running";

        public Window1()
        {
            InitializeComponent();
            this.StatusText = "running...";
        }
        /// <summary>
        /// Gets or sets the current status text to display
        /// </summary>
        /// <summary>
        /// Gets or sets the current status text to display
        /// </summary>
        public string StatusText
        {
            get
            {
                return this.statusText;
            }

            set
            {
                  this.statusText = value; 
            }
        }

        private void startExam(tab_person p)
        {
          
            tab_exam exam = new tab_exam();
            exam.exam_date = DateTime.Now;
            exam.operater = "系统操作员";
            exam.exam_age = getPersonAge((DateTime)p.birthday);
            exam.finish_flag = tab_exam.FinishFlagStart;

            using (var ctx = new jointexamEntities())
            {
                exam.person_id = p.id;
                ctx.tab_exam.Add(exam);
                ctx.SaveChanges();
            }

            MainWindow m = new MainWindow(exam);
            m.ShowDialog();
        }

        private int getPersonAge(DateTime birthDay)
        {
            int m_Y1 = birthDay.Year;
            int m_Y2 = DateTime.Now.Year;

            return m_Y2 - m_Y1;
        }



        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem m = (MenuItem)sender;
            string header = m.Header.ToString();
            switch(header){
                case "退出":
                    Application.Current.Shutdown();
                    break;
                case "新检查":
                    this.tcMain.SelectedIndex = 0;
                    break;
                case "检查浏览":
                    this.tcMain.SelectedIndex = 1;
                    
                    break;
            }
        }


        private void examSearch_Loaded(object sender, RoutedEventArgs e)
        {
            using (var ctx = new jointexamEntities())
            {
                var persons = from c in ctx.tab_person select c;
                this.dgPerson.ItemsSource = persons.ToList();
            }
        }

        private void dgPerson_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(((DataGrid)sender).SelectedItem!=null){
                tab_person p =  ((tab_person)this.dgPerson.SelectedItem);
                txtName.Text =p.name;
                this.txtID.Text = p.id.ToString();
                this.txtSex.Text = p.sex;
                this.txtRace.Text = p.race;
                this.txtWeight.Text = p.weight.ToString();
                this.txtHeight.Text = p.height.ToString();
                this.txtBirthday.Text = p.birthday.ToString();
                this.btnStartNew.IsEnabled = true;

                using (var ctx = new jointexamEntities())
                {

                    DbSet<tab_exam> set = ctx.Set<tab_exam>();

                    List<tab_exam> list = set.SqlQuery("select * from tab_exam where person_id='"+p.id+"'").ToList();
                    this.dgExam.ItemsSource = list;
                }
                
            }
        }

        private void btnClean_Click(object sender, RoutedEventArgs e)
        {
            this.txtStartDate.Text = "";
            this.txtEndDate.Text = "";
            this.txtSID.Text = "";
            this.txtSName.Text = "";
        }

     
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtName.Text = "";
                this.txtID.Text = "";
                this.txtSex.Text = "";
                this.txtRace.Text = "";
                this.txtWeight.Text = "";
                this.txtHeight.Text = "";
                this.txtBirthday.Text = "";
                this.btnStartNew.IsEnabled = false;

                using (var ctx = new jointexamEntities())
                {
                    DbSet<tab_exam> set = ctx.Set<tab_exam>();
                    List<tab_exam> list = set.SqlQuery("select * from tab_exam where person_id='" + this.txtID.Text + "'").ToList();
                    this.dgExam.ItemsSource = list;



                    StringBuilder b = new StringBuilder();
                    b.Append("select * from tab_person a where 1=1 ");


                    if (this.txtSID.Text.Length > 0)
                    {
                        b.Append(" and a.id like '%" + this.txtSID.Text + "%' ");
                    }

                    if (this.txtSName.Text.Length > 0)
                    {
                        b.Append(" and  a.`name` like '%" + this.txtSName.Text + "%' ");
                    }

                    if (this.txtStartDate.Text.Length > 0 && this.txtEndDate.Text.Length > 0)
                    {
                        b.Append(" and exists(select * from tab_exam r where DATE_FORMAT(r.exam_date,'%Y%m%d') > '" + this.txtStartDate.Text + "'  ");
                        b.Append(" and DATE_FORMAT(r.exam_date,'%Y%m%d')<'" + this.txtEndDate.Text + "' and r.person_id = a.id)");
                    }
                    else if (this.txtStartDate.Text.Length > 0)
                    {
                        b.Append(" and  exists(select * from tab_exam r where DATE_FORMAT(r.exam_date,'%Y%m%d') > '" + this.txtStartDate.Text + "' and r.person_id = a.id) ");
                    }
                    else if (this.txtEndDate.Text.Length > 0)
                    {
                        b.Append(" and  exists(select * from tab_exam r where DATE_FORMAT(r.exam_date,'%Y%m%d') < '" + this.txtEndDate.Text + "' and r.person_id = a.id) ");
                    }

                    DbSet<tab_person> pset = ctx.Set<tab_person>();
                    List<tab_person> pl = pset.SqlQuery(b.ToString()).ToList<tab_person>();
                    this.dgPerson.ItemsSource = pl;
                }

            }
            catch (Exception e1)
            {
                Console.WriteLine(e1);
            }
            finally
            {
            }
        }

        private void DatePicker_CalendarClosed(object sender, RoutedEventArgs e)
        {
            tAge.Text = getPersonAge((DateTime) dpBirthday.SelectedDate).ToString();
        }

        private void btnStartNew_Click(object sender, RoutedEventArgs e)
        {
            startExam(((tab_person)dgPerson.SelectedItem));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            tab_person p = new tab_person();
            p.name = tName.Text;
            p.race = tRace.Text;
            p.sex = tSex.Text;

            try
            {
                p.weight = float.Parse(tWeight.Text);
                p.height = float.Parse(tHeight.Text);
                p.birthday = dpBirthday.SelectedDate;
            }
            catch (Exception e1)
            {
                
            }


            using (var ctx = new jointexamEntities())
            {
                ctx.tab_person.Add(p);
                ctx.SaveChanges();
            }

            startExam(p);

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }


        private void dgExam_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
              if (dgExam.SelectedItem != null)
            {
                Window3 w = new Window3(((tab_exam)dgExam.SelectedItem).id);
                w.ShowDialog();
            }
        }
    }

    

   
}
