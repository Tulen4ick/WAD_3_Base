using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RWP3
{
    public class TruckModel: Car_Model
    {
        public int VanVolume;
        public int CountOfWheels;
        public int registerNumber;
        public TruckModel(int RN, int CoW, int VV)
        {
            registerNumber = RN;
            CountOfWheels = CoW;
            VanVolume = VV;
        }
        public TruckModel()
        {
            registerNumber = 0;
            CountOfWheels = 0;
            VanVolume = 0;
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
