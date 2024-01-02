using System;
using CommerceProject.ViewMoudel;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.Collections.ObjectModel;
using CommerceProject.Moudel;
using DataAccess.Models;

namespace CommerceProject
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        ProductOfFactor pf = new ProductOfFactor();
        DataTable dt2 = new DataTable();
        DataTable dt = new DataTable();
        int FactorId;
        public Window1(DataTable data , int factorid)
        {
            InitializeComponent();
            FactorId = factorid;
            dt = data;
            dt2 = data;
            dtgmenu.ItemsSource = data.DefaultView;
        }

        private void dtgmenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(dtgmenu.SelectedIndex >= 0)
            {
                int index = dtgmenu.SelectedIndex;
                txtname.Text = dt.Rows[index]["NameProduct"].ToString();
                txtprice.Text = dt.Rows[index]["Price"].ToString();
                if (dt.Columns.Count > 7)
                    txtid.Text = dt.Rows[index]["ProductID"].ToString();
                else
                {
                    idofproduct.Content = "تخفیف";
                    txtid.Text = dt.Rows[index]["Takhfif"].ToString();
                }
                if (dt.Columns.Count > 7)
                    txtmojoodi.Text = dt.Rows[index]["Mojoodi"].ToString();
                else if((int)dt.Rows[index]["FactorItemID"] == 1)
                    txtmojoodi.Text = dt.Rows[index]["ProductOut"].ToString();
                else if ((int)dt.Rows[index]["FactorItemID"] == 0)
                    txtmojoodi.Text = dt.Rows[index]["ProductIn"].ToString();

            }
        }

        private void btnsubmit_Click(object sender, RoutedEventArgs e)
        {
            int index = dtgmenu.SelectedIndex;
            if (dt.Columns.Count > 7)
            {
                
                dt.Rows[index]["NameProduct"] = txtname.Text;
                dt.Rows[index]["Price"] = Convert.ToDecimal(txtprice.Text);
                dt.Rows[index]["ProductID"] = Convert.ToInt32(txtid.Text);
                dt.Rows[index]["Mojoodi"] = Convert.ToInt32(txtmojoodi.Text);
            }
            else
            {
                dt.Rows[index]["NameProduct"] = txtname.Text;
                dt.Rows[index]["Price"] = Convert.ToDecimal(txtprice.Text);
                dt.Rows[index]["Takhfif"] = Convert.ToDecimal(txtid.Text);
                if((int)dt.Rows[index]["FactorItemID"] == 1)
                    dt.Rows[index]["ProductOut"] = Convert.ToInt32(txtmojoodi.Text);
                else
                    dt.Rows[index]["ProductIn"] = Convert.ToInt32(txtmojoodi.Text);
                dt.Rows[index]["PriceTootal"] = (Convert.ToInt32(txtmojoodi.Text) * Convert.ToDecimal(txtprice.Text)) - Convert.ToDecimal(txtid.Text);

            }
        }

        private void btnclose_Click(object sender, RoutedEventArgs e)
        {
            if (dt.Columns.Count > 7)

                PushData(dt);
            else
                PushData2(dt);
            this.Close();
            
        }

        public void PushData(DataTable data)
        {

            string connString = @"Data Source = 2.180.43.85; Initial Catalog = TestDB; User ID = TestUser; Password = S13791381";

            SqlConnection conn = new SqlConnection(connString);
            SqlDataAdapter adapter = new SqlDataAdapter();
            conn.Open();

            foreach (DataRow item in data.Rows)
            {
                string query1 = "UPDATE CommerceProducts SET Price = " + item["Price"] + ", Mojoodi = " + item["Mojoodi"] + " WHERE ProductID = " + item["ProductID"] + "; ";
                adapter.InsertCommand = new SqlCommand(query1, conn);
                adapter.InsertCommand.ExecuteNonQuery();
            }



            conn.Close();

        }

        public void PushData2(DataTable dt)
        {
            string connString = @"Data Source = 2.180.43.85; Initial Catalog = TestDB; User ID = TestUser; Password = S13791381";
            SqlConnection conn = new SqlConnection(connString);
            conn.Open();
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlCommandBuilder cb = new SqlCommandBuilder(adapter);
            decimal Result = 0 , takhfif = 0;
            foreach (DataRow item in dt.Rows)
            {
                string query;
                if ((int)dt.Rows[1]["FactorItemID"] == 1)
                {
                    query = "UPDATE CommerceFactorItem SET ProductOut = " +
                    item["ProductOut"] + " , Takhfif = " + item["Takhfif"] + " WHERE ProductID = " + item["ProductID"] + ";";
                }
                else
                {
                    query = "UPDATE CommerceFactorItem SET ProductOut = " +
                    item["ProductIn"] + " , Takhfif = " + item["Takhfif"] +
                    " WHERE ProductID = " + item["ProductID"] + ";";
                }
                Result = Convert.ToDecimal(item["PriceTootal"]) + Result;

                adapter.InsertCommand = new SqlCommand(query, conn);
                adapter.InsertCommand.ExecuteNonQuery();
            }
            string s = "select Takhfif from CommerceFactor where FactorID = " + FactorId + ";";
            SqlCommand sq = new SqlCommand(s, conn);
            SqlDataReader sr;
            sr = sq.ExecuteReader();
            if(sr.Read())
                takhfif = Convert.ToDecimal(sr.GetValue(0));
            sr.Close();
            sq.Dispose();
          
            string query1 = "UPDATE CommerceFactor SET PriceTotal = "+(Result - takhfif)+ "where FactorID = " + FactorId + "; ";
            adapter.InsertCommand = new SqlCommand(query1, conn);
            adapter.InsertCommand.ExecuteNonQuery();
            
            conn.Close();

        }
    }
}
