using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace RWP3
{
    public partial class Form1 : Form
    {
        List<CarBrand> cars = new List<CarBrand>();
        static Dictionary<CarBrand, List<Car_Model>> data = new Dictionary<CarBrand, List<Car_Model>>();
       /* Loader loader = new Loader();*/
        Thread t2;
        int process;

        public Form1()
        {
            InitializeComponent();
            dataGridView2.Visible = false;
            ProcessOfLoad.Visible = false;
            bindingCars.DataSource = cars;
            dataGridView1.DataSource = bindingCars;
            dataGridView1.Columns["Type"].Visible = false;
            DataGridViewComboBoxColumn cmbCol = new DataGridViewComboBoxColumn();
            cmbCol.HeaderText = "Type";
            cmbCol.Name = "Tip";
            cmbCol.Items.AddRange("Легковой", "Грузовой", "Танк");
            dataGridView1.Columns.Add(cmbCol);
            dataGridView1.Columns["Tip"].DisplayIndex = dataGridView1.ColumnCount - 1;
        }
        public static Car_Model CarModelToCar_Model(CarModel cm)
        {
            return (Car_Model)cm;
        }
        public static Car_Model TruckModelToCar_Model(TruckModel cm)
        {
            return (Car_Model)cm;
        }
        public static Car_Model TankModelToCar_Model(TankModel cm)
        {
            return (Car_Model)cm;
        }

        public void DrawTable2()
        {
            dataGridView2.Rows.Clear();
            if ((data.Keys.Count > 0) && (dataGridView1.SelectedRows.Count > 0))
            {
                if (data.Keys.Contains((CarBrand)dataGridView1.SelectedRows[0].Tag))
                {
                    for (int i = 0; i < data[(CarBrand)dataGridView1.SelectedRows[0].Tag].Count; ++i)
                    {
                        if (data[(CarBrand)dataGridView1.SelectedRows[0].Tag][i] is CarModel cm)
                        {
                            int rowId = dataGridView2.Rows.Add();
                            dataGridView2.Rows[rowId].Cells[0].Value = cm.RegisterNumber;
                            dataGridView2.Rows[rowId].Cells[1].Value = cm.MultyMediaName;
                            dataGridView2.Rows[rowId].Cells[2].Value = cm.NumerOfAirbags;
                        }
                        else if (data[(CarBrand)dataGridView1.SelectedRows[0].Tag][i] is TruckModel tm)
                        {
                            int rowId = dataGridView2.Rows.Add();
                            dataGridView2.Rows[rowId].Cells[0].Value = tm.RegisterNumber;
                            dataGridView2.Rows[rowId].Cells[1].Value = tm.CountOfWheels;
                            dataGridView2.Rows[rowId].Cells[2].Value = tm.VanVolume;
                        }
                        else if (data[(CarBrand)dataGridView1.SelectedRows[0].Tag][i] is TankModel tankm)
                        {
                            int rowId = dataGridView2.Rows.Add();
                            dataGridView2.Rows[rowId].Cells[0].Value = tankm.RegisterNumber;
                            dataGridView2.Rows[rowId].Cells[1].Value = tankm.CrewCount;
                            dataGridView2.Rows[rowId].Cells[2].Value = tankm.AmmoType;
                        }
                    }
                    dataGridView2.Visible = true;
                    TimerForLoad.Stop();
                }
            }
        }
        static async void SendBrand(int port, CarBrand Tag)
        {
            byte[] bytes = new byte[8000];
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);
            try
            {
                Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                /*sender.Connect(ipEndPoint);*/
                await sender.ConnectAsync(ipEndPoint);
                XmlSerializer xmlSerializerModels = new XmlSerializer(typeof(CarBrand));
                XmlDocument xmlDocument = new XmlDocument();
                using (MemoryStream ms = new MemoryStream())
                {
                    xmlSerializerModels.Serialize(ms, Tag);

                    ms.Position = 0;
                    xmlDocument.Load(ms);
                }
                string xmlData;
                using (StringWriter stringWriter = new StringWriter())
                {
                    xmlDocument.Save(stringWriter);
                    xmlData = stringWriter.ToString();
                }
                byte[] msg = Encoding.UTF8.GetBytes(xmlData);
                int bytesSent = sender.Send(msg);
                bytesSent = sender.Receive(bytes);
                string str = null;
                str += Encoding.UTF8.GetString(bytes, 0, bytesSent);
                XmlDocument xmlbrand = new XmlDocument();
                xmlbrand.LoadXml(str);
                StringReader stringReader = new StringReader(xmlbrand.OuterXml);
                if (Tag.Type == "Легковой")
                {
                    XmlSerializer xmlSerializerCars = new XmlSerializer(typeof(List<CarModel>));
                    List<CarModel> carModels = xmlSerializerCars.Deserialize(stringReader) as List<CarModel>;
                    List<Car_Model> cm = carModels.ConvertAll(new Converter<CarModel, Car_Model>(CarModelToCar_Model));
                    data.Add(Tag, cm);
                }
                else if (Tag.Type == "Грузовой")
                {
                    XmlSerializer xmlSerializerTrucks = new XmlSerializer(typeof(List<TruckModel>));
                    List<TruckModel> carModels = xmlSerializerTrucks.Deserialize(stringReader) as List<TruckModel>;
                    List<Car_Model> cm = carModels.ConvertAll(new Converter<TruckModel, Car_Model>(TruckModelToCar_Model));
                    data.Add(Tag, cm);
                }
                else if (Tag.Type == "Танк")
                {
                    XmlSerializer xmlSerializerTanks = new XmlSerializer(typeof(List<TankModel>));
                    List<TankModel> carModels = xmlSerializerTanks.Deserialize(stringReader) as List<TankModel>;
                    List<Car_Model> cm = carModels.ConvertAll(new Converter<TankModel, Car_Model>(TankModelToCar_Model));
                    data.Add(Tag, cm);
                }
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            

        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if(e.RowIndex > -1)
            {
                var value = dataGridView1.Rows[e.RowIndex].Cells[dataGridView1.ColumnCount - 1].Value;
                if (value != null)
                {
                    value = value.ToString();
                    if (value.ToString() == "Легковой")
                    {
                        dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.SeaGreen;
                    }
                    else if (value.ToString() == "Грузовой")
                    {
                        dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Moccasin;
                    }
                    else if(value.ToString() == "Танк")
                    {
                        dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Khaki;
                    }
                    else
                    {
                        dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                    }
                }
            }
                
            
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex > -1)
            {
                int count = dataGridView1.Rows[e.RowIndex].Cells.Count;
                var value = dataGridView1.Rows[e.RowIndex].Cells[count - 1].Value;
                if(value != null)
                {
                    value = value.ToString();
                    dataGridView1.Rows[e.RowIndex].Tag = new CarBrand((string)dataGridView1.Rows[e.RowIndex].Cells["BrandName"].Value, (string)dataGridView1.Rows[e.RowIndex].Cells["ModelName"].Value, (int)dataGridView1.Rows[e.RowIndex].Cells["Horsepower"].Value, (int)dataGridView1.Rows[e.RowIndex].Cells["Maxspeed"].Value, (string)dataGridView1.Rows[e.RowIndex].Cells["Type"].Value);
                }
                dataGridView1.Rows[e.RowIndex].Cells["Type"].Value = dataGridView1.Rows[e.RowIndex].Cells["Tip"].Value;

            }

        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                if ((dataGridView1.SelectedRows[0].Cells[0].Value != null) && (dataGridView1.SelectedRows[0].Cells[0].Value != "") && (dataGridView1.SelectedRows[0].Cells[1].Value != null) && (dataGridView1.SelectedRows[0].Cells[1].Value != "") && (dataGridView1.SelectedRows[0].Cells["Tip"].Value != null) && (dataGridView1.SelectedRows[0].Cells["Tip"].Value != ""))
                {
                    TimerForLoad.Stop();
                    if (dataGridView1.SelectedRows[0].Cells["Tip"].Value.ToString() == "Легковой")
                    {
                        dataGridView2.Visible = false;
                        dataGridView2.Columns[1].HeaderText = "Название мультимедиа";
                        dataGridView2.Columns[2].HeaderText = "Количество подушек безопасности";
                        /*ProcessOfLoad.Visible = true;
                        ProcessOfLoad.Value = 0;*/
                        process = 0;
                        TimerForLoad.Start();
                        /*if (!data.ContainsKey((CarBrand)dataGridView1.SelectedRows[0].Tag))
                        {
                            t2 = new Thread(delegate ()
                            {
                                try
                                {
                                    SendBrand(11000, (CarBrand)dataGridView1.SelectedRows[0].Tag);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.ToString());
                                }
                                finally
                                {
                                    Console.ReadLine();
                                }
                                *//*data = loader.Load((CarBrand)dataGridView1.SelectedRows[0].Tag);*//*
                                process = 100;
                            });
                            t2.Start();
                        }
                        else
                        {
                            process = 100;
                        }*/
                        t2 = new Thread(delegate ()
                        {
                            try
                            {
                                SendBrand(11000, (CarBrand)dataGridView1.SelectedRows[0].Tag);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                            finally
                            {
                                Console.ReadLine();
                            }
                            /*data = loader.Load((CarBrand)dataGridView1.SelectedRows[0].Tag);*/
                            process = 100;
                        });
                        t2.Start();
                    }
                    else
                    if (dataGridView1.SelectedRows[0].Cells["Tip"].Value.ToString() == "Грузовой")
                    {
                        dataGridView2.Visible = false;
                        dataGridView2.Columns[1].HeaderText = "Количество колёс";
                        dataGridView2.Columns[2].HeaderText = "Объём кузова";
                        /*ProcessOfLoad.Visible = true;
                        ProcessOfLoad.Value = 0;*/
                        process = 0;
                        TimerForLoad.Start();
                        /*if (!data.ContainsKey((CarBrand)dataGridView1.SelectedRows[0].Tag))
                        {
                            t2 = new Thread(delegate ()
                            {
                                try
                                {
                                    SendBrand(11000, (CarBrand)dataGridView1.SelectedRows[0].Tag);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.ToString());
                                }
                                finally
                                {
                                    Console.ReadLine();
                                }
                                *//*data = loader.Load((CarBrand)dataGridView1.SelectedRows[0].Tag);*//*
                                process = 100;
                            });
                            t2.Start();
                        }
                        else
                        {
                            process = 100;
                        }*/
                        t2 = new Thread(delegate ()
                        {
                            try
                            {
                                SendBrand(11000, (CarBrand)dataGridView1.SelectedRows[0].Tag);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                            finally
                            {
                                Console.ReadLine();
                            }
                            /*data = loader.Load((CarBrand)dataGridView1.SelectedRows[0].Tag);*/
                            process = 100;
                        });
                        t2.Start();
                    }
                    else 
                    if(dataGridView1.SelectedRows[0].Cells["Tip"].Value.ToString() == "Танк")
                    {
                        dataGridView2.Visible = false;
                        dataGridView2.Columns[1].HeaderText = "Кол-во человек в экипаже";
                        dataGridView2.Columns[2].HeaderText = "Тип боеприпасов";
                        /*ProcessOfLoad.Visible = true;
                        ProcessOfLoad.Value = 0;*/
                        process = 0;
                        TimerForLoad.Start();
                        /*if (!data.ContainsKey((CarBrand)dataGridView1.SelectedRows[0].Tag))
                        {
                            t2 = new Thread(delegate ()
                            {
                                try
                                {
                                    SendBrand(11000, (CarBrand)dataGridView1.SelectedRows[0].Tag);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.ToString());
                                }
                                finally
                                {
                                    Console.ReadLine();
                                }
                                *//*data = loader.Load((CarBrand)dataGridView1.SelectedRows[0].Tag);*//*
                                process = 100;
                            });
                            t2.Start();
                        }
                        else
                        {
                            process = 100;
                        }*/
                        t2 = new Thread(delegate ()
                        {
                            try
                            {
                                SendBrand(11000, (CarBrand)dataGridView1.SelectedRows[0].Tag);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                            finally
                            {
                                Console.ReadLine();
                            }
                            /*data = loader.Load((CarBrand)dataGridView1.SelectedRows[0].Tag);*/
                            process = 100;
                        });
                        t2.Start();
                    }
                }
                else
                {
                    TimerForLoad.Stop();
                    dataGridView2.Visible = false;
                    ProcessOfLoad.Visible = false;
                }
            }
            
        }

        private void TimerForLoad_Tick(object sender, EventArgs e)
        {
            /*int process = loader.getProcess();*/
            /*ProcessOfLoad.Value = process;*/
            if (process == 100)
            {
                DrawTable2();
            }
        }

        public void сохранитьСписокToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveCars = new SaveFileDialog();
            saveCars.Filter = "Extensible Markup files (*.xml)|*.xml|All files(*.*)|*.*";
            saveCars.FilterIndex = 0;
            saveCars.RestoreDirectory = true;
            saveCars.InitialDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            saveCars.Title = "Сохранение списка машин";
            if (saveCars.ShowDialog() == DialogResult.OK)
            {
                if (saveCars.FileName != "")
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<CarBrand>));
                    using(FileStream fs = new FileStream(saveCars.FileName, FileMode.OpenOrCreate))
                    {
                        xmlSerializer.Serialize(fs, cars);
                    }
                }
                else
                {
                    MessageBox.Show("Вы ввели пустое имя файла", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void загрузитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openCars = new OpenFileDialog();
            openCars.Filter = "Extensible Markup files (*.xml)|*.xml|All files(*.*)|*.*";
            openCars.FilterIndex = 0;
            openCars.RestoreDirectory = true;
            openCars.InitialDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            openCars.Title = "Загрузка списка машин";
            if (openCars.ShowDialog() == DialogResult.OK)
            {
                if(openCars.FileName != "")
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<CarBrand>));
                    using (FileStream fs = new FileStream(openCars.FileName, FileMode.OpenOrCreate))
                    {
                        cars = xmlSerializer.Deserialize(fs) as List<CarBrand>;
                        bindingCars.DataSource = null;
                        bindingCars.DataSource = cars;
                        dataGridView1.DataSource = bindingCars;
                        dataGridView1.Columns["Type"].Visible = false;
                        dataGridView1.Columns["Tip"].DisplayIndex = dataGridView1.ColumnCount - 1;
                        for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                        {
                            dataGridView1.Rows[i].Cells["Tip"].Value = dataGridView1.Rows[i].Cells["Type"].Value;
                            dataGridView1.Rows[i].Tag = new CarBrand((string)dataGridView1.Rows[i].Cells["BrandName"].Value, (string)dataGridView1.Rows[i].Cells["ModelName"].Value, (int)dataGridView1.Rows[i].Cells["Horsepower"].Value, (int)dataGridView1.Rows[i].Cells["Maxspeed"].Value, (string)dataGridView1.Rows[i].Cells["Type"].Value);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Вы ввели пустое имя файла", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
