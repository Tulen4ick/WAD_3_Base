using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RWP3
{
    public class CarBrand: Car_Brand
    {
        public string Name;
        public string Model;
        public int Power;
        public int Speed;
        public string type;
        public string BrandName
        {
            get
            {
                return Name;
            }
            set
            {
                Name = value;
            }
        }
        public string ModelName
        {
            get
            {
                return Model;
            }
            set
            {
                Model = value;
            }
        }

        public int Horsepower
        {
            get
            {
                return Power;
            }
            set
            {
                Power = value;
            }
        }
        public int Maxspeed
        {
            get
            {
                return Speed;
            }
            set
            {
                Speed = value;
            }
        }

        public string Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }
        public CarBrand(string N, string M, int P, int MS, string T)
        {
            Name = N;
            Model = M;
            Power = P;
            Speed = MS;
            Type = T;
        }

        public CarBrand()
        {
            Name = "";
            Model = "";
            Power = 0;
            Speed = 0;
            Type = "";
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Power.GetHashCode();
                hash = hash * 23 + (ModelName?.GetHashCode() ?? 0);
                hash = hash * 23 + (BrandName?.GetHashCode() ?? 0);
                hash = hash * 23 + Speed.GetHashCode();
                hash = hash * 23 + (Type?.GetHashCode() ?? 0);
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            CarBrand cb = obj as CarBrand;
            return this.Power == cb.Power && this.ModelName == cb.ModelName && this.BrandName == cb.BrandName && this.Speed == cb.Speed && this.Type == cb.Type;
        }
    }
}
