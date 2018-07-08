using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SelectDataTable;

namespace Demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        DataTable dt;

        private void Form1_Load(object sender, EventArgs e)
        {
            dt = new DataTable();

            DataColumn dataColumn = new DataColumn("no", typeof(string));

            dt.Columns.Add(dataColumn);

            dataColumn = new DataColumn("name", typeof(string));

            dt.Columns.Add(dataColumn);

            dataColumn = new DataColumn("salary", typeof(int));
            dt.Columns.Add(dataColumn);


            dataColumn = new DataColumn("create_date", typeof(DateTime));
            dt.Columns.Add(dataColumn);

            DataRow dr;

            for(int i = 0; i<10; i++)
            {
                dr = dt.NewRow();

                dr["no"] = i.ToString("00");
                dr["name"] = "小明" + i.ToString("00");
                dr["salary"] = 1000 * i;
                //dr.SetField<int>("salary", );
                dr["create_date"] = DateTime.Now.Date.AddYears(i * -1);
                //dr.SetField<DateTime>("create_date", DateTime.Now.Date.AddYears(i * -1));
                //dr.ItemArray = new object[] { i.ToString("00"), "小明" + i.ToString("00"), 1000 * 1, DateTime.Now.Date.AddYears(i * -1) };

                

                dt.Rows.Add(dr);
            }

            gridSource.DataSource = dt;

            txtSql.Text = @"  select no, name, salary from dt where salary>= 2000 and salary<=8000 


and (not(1=1) or (1+10 !=12 and not not not (1+20<21 and 1=1 and (4/2=1*2))) )


and salary!=800       and  create_date > '2012-07-08'

and '2016-07-08' > create_date";

        }

        
        private void btnSelect_Click(object sender, EventArgs e)
        {
            
            try
            {
                DataTable dt2 = S.Select(dt, txtSql.Text);

                gridSelect.DataSource = dt2;
            }
            catch(Exception ex)
            {
                WriteMsg(ex.ToString());
            }
            
            //DataTable dt2 = S.Select(dt, " select * from ......... ");
        }


        private void WriteMsg(string msg)
        {
            txtMsg.AppendText(msg + "\r\n\r\n");
        }
        
    }
}
