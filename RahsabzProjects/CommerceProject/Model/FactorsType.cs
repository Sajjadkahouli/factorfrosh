using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Models
{
    class FactorsType
    {
        public int شماره_فاکتور { get; set; }
        public string نوع_فاکتور { get; set; }

        public decimal قیمت_کل { get; set; }
        public decimal تخفیف_کل { get; set; }

        public string زمان { get; set; }

    }
}
