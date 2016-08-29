using SharpNeat.Genomes.Neat;

public interface INeuralNetworkConnection
{
    ConnectionGene Gene { set; }
    uint SourceId { get; }
    uint TargetId { get; }
    bool Mutated { get; }
    double Weight { get; }
}