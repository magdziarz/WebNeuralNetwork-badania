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
        int proba = 1;
        double[] wynik = new double[1839];

        int warstwy;
        int neurony;
        double lerningfact;
        double k = 0.8;
      


        public MainWindow()
        {
            InitializeComponent();
            TXBwarstwa.IsEnabled = false;
            TXBneuron.IsEnabled = false;
            TXBwspucz.IsEnabled = false;
            TXBwaga.IsEnabled = false;
            LerningStartStopButton.Visibility = Visibility.Hidden;
            test.Visibility = Visibility.Hidden;
            BPortfel.Visibility = Visibility.Hidden;
            ButtonWczytaj.Visibility = Visibility.Hidden;
            koniec.Visibility = Visibility.Hidden;
            poczatek.Visibility = Visibility.Hidden;
            radioButtonlosowe.Visibility = Visibility.Hidden;
            radioButtonselekcja.Visibility = Visibility.Hidden;

        }

        private void Button_Click(object sender, RoutedEventArgs e)//wczytaj plik
        {
            if (radioButton1.IsChecked == true)
            {
                warstwy = 1;
                neurony = 30;
                lerningfact = 0.005;
                k = 0.8;
            }
            if (radioButton4.IsChecked == true)
            {
                warstwy = 3;
                neurony = 15;
                lerningfact = 0.005;
                k = 0.4;
            }
            if (radioButton10.IsChecked == true)
            {
                warstwy = 2;
                neurony = 10;
                lerningfact = 0.01;
                k = 0.6;
            }
            if (radioButtonSam.IsChecked == true)
            {
                warstwy = int.Parse(TXBwarstwa.Text);
                neurony = int.Parse(TXBneuron.Text);
                lerningfact = double.Parse(TXBwspucz.Text);
                k = double.Parse(TXBwaga.Text);
            }

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
            // filename = @"C:\Users\Magdalena\Desktop\Praca inżynierska MD\dane251117proba.txt";
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
            int ilosc = 0;
            /////////////////////////////////////////////////  wprowadzene danych do nauki, wejścia
            int outputId = 0;


            if (radioButtonselekcja.IsChecked == true)
            {
                for (int j = 0; j < 1840; j = j + 1)

                    // I wersja               // if (j<20)
                    if (j == 94 || j == 113 || j == 237 || j == 256 || j == 365 || j == 406 || j == 624 || j == 791 || j == 810 || j == 955
                    || j == 974 || j == 1113 || j == 1259 || j == 1278 || j == 1404 || j == 1423 || j == 1581 || j == 1623 || j == 1751 || j == 1790)
                    {
                        ilosc++;
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
        
                if (radioButtonlosowe.IsChecked == true)
                {
                    Random rnd = new Random();
                    int los=rnd.Next(10,100);
                    int licznik_danych_uczacych = 1;


                for (int j = 0; j < 1840; j = j + 1)

                    // I wersja               // if (j<20)
                   if (licznik_danych_uczacych < 21 && j==los)
                 
                    {
                        licznik_danych_uczacych++;
                        los = los + rnd.Next(50, 90);
                        ilosc++;
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
            net = new NeuralNetwork(77, IleWyjsc, warstwy, neurony, lerningfact);//siec!!!!!!!!!!!!
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
      

        private void testPrzycisk(object sender, RoutedEventArgs e)
        
        {
            
            int poprawne = 0;
            int niepoprawne = 0;

            for (int i = 0; i < 1839; i++)
                if (i >= (int.Parse(poczatek.Text) ) && i <= (int.Parse(koniec.Text)))
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
                //decyzja
               
                wynik[i] = CzyKupil(wyjscia);
            }


            string sciezka = "e:\\wynik" + proba + ".txt";
            TextWriter tr = new StreamWriter(sciezka);
            // read a line of text
            for (int i = 0; i < 1839; i++)
              
                        tr.WriteLine(wynik[i]);

            // close the stream
            tr.Close();
            proba++;
            CalculationTextBlock.Text = "Poprawne: " + poprawne.ToString() + ", niepoprawne: " + niepoprawne.ToString();
           
          
        }

    
       

        private int CzyKupil(double[] wyjscia)
        {
            double maxValue = wyjscia.Max();
            int maxIndex = wyjscia.ToList().IndexOf(maxValue);
           

            if (maxIndex == 0 && maxValue>k)
                return 0;
            else if (maxIndex == 1 && maxValue > k)
                return 1;
            else if (maxIndex == 2 && maxValue > k)
                return 2;
            else
                return 3;
        }



        private void button1_Click(object sender, RoutedEventArgs e)
        {

            double kwota  = 10000;
            double kwota1 = 10000;
            double kwota2 = 10000;
            double kwota3 = 10000;//kwota, którą zarobimy kupując wig20 i odkupując go po okresie n , strategia "kup i czekaj"
           
            double iloscjednostek  = 0;
            double iloscjednostek1 = 0;
            double iloscjednostek2 = 0;
            double iloscjednostek3 = 0;

            double cenaotwarcia;
            

            bool czynagiełdzie1 = false; 
            bool czynagiełdzie2 = false;
            bool czynagiełdzie3 = false;

            int ildecyzji1=0;
            int ildecyzji2 = 0;
            int ildecyzji3 = 0;



            int okrespoczatek = int.Parse(  poczatek.Text);

            int okreskoniec = int.Parse(koniec.Text);

            cenaotwarcia = Dane[okrespoczatek,0 ];
            iloscjednostek = kwota / cenaotwarcia;

            cenaotwarcia = Dane[okreskoniec  , 0];
            kwota = iloscjednostek * cenaotwarcia;

            int kup = 0;
            int sprzedaj = 1;


            for (int i = okrespoczatek; i <okreskoniec; i++)
            {

                //  decyzja podjęta na podstawie 1 dnia
                if (wynik[i] == kup && !czynagiełdzie1)//kupuj
                {
                    czynagiełdzie1 = true;
                    cenaotwarcia = Dane[i , 0];
                    iloscjednostek1 = kwota1 / cenaotwarcia;
                   
                    ildecyzji1++;
                 
                  
                }
                if (wynik[i] == sprzedaj && czynagiełdzie1)//sprzedaj
                {
                    czynagiełdzie1 = false;
                    cenaotwarcia = Dane[i , 0];
                    kwota1 = iloscjednostek1 * cenaotwarcia;
                    ildecyzji1++;
                    i++;
                   


                }
                // decyzja podjęta na podstawie 2 dni
                if (wynik[i] == kup && wynik[i - 1] == kup && !czynagiełdzie2)//kupuj
                {
                    czynagiełdzie2 = true;
                    cenaotwarcia = Dane[i, 0];
                    iloscjednostek2 = kwota2 / cenaotwarcia;
                    ildecyzji2++;
                }
                if (wynik[i] == sprzedaj && wynik[i - 1] == sprzedaj && czynagiełdzie2)//sprzedaj
                {
                    czynagiełdzie2 = false;
                    cenaotwarcia = Dane[i, 0];
                    kwota2 = iloscjednostek2 * cenaotwarcia;
                    ildecyzji2++;
                }
                // decyzja podjęta na podstawie 3 dni pronoz
                if (wynik[i] == kup && wynik[i - 1] == kup && wynik[i - 2] == kup && !czynagiełdzie3)//kupuj
                {
                    czynagiełdzie3 = true;
                    cenaotwarcia = Dane[i, 0];
                    iloscjednostek3 = kwota3 / cenaotwarcia;
                    ildecyzji3++;

                }
                if (wynik[i] == sprzedaj && wynik[i - 1] == sprzedaj && wynik[i - 2] == sprzedaj && czynagiełdzie3)//sprzedaj
                {
                    czynagiełdzie3 = false;
                    cenaotwarcia = Dane[i, 0];
                    kwota3 = iloscjednostek3 * cenaotwarcia;
                    ildecyzji3++;

                }


            }
            cenaotwarcia = Dane[okreskoniec, 0];
           
            if (czynagiełdzie1) { kwota1= iloscjednostek1 * cenaotwarcia; }
            if (czynagiełdzie2) { kwota2 = iloscjednostek2 * cenaotwarcia; }
            if (czynagiełdzie3) { kwota3 = iloscjednostek3 * cenaotwarcia; }

            PoprawneTextBox.Text = "sieć 1 decyzja: " + kwota1.ToString("0.00") + ",  ilość decyzji  "+ildecyzji1.ToString()+Environment.NewLine
                + "siec 2 decyzje : " + kwota2.ToString("0.00") + ",  ilość decyzji  " + ildecyzji2.ToString() + Environment.NewLine
                 + "siec 3 decyzje : " + kwota3.ToString("0.00") + ",  ilość decyzji  " + ildecyzji3.ToString() + Environment.NewLine+Environment.NewLine
                + " kwota zarobiona strategią kup i czekaj " + kwota.ToString("0.00");




        }

        private void aktywny(object sender, RoutedEventArgs e)
        {
            TXBwarstwa.IsEnabled = true;
            TXBneuron.IsEnabled = true;
            TXBwspucz.IsEnabled = true;
            TXBwaga.IsEnabled = true;
            radioButtonselekcja.Visibility=Visibility.Visible;
            radioButtonlosowe.Visibility = Visibility.Visible;
            label.Content = "wybierz dane uczące,";
           
        }

        private void radioButton1_Checked(object sender, RoutedEventArgs e)
        {

            TXBwarstwa.IsEnabled = false;
            TXBneuron.IsEnabled = false;
            TXBwspucz.IsEnabled = false;
            TXBwaga.IsEnabled = false;
            radioButtonselekcja.Visibility = Visibility.Visible;
            radioButtonlosowe.Visibility = Visibility.Visible;
            label.Content = "wybierz dane uczące,";

        }

        private void radioButton4_Checked(object sender, RoutedEventArgs e)
        {
            TXBwarstwa.IsEnabled = false;
            TXBneuron.IsEnabled = false;
            TXBwspucz.IsEnabled = false;
            TXBwaga.IsEnabled = false;
            radioButtonselekcja.Visibility = Visibility.Visible;
            radioButtonlosowe.Visibility = Visibility.Visible;
            label.Content = "wybierz dane uczące,";
        }

        private void radioButton10_Checked(object sender, RoutedEventArgs e)
        {
            TXBwarstwa.IsEnabled = false;
            TXBneuron.IsEnabled = false;
            TXBwspucz.IsEnabled = false;
            TXBwaga.IsEnabled = false;
            radioButtonselekcja.Visibility = Visibility.Visible;
            radioButtonlosowe.Visibility = Visibility.Visible;
            label.Content = "wybierz dane uczące,";
        }

        

        private void aktywnewarstwy(object sender, RoutedEventArgs e)
        {
            TXBwarstwa.Text = "";
        }

        private void aktywneneuron(object sender, RoutedEventArgs e)
        {
            TXBneuron.Text = "";
        }

        private void aktywnelearning(object sender, RoutedEventArgs e)
        {
            TXBwspucz.Text = "";
        }

        private void aktywnewyjscie(object sender, RoutedEventArgs e)
        {
            TXBwaga.Text = "";
        }

        private void nieaktywnewarswty(object sender, RoutedEventArgs e)
        {
            if (TXBwarstwa.Text == "")
            { TXBwarstwa.Text = "ilość warstw"; }
        }

        private void nieaktywneneuron(object sender, RoutedEventArgs e)
        {
            if (TXBneuron.Text == "")
            { TXBneuron.Text = "ilość neuronów"; }
        }

        private void nieaktywnelearning(object sender, RoutedEventArgs e)
        {
            if (TXBwspucz.Text == "")
            { TXBwspucz.Text = "wsp. uczenia"; }
        }

        private void nieaktywnawaga(object sender, RoutedEventArgs e)
        {
            if (TXBwaga.Text == "")
            { TXBwaga.Text = "min. waga wyjścia"; }
        }

        private void radioButtonselekcja_Checked(object sender, RoutedEventArgs e)
        {
            koniec.Visibility = Visibility.Visible;
            poczatek.Visibility = Visibility.Visible;
            label.Content = "... a następnie określ badany okres";
           
        }

      

        private void radioButtonlosoweChecked(object sender, RoutedEventArgs e)
        {
            koniec.Visibility = Visibility.Visible;
            poczatek.Visibility = Visibility.Visible;
            label.Content = "... a następnie określ badany okres";
        }

        private void aktywujbuttony(object sender, RoutedEventArgs e)
        {
            BPortfel.Visibility = Visibility.Visible;
            ButtonWczytaj.Visibility = Visibility.Visible;
            LerningStartStopButton.Visibility = Visibility.Visible;
            test.Visibility = Visibility.Visible;
            label.Content = " Naucz sieć!";
        }
    }
}
