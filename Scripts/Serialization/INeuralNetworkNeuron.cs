using SharpNeat.Genomes.Neat;

public interface INeuralNetworkNeuron
{
    NeuronGene Gene { set; }
    string NodeType { get; }
    uint Id { get; }
}