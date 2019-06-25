using System;

namespace WebNeuralNetwork.ANN
{
    [Serializable]
    public class Packet
    {
        public Packet(double[] inputs, double[] outputs, int id)
        {
            Inputs = inputs;
            Outputs = outputs;
            Id = id;
        }

        public int Id { get; set; }

        public double[] Inputs { get; set; }

        public double[] Outputs { get; set; }
    }
}
