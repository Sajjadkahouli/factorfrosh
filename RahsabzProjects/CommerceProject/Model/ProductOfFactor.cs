using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Models
{
    public class ProductOfFactor
    {
        public int کدکالا { get ; set ; }
        public int تعداد { get ; set ; }
        
        public string نام_کالا { get ; set ; }
        public decimal قیمت { get ; set ; }

        public decimal تخفیف { get; set; }
        public decimal قیمت_کل { get; set; }
    }
}
