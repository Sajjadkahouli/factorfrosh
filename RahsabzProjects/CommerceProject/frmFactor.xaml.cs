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
using CommerceProject.Model;

namespace CommerceProject
{

    /// <summary>
    /// Interaction logic for frmFactor.xaml
    /// </summary>
    public partial class frmFactor : Window
    {

        ObservableCollection<ProductOfFactor> factorkharid = new ObservableCollection<ProductOfFactor>();
        Product p = new Product();
        ProductOfFactor pf = new ProductOfFactor();
        Dictionary<int, DataTable> dictfactors = new Dictionary<int, DataTable>();
        Dictionary<int, DataTable> dictcosts = new Dictionary<int, DataTable>();
        Dictionary<int, FactorsType> dictfactorsP = new Dictionary<int, FactorsType>();
        ObservableCollection<FactorsType> Factors = new ObservableCollection<FactorsType>();
        ObservableCollection<Cost> Costs = new ObservableCollection<Cost>();
        ObservableCollection<detailscost> Costsdetails = new ObservableCollection<detailscost>();
        ObservableCollection<mandehesab> mandehesab = new ObservableCollection<mandehesab>();
        int IDCURRENTPRODUCT = 0;
        int num = 0;
        static int Id = 0;
        int sell = 2;
        string text;
        DataTable custTable = new DataTable("factor");
        public frmFactor()
        {
            WelcomePage w = new WelcomePage();
            w.ShowDialog();
            InitializeComponent();
            HotkeysManager.SetupSystemHook();
            txtanbarnumber.Focus();
            FillPastFactors();
            // You can create a globalhotkey object and pass it like so
            HotkeysManager.AddHotkey(new GlobalHotkey(ModifierKeys.Control, Key.D5, () => { showfactorforosh(); }));
            HotkeysManager.AddHotkey(new GlobalHotkey(ModifierKeys.Control, Key.D6, () => { showfactorkhariod(); }));
            HotkeysManager.AddHotkey(new GlobalHotkey(ModifierKeys.Control, Key.D7, () => { editanbar(); }));
            
            // or do it like this. both end up doing the same thing, but this is probably simpler.
            /* HotkeysManager.AddHotkey(ModifierKeys.Control, Key.A, () => { AddToList("Ctrl+A Fired"); });
             HotkeysManager.AddHotkey(ModifierKeys.Control, Key.D, () => { AddToList("Ctrl+D Fired"); });
             HotkeysManager.AddHotkey(ModifierKeys.Shift, Key.D, () => { AddToList("Shift+D Fired"); });
            */
            Closing += MainWindow_Closing;
            dtgmandehesab.ItemsSource = mandehesab;
            dgproduct.ItemsSource = factorkharid;
            DtgFactors.ItemsSource = Factors;
            GlobalDataGrid.ItemsSource = Costsdetails;
            GetId();
            dictfactors = factors();
            combosearch.SelectionChanged += new SelectionChangedEventHandler(OnMyComboBoxChanged);
            DispatcherTimer LiveTime = new DispatcherTimer();
            LiveTime.Interval = TimeSpan.FromSeconds(1);
            LiveTime.Tick += timer_Tick;
            LiveTime.Start();
            // this.Closed += new EventHandler(MainWindow_Closed);
            LoadCosts();
            filldictcost();
        }
        public static List<Cost> ListCosts; 
        private void LoadCosts()
        {
            /*
            Cost c = new Cost();
            c.CostId = 1;
            c.NameCost = "هزینه حمل ";
            Cost c2 = new Cost();
            c2.CostId = 2;
            c2.NameCost = "هزینه بارگیری ";
            Cost c3 = new Cost();
            c3.CostId = 3;
            c3.NameCost = "هزینه ترخیصیه ";
            */
            string connString = @"Data Source = 2.180.43.85; Initial Catalog = TestDB; User ID = TestUser; Password = S13791381";
            string query = "select * from dbo.Costs;";
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            // this will query your database and return the result to your datatable
            DataTable dataTable = new DataTable();
            da.Fill(dataTable);
            foreach (DataRow item in dataTable.Rows)
            {
                Cost c = new Cost();
                c.CostId = (int)item["CostID"];
                c.NameCost = item["CostName"].ToString();
                Costs.Add(c);
            }
        }
        DataTable dt = PullData();

        void filldictcost()
        {
            dictcosts.Clear();
            foreach (DataRow item in dt.Rows)
            {
               
            string connString = @"Data Source = 2.180.43.85; Initial Catalog = TestDB; User ID = TestUser; Password = S13791381";
            string query = "select * from dbo.ProductCosts where ProductID = "+ item["ProductID"].ToString() + "; ";
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            // this will query your database and return the result to your datatable
            DataTable dataTable = new DataTable();
            da.Fill(dataTable);
            dictcosts.Add(Convert.ToInt32(item["ProductID"]) , dataTable);
            }
        }

        /*
       private void MainWindow_Closed(object sender, EventArgs e)
       {
           string connString = @"Data Source = 2.180.43.85; Initial Catalog = TestDB; User ID = TestUser; Password = S13791381";
           SqlConnection conn = new SqlConnection(connString);
           SqlDataAdapter adapter = new SqlDataAdapter();
           conn.Open();
           for (int i = 0; i < dictfactors.Count; i ++)
           {
           DataTable datum = dictfactors[i];
               string query2 = "SELECT (ProductID) FROM CommerceFactorItem WHERE FactorID = " + (int)datum.Rows[0]["FactorID"] + "; ";
               SqlCommand cmd2 = new SqlCommand(query2, conn);


               // create data adapter
               SqlDataAdapter da = new SqlDataAdapter(cmd2);
               // this will query your database and return the result to your datatable
               DataTable dataTable = new DataTable();
               da.Fill(dataTable);
               foreach (DataRow item in dataTable.Rows)
               {
                   bool del = false;
                   foreach (DataRow item2 in datum.Rows)
                   {
                      if( (int)item2["ProductID"] == (int)item["ProductID"])
                       {
                           del = true;
                           break;
                       }
                   }
                   if (!del)
                   {
                       string query = "DELETE FROM CommerceFactorItem WHERE ProductID = " + (int)item["ProductID"] + " and FactorID = " + (int)datum.Rows[0]["FactorID"] + "; ";
                       SqlCommand cmd = new SqlCommand(query, conn);
                       adapter.InsertCommand = new SqlCommand(query, conn);
                       adapter.InsertCommand.ExecuteNonQuery();

                   }
               }
               foreach (DataRow item in datum.Rows)
               {
                   if ((int)item["FactorItemID"] == 0)
                   {
                       string query = "UPDATE CommerceFactorItem SET Price = " + Convert.ToDecimal(item["Price"]) + ", Takhfif = " + Convert.ToDecimal(item["Takhfif"]) + " , ProductIn = " + (int)item["ProductIn"] + " WHERE ProductID = " + (int)item["ProductID"] + " and FactorID = " + (int)item["FactorID"] + "; ";
                       SqlCommand cmd = new SqlCommand(query, conn);
                       adapter.InsertCommand = new SqlCommand(query, conn);
                       adapter.InsertCommand.ExecuteNonQuery();
                   }else if((int)item["FactorItemID"] == 1)
                   {
                       string query = "UPDATE CommerceFactorItem SET Price = " + Convert.ToDecimal(item["Price"]) + ", Takhfif = " + Convert.ToDecimal(item["Takhfif"]) + " , ProductOut = " + (int)item["ProductOut"] + " WHERE ProductID = " + item["ProductID"].ToString() + " and FactorID = " + item["FactorID"].ToString() + "; ";
                       SqlCommand cmd = new SqlCommand(query, conn);
                       adapter.InsertCommand = new SqlCommand(query, conn);
                       adapter.InsertCommand.ExecuteNonQuery();
                   }

               }

           }





           conn.Close();
       }
*/
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Need to shutdown the hook. idk what happens if
            // you dont, but it might cause a memory leak.
            HotkeysManager.ShutdownSystemHook();
        }
        void timer_Tick(object sender, EventArgs e)
        {
            //LiveTimeLabel.Content = DateTime.Now.ToString("HH:mm:ss");
            LiveTimeLabel.Content = DateTime.Now.ToString(" dd MM yyyy ");

        }
        private void OnMyComboBoxChanged(object sender, SelectionChangedEventArgs e)
        {
            text = (e.AddedItems[0] as ComboBoxItem).Content as string;
            listofproduct();
        }
        public void listofproduct()
        {
            try
            {
                if (text == "tiles")
                {
                    gridProductss.Visibility = Visibility.Hidden;
                    List<string> items = new List<string>();
                    foreach (DataRow item in dt.Rows)
                    {
                        if (item["NameProduct"].ToString().Contains(txtsearch.Text))
                            items.Add(item["ProductID"].ToString() + "............." + item["NameProduct"].ToString() + item["PriceTotal"].ToString());
                    }
                    listproduct.ItemsSource = items;

                    gridProducts.Visibility = Visibility.Visible;
                }
                if (text == "list")
                {
                    gridProducts.Visibility = Visibility.Hidden;
                    gridProductss.Children.Clear();
                    gridProductss.RowDefinitions.Clear();
                    gridProductss.ColumnDefinitions.Clear();
                    int i1 = 0;
                    int j1 = 0;

                    //   int k = 0, j = 0, i = 0;
                    int col = Convert.ToInt32(2);
                    int row;//= Convert.ToInt32(5);
                    row = dt.Rows.Count / col;
                    for (int i = 0; i < row; i++)
                    {
                        RowDefinition rd = new RowDefinition();
                        rd.Height = new GridLength(1, GridUnitType.Star);
                        gridProductss.RowDefinitions.Add(rd);

                    }
                    for (int j = 0; j < col; j++)
                    {
                        ColumnDefinition cd = new ColumnDefinition();
                        cd.Width = new GridLength(1, GridUnitType.Star);
                        gridProductss.ColumnDefinitions.Add(cd);

                    }
                    foreach (DataRow item in dt.Rows)
                    {

                        #region create btn
                        //   DockPanel dp = new DockPanel();
                        // dp.Height = 55;
                        //  dp.Background = new SolidColorBrush(Colors.Yellow);
                        if (item["NameProduct"].ToString().Contains(txtsearch.Text))
                        {
                            Button btn = new Button();
                            btn.Name = "s" + item["ProductID"].ToString();
                            btn.MouseDoubleClick += btnProduct_Click;
                            btn.Click += btnProductmake_Click;
                            btn.MouseEnter += Btn_MouseMove;
                            Image myimage = new Image();
                            if (btn.IsMouseOver)
                                btn.Background = Brushes.Green;
                            btn.MouseLeave += Btn_MouseLeave;
                            btn.Background = Brushes.WhiteSmoke;
                            btn.Content = item["NameProduct"].ToString();
                            btn.Margin = new Thickness(2);
                            //dp.Children.Add(btn);
                            #endregion



                            Grid.SetRow(btn, i1);
                            Grid.SetColumn(btn, j1);
                            gridProductss.Children.Add(btn);
                            if (j1 < col - 1)
                            {
                                j1++;
                            }
                            else
                            {

                                j1 = 0;
                                i1++;
                            }
                        }
                    }
                    gridProductss.Visibility = Visibility.Visible;

                }
            }catch(Exception t)
            {
                MessageBox.Show(t.Message);
            }
        }

        private void Btn_MouseLeave(object sender, MouseEventArgs e)
        {
            Button bt = (sender as Button);
            bt.Background = Brushes.WhiteSmoke;
        }

        private void Btn_MouseMove(object sender, MouseEventArgs e)
        {

            Button bt = (sender as Button);
            bt.Background = Brushes.Green;
        }

        private void GetId()
        {

            readid();
            // lblid.Content = Id;
        }
        public void addfactor()
        {
            // Create a new DataTable.
            DataColumn dtColumn;


            // Create id column
            dtColumn = new DataColumn();
            dtColumn.DataType = typeof(Int32);
            dtColumn.ColumnName = "id";
            dtColumn.Caption = "Cust ID";
            dtColumn.ReadOnly = false;
            dtColumn.Unique = true;
            // Add column to the DataColumnCollection.
            custTable.Columns.Add(dtColumn);

            // Create Name column.
            dtColumn = new DataColumn();
            dtColumn.DataType = typeof(String);
            dtColumn.ColumnName = "غذا";
            dtColumn.Caption = "Cust Name";
            dtColumn.AutoIncrement = false;
            dtColumn.ReadOnly = false;
            dtColumn.Unique = false;
            /// Add column to the DataColumnCollection.
            custTable.Columns.Add(dtColumn);

            // Create Address column.
            dtColumn = new DataColumn();
            dtColumn.DataType = typeof(String);
            dtColumn.ColumnName = "قیمت";
            dtColumn.Caption = "Value";
            dtColumn.ReadOnly = false;
            dtColumn.Unique = false;
            // Add column to the DataColumnCollection.
            custTable.Columns.Add(dtColumn);

            // Create Address column.
            dtColumn = new DataColumn();
            dtColumn.DataType = typeof(String);
            dtColumn.ColumnName = "تعداد";
            dtColumn.Caption = "Number";
            dtColumn.ReadOnly = false;
            dtColumn.Unique = false;
            // Add column to the DataColumnCollection.
            custTable.Columns.Add(dtColumn);


            // Create Address column.
            dtColumn = new DataColumn();
            dtColumn.DataType = typeof(String);
            dtColumn.ColumnName = "قیمت کل";
            dtColumn.Caption = "PrimaryValue";
            dtColumn.ReadOnly = false;
            dtColumn.Unique = false;
            // Add column to the DataColumnCollection.
            custTable.Columns.Add(dtColumn);


            // Make id column the primary key column.
            DataColumn[] PrimaryKeyColumns = new DataColumn[1];
            PrimaryKeyColumns[0] = custTable.Columns["id"];
            custTable.PrimaryKey = PrimaryKeyColumns;

            // Create a new DataSet
            DataSet DtSet = new DataSet();
            // Add custTable to the DataSet.
            DtSet.Tables.Add(custTable);
        }

        private void btnProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string s, sss = null;
                int ss = 0;
                if (combosearch.Text == "list")
                {
                    s = (sender as Button).Name;
                    s = s.Remove(0, 1);
                    ss = Convert.ToInt32(s);
                }
                else if (combosearch.Text == "tiles")
                {
                    s = listproduct.SelectedItem.ToString();
                    int iii = 0;
                    while (s[iii] != '.')
                    {
                        if (Char.IsDigit(s[iii]))
                        {
                            sss += s[iii];
                        }
                        iii++;
                    }
                    ss = Convert.ToInt32(sss);

                }
                bool b;
                if (TxtNumbers.Text != null && TxtNumbers.Text != "0")
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        if (item["ProductID"].ToString() == ss.ToString())
                        {
                            if (sell < 2)
                            {

                                b = false;
                                foreach (ProductOfFactor item2 in factorkharid)
                                {
                                    if (item2.کدکالا == ss)

                                    {


                                        if (TxtNumbers.Text == null)
                                        {
                                            MessageBox.Show("Please Enter Numbers", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                                            break;
                                        }
                                        else
                                        {
                                            item2.تعداد = item2.تعداد + Convert.ToInt32(TxtNumbers.Text);
                                        }
                                        b = true;
                                        TotalPrice();
                                        CollectionViewSource.GetDefaultView(dgproduct.ItemsSource).Refresh();

                                        break;
                                    }
                                }
                                if (!b)
                                {
                                    ProductOfFactor pf = new ProductOfFactor();

                                    pf.کدکالا = Convert.ToInt32(item["ProductID"]);
                                    pf.نام_کالا = item["NameProduct"].ToString();
                                    pf.قیمت = Convert.ToDecimal(item["PriceTotal"]);
                                    pf.تخفیف = Convert.ToDecimal("0");

                                    pf.تعداد = Convert.ToInt32(TxtNumbers.Text);

                                    factorkharid.Add(pf);
                                    TotalPrice();
                                    CollectionViewSource.GetDefaultView(dgproduct.ItemsSource).Refresh();

                                }
                                // dgproduct.Columns[2].IsReadOnly = true;
                                // dgproduct.Columns[0].IsReadOnly = true;
                                // dgproduct.Columns[3].IsReadOnly = true;
                                if (sell == 0)
                                {
                                    item["Mojoodi"] = Convert.ToInt32(item["Mojoodi"]) + Convert.ToInt32(TxtNumbers.Text);
                                }
                                else if (sell == 1)
                                {
                                    item["Mojoodi"] = Convert.ToInt32(item["Mojoodi"]) - Convert.ToInt32(TxtNumbers.Text);
                                    item["Vahed"] = Convert.ToInt32(item["Vahed"]) + Convert.ToInt32(TxtNumbers.Text);
                                }

                                break;
                            }
                            else
                            {

                                MessageBox.Show("Plese Enter Sell Or Buy", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please Enter The Number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                }


                result();
            }catch(Exception q)
            {
                MessageBox.Show(q.Message);
            }
        }


        public void result()
        {

            Decimal result = 0, Result = 0;
            foreach (ProductOfFactor item in factorkharid)
            {
                result = result + item.قیمت_کل;
            }
            lblresult.Content = result;
            if (txthazine.Text == null)
                txthazine.Text = "0";
            if (result == 0)
            {
            }
            else
            {
                if (!txtoff.IsEnabled)
                {
                    Result = result - Convert.ToDecimal(txtoffN.Text) + Convert.ToDecimal(txthazine.Text);
                    txtoff.Text = Convert.ToDouble((result - Result + Convert.ToDecimal(txthazine.Text)) / result * 100).ToString();
                }
                else
                {
                    Result = result - (result * Convert.ToInt32(txtoff.Text) / 100);
                    txtoffN.Text = (result - Result).ToString();
                }
            }
            lblResult.Content = Result;
        }


        /* public DataTable CreateCustomersTable(int i , string name , string value)
         {
             DataRow myDataRow;
             myDataRow = custTable.NewRow();
             myDataRow["Id"] = i;
             myDataRow["Value"] = value;
             myDataRow["Name"] = name;
             custTable.Rows.Add(myDataRow);

             // Add data rows to the custTable using NewRow method
             // I add three customers with their addresses, names and ids
               myDataRow = custTable.NewRow();
               myDataRow["id"] = 1;
               myDataRow["Value"] = "100";
               myDataRow["Name"] = "George Bishop";
               custTable.Rows.Add(myDataRow);
               myDataRow = custTable.NewRow();
               myDataRow["id"] = 2;
               myDataRow["Name"] = "Rock joe";
               myDataRow["Value"] = "200";
               custTable.Rows.Add(myDataRow);
               myDataRow = custTable.NewRow();
               myDataRow["id"] = 3;
               myDataRow["Name"] = "Miranda";
               myDataRow["Value"] = "279";
               custTable.Rows.Add(myDataRow);


             return custTable;
         }
     */

        private void btnProductmake_Click(object sender, RoutedEventArgs e)
        {
            
            num++;
            int id = Convert.ToInt32(((sender as Button).Name).Remove(0, 1));
            lblinformation.Content = information(id);
            showhazine(id);
        }




        /*  private void btnLoadProduct_Click(object sender, RoutedEventArgs e )
          {
              if (combosearch.Text == "tiles") {
                  List<string> items = new List<string>();
                  foreach (DataRow item in dt.Rows)
                  {
                      items.Add(item["ProductID"].ToString() + item["NameProduct"].ToString() + item["Price"].ToString());
                  }
                  listproduct.ItemsSource = items;
              }else if (combosearch.Text == "list") {
                  listproduct.Visibility = Visibility.Hidden;
                  int i1 = 0;
                  int j1 = 0;

                  //   int k = 0, j = 0, i = 0;
                  int col = Convert.ToInt32(2);
                  int row;//= Convert.ToInt32(5);
                  row = dt.Rows.Count / col;
                  for (int i = 0; i < row; i++)
                  {
                      RowDefinition rd = new RowDefinition();
                      rd.Height = new GridLength(1, GridUnitType.Star);
                      gridProducts.RowDefinitions.Add(rd);

                  }
                  for (int j = 0; j < col; j++)
                  {
                      ColumnDefinition cd = new ColumnDefinition();
                      cd.Width = new GridLength(1, GridUnitType.Star);
                      gridProducts.ColumnDefinitions.Add(cd);

                  }
                  foreach (DataRow item in dt.Rows)
                  {

                      #region create btn
                      //   DockPanel dp = new DockPanel();
                      // dp.Height = 55;
                      //  dp.Background = new SolidColorBrush(Colors.Yellow);

                      Button btn = new Button();
                      btn.Name = "s" + item["ProductID"].ToString();
                      btn.Click += btnProduct_Click;
                      btn.Click += btnProductmake_Click;
                      btn.Content = item["NameProduct"].ToString();
                      btn.ToolTip = Convert.ToDouble(item["Price"].ToString()).ToString();
                      btn.Margin = new Thickness(2);
                      //dp.Children.Add(btn);
                      #endregion



                      Grid.SetRow(btn, i1);
                      Grid.SetColumn(btn, j1);
                      gridProducts.Children.Add(btn);
                      if (j1 < col - 1)
                      {
                          j1++;
                      }
                      else
                      {

                          j1 = 0;
                          i1++;
                      }

                  }

              }
              else
              {
                  MessageBox.Show(combosearch.Text);
              }
          }
        */
        public static DataTable PullData()
        {
            string connString = @"Data Source = 2.180.43.85; Initial Catalog = TestDB; User ID = TestUser; Password = S13791381";
            string query = "select * from View_ProductTotal";
            string query2 = "SELECT TOP 1 FactorId FROM CommerceFactor ORDER BY FactorID DESC";
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            SqlCommand cmd2 = new SqlCommand(query2, conn);
            conn.Open();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            SqlDataReader re;
            // this will query your database and return the result to your datatable
            DataTable dataTable = new DataTable();
            da.Fill(dataTable);
            re = cmd2.ExecuteReader();
            if (re.Read())
                Id = Convert.ToInt32(re.GetValue(0).ToString());
            conn.Close();
            da.Dispose();
            return dataTable;
        }

        public void PushData(ObservableCollection<ProductOfFactor> data)
        {

            string connString = @"Data Source = 2.180.43.85; Initial Catalog = TestDB; User ID = TestUser; Password = S13791381";

            SqlConnection conn = new SqlConnection(connString);
            SqlDataAdapter adapter = new SqlDataAdapter();
            conn.Open();
           
            foreach (ProductOfFactor item in data)
            {
                string query1 = "UPDATE CommerceProducts SET Vahed = " + dt.Rows[item.کدکالا - 1]["Vahed"] + ", Mojoodi = " + dt.Rows[item.کدکالا - 1]["Mojoodi"] + " WHERE ProductID = " + item.کدکالا + "; ";
                adapter.InsertCommand = new SqlCommand(query1, conn);
                adapter.InsertCommand.ExecuteNonQuery();
                if (sell == 1)
                {
                    string query2 = "Insert into CommerceFactorItem (FactorItemID,FactorID,ProductID,Takhfif , Price , ProductOut)" + "VALUES('" + sell + "' ,'" + Convert.ToInt32(lblsarbarg.Content) + "' ,'" + item.کدکالا + "','" + item.تخفیف + "','" + item.قیمت + "','" + item.تعداد + "')";
                    adapter.InsertCommand = new SqlCommand(query2, conn);
                    adapter.InsertCommand.ExecuteNonQuery();
                }
                else if (sell == 0)
                {
                    string query2 = "Insert into CommerceFactorItem (FactorItemID,FactorID,ProductID,Takhfif , Price , ProductIn)" + "VALUES('" + sell + "' ,'" + Convert.ToInt32(lblsarbarg.Content) + "' ,'" + item.کدکالا + "','" + item.تخفیف + "','" + item.قیمت + "','" + item.تعداد + "')";
                    adapter.InsertCommand = new SqlCommand(query2, conn);
                    adapter.InsertCommand.ExecuteNonQuery();
                }
            }

            string query = "Insert into CommerceFactor (FactorID,SanadID,PriceTotal,Takhfif , Time)" + "VALUES('" + Convert.ToInt32(lblsarbarg.Content) + "' ,'" + sell + "' ,'" + Convert.ToDecimal(lblResult.Content) + "','" + Convert.ToDecimal(txtoffN.Text) + "','" + LiveTimeLabel.Content + "')";
            SqlCommand cmd = new SqlCommand(query, conn);
            adapter.InsertCommand = new SqlCommand(query, conn);
            adapter.InsertCommand.ExecuteNonQuery();





            conn.Close();
            factorkharid.Clear();
        }

        private void UpdateFactors(ObservableCollection<ProductOfFactor> data , int idfactor , decimal result , decimal takhfif)
        {
            DataTable T = new DataTable();
            T = dictfactors[idfactor];
            string connString = @"Data Source = 2.180.43.85; Initial Catalog = TestDB; User ID = TestUser; Password = S13791381";
            int sell = Convert.ToInt32(T.Rows[0]["FactorItemId"]);
            SqlConnection conn = new SqlConnection(connString);
            SqlDataAdapter adapter = new SqlDataAdapter();
            conn.Open();
            int i = 0;
            foreach (ProductOfFactor item in data)
            {
                
                if (sell == 1)
                {
                    try
                    {
                        string query1 = "update CommerceFactorItem set Price = " + Convert.ToDouble(item.قیمت) + " ,Takhfif = " + Convert.ToDouble(item.تخفیف) + " , ProductOut = "+item.تعداد+"  where FactorId = " + idfactor + " and ProductID = "+item.کدکالا+";";
                        SqlCommand cmd1 = new SqlCommand(query1, conn);
                        adapter.InsertCommand = new SqlCommand(query1, conn);
                        adapter.InsertCommand.ExecuteNonQuery();
                        T.Rows[i]["Price"] = Convert.ToDecimal(item.قیمت);
                        T.Rows[i]["Takhfif"] = Convert.ToDecimal(item.تخفیف);
                        T.Rows[i]["ProductIn"] = Convert.ToInt32(item.تعداد);

                        
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show(e.Message.ToString());
                    }
                }
                else if (sell == 0)
                {
                    try
                    {
                        string query1 = "update CommerceFactorItem set Price = " + Convert.ToDouble(item.قیمت) + " ,Takhfif = " + Convert.ToDouble(item.تخفیف) + " , ProductIn = " + item.تعداد + "  where FactorId = " + idfactor + " and ProductID = " + item.کدکالا + ";";
                        SqlCommand cmd1 = new SqlCommand(query1, conn);
                        adapter.InsertCommand = new SqlCommand(query1, conn);
                        adapter.InsertCommand.ExecuteNonQuery();
                        T.Rows[i]["Price"] = Convert.ToDecimal(item.قیمت);
                        T.Rows[i]["Takhfif"] = Convert.ToDecimal(item.تخفیف);
                        T.Rows[i]["ProductOut"] = Convert.ToInt32(item.تعداد);


                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message.ToString());
                    }
                }
                i++;
            }
            double takhfif1 = Convert.ToDouble(takhfif);
            double result1 = Convert.ToDouble(result);

            string query = "update CommerceFactor set PriceTotal = "+result1+" ,Takhfif = "+takhfif1+"  where FactorId = "+idfactor+";" ;
            SqlCommand cmd = new SqlCommand(query, conn);
            adapter.InsertCommand = new SqlCommand(query, conn);
            adapter.InsertCommand.ExecuteNonQuery();





            conn.Close();
            factorkharid.Clear();
        }

        private void btnShowProducts_Click(object sender, RoutedEventArgs e)
        {
            if (gridProducts.Visibility == Visibility.Collapsed)
                gridProducts.Visibility = Visibility.Visible;
            else
                gridProducts.Visibility = Visibility.Collapsed;

        }


        /*
        private void btndelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgproduct.SelectedIndex >= 0)
            {
                int index = dgproduct.SelectedIndex;
                try
                {
                    if (factorkharid[index].Number - 1 == 0)
                    {
                        factorkharid.RemoveAt(index);
                        CollectionViewSource.GetDefaultView(dgproduct.ItemsSource).Refresh();
                    }
                    else
                    {
                        factorkharid[index].Number = factorkharid[index].Number - 1;
                        CollectionViewSource.GetDefaultView(dgproduct.ItemsSource).Refresh();
                    }
                }
                catch
                {
                    MessageBox.Show("Please Choose What You Want To Delete");
                }

            }
            result();
        }
        */
        private void btnsubmit_Click(object sender, RoutedEventArgs e)
        {
            if (lblResult.Content.ToString() != "0")
            {
                DataTable td = new DataTable();
                DataColumn tr1 = new DataColumn("Takhfif", typeof(int));
                DataColumn tr2 = new DataColumn("Price", typeof(decimal));
                DataColumn tr3 = new DataColumn("ProductOut", typeof(decimal));
                DataColumn tr4 = new DataColumn("ProductIn", typeof(string));
                DataColumn tr5 = new DataColumn("ProductID", typeof(string));
                DataColumn tr = new DataColumn("FactorItemID", typeof(string));
                td.Columns.Add(tr);
                td.Columns.Add(tr5);
                td.Columns.Add(tr4);
                td.Columns.Add(tr3);
                td.Columns.Add(tr2);
                td.Columns.Add(tr1);
                foreach (var item in factorkharid)
                {
                    var dr = td.NewRow();
                    dr["Takhfif"] = item.تخفیف;
                    if(sell == 1)
                        dr["ProductOut"] = item.تعداد; 
                    else if(sell == 0)
                        dr["ProductIn"] = item.تعداد;
                    dr["Price"] = item.قیمت; 
                    dr["FactorItemID"] = sell; 
                    dr["ProductID"] = item.کدکالا;
                    td.Rows.Add(dr);
                }
                MessageBox.Show("فاکتور ثبت شد");
                dictfactors.Add(Convert.ToInt32(txtfactornumber.Text), td);
                PushData(factorkharid);
                lbliddoreii.Content = ((Convert.ToInt32(lbliddoreii.Content) + 1) % 100).ToString();
                lbliddorei.Content = ((Convert.ToInt32(lbliddoreii.Content) / 100) + 1).ToString();
                lblsarbarg.Content = (Convert.ToInt32(lblsarbarg.Content) + 1).ToString();
                txtfactornumber.Text = (Convert.ToInt32(txtfactornumber.Text) + 1).ToString();
                if (sell == 0)
                    lblidkh.Content = (Convert.ToInt32(lblidkh.Content) + 1).ToString();
                if (sell == 1)
                    lblidf.Content = (Convert.ToInt32(lblidf.Content) + 1).ToString();

            }
            else
            {
                MessageBox.Show("Please First Order", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }


            writeid();
            lblResult.Content = 0;
            lblresult.Content = 0;
            txtoff.Text = "0";
            txtoffN.Text = "0";
        }

        private void editanbar()
        {
            Window1 edit = new Window1(dt, 0);
            edit.ShowDialog();
        }
        private void btnedit_Click(object sender, RoutedEventArgs e)
        {
            editanbar();
        }
        private void dgvMailingList_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.CellStyle = new Style(typeof(DataGridCell));
            e.Column.CellStyle.Setters.Add(new Setter(DataGridCell.BackgroundProperty, new SolidColorBrush(Colors.LightBlue)));
        }
        private void showfactorforosh()
        {
            wrapfactornumber.Visibility = Visibility.Visible;
            wrapfishdasti.Visibility = Visibility.Visible;
            wrapids.Visibility = Visibility.Visible;
            wrapsubmit.Visibility = Visibility.Visible;
            docksum.Visibility = Visibility.Visible;
            wrapdetails.Visibility = Visibility.Visible;
            gridProducts.Visibility = Visibility.Visible;
            gridsearch.Visibility = Visibility.Visible;
            dtgmandehesab.Visibility = Visibility.Collapsed;
            txtfactornumber.Text = lblsarbarg.Content.ToString();
            lblidf.Visibility = Visibility.Visible;
            lblidkh.Visibility = Visibility.Hidden;
            dgproduct.Visibility = Visibility.Visible;
            MessageBox.Show("You Are In The Sell Factor", "Notice", MessageBoxButton.OK, MessageBoxImage.Information);
            dgproduct.Columns[2].Header = "نام کالا";
            dgproduct.Columns[5].Header = "قیمت کل";
            dgproduct.CanUserAddRows = true;
            dgproduct.Background = Brushes.BurlyWood;
            sell = 1;
        }
        private void btnfctorforosh_Click(object sender, RoutedEventArgs e)
        {
            
            showfactorforosh();
        }

        private void showfactorkhariod()
        {
            wrapfactornumber.Visibility = Visibility.Visible;
            wrapfishdasti.Visibility = Visibility.Visible;
            wrapids.Visibility = Visibility.Visible;
            wrapsubmit.Visibility = Visibility.Visible;
            docksum.Visibility = Visibility.Visible;
            wrapdetails.Visibility = Visibility.Visible;
            gridProducts.Visibility = Visibility.Visible;
            gridsearch.Visibility = Visibility.Visible;
            dtgmandehesab.Visibility = Visibility.Collapsed;
            lblidkh.Visibility = Visibility.Visible;
            lblidf.Visibility = Visibility.Hidden;
            dgproduct.Visibility = Visibility.Visible;
            txtfactornumber.Text = lblsarbarg.Content.ToString();
            MessageBox.Show("You Are In The Buy Factor", "Notice", MessageBoxButton.OK, MessageBoxImage.Information);
            dgproduct.Columns[2].Header = "نام کالا";
            dgproduct.Columns[5].Header = "قیمت کل";
            dgproduct.Background = Brushes.BlanchedAlmond;
            sell = 0;

        }
        private void btnfctorkharid_Click(object sender, RoutedEventArgs e)
        {
            
            showfactorkhariod();
        }

        private void btnforoshkol_Click(object sender, RoutedEventArgs e)
        {
            showfactors();
        }

        private void btnone_Click(object sender, RoutedEventArgs e)
        {
            TxtNumbers.Text += (sender as Button).Content.ToString();
        }



        private void btnC_Click(object sender, RoutedEventArgs e)
        {
            TxtNumbers.Text = null;
        }

        private void txtsearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            listofproduct();
        }

        public string information(int id)
        {
            foreach (DataRow item in dt.Rows)
            {
                if (item["ProductID"].ToString() == id.ToString())
                {
                    string result = "قیمت:" + item["PriceTotal"].ToString() + "\n موجودی: " + item["Mojoodi"].ToString() + "";
                    return result;
                }
            }
            return null;
        }

        private void listproduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int id = listproduct.SelectedIndex + 1;
            lblinformation.Content = information(id);
            showhazine(id);
        }

        private void txtoff_SelectionChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Convert.ToDouble(txtoff.Text) > 0)
                    result();
            }
            catch
            {
                MessageBox.Show("No Set Number to Off", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void TotalPrice()
        {
            try
            {
                foreach (ProductOfFactor item in factorkharid)
                {
                    item.قیمت_کل = Convert.ToDecimal(item.قیمت * item.تعداد) - item.تخفیف;
                }
                CollectionViewSource.GetDefaultView(dgproduct.ItemsSource).Refresh();
            }
            catch(Exception q)
            {
                MessageBox.Show(q.Message);
            }
        }
        private void dgproduct_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                Costsdetails.Clear();
                DataGrid dt1 = sender as DataGrid;
                int columnIndex = dt1.CurrentColumn.DisplayIndex;
                TextBlock targetCell = (TextBlock)dt1.SelectedCells[0].Column.GetCellContent(dt1.SelectedItem);
                IDCURRENTPRODUCT = Convert.ToInt32(targetCell.Text);
                foreach (DataRow item in dt.Rows)
                {
                    if (item["ProductID"].ToString() == targetCell.Text)
                    {
                        showhazine((int)item["ProductID"]);
                        break;
                    }
                }
                foreach (var item in listhazineha.Items)
                {
                    Costsdetails.Add((detailscost)item);
                }
                
            }
            catch(Exception ew)
            {
                MessageBox.Show(ew.Message);
            }





            try
            {
                /*
                bool valid = false;
                DataGrid dt1 = sender as DataGrid;
                int columnIndex = dt1.CurrentColumn.DisplayIndex;
                TextBlock targetCell = (TextBlock)dt1.SelectedCells[0].Column.GetCellContent(dt1.SelectedItem);
                
                */
                TotalPrice();
                result();
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
            }
        }
        private void writeid()
        {
            string file = @"C:\Users\user\source\repos\RahsabzProjects\FactorsId.txt";
            StreamReader Textfile = new StreamReader(file);
            string line;
            string result = null;

            while ((line = Textfile.ReadLine()) != null)
            {
                if (line.Contains("شماره فاکتور خرید"))
                {
                    for (int i = 0; i < line.Length; i++)
                    {
                        if (Char.IsDigit(line[i]))
                        {
                            line = line.Remove(i);
                            break;
                        }

                    }
                    line = line + lblidkh.Content;
                }
                if (line.Contains("شماره فاکتور فروش"))
                {
                    for (int i = 0; i < line.Length; i++)
                    {
                        if (Char.IsDigit(line[i]))
                        {
                            line = line.Remove(i);
                            break;
                        }

                    }
                    line = line + lblidf.Content;
                }
                if (line.Contains("شماره سربرگ"))
                {
                    for (int i = 0; i < line.Length; i++)
                    {
                        if (Char.IsDigit(line[i]))
                        {
                            line = line.Remove(i);
                            break;
                        }

                    }
                    line = line + lblsarbarg.Content;
                }
                if (line.Contains("شماره فاکتور دوره ای"))
                {
                    for (int i = 0; i < line.Length; i++)
                    {
                        if (Char.IsDigit(line[i]))
                        {
                            line = line.Remove(i);
                            break;
                        }

                    }
                    line = line + lbliddoreii.Content;
                }
                result = result + line + "\n";
            }
            Textfile.Close();
            File.WriteAllText(file, string.Empty);
            File.WriteAllText(file, result);

        }
        private void readid()
        {
            string file = @"C:\Users\user\source\repos\RahsabzProjects\FactorsId.txt";
            StreamReader Textfile = new StreamReader(file);
            string line;
            while ((line = Textfile.ReadLine()) != null)
            {

                if (line.Contains("شماره سربرگ"))
                {
                    for (int i = 0; i < line.Length; i++)
                    {
                        if (Char.IsDigit(line[i]))
                        {
                            lblsarbarg.Content += line[i].ToString();

                        }
                    }

                }
                if (line.Contains("شماره فاکتور فروش"))
                {
                    for (int i = 0; i < line.Length; i++)
                    {
                        if (Char.IsDigit(line[i]))
                            lblidf.Content += line[i].ToString();

                    }
                }
                if (line.Contains("شماره فاکتور دوره ای"))
                {
                    for (int i = 0; i < line.Length; i++)
                    {
                        if (Char.IsDigit(line[i]))
                            lbliddoreii.Content += line[i].ToString();
                            
                    }
                }
                if (line.Contains("شماره فاکتور خرید"))
                {
                    for (int i = 0; i < line.Length; i++)
                    {
                        if (Char.IsDigit(line[i]))
                            lblidkh.Content += line[i].ToString();

                    }
                }

            }
            lbliddoreii.Content = (Convert.ToInt32(lblsarbarg.Content) % 100).ToString();
            lbliddorei.Content = ((Convert.ToInt32(lblsarbarg.Content) / 100) + 1).ToString();
        }
        private void FillPastFactors()
        {
            Factors.Clear();
            dictfactorsP.Clear();
            DataTable factorss = new DataTable();
            factorss = PullFactors();

            foreach (DataRow item in factorss.Rows)
            {
                FactorsType CurrentFactorsType = new FactorsType();
                CurrentFactorsType.شماره_فاکتور = Convert.ToInt32(item["FactorID"]);
                if (Convert.ToInt32(item["SanadID"]) == 1)
                    CurrentFactorsType.نوع_فاکتور = "فاکتور فروش";
                if (Convert.ToInt32(item["SanadID"]) == 0)
                    CurrentFactorsType.نوع_فاکتور = "فاکتور خرید";
                CurrentFactorsType.قیمت_کل = Convert.ToDecimal(item["PriceTotal"]);
                CurrentFactorsType.تخفیف_کل = Convert.ToDecimal(item["Takhfif"]);
                CurrentFactorsType.زمان = item["Time"].ToString();
                Factors.Add(CurrentFactorsType);

                //MessageBox.Show(CurrentFactorsType.شماره_فاکتور.ToString());
            }
            makedictfactorsP();
        }

        private void makedictfactorsP()
        {
            foreach (var item in Factors)
            {
                dictfactorsP.Add(item.شماره_فاکتور, item);
            }
            
        }

        private void showfactors()
        {
            FillPastFactors();
            DtgFactors.ItemsSource = Factors;
            dictfactors = factors();
            ListFactors listfactors = new ListFactors(Factors, dictfactors, dt);
            listfactors.ShowDialog();
        }
        private void btnfactors_Click(object sender, RoutedEventArgs e)
        {
            wrapfactornumber.Visibility = Visibility.Visible;
            wrapfishdasti.Visibility = Visibility.Visible;
            wrapids.Visibility = Visibility.Visible;
            wrapsubmit.Visibility = Visibility.Visible;
            docksum.Visibility = Visibility.Visible;
            wrapdetails.Visibility = Visibility.Visible;
            gridProducts.Visibility = Visibility.Visible;
            gridsearch.Visibility = Visibility.Visible;
            dtgmandehesab.Visibility = Visibility.Collapsed;
            txtfactornumber.Text = "1";
            factorsshow();
            dgproduct.Visibility = Visibility.Visible;
            MessageBox.Show("نمایش فاکتورها");
            dgproduct.Columns[2].Header = "نام کالا";
            dgproduct.Columns[5].Header = "قیمت کل";
            dgproduct.Background = Brushes.LightYellow;
        }

        private void chdarsad_Checked(object sender, RoutedEventArgs e)
        {
            txtoff.IsEnabled = true;
            txtoffN.IsEnabled = false;
        }

        private void chdarsad_Unchecked(object sender, RoutedEventArgs e)
        {
            txtoff.IsEnabled = false;
            txtoffN.IsEnabled = true;
        }





        private void txtoffN_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtoffN.Text == null)
            {
                txtoffN.Text = "0";
            }
            try
            {
                txtoffN.Background = Brushes.White;
                if (Convert.ToDouble(txtoffN.Text) != 0)
                    result();
            }
            catch
            {
                txtoffN.Background = Brushes.Red;
            }
        }

        public DataTable PullFactors()
        {
            string connString = @"Data Source = 2.180.43.85; Initial Catalog = TestDB; User ID = TestUser; Password = S13791381";
            string query = "select * from CommerceFactor";
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            DataTable dataTables = new DataTable();
            da.Fill(dataTables);
            conn.Close();
            da.Dispose();



            return dataTables;
        }
        DataTable PullPrivateFactors(int id)
        {
            string connString = @"Data Source = 2.180.43.85; Initial Catalog = TestDB; User ID = TestUser; Password = S13791381";
            string query = "SELECT FactorItemID , ProductID ,ProductIn ,  ProductOut ,  Price , Takhfif  FROM CommerceFactorItem WHERE FactorID = " + id + ";";
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            DataTable dataTa = new DataTable();
            da.Fill(dataTa);
            conn.Close();
            da.Dispose();



            return dataTa;
        }
        private Dictionary<int, DataTable> factors()
        {
            Dictionary<int, DataTable> My_dict1 =
                       new Dictionary<int, DataTable>();
            int end = Convert.ToInt32(lblsarbarg.Content);
            foreach (var item in dictfactorsP)
            {
                My_dict1.Add(item.Key, PullPrivateFactors(item.Key));

            }
            return My_dict1;
        }
        private void DtgFactors_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            /*
            DataTable dtt = new DataTable();
            ObservableCollection<ProductOfFactor> pf = new ObservableCollection<ProductOfFactor>();
            DataGrid dt1 = sender as DataGrid;
            FactorsType ft = new FactorsType();
            int columnIndex = dt1.CurrentColumn.DisplayIndex;
            TextBlock targetCell = (TextBlock)dt1.SelectedCells[0].Column.GetCellContent(dt1.SelectedItem);
            ft.شماره_فاکتور = Convert.ToInt32(targetCell.Text);
            targetCell = (TextBlock)dt1.SelectedCells[2].Column.GetCellContent(dt1.SelectedItem);
            ft.قیمت_کل = Convert.ToDecimal(targetCell.Text);
            targetCell = (TextBlock)dt1.SelectedCells[1].Column.GetCellContent(dt1.SelectedItem);
            ft.نوع_فاکتور = targetCell.Text.ToString();
            targetCell = (TextBlock)dt1.SelectedCells[4].Column.GetCellContent(dt1.SelectedItem);
            ft.زمان = targetCell.Text.ToString();
            targetCell = (TextBlock)dt1.SelectedCells[3].Column.GetCellContent(dt1.SelectedItem);
            ft.تخفیف_کل = Convert.ToDecimal(targetCell.Text);
            dtt = PullPrivateFactors(ft);
            dtt.Columns.Add("PriceTootal", typeof(System.Decimal));
            dtt.Columns.Add("NameProduct", typeof(System.String));
            if ((ft.نوع_فاکتور) =="فاکتور فروش")
            {
                dtt.Columns.Remove("ProductIn");

            }
            else
            {
                dtt.Columns.RemoveAt(3);
            }
            foreach (DataRow item in dtt.Rows)
            {
                ProductOfFactor pof = new ProductOfFactor();
                pof.Id = Convert.ToInt32(item["ProductID"]);
                
                pof.Off = Convert.ToDecimal(item["Takhfif"]);
                pof.Price = Convert.ToDecimal(item["Price"]);
                
                if ((ft.نوع_فاکتور) == "فاکتور فروش")
                {
                    
                    pof.PriceTootal = Convert.ToDecimal(Convert.ToInt32(item["Price"]) * Convert.ToInt32(item["ProductOut"])) - Convert.ToDecimal(item["Takhfif"]);
                }
                else
                    pof.PriceTootal = Convert.ToDecimal(Convert.ToInt32(item["Price"]) * Convert.ToInt32(item["ProductIn"])) - Convert.ToDecimal(item["Takhfif"]);
                item["PriceTootal"] = pof.PriceTootal;
                foreach (DataRow item2 in dt.Rows)
                {
                    if (Convert.ToInt32(item2["ProductID"]) == pof.Id)
                    {
                        pof.Name = item2["NameProduct"].ToString();
                        item["NameProduct"] = item2["NameProduct"].ToString();
                        break;
                    }
                }
                pf.Add(pof);
                
            }
            Window1 Factor = new Window1(dtt , ft.شماره_فاکتور);
            Factor.ShowDialog();
            */
        }

        private void txthazine_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnedit_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void btnaddfactor_Click(object sender, RoutedEventArgs e)
        {
            txtfactornumber.Text = lblsarbarg.Content.ToString();
        }

        private void btnlast_Click(object sender, RoutedEventArgs e)
        {
            txtfactornumber.Text = (Convert.ToInt32(lblsarbarg.Content) - 1).ToString();
        }

        private void btnfirst_Click(object sender, RoutedEventArgs e)
        {
            txtfactornumber.Text = "1";
        }

        private void btnnext_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(txtfactornumber.Text) != (Convert.ToInt32(lblsarbarg.Content) - 1))
                txtfactornumber.Text = (Convert.ToInt32(txtfactornumber.Text) + 1).ToString();
        }

        private void btnpreviuos_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(txtfactornumber.Text) != 1)
                txtfactornumber.Text = (Convert.ToInt32(txtfactornumber.Text) - 1).ToString();
        }

        private void factorsshow()
        {
            try
            {
                factorkharid.Clear();

                string id = txtfactornumber.Text;
                int Id = Convert.ToInt32(id);
                DataTable dtt = new DataTable();

                dtt = dictfactors[Id];
                btnsubmit.Visibility = Visibility.Collapsed;
                btneditfactor.Visibility = Visibility.Visible;
                btndelete.Visibility = Visibility.Visible;
                btnedits.Visibility = Visibility.Collapsed;
                foreach (DataRow item in dtt.Rows)
                {

                    ProductOfFactor pof = new ProductOfFactor();
                    pof.کدکالا = Convert.ToInt32(item["ProductID"]);

                    pof.تخفیف = Convert.ToDecimal(item["Takhfif"]);
                    pof.قیمت = Convert.ToDecimal(item["Price"]);
                    try
                    {
                        pof.تعداد = Convert.ToInt32(item["ProductOut"]);
                        dgproduct.Background = Brushes.DarkRed;
                    }
                    catch
                    {
                        pof.تعداد = Convert.ToInt32(item["ProductIn"]);
                        dgproduct.Background = Brushes.DarkGreen;

                    }
                    try
                    {

                        pof.قیمت_کل = Convert.ToDecimal(Convert.ToInt32(item["Price"]) * Convert.ToInt32(item["ProductOut"])) - Convert.ToDecimal(item["Takhfif"]);
                    }
                    catch
                    {
                        pof.قیمت_کل = Convert.ToDecimal(Convert.ToInt32(item["Price"]) * Convert.ToInt32(item["ProductIn"])) - Convert.ToDecimal(item["Takhfif"]);
                    }

                    foreach (DataRow item2 in dt.Rows)
                    {
                        if (Convert.ToInt32(item2["ProductID"]) == pof.کدکالا)
                        {
                            pof.نام_کالا = item2["NameProduct"].ToString();

                            break;
                        }
                    }
                    factorkharid.Add(pof);
                }
                
                    txtoffN.Text = dictfactorsP[Id].تخفیف_کل.ToString();
                    lblResult.Content = dictfactorsP[Id].قیمت_کل;
                    lblresult.Content = Convert.ToDecimal(txtoffN.Text) + Convert.ToDecimal(lblResult.Content);
                    txtoff.Text = ((Convert.ToDecimal(txtoffN.Text) + Convert.ToDecimal(lblResult.Content)) / Convert.ToDecimal(Factors[Id - 2].تخفیف_کل)).ToString();
            }
            catch (Exception er)
            {
                if (txtfactornumber.Text == lblsarbarg.Content)
                {
                    btnsubmit.Visibility = Visibility.Visible;
                    btnedits.Visibility = Visibility.Visible;
                    btndelete.Visibility = Visibility.Collapsed;
                    btneditfactor.Visibility = Visibility.Collapsed;
                }
            }
            
        }
        private void txtfactornumber_SelectionChanged(object sender, RoutedEventArgs e)
        {
            factorsshow();
            
        }

        private void btnother_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnmande_Click(object sender, RoutedEventArgs e)
        {
            
        }
        
        private void btnsalary_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btncheck_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnbank_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnpersons_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnhazine_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnsandogh_Click(object sender, RoutedEventArgs e)
        {

        }
 
        private void txtanbarnumber_GotFocus(object sender, RoutedEventArgs e)
        {
            txtanbarnumber.Background = Brushes.LightYellow;
            HotkeysManager.AddHotkey(new GlobalHotkey(ModifierKeys.None, Key.DbeEnterDialogConversionMode, () => {  }));
            

            
            
        }

        private void txtanbarnumber_LostFocus(object sender, RoutedEventArgs e)
        {
            txtanbarnumber.Background = Brushes.WhiteSmoke;
        }

        private void txtclientnumber_GotFocus(object sender, RoutedEventArgs e)
        {
            txtclientnumber.Background = Brushes.LightYellow;
        }

        private void btneditfactor_Click(object sender, RoutedEventArgs e)
        {
            UpdateFactors(factorkharid , Convert.ToInt32(txtfactornumber.Text) , Convert.ToDecimal(lblResult.Content) , Convert.ToDecimal(txtoffN.Text));
            
        }

        private void txtclientnumber_LostFocus(object sender, RoutedEventArgs e)
        {
            txtclientnumber.Background = Brushes.WhiteSmoke;
        }

        private void btnother_GotFocus(object sender, RoutedEventArgs e)
        {
            btnother.Background = Brushes.LightYellow;
        }

        private void btnother_LostFocus(object sender, RoutedEventArgs e)
        {
            txtclientnumber.Background = Brushes.WhiteSmoke;
        }

        private void txtfishdasti_GotFocus(object sender, RoutedEventArgs e)
        {
            txtfishdasti.Background = Brushes.LightYellow;
        }

        private void txtfishdasti_LostFocus(object sender, RoutedEventArgs e)
        {
            txtfishdasti.Background = Brushes.WhiteSmoke;
        }

        private void txtsearch_GotFocus(object sender, RoutedEventArgs e)
        {
            txtsearch.Background = Brushes.LightYellow;
        }

        private void txtsearch_LostFocus(object sender, RoutedEventArgs e)
        {
            txtsearch.Background = Brushes.WhiteSmoke;
        }

        private void combosearch_GotFocus(object sender, RoutedEventArgs e)
        {
            combosearch.Background = Brushes.LightYellow;
        }

        private void combosearch_LostFocus(object sender, RoutedEventArgs e)
        {
            combosearch.Background = Brushes.WhiteSmoke;
        }

        private void txtdetalis_GotFocus(object sender, RoutedEventArgs e)
        {
            txtdetalis.Background = Brushes.LightYellow;
        }

        private void txtdetalis_LostFocus(object sender, RoutedEventArgs e)
        {
            txtdetalis.Background = Brushes.WhiteSmoke;
        }

        private void txtoffN_GotFocus(object sender, RoutedEventArgs e)
        {
            txtoffN.Background = Brushes.LightYellow;
        }

        private void txtoffN_LostFocus(object sender, RoutedEventArgs e)
        {
            txtoffN.Background = Brushes.WhiteSmoke;
        }

        private void txthazine_GotFocus(object sender, RoutedEventArgs e)
        {
            txthazine.Background = Brushes.LightYellow;
        }

        private void txthazine_LostFocus(object sender, RoutedEventArgs e)
        {
            txthazine.Background = Brushes.WhiteSmoke;
        }

        private void btnsubmit_GotFocus(object sender, RoutedEventArgs e)
        {
            btnsubmit.Background = Brushes.LightYellow;
        }

        private void btnsubmit_LostFocus(object sender, RoutedEventArgs e)
        {
            btnsubmit.Background = Brushes.WhiteSmoke;
        }

        private void btndelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtfactornumber.Text = (Convert.ToInt32(txtfactornumber.Text) - 1).ToString();
                dictfactors.Remove(Convert.ToInt32(txtfactornumber.Text));
                foreach (var item in Factors)
                {
                    if ((int)item.شماره_فاکتور == Convert.ToInt32(txtfactornumber.Text))
                    {
                        Factors.Remove(item);
                        break;
                    }
                }
                string connString = @"Data Source = 2.180.43.85; Initial Catalog = TestDB; User ID = TestUser; Password = S13791381";
                SqlConnection conn = new SqlConnection(connString);
                SqlDataAdapter adapter = new SqlDataAdapter();
                conn.Open();
                string query = "DELETE FROM CommerceFactor WHERE FactorID= " + Convert.ToInt32(txtfactornumber.Text) + ";";
                string query2 = "DELETE FROM CommerceFactorItem WHERE FactorID= " + Convert.ToInt32(txtfactornumber.Text) + ";";
                SqlCommand cmd = new SqlCommand(query, conn);
                adapter.InsertCommand = new SqlCommand(query, conn);
                adapter.InsertCommand.ExecuteNonQuery();
                SqlCommand cmd2 = new SqlCommand(query2, conn);
                adapter.InsertCommand = new SqlCommand(query2, conn);
                adapter.InsertCommand.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("فاکتور با موفقیت حذف شد");
            }
            catch (Exception eee)
            {
                MessageBox.Show(eee.Message);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            wrapfactornumber.Visibility = Visibility.Collapsed;
            wrapfishdasti.Visibility = Visibility.Collapsed;
            wrapids.Visibility = Visibility.Collapsed;
            wrapsubmit.Visibility = Visibility.Collapsed;
            docksum.Visibility = Visibility.Collapsed;
            dgproduct.Visibility = Visibility.Collapsed;
            wrapdetails.Visibility = Visibility.Collapsed;
            gridProducts.Visibility = Visibility.Collapsed;
            gridsearch.Visibility = Visibility.Collapsed;
            dtgmandehesab.Visibility = Visibility.Visible;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            wrapfactornumber.Visibility = Visibility.Collapsed;
            wrapfishdasti.Visibility = Visibility.Collapsed;
            wrapids.Visibility = Visibility.Collapsed;
            wrapsubmit.Visibility = Visibility.Collapsed;
            docksum.Visibility = Visibility.Collapsed;
            dgproduct.Visibility = Visibility.Collapsed;
            wrapdetails.Visibility = Visibility.Collapsed;
            gridProducts.Visibility = Visibility.Collapsed;
            gridsearch.Visibility = Visibility.Collapsed;
            dtgmandehesab.Visibility = Visibility.Visible;
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            wrapfactornumber.Visibility = Visibility.Collapsed;
            wrapfishdasti.Visibility = Visibility.Collapsed;
            wrapids.Visibility = Visibility.Collapsed;
            wrapsubmit.Visibility = Visibility.Collapsed;
            docksum.Visibility = Visibility.Collapsed;
            dgproduct.Visibility = Visibility.Collapsed;
            wrapdetails.Visibility = Visibility.Collapsed;
            gridProducts.Visibility = Visibility.Collapsed;
            gridsearch.Visibility = Visibility.Collapsed;
            dtgmandehesab.Visibility = Visibility.Visible;
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            wrapfactornumber.Visibility = Visibility.Collapsed;
            wrapfishdasti.Visibility = Visibility.Collapsed;
            wrapids.Visibility = Visibility.Collapsed;
            wrapsubmit.Visibility = Visibility.Collapsed;
            docksum.Visibility = Visibility.Collapsed;
            dgproduct.Visibility = Visibility.Collapsed;
            wrapdetails.Visibility = Visibility.Collapsed;
            gridProducts.Visibility = Visibility.Collapsed;
            gridsearch.Visibility = Visibility.Collapsed;
            dtgmandehesab.Visibility = Visibility.Visible;
        }


        private void txtanbarnumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                txtclientnumber.Focus();
            }
        }

        private void txtclientnumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                
                dgproduct.Focus();
            }
        }

        private void guess_product(int id , int place)
        {
            ProductOfFactor pf = new ProductOfFactor();
            foreach (DataRow item in dt.Rows)
            {
                if (item["ProductID"].ToString() == id.ToString())
                {
                    pf.کدکالا = Convert.ToInt32(item["ProductID"]);
                    pf.نام_کالا = item["NameProduct"].ToString();
                    pf.قیمت = Convert.ToDecimal(item["Price"]);
                    pf.تخفیف = Convert.ToDecimal("0");

                    pf.تعداد = Convert.ToInt32(TxtNumbers.Text);


                    factorkharid[place] = pf;

                }
            }
            
        }
/*
        private void dgproduct_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            
            // هر چی تلاش کردم نشد حدس بزنه از کد محصول انتخابی رو :(
            try {
                bool valid = false;
                DataGrid dt1 = sender as DataGrid;
                int columnIndex = dt1.CurrentColumn.DisplayIndex;
                TextBox targetCell = (TextBox)dt1.SelectedCells[0].Column.GetCellContent(dt1.SelectedItem);
                if (columnIndex == 0)
                {
                    
                    foreach (var item in factorkharid)
                    {
                        bool val = false;
                        
                        foreach (DataRow item2 in dt.Rows)
                        {
                            if (item.کدکالا == Convert.ToInt32(item2["ProductID"]))
                            {
                                if (item.نام_کالا == item2["NameProduct"].ToString())
                                {
                                    val = true;
                                    break;
                                }
                            }
                        }
                        
                        if (!val)
                        {
                            int index = factorkharid.IndexOf(item);
                            guess_product(Convert.ToInt32(targetCell.Text), index);
                        }
                    }
                    
                
                    foreach (var item in factorkharid)
                    {
                        foreach (var item2 in factorkharid)
                        {
                            if (item.کدکالا == item2.کدکالا && factorkharid.IndexOf(item) != factorkharid.IndexOf(item2))
                            {
                                item.تعداد = item.تعداد + item2.تعداد;
                                factorkharid.Remove(item2);
                            }
                        }
                    }
                    
                    foreach (var item in factorkharid)
                    {

                        if (Convert.ToInt32(targetCell.Text) == item.کدکالا && item.تعداد > 0)
                        {
                            valid = true;
                            item.تعداد = item.تعداد + Convert.ToInt32(TxtNumbers.Text);
                            break;
                        }
                    }
                    
                    if (!valid)
                    {
                        guess_product(Convert.ToInt32(targetCell.Text) , 1);
                    }
                }
            }catch(Exception w)
            {
                MessageBox.Show(w.Message);
            }
            //factorkharid.Add(new ProductOfFactor());


        }
*/
        private void dgproduct_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
            }
        }

        private void details_Click(object sender, RoutedEventArgs e)
        {
            lblinformation.Visibility = Visibility.Visible;
            lblhazinema.Visibility = Visibility.Collapsed;
            listhazineha.Visibility = Visibility.Collapsed;
            lblhazinemoshtari.Visibility = Visibility.Collapsed;
        }

        private void hesabsandogh_Click(object sender, RoutedEventArgs e)
        {
            lblhazinema.Visibility = Visibility.Visible;
            lblhazinemoshtari.Visibility = Visibility.Collapsed;
            listhazineha.Visibility = Visibility.Collapsed;
            lblinformation.Visibility = Visibility.Collapsed;
        }

        private void hesabkala_Click(object sender, RoutedEventArgs e)
        {
            lblhazinemoshtari.Visibility = Visibility.Collapsed;
            lblhazinema.Visibility = Visibility.Collapsed;
            lblinformation.Visibility = Visibility.Collapsed;
            listhazineha.Visibility = Visibility.Visible;

        }

        void showhazine(int id)
        {
            listhazineha.Items.Clear();
            DataTable data = new DataTable();
            data = dictcosts[id];
            foreach (DataRow item in data.Rows)
            {
           
                detailscost d = new detailscost();
                d.price = Convert.ToDecimal(item["Price"]);
                foreach (var item2 in Costs)
                {
                    if(Convert.ToInt32(item["CostID"]) == item2.CostId)
                    {
                        d.name = item2.NameCost;
                        break;
                    }
                }
                listhazineha.Items.Add(d);
            }
        }

        private void GlobalDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            decimal re = 0;
            foreach (var item in Costsdetails)
            {
                re = re + item.price;
            }
            lblcosts.Content = re.ToString();

        }

        private void btncosts_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Costsdetails.Count == 0)
                {
                    MessageBox.Show("محصولی انتخاب نشده است");
                }
                else
                {

                    pushdetailscost(Costsdetails);
                }
            }catch(Exception q)
            {
                MessageBox.Show(q.Message);
            }
        }

        private void pushdetailscost(ObservableCollection<detailscost> costdetails)
        {
            string connString = @"Data Source = 2.180.43.85; Initial Catalog = TestDB; User ID = TestUser; Password = S13791381";
            decimal resultkol = 0;
            SqlConnection conn = new SqlConnection(connString);
            SqlDataAdapter adapter = new SqlDataAdapter();
            conn.Open();
            foreach (var item in costdetails)
            {
                foreach (var item2 in Costs)
                {
                    if(item2.NameCost == item.name)
                    {
                        string query = "update dbo.ProductCosts set Price = "+item.price+" where CostID = "+item2.CostId+" and ProductID = "+IDCURRENTPRODUCT+";";
                        
                        resultkol = item.price + resultkol;
                        SqlCommand cmd = new SqlCommand(query, conn);
                        adapter.InsertCommand = new SqlCommand(query, conn);
                        adapter.InsertCommand.ExecuteNonQuery();
                        break;

                    }
                }
            }
            foreach (var item in factorkharid)
            {
                if(item.کدکالا == IDCURRENTPRODUCT)
                {
                    item.قیمت = resultkol;
                    break;
                }
            }
            
            filldictcost();
            showhazine(IDCURRENTPRODUCT);
            result();
            TotalPrice();
            CollectionViewSource.GetDefaultView(dgproduct.ItemsSource).Refresh();

        }

        private void GlobalDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void GlobalDataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void GlobalDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            decimal re = 0;
            foreach (var item in Costsdetails)
            {
                re = re + item.price;
            }
            lblcosts.Content = re.ToString();
        }

        private void GlobalDataGrid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            decimal re = 0;
            foreach (var item in Costsdetails)
            {
                re = re + item.price;
            }
            lblcosts.Content = re.ToString();

        }

        private void GlobalDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            decimal re = 0;
            foreach (var item in Costsdetails)
            {
                re = re + item.price;
            }
            lblcosts.Content = re.ToString();
        }

        private void GlobalDataGrid_ColumnDisplayIndexChanged(object sender, DataGridColumnEventArgs e)
        {
            decimal re = 0;
            foreach (var item in Costsdetails)
            {
                re = re + item.price;
            }
            lblcosts.Content = re.ToString();
        }
    }
}
