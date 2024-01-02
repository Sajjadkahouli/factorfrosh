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
using System.Threading;
using System.Windows.Threading;
using System.IO;
namespace CommerceProject
{
    /// <summary>
    /// Interaction logic for ListFactors.xaml
    /// </summary>
    public partial class ListFactors : Window
    {
        Dictionary<int, DataTable> factorsdetails;
        ObservableCollection<ProductOfFactor> factorkharid = new ObservableCollection<ProductOfFactor>();
        ObservableCollection<FactorsType> factors = new ObservableCollection<FactorsType>();
        ObservableCollection<FactorsType> factorscopy = new ObservableCollection<FactorsType>();
        DataTable data = new DataTable();
        internal ListFactors(ObservableCollection<FactorsType> factors , Dictionary<int , DataTable> factorsdetails , DataTable data)
        {
            InitializeComponent();
            dtgfactorsdetails.ItemsSource = factorkharid;
            this.factors = factors;
            copy();
            dtgfactors.ItemsSource = factorscopy;
            this.factorsdetails = factorsdetails;
            this.data = data;
        }

        public void copy()
        {
            foreach (var item in factors)
            {
                factorscopy.Add(item);
            }
        }
        private void dtgfactors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                factorkharid.Clear();
                DataGrid dt1 = sender as DataGrid;
                int columnIndex = dt1.CurrentColumn.DisplayIndex;
                TextBlock targetCell = (TextBlock)dt1.SelectedCells[0].Column.GetCellContent(dt1.SelectedItem);
                dt = factorsdetails[Convert.ToInt32(targetCell.Text)];
                foreach (DataRow item in dt.Rows)
                {

                    ProductOfFactor pof = new ProductOfFactor();
                    pof.کدکالا = Convert.ToInt32(item["ProductID"]);

                    pof.تخفیف = Convert.ToDecimal(item["Takhfif"]);
                    pof.قیمت = Convert.ToDecimal(item["Price"]);
                    try
                    {
                        pof.تعداد = Convert.ToInt32(item["ProductOut"]);

                    }
                    catch
                    {
                        pof.تعداد = Convert.ToInt32(item["ProductIn"]);

                    }
                    try
                    {

                        pof.قیمت_کل = Convert.ToDecimal(Convert.ToInt32(item["Price"]) * Convert.ToInt32(item["ProductOut"])) - Convert.ToDecimal(item["Takhfif"]);
                    }
                    catch
                    {
                        pof.قیمت_کل = Convert.ToDecimal(Convert.ToInt32(item["Price"]) * Convert.ToInt32(item["ProductIn"])) - Convert.ToDecimal(item["Takhfif"]);
                    }

                    foreach (DataRow item2 in data.Rows)
                    {
                        if (Convert.ToInt32(item2["ProductID"]) == pof.کدکالا)
                        {
                            pof.نام_کالا = item2["NameProduct"].ToString();

                            break;
                        }
                    }
                    factorkharid.Add(pof);

                }
            }
            catch
            {

            }
        }

        private void btnemal_Click(object sender, RoutedEventArgs e)
        {
            decimal Result = 0 , Off = 0;
            foreach (var item in factorscopy)
            {
                Result = Result + item.قیمت_کل;
                Off = Off + item.تخفیف_کل;
            }
            string result = Result.ToString() + " : قیمت کل " + "\n" + Off.ToString() + " : تخفیف کل"+
                "\n" + (Result - Off).ToString() + " : فروش-تخفیف";
            MessageBox.Show(result);
        }

        private void btnsum_Click(object sender, RoutedEventArgs e)
        {

            factorscopy.Clear();

            int start = 1, end = factors.Count;
            try
            {
                if(Convert.ToInt32(txtstartF.Text) > 0 && Convert.ToInt32(txtstartF.Text) < factors.Count + 2)
                    start = Convert.ToInt32(txtstartF.Text);
                else
                {
                    start = 1;
                }
                
            }
            catch
            {
                start = 1;
            }
            try
            {
                if (Convert.ToInt32(txtendF.Text) > 0 && Convert.ToInt32(txtendF.Text) < factors[factors.Count - 1].شماره_فاکتور + 1)
                    end = Convert.ToInt32(txtendF.Text);
                else
                    end = factors[factors.Count - 1].شماره_فاکتور;
            }
            catch
            {
                end = factors[factors.Count - 1].شماره_فاکتور;
            }
            if (end < start)
                end = factors[factors.Count - 1].شماره_فاکتور;
            foreach (FactorsType item in factors)
            {
                if(item.شماره_فاکتور >= start && item.شماره_فاکتور <= end)
                {
                    factorscopy.Add(item);
                }
            }
            dtgfactors.Visibility = Visibility.Visible;

        }

        private void btnnull_Click(object sender, RoutedEventArgs e)
        {
            comboclient.ItemsSource = null;
        }

        private void txtsearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            factorscopy.Clear();
            copy();
            foreach (FactorsType item in factors)
            {
                bool valid = false;
                if (item.تخفیف_کل.ToString().Contains(txtsearch.Text))
                {
                    valid = true;
                }
                if (item.شماره_فاکتور.ToString().Contains(txtsearch.Text))
                {
                    valid = true;

                }
                if (item.قیمت_کل.ToString().Contains(txtsearch.Text))
                {
                    valid = true;
                }
                if (item.نوع_فاکتور.ToString().Contains(txtsearch.Text))
                {
                    valid = true;

                }
                if (!valid)
                {
                    factorscopy.Remove(item);
                }
            }
        }

        
    }
}
