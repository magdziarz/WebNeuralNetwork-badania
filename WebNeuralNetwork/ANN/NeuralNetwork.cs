using System;
using System.IO;

namespace WebNeuralNetwork.ANN
{
    [Serializable]
    public class NeuralNetwork
    {
        private readonly double _lerningFactor;
        private const int SanityBarrier = -700;
        private readonly int _layerCount;
        private readonly Layer[] _layers;
        private Packet _curenPacket;
        private double[] _expectedValues;
        private Packet[] _packets;
        private int _checkInterval = 50;
        public int Epoach;

        public int OutputsCount()
        {
            return _expectedValues.Length;
        }

        public string Name()
        {
            return "NET-L" + (_layers.Length - 2) + "N" + _layers[0].Neurons.GetLength(0) + "LF" + String.Format("{0:F20}", _lerningFactor);
        }

        public NeuralNetwork(int inputCount, int outputCount)
        {
            _lerningFactor = 0.001;
            Inputs = new double[inputCount];
            _expectedValues = new double[outputCount];
            _layerCount = 3;
            _layers = new Layer[_layerCount];
            for (var i = 0; i < _layerCount; i++)
            {
                if (i == 0)
                    _layers[i] = new Layer(150, inputCount);
                else
                    if (i == _layerCount - 1)
                    _layers[i] = new Layer(outputCount, 150);
                else
                    _layers[i] = new Layer(150, _layers[i - 1].NeuronCount);
            }
           // Answers = _layers[2].Scores;
        }

        public NeuralNetwork(int inputCount, int outputCount, int hidenLayerCount, int neuronCount, double lerningFactor)
        {
            _lerningFactor = lerningFactor;
            Inputs = new double[inputCount];
            _expectedValues = new double[outputCount];
            _layerCount = hidenLayerCount + 2;
            _layers = new Layer[_layerCount];
            for (var i = 0; i < _layerCount; i++)
            {
                if (i == 0)
                    _layers[i] = new Layer(neuronCount, inputCount);
                else
                    if (i == _layerCount - 1)
                    _layers[i] = new Layer(outputCount, neuronCount);
                else
                    _layers[i] = new Layer(neuronCount, _layers[i - 1].NeuronCount);
            }
         //   Answers = _layers[hidenLayerCount + 1].Scores;
        }

        public double[] Answers
        {
            get
            {
                return _layers[3].Scores;
            }
            private set
            {
                Answers = value;
            }
        }

        public double[] Inputs { set; get; }

        public int SerialNumber { get; set; }

        public double ActivationFoo(double x)
        {
            return x > SanityBarrier ? 2 / (1 + Math.Exp(-2 * x)) - 1 : -1;
        }

        public double ActivationFooDerivative(double x)
        {
            return x > SanityBarrier ? (1 - Math.Pow(ActivationFoo(x), 2)) : 1;
        }

        public void AddData(Packet[] packets)
        {
            _packets = packets;
        }

        public void ClearData()
        {
            _packets = null;
        }

        public void FlushErrors()
        {
            for (var i = _layerCount - 2; i >= 0; i--)
                for (var j = 0; j < _layers[i].NeuronCount; j++)
                    _layers[i].Errors[j] = 0;
        }

        public void FlushSums()
        {
            for (var i = _layerCount - 1; i >= 0; i--)
                for (var j = 0; j < _layers[i].NeuronCount; j++)
                    _layers[i].Sums[j] = 0;
        }

        public bool LerningIsEnded(ref System.ComponentModel.BackgroundWorker worker)
        {
            var endingFlag = true;
            double errorSum = 0;
            var endTime = DateTime.Now;
            for (var i = 0; i < _packets.Length; i++)
            {
                LoadPacket((uint)i);
                SignalPropagation();

                for (var j = 0; j < _expectedValues.Length; j++)
                {
                    var difference = Math.Abs(_expectedValues[j] - _layers[_layerCount - 1].Scores[j]);
                    errorSum += difference;
                    if (difference > 0.05)
                        endingFlag = false;
                }
            }
            var span = new TimeSpan(0);
            var velocity = (_lastError - errorSum) / _checkInterval * 10;
            if (_checkedTime)
            {
                span = endTime.Subtract(CheckTime);
                _lastError = errorSum;
            }
            CheckTime = endTime;
            _checkedTime = true;
            var progres = (_packets.Length * 0.05) / (errorSum /11)* 100;
            worker.ReportProgress(4, string.Format("Lerning... ErrorSum: {0:0.00} Interations/s: {1:0.00} Error fall: {2:0.0} Progress: {3:0.0}%", errorSum/11,
                _checkInterval / span.TotalSeconds, velocity, progres));
            return endingFlag;
        }

        public bool LerningIsEnded(ref System.ComponentModel.BackgroundWorker worker, ref string status)
        {
            var endingFlag = true;
            double errorSum = 0;
            var endTime = DateTime.Now;
            for (var i = 0; i < _packets.Length; i++)
            {
                LoadPacket((uint)i);
                SignalPropagation();

                for (var j = 0; j < _expectedValues.Length; j++)
                {
                    var difference = Math.Abs(_expectedValues[j] - _layers[_layerCount - 1].Scores[j]);
                    errorSum += difference;
                    if (difference > 0.05)//error   0,05
                        endingFlag = false;
                }
            }
            var span = new TimeSpan(0);
            var velocity = (_lastError - errorSum) / _checkInterval * 10;
            if (_checkedTime)
            {
                span = endTime.Subtract(CheckTime);
                _lastError = errorSum;
            }
            CheckTime = endTime;
            _checkedTime = true;

            var progres = (_packets.Length * 0.05) / errorSum * 100;
            status = string.Format("Lerning... ErrorSum: {0:0.00} Interations/s: {1:0.00} Error fall: {2:0.0} Progress: {3:0.0}%", errorSum,
                _checkInterval / span.TotalSeconds, velocity, progres);
            worker.ReportProgress(4, status);
            return endingFlag;
        }

        public DateTime CheckTime { get; set; }
        private bool _checkedTime;
        public int Krok;
        private double _lastError;

        public void LoadSynapse(string filepath)
        {
            TextReader tr = new StreamReader(filepath);

            // read a line of text
            for (var i = 0; i < _layerCount; i++)
                for (var j = 0; j < _layers[i].NeuronCount; j++)
                    for (var u = 0; u < _layers[i].Neurons.GetLength(1); u++)
                        _layers[i].Neurons[j, u] = Convert.ToDouble(tr.ReadLine());

            // close the stream
            tr.Close();
        }

        public void OutputLayerError()
        {
            for (var i = 0; i < _layers[_layerCount - 1].NeuronCount; i++)
                _layers[_layerCount - 1].Errors[i] = (_expectedValues[i] - _layers[_layerCount - 1].Scores[i]);
        }

        public void RandomWeights()
        {
            var rand = new Random();
            for (var i = 0; i < _layerCount; i++)
                for (var j = 0; j < _layers[i].NeuronCount; j++)
                    for (var u = 0; u < _layers[i].Neurons.GetLength(1); u++)
                        _layers[i].Neurons[j, u] = ((double)(rand.Next() % 200 - 100)) / 1000;
        }

        public void ReverseErrorPropagation()
        {
            FlushErrors();
            for (var warstwa = _layerCount - 2; warstwa >= 0; warstwa--)
                for (var neuron = 0; neuron < _layers[warstwa].NeuronCount; neuron++)
                    for (var neuron2 = 0; neuron2 < _layers[warstwa + 1].NeuronCount; neuron2++)
                    {
                        _layers[warstwa].Errors[neuron] += _layers[warstwa + 1].Errors[neuron2] * _layers[warstwa + 1].Neurons[neuron2, neuron];
                        if (neuron2 == _layers[warstwa + 1].NeuronCount - 1)
                            _layers[warstwa].Errors[neuron] = _layers[warstwa].Errors[neuron] * (ActivationFooDerivative(_layers[warstwa].Scores[neuron]));
                    }
        }

        public void SaveSynapse(string filepath)
        {
            TextWriter tr = new StreamWriter(filepath);
            // read a line of text
            for (var i = 0; i < _layerCount; i++)
                for (var j = 0; j < _layers[i].NeuronCount; j++)
                    for (var u = 0; u < _layers[i].Neurons.GetLength(1); u++)
                        tr.WriteLine(_layers[i].Neurons[j, u]);

            // close the stream
            tr.Close();
        }

        public void SignalPropagation()
        {
            FlushSums();

            for (var i = 0; i < _layerCount; i++)
                for (var j = 0; j < _layers[i].NeuronCount; j++)
                    for (var u = 0; u < (_layers[i].Neurons.GetLength(1)); u++)    // 23:23
                    {
                        if (i == 0)
                            (_layers[i].Sums[j]) = (_layers[i].Sums[j]) + _layers[i].Neurons[j, u] * Inputs[u];
                        else
                            (_layers[i].Sums[j]) = (_layers[i].Sums[j]) + _layers[i].Neurons[j, u] * _layers[i - 1].Scores[u];
                        if (u == ((_layers[i].Neurons.GetLength(1)) - 1))
                            (_layers[i].Scores[j]) = ActivationFoo(_layers[i].Sums[j]);
                    }
        }

        public void StartLerning()
        {
            for (var warstwa = _layerCount - 1; warstwa >= 0; warstwa--)
                for (var neuron = 0; neuron < _layers[warstwa].NeuronCount; neuron++)
                    for (var synapsa = 0; synapsa < _layers[warstwa].Neurons.GetLength(1); synapsa++)
                        if (warstwa != 0)
                            _layers[warstwa].Neurons[neuron, synapsa] += _lerningFactor * _layers[warstwa].Errors[neuron] * _layers[warstwa - 1].Scores[synapsa];
                        else
                            _layers[warstwa].Neurons[neuron, synapsa] += _lerningFactor * _layers[warstwa].Errors[neuron] * Inputs[synapsa];
        }

        public void Ucz(ref System.ComponentModel.BackgroundWorker worker)
        {
            Krok = 0;
            worker.ReportProgress(3, "working... ");
            while (true)
            {
                Krok++;
                LoadRandomPacket();
                var r = new Random();

                for (var i = 1; i < _curenPacket.Inputs.Length; i++)
                    if (r.Next(0, 2) == 0)
                        Inputs[i] -= 0.005;
                    else
                        Inputs[i] += 0.005;

                SignalPropagation();
                if (Krok % _checkInterval == 0 && LerningIsEnded(ref worker))
                    break;

                OutputLayerError();
                ReverseErrorPropagation();
                StartLerning();
            }
            Epoach = Krok;
        }

        public void Ucz(ref System.ComponentModel.BackgroundWorker worker, ref string status)
        {
            Krok = 0;
            status = "working... ";
            worker.ReportProgress(3, "working... ");
            while (true)
            {
                Krok++;
                LoadRandomPacket();
                var r = new Random();

                for (var i = 1; i < _curenPacket.Inputs.Length; i++)
                    if (r.Next(0, 2) == 0)
                        Inputs[i] -= 0.005;
                    else
                        Inputs[i] += 0.005;

                SignalPropagation();
                if (Krok % _checkInterval == 0 && LerningIsEnded(ref worker, ref status))
                    break;

                OutputLayerError();
                ReverseErrorPropagation();
                StartLerning();
            }
            Epoach = Krok;
        }

        public double[] Ask(Packet packet)
        {
            _packets = new[] { packet };
            LoadPacket(0);
            SignalPropagation();

            var ouput = new double[_expectedValues.Length];
            for (var j = 0; j < _expectedValues.Length; j++)
                ouput[j] = _layers[_layerCount - 1].Scores[j];

            return ouput;
        }

        private void LoadPacket(uint i)
        {
            _expectedValues = _packets[i].Outputs;
            _curenPacket = _packets[i];
            Array.Copy(_curenPacket.Inputs, Inputs, Inputs.Length);
        }

        private void LoadRandomPacket()
        {
            var random = new Random();
            int i = random.Next(0, _packets.Length);
            LoadPacket((uint)i);
        }
    }
}
