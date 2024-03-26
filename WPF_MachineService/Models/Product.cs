using System;
using System.Collections.Generic;

namespace WPF_MachineService.Models
{
    public partial class Product
    {
        public int ProId { get; set; }
        public string? ProName { get; set; }
        public string? ProDescription { get; set; }
        public string? Ingredient { get; set; }
        public string? Brand { get; set; }
    }
}
