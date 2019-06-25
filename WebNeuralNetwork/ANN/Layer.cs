using System;

namespace WebNeuralNetwork.ANN
{
    [Serializable]
    public class Layer
    {
        public Layer(int neuronCount, int inputCount)
        {
            NeuronCount = neuronCount;
            InputCount = inputCount;
            Neurons = new double[neuronCount, inputCount];

            Scores = new double[neuronCount];
            Sums = new double[neuronCount];
            Errors = new double[neuronCount];
        }

        public double[] Errors { get; set; }

        public int InputCount { get; set; }

        public int NeuronCount { get; set; }

        public double[,] Neurons { get; set; }

        public double[] Scores { get; set; }

        public double[] Sums { get; set; }
    }
}
