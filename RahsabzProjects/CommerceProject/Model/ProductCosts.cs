using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommerceProject.Model
{
    class ProductCosts
    {

        public int ProductCostId { get; set; }
        public int CostId { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }

    }
}
