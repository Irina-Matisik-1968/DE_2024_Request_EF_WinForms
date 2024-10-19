using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ОООТехносервис.View
{
    public partial class Reports : Form
    {
        public Reports()
        {
            InitializeComponent();
        }

        private void Reports_Load(object sender, EventArgs e)
        {
            this.labelCount.Text = "Количество выполненных заявок = "+Classes.Helper.DB.Request.Count(r=>r.RequestStatusId==3).ToString();
            this.labelAvg.Text = "Среднее время выполненных заявок (часов) = " + (Classes.Helper.DB.Request.Average(r=>r.RequestTime)/60.0).ToString("F2");
            //this.labelCount.Text = "Количество выполненных заявок = " + Convert.ToInt32(Classes.Helper.DB.CountRequestsReady());
            //this.labelAvg.Text = "Среднее время выполненных заявок = " + Convert.ToInt32(Classes.Helper.DB.AvgRequestsReady());
            this.dataGridViewStatic.DataSource=Classes.Helper.DB.ViewGroupProblem.ToList();
            this.dataGridViewStatic.Columns[0].HeaderText = "Тип неисправности";
            this.dataGridViewStatic.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewStatic.Columns[1].HeaderText = "Количество неисправности";
            this.dataGridViewStatic.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

        }

        private void bntExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
