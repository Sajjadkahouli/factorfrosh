using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CommerceProject.Moudel
{
    class Product
    {
        public int id
        {
            set;
            get;
        }
        public string name
        {
            set;
            get;
        }

        public int Value
        {
            set;
            get;
        }
        public int Number
        {
            set;
            get;
        }
        
        public DataSet DtSet { get => dtSet; set => dtSet = value; }

        private DataSet dtSet;

       
    }
}
