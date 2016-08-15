using SharpNeat.Decoders;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using UnityEngine;

public class ExperimentConfig : MonoBehaviour
{
    public string Description;

    public int InputNodes = 5;
    public int OutputNodes = 2;

    public int PopulationSize = 50;
    public int SpeciesCount = 10;

    public string ActivationSchemeType = "Acyclic";
    public int ActivationIterationCount = 0;
    public double ActivationThreshold = 0d;

    public string ComplexityRegulationStrategy = "Absolute";
    public int ComplexityThreshold = 10;

    public NeatEvolutionAlgorithmParameters AlgorithmParameters
    {
        get
        {
            NeatEvolutionAlgorithmParameters neap = new NeatEvolutionAlgorithmParameters();
            neap.SpecieCount = SpeciesCount;
            return neap;
        }
    }

    public NeatGenomeParameters GenomeParameters
    {
        get
        {
            NeatGenomeParameters ngp = new NeatGenomeParameters();
            ngp.FeedforwardOnly = ActivationSchemeType.Equals("Acyclic");
            return ngp;
        }
    }

    public NetworkActivationScheme ActivationScheme
    {
        get
        {
            switch (ActivationSchemeType)
            {
                case "Acyclic":
                    return NetworkActivationScheme.CreateAcyclicScheme();
                case "CyclicFixedIters":
                    return NetworkActivationScheme.CreateCyclicFixedTimestepsScheme(ActivationIterationCount);
                case "CyclicRelax":
                    return NetworkActivationScheme.CreateCyclicRelaxingActivationScheme(ActivationThreshold,
                        ActivationIterationCount);
            }
            return null;
        }
    }
}