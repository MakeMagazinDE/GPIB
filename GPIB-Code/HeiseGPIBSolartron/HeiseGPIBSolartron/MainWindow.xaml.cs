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

namespace HeiseGPIBSolartron
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NationalInstruments.NI4882.Board myHostController;
        NationalInstruments.NI4882.Device mySolartron;
        NationalInstruments.NI4882.Device myHP;
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                myHostController = new NationalInstruments.NI4882.Board(0);
                myHP = new Device(0, 5);
                myHP.Write("VSET 1,9;ISET 1,0.5;\n");
                mySolartron = new Device(0, 12);
                mySolartron.Write(data: "M0R3I3N1T0\n");
                Console.WriteLine("GPIB-Initialisierung OK!");
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
                }
                catch (Exception e)
                {
                    Console.WriteLine("SolarTron Fehler: " + e.ToString());
                }
            }
            catch (NationalInstruments.NI4882.GpibException e)
            {
                Console.WriteLine("FEHLER: GPIB-Problem!");
            }
        }
    }
}
