using System;
using System.Collections.Generic;
using System.Text;

namespace BarcodeScanSF
{
    public class Product
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductCode { get; set; }
        public string Id { get; set; }
        public bool IsVisible { get; set; }
    }
}
