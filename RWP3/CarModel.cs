using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RWP3
{
    public class CarModel: Car_Model
    {
        public string MultyMediaName;
        public int NumerOfAirbags;
        public int registerNumber;
        public CarModel(int RN, string Mmn, int NoA)
        {
            registerNumber = RN;
            MultyMediaName = Mmn;
            NumerOfAirbags = NoA;
        }
        public CarModel()
        {
            registerNumber = 0;
            MultyMediaName = "";
            NumerOfAirbags = 0;
        }
        public int RegisterNumber
        {
            get
            {
                return this.registerNumber;
            }
            set
            {
                registerNumber = value;
            }
        }
    }
}
