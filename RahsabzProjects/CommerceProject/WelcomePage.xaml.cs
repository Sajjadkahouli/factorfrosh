using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CommerceProject
{
    /// <summary>
    /// Interaction logic for WelcomePage.xaml
    /// </summary>
    public partial class WelcomePage : Window
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(comboname.Text == "")
            {
                MessageBox.Show("نام کاربری را وارد کنید");
            }else if(txtpass.Password == "")
            {
                MessageBox.Show("پسورد را وارد کنید");

            }else if(txtpass.Password == "1" && Convert.ToInt32(comboname.Text) > 0 && Convert.ToInt32(comboname.Text) < 3)
            {
                
                
                this.Close();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            System.Windows.Application.Current.Shutdown();

        }
    }
}
