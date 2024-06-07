using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RWP3
{
    public class TankModel: Car_Model
    {
        public int CrewCount;
        public string AmmoType;
        public int registerNumber;
        public TankModel(int RN, int CC, string AT)
        {
            registerNumber = RN;
            CrewCount = CC;
            AmmoType = AT;
        }

        public TankModel()
        {
            registerNumber = 0;
            CrewCount = 0;
            AmmoType = "";
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
