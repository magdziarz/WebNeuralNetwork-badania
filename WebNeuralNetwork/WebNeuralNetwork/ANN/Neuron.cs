using System;

namespace WebNeuralNetwork
{
    [Serializable]
    public class Neuron
    {
        public Neuron(int x)
        {
            InputWeight = new double[x];
            NeuronCount = x;
        }

        public double[] InputWeight { get; set; }

        public int NeuronCount { get; set; }
    }
}
