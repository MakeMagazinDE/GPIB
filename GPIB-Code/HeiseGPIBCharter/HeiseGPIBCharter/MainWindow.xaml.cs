using NationalInstruments.NI4882;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HeiseGPIBCharter
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NationalInstruments.NI4882.Board myHostController;
        NationalInstruments.NI4882.Device mySolartron;
        NationalInstruments.NI4882.Device myHP;


        List<double> dataX;
        List<double> dataY;

        public void conjureHPTargetVoltage(int what)
        {
            String aStr = "VSET 1,";
            aStr = aStr + what.ToString();
            aStr = aStr + ";ISET 1,0.5;\n";
            myHP.Write(aStr);
        }

        public double getSolartronDataPoint()
        {
            mySolartron.Write("G\n");
            Thread.Sleep(900);
            try
            {
                String myStr = "";
                try
                {
                    while (1 == 1)
                    {
                        mySolartron.IOTimeout = TimeoutValue.T100ms;
                        String x = mySolartron.ReadString(1);
                        myStr = myStr + x;
                    }
                }
                catch (Exception) { }
                Console.WriteLine(myStr);
                myStr = myStr.Replace("00.", "0,");
                myStr = myStr.Replace(".", ",");
                double aDouble = Convert.ToDouble(myStr);
                Console.WriteLine(aDouble);
                return aDouble;
            }
            catch (Exception) { }
            return -200;
        }

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                myHostController = new NationalInstruments.NI4882.Board(0);
                myHP = new Device(0, 5);
                mySolartron = new Device(0, 12);
                mySolartron.Write(data: "M0R3I3N1T0\n");
                Console.WriteLine("GPIB-Initialisierung OK!");

                dataX = new List<double>();
                dataY = new List<double>();
                WpfPlot1.Plot.Add.Scatter(dataX, dataY);
                WpfPlot1.Refresh();

                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    while (1 == 1)
                    {
                        for (int i = 7; i < 20; i++)
                        {
                            conjureHPTargetVoltage(i);
                            Thread.Sleep(50);
                            for (int j = 0; j < 20; j++)
                            {
                                double whatsit = getSolartronDataPoint();
                                if (whatsit > -2) {
                                    dataY.Add(whatsit);
                                    dataX.Add(i);
                                    Application.Current.Dispatcher.Invoke(
                                    () =>
                                    {
                                        WpfPlot1.Reset();
                                        WpfPlot1.Plot.Add.Scatter(dataX, dataY);
                                        WpfPlot1.Refresh();
                                        
                                    });
                                }
                            }
                        }
                    }


                }).Start();


                

            }
            catch (NationalInstruments.NI4882.GpibException e)
            {
                Console.WriteLine("FEHLER: GPIB-Problem!");
            }
        }
    }
}
