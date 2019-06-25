using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.IO;
using WebNeuralNetwork.ANN;
using System.ComponentModel;
using System.Threading;

namespace WebNeuralNetwork
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Packet> ans = new List<Packet>();
        private readonly BackgroundWorker _bw = new BackgroundWorker();
        NeuralNetwork net;
        double[,] Dane;
        int IleWyjsc = 3;
        private RoutedEventArgs e;
        private readonly object sender;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)//wczytaj plik
        {
            string line;

            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".txt";
            dlg.Filter = "txt Files (*.txt)|*.txt";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            string filename = "";

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                filename = dlg.FileName;
            }

            // Read the file and display it line by line.
            using (StreamReader file = new StreamReader(filename))
            {
                Dane = new double[1840, 77];
                int j = 0;
                while ((line = file.ReadLine()) != null)
                {
                    char[] delimiters = new char[] { '\t' };
                    string[] parts = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < 77; i++)
                    {
                        Dane[j, i] = Convert.ToDouble(parts[i]);
                    }
                    j++;
                }
                file.Close();

            }

            /////////////////////////////////////////////////  wprowadzene danych do nauki, wejścia
            int outputId = 0;
            for (int j = 0; j < 1840; j = j + 1)
            {
                if (j < 10)
                {
                    double[] inputs = new double[77];
                    double[] outputs = new double[IleWyjsc];
                    int licznik = 1;
                    for (int i = 0; i < 76; i++)
                    {
                        inputs[licznik] = Dane[j, i];
                        licznik++;
                    }
                    inputs[0] = 1;
                    for (int k = 0; k < IleWyjsc; k++)
                        outputs[k] = -1;

                    if (Dane[j, 76] == 1)
                        outputs[0] = 1;
                    else if (Dane[j, 76] == -1)
                        outputs[1] = 1;
                    else
                        outputs[2] = 1;

                    Packet p = new Packet(inputs, outputs, outputId);
                    ans.Add(p);
                    outputId++;
                }
            }

            _bw.WorkerReportsProgress = true;
            _bw.WorkerSupportsCancellation = true;
            _bw.DoWork += FakeLerning;
            _bw.ProgressChanged += bw_ProgressChanged;
            _bw.RunWorkerCompleted += bw_RunWorkerCompleted;

        }
        private void Button_Click()
        {

          
            string line;
          
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".txt";
            dlg.Filter = "txt Files (*.txt)|*.txt";
            
            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            string filename = "";

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                filename = dlg.FileName;
            }

            // Read the file and display it line by line.
            using (StreamReader file = new StreamReader(filename))
            {
                Dane = new double[1840, 77];
                int j = 0;
                while ((line = file.ReadLine()) != null)
                {
                    char[] delimiters = new char[] { '\t' };
                    string[] parts = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < 77; i++)
                    {
                        Dane[j, i] = Convert.ToDouble(parts[i]);
                    }
                    j++;
                }            
                file.Close();
              
            }
          
            /////////////////////////////////////////////////  wprowadzene danych do nauki, wejścia
            int outputId = 0;
            for (int j = 0; j < 1840; j=j+1)
            {
               if (j<10)
                {
                    double[] inputs = new double[77];
                    double[] outputs = new double[IleWyjsc];
                    int licznik = 1;
                    for (int i = 0; i < 76; i++)
                    {
                        inputs[licznik] = Dane[j, i];
                        licznik++;
                    }
                    inputs[0] = 1;
                    for (int k = 0; k < IleWyjsc; k++)
                        outputs[k] = -1;

                    if (Dane[j, 76] == 1)
                        outputs[0] = 1;
                    else if (Dane[j, 76] == -1)
                        outputs[1] = 1;
                    else
                        outputs[2] = 1;

                    Packet p = new Packet(inputs, outputs, outputId);
                    ans.Add(p);
                    outputId++;
                }      
            }

            _bw.WorkerReportsProgress = true;
            _bw.WorkerSupportsCancellation = true;
            _bw.DoWork += FakeLerning;
            _bw.ProgressChanged += bw_ProgressChanged;
            _bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            //////////////
            StartstopLerning();
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            CalculationTextBlock.Text = ("Step: " + e.ProgressPercentage + " " + e.UserState);
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                CalculationTextBlock.Text = "Canceled!";
            }
            else if (e.Error != null)
            {
                CalculationTextBlock.Text = ("Error: " + e.Error.Message);
            }
      
           

        }

        private void FakeLerning(object sender, DoWorkEventArgs e)
        {
            var startTime = DateTime.Now;
            var worker = sender as BackgroundWorker;
            if (worker == null) return;

            worker.ReportProgress(1, "Starting");
            net = new NeuralNetwork(77, IleWyjsc, 1, 15, 0.005);
            net.ClearData();
            net.AddData(ans.ToArray());
            net.RandomWeights();
            //net.LoadSynapse("e:\\synapsy.txt");
            net.Ucz(ref worker);
            var endTime = DateTime.Now;
            var span = endTime.Subtract(startTime);
            worker.ReportProgress(6, "Done. " + span.TotalSeconds + " seconds. " + net.Krok + " steps.");
            net.SaveSynapse("e:\\synapsy.txt");
        }

        private void StartstopLerning(object sender, RoutedEventArgs e)
        {
            if (LerningStartStopButton.Content.ToString() == "Start")
            {
                if (_bw.IsBusy) return;
                _bw.RunWorkerAsync();
                LerningStartStopButton.Content = "Stop";

                return;
            }
            if (_bw.IsBusy) return;
            _bw.CancelAsync();

            LerningStartStopButton.Content = "Start";



        }
        private void StartstopLerning()
        {
            if (LerningStartStopButton.Content.ToString() == "Start")
            {
                if (_bw.IsBusy) return;
                _bw.RunWorkerAsync();
                LerningStartStopButton.Content = "Stop";
               
                return;
            }
            if (_bw.IsBusy) return;
            _bw.CancelAsync();
          
            LerningStartStopButton.Content = "Start";
            
           

        }

        private void testPrzycisk(object sender, RoutedEventArgs e)
        
        {
            int poprawne = 0;
            int niepoprawne = 0;

            for (int i = 0; i < 1840; i++)
              //  if  (i==10)
             {
                    double[] wejscia = new double[77];
                    int licznik = 1;
                    for (int j = 0; j < 76; j++)
                    {
                        wejscia[licznik] = Dane[i, j];
                        licznik++;  
                    }
                    wejscia[0] = 1;

                    net.Inputs = wejscia;
                    var packet = new Packet(wejscia, new double[net.OutputsCount()], -1);
                    double[] wyjscia = new double[IleWyjsc];
                    wyjscia = net.Ask(packet);
                    if (CzyKupil(wyjscia) == 0 && Dane[i, 76] == 1)
                        poprawne++;
                    else if (CzyKupil(wyjscia) == 1 && Dane[i, 76] == -1)
                        poprawne++;
                    else if (CzyKupil(wyjscia) == 2 && Dane[i, 76] == 0)
                        poprawne++;
                    else
                        niepoprawne++;
                }

            CalculationTextBlock.Text = "Poprawne: " + poprawne.ToString() + ", niepoprawne: " + niepoprawne.ToString();
            //odp1.Content = wyjscia[0];
            //odp2.Content = wyjscia[1];
            //odp3.Content = wyjscia[2];
          
        }

        private void Test()
        {
          // while
            int poprawne = 0;
            int niepoprawne = 0;

            for (int i = 0; i < 1840; i++)
            //  if  (i==10)
            {
                double[] wejscia = new double[77];
                int licznik = 1;
                for (int j = 0; j < 76; j++)
                {
                    wejscia[licznik] = Dane[i, j];
                    licznik++;
                }
                wejscia[0] = 1;

                net.Inputs = wejscia;
                var packet = new Packet(wejscia, new double[net.OutputsCount()], -1);
                double[] wyjscia = new double[IleWyjsc];
                wyjscia = net.Ask(packet);
                if (CzyKupil(wyjscia) == 0 && Dane[i, 76] == 1)
                    poprawne++;
                else if (CzyKupil(wyjscia) == 1 && Dane[i, 76] == -1)
                    poprawne++;
                else if (CzyKupil(wyjscia) == 2 && Dane[i, 76] == 0)
                    poprawne++;
                else
                    niepoprawne++;
            }

            CalculationTextBlock.Text = "Poprawne: " + poprawne.ToString() + ", niepoprawne: " + niepoprawne.ToString();
            PoprawneTextBox.Text += "Poprawne: " + poprawne.ToString() + ", niepoprawne: " + niepoprawne.ToString();
            //odp1.Content = wyjscia[0];
            //odp2.Content = wyjscia[1];
            //odp3.Content = wyjscia[2];

        }

        private int CzyKupil(double[] wyjscia)
        {
            double maxValue = wyjscia.Max();
            int maxIndex = wyjscia.ToList().IndexOf(maxValue);
            
            if (maxIndex == 0 && maxValue > 0.6)
                return 0;
            else if (maxIndex == 1 && maxValue > 0.6)
                return 1;
            else if (maxIndex == 2 && maxValue > 0.6)
                return 2;
            else
                return 3;
        }



        private void button1_Click(object sender, RoutedEventArgs e)
        {
            
            Button_Click(sender,e);
         StartstopLerning();
            
            Button_Click(sender, e);
            StartstopLerning();
            Test();
           

        }
    }
}
