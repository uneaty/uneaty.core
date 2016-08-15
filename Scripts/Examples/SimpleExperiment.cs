using SharpNeat.Domains;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using SharpNeat.Decoders;
using System.Collections.Generic;
using System.Xml;
using SharpNeat.Core;
using SharpNeat.Phenomes;
using SharpNeat.Decoders.Neat;
using SharpNeat.DistanceMetrics;
using SharpNeat.SpeciationStrategies;
using SharpNeat.EvolutionAlgorithms.ComplexityRegulation;
using SharpNEAT.Core;

public class SimpleExperiment : INeatExperiment
{
    NeatEvolutionAlgorithmParameters _eaParams;
    NeatGenomeParameters _neatGenomeParams;
    string _name;
    int _populationSize;
    int _specieCount;
    NetworkActivationScheme _activationScheme;
    string _complexityRegulationStr;
    int? _complexityThreshold;
    string _description;
    Optimizer _optimizer;
    int _inputCount;
    int _outputCount;

    public string Name
    {
        get { return _name; }
    }

    public string Description
    {
        get { return _description; }
    }

    public int InputCount
    {
        get { return _inputCount; }
    }

    public int OutputCount
    {
        get { return _outputCount; }
    }

    public int DefaultPopulationSize
    {
        get { return _populationSize; }
    }

    public NeatEvolutionAlgorithmParameters NeatEvolutionAlgorithmParameters
    {
        get { return _eaParams; }
    }

    public NeatGenomeParameters NeatGenomeParameters
    {
        get { return _neatGenomeParams; }
    }

    public void SetOptimizer(Optimizer se)
    {
        this._optimizer = se;
    }


    public void Initialize(string name, XmlElement xmlConfig)
    {
        Initialize(name, xmlConfig, 6, 3);
    }

    public void Initialize(string name, XmlElement xmlConfig, int input, int output)
    {
        string activationSchemeType = "";
        XmlNodeList nodeList = xmlConfig.GetElementsByTagName("Activation", "");
        XmlElement xmlActivation = nodeList[0] as XmlElement;
        string schemeStr = XmlUtils.TryGetValueAsString(xmlActivation, "Scheme");
        int iterations = 0;
        double deltaThreshold = 0d;
        if (schemeStr.Equals("CyclicFixedIters"))
        {
            iterations = XmlUtils.GetValueAsInt(xmlActivation, "Iters");
        }
        else if (schemeStr.Equals("CyclicRelax"))
        {
            iterations = XmlUtils.GetValueAsInt(xmlActivation, "MaxIters");
            deltaThreshold = XmlUtils.GetValueAsDouble(xmlActivation, "Threshold");
        }

        Initialize(name,
            input,
            output,
            XmlUtils.GetValueAsInt(xmlConfig, "PopulationSize"),
            XmlUtils.GetValueAsInt(xmlConfig, "SpecieCount"),
            activationSchemeType,
            iterations,
            deltaThreshold,
            XmlUtils.TryGetValueAsString(xmlConfig, "ComplexityRegulationStrategy"),
            XmlUtils.TryGetValueAsInt(xmlConfig, "ComplexityThreshold"),
            XmlUtils.TryGetValueAsString(xmlConfig, "Description")
            );
    }

    public void Initialize(ExperimentConfig configuration)
    {
        _name = configuration.name;
        _populationSize = configuration.PopulationSize;
        _specieCount = configuration.SpeciesCount;
        _activationScheme = configuration.ActivationScheme;
        _description = configuration.Description;
        _inputCount = configuration.InputNodes;
        _outputCount = configuration.OutputNodes;
        _eaParams = configuration.AlgorithmParameters;
        _neatGenomeParams = configuration.GenomeParameters;
    }

    public void Initialize(string name, int input, int output, int populationSize, int speciesCount,
        string activationSchemeType,
        int iterations, double threshold, string complexityRegulationStrategy, int? complexityThreshold,
        string description)
    {
        _name = name;
        _populationSize = populationSize;
        _specieCount = speciesCount;
        _activationScheme = GetActivationScheme(activationSchemeType, iterations, threshold);
        _description = description;
        _inputCount = input;
        _outputCount = output;

        _eaParams = new NeatEvolutionAlgorithmParameters();
        _eaParams.SpecieCount = _specieCount;
        _neatGenomeParams = new NeatGenomeParameters();
        _neatGenomeParams.FeedforwardOnly = _activationScheme.AcyclicNetwork;
    }

    public NetworkActivationScheme GetActivationScheme(string type, int iterations, double threshold)
    {
        switch (type)
        {
            case "Acyclic":
                return NetworkActivationScheme.CreateAcyclicScheme();
            case "CyclicFixedIters":
                return NetworkActivationScheme.CreateCyclicFixedTimestepsScheme(iterations);
            case "CyclicRelax":
                return NetworkActivationScheme.CreateCyclicRelaxingActivationScheme(threshold, iterations);
        }
        return null;
    }

    public List<NeatGenome> LoadPopulation(XmlReader xr)
    {
        NeatGenomeFactory genomeFactory = (NeatGenomeFactory) CreateGenomeFactory();
        return NeatGenomeXmlIO.ReadCompleteGenomeList(xr, false, genomeFactory);
    }

    public void SavePopulation(XmlWriter xw, IList<NeatGenome> genomeList)
    {
        NeatGenomeXmlIO.WriteComplete(xw, genomeList, false);
    }

    public IGenomeDecoder<NeatGenome, IBlackBox> CreateGenomeDecoder()
    {
        return new NeatGenomeDecoder(_activationScheme);
    }

    public IGenomeFactory<NeatGenome> CreateGenomeFactory()
    {
        return new NeatGenomeFactory(InputCount, OutputCount, _neatGenomeParams);
    }

    public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm()
    {
        var genomeFactory = CreateGenomeFactory();
        List<NeatGenome> genomeList = null;
        try
        {
            genomeList = _optimizer.PopulationPersistenceService.Get();
        }
        catch (System.Exception e1)
        {
            Utility.Log("Error loading genome from file!\nLoading aborted: " + e1.Message);

            genomeList = genomeFactory.CreateGenomeList(_populationSize, 0);
        }

        return CreateEvolutionAlgorithm(genomeFactory, genomeList);
    }

    public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(int populationSize)
    {
        IGenomeFactory<NeatGenome> genomeFactory = CreateGenomeFactory();
        List<NeatGenome> genomeList = genomeFactory.CreateGenomeList(populationSize, 0);
        return CreateEvolutionAlgorithm(genomeFactory, genomeList);
    }

    public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(IGenomeFactory<NeatGenome> genomeFactory,
        List<NeatGenome> genomeList)
    {
        IDistanceMetric distanceMetric = new ManhattanDistanceMetric(1.0, 0.0, 10.0);
        ISpeciationStrategy<NeatGenome> speciationStrategy = new KMeansClusteringStrategy<NeatGenome>(distanceMetric);

        IComplexityRegulationStrategy complexityRegulationStrategy =
            ExperimentUtils.CreateComplexityRegulationStrategy(_complexityRegulationStr, _complexityThreshold);

        NeatEvolutionAlgorithm<NeatGenome> ea = new NeatEvolutionAlgorithm<NeatGenome>(_eaParams, speciationStrategy,
            complexityRegulationStrategy);

        // Create black box evaluator       
        SimpleEvaluator evaluator = new SimpleEvaluator(_optimizer);

        IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder = CreateGenomeDecoder();


        IGenomeListEvaluator<NeatGenome> innerEvaluator =
            new UnityParallelListEvaluator<NeatGenome, IBlackBox>(genomeDecoder, evaluator, _optimizer);

        IGenomeListEvaluator<NeatGenome> selectiveEvaluator =
            new SelectiveGenomeListEvaluator<NeatGenome>(innerEvaluator,
                SelectiveGenomeListEvaluator<NeatGenome>.CreatePredicate_OnceOnly());

        //ea.Initialize(selectiveEvaluator, genomeFactory, genomeList);
        ea.Initialize(innerEvaluator, genomeFactory, genomeList);

        return ea;
    }
}