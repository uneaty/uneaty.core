using System.Collections.Generic;
using SharpNeat.Genomes.Neat;

public interface INeuralNetwork
{
    NeatGenome Genome { set; }
    List<INeuralNetworkNeuron> Neurons { get; }
    List<INeuralNetworkConnection> Connections { get; }
}