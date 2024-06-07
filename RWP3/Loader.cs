using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RWP3
{
    internal class Loader
    {
        public int process;
        public Dictionary<CarBrand, List<Car_Model>> data = new Dictionary<CarBrand, List<Car_Model>>();
        public Dictionary<CarBrand, List<Car_Model>> Load(CarBrand cb)
        {
            process = 0;
            if (!data.ContainsKey(cb))
            {
                data.Add(cb, new List<Car_Model>());
                Random rnd = new Random();
                int count = rnd.Next(10, 20);
                for (int i = 0; i < count; ++i)
                {
                    if (cb.Type == "Легковой")
                    {
                        string[] MultyMedia = { "Pioneer", "Alpine", "Sony", "Prology"};
                        CarModel cm = new CarModel(rnd.Next(100000, 999999), MultyMedia[rnd.Next(0, 3)], rnd.Next(1, 3));
                        data[cb].Add(cm);
                    }else if(cb.Type == "Грузовой")
                    {
                        TruckModel tm = new TruckModel(rnd.Next(100000, 999999), rnd.Next(4, 20), rnd.Next(100, 1000));
                        data[cb].Add(tm);
                    }else if(cb.Type == "Танк")
                    {
                        string[] Ammo = { "Cumulative", "Armor-piercing", "Fragmentation" };
                        TankModel tm = new TankModel(rnd.Next(100000, 999999), rnd.Next(3, 8), Ammo[rnd.Next(0, 2)]);
                        data[cb].Add(tm);
                    }
                    process = (int)(((double)(i+1) / count) * 100);
                    Thread.Sleep(rnd.Next(0, 500));
                }
            }
            else
            {
                process = 100;
            }
            return data;
        }
        public int getProcess()
        {
            return process;
        }
    }
}
