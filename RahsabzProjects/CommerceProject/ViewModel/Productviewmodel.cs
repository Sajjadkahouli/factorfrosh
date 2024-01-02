using CommerceProject.Moudel;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

namespace CommerceProject.ViewMoudel
{

   
    class Productviewmodel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }
        
        private ObservableCollection<Product> Fish = new ObservableCollection<Product>
        {

        };
        public ObservableCollection<Product> Product
        {
            get
            {
                return Product;
            }
            set
            {
                Product = value;
            }
        }

        private Product SelectedProduct;

        public Product SelectedProduct1
        {
            get
            {
                return SelectedProduct1;
            }

            set
            {
                SelectedProduct = value;
                OnPropertyChanged("SelectedProduct");
            }
        }

        internal ObservableCollection<Product> Fish1 { get => Fish; set => Fish = value; }
        public DataSet DtSet { get; private set; }

        

    }
}
