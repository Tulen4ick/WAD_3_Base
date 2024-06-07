using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RWP3
{
    public interface Car_Brand
    {
        string BrandName { get; set; }
        string ModelName { get; set; }
        int Horsepower { get; set; }
        int Maxspeed { get; set; }
        string Type { get; set; }
    }
}
