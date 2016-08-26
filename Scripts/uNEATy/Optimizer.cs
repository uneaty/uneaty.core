using UnityEngine;
using SharpNeat.Phenomes;
using System.Collections.Generic;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using System;
using System.Xml;
using System.IO;
using System.Linq;
using uNEATy.core.uNEATy;

public class Optimizer : GenomeProvider
{
    public ExperimentConfig ExperimentConfig;
    public IPersistenceService<string, NeatGenome> GenomePersistenceService;
    public IPersistenceService<string, List<NeatGenome>> PopulationPersistenceService;

    public int Trials = 1;
    public float TrialDuration = 30f;
    public float StoppingFitness = 15f;
    bool EARunning;

    public SimpleExperiment Experiment { get; private set; }
    static NeatEvolutionAlgorithm<NeatGenome> _ea;

    public GameObject Unit;

    Dictionary<IBlackBox, UnitController> ControllerMap = new Dictionary<IBlackBox, UnitController>();
    private DateTime startTime;
    private float timeLeft;
    private float accum;
    private int frames;
    private float updateInterval = 12;

    private uint Generation;
    private double Fitness;

    // Use this for initialization
    void Start()
    {
        Utility.DebugLog = true;
        Experiment = new SimpleExperiment();
        Experiment.SetOptimizer(this);
        Experiment.Initialize(ExperimentConfig);
        GenomePersistenceService = new NeatGenomePersistenceService("champ", ExperimentConfig);
        PopulationPersistenceService = new NeatGenomePopulationPersistenceService("population", ExperimentConfig);
    }

    public NeatEvolutionAlgorithm<NeatGenome> EvolutionAlgorithm
    {
        get { return _ea; }
    }

    // Update is called once per frame
    void Update()
    {
        //  evaluationStartTime += Time.deltaTime;

        timeLeft -= Time.deltaTime;
        accum += Time.timeScale/Time.deltaTime;
        ++frames;

        if (timeLeft <= 0.0)
        {
            var fps = accum/frames;
            timeLeft = updateInterval;
            accum = 0.0f;
            frames = 0;
            if (fps < 10)
            {
                Time.timeScale = Time.timeScale - 1;
                print("Lowering time scale to " + Time.timeScale);
            }
        }
    }

    public void StartEA()
    {
        Utility.DebugLog = true;
        Utility.Log("Starting PhotoTaxis experiment");
        // print("Loading: " + popFileLoadPath);
        _ea = Experiment.CreateEvolutionAlgorithm();
        startTime = DateTime.Now;

        _ea.UpdateEvent += new EventHandler(ea_UpdateEvent);
        _ea.PausedEvent += new EventHandler(ea_PauseEvent);

        var evoSpeed = 25;

        //   Time.fixedDeltaTime = 0.045f;
        Time.timeScale = evoSpeed;
        _ea.StartContinue();
        EARunning = true;
    }

    void ea_UpdateEvent(object sender, EventArgs e)
    {
        Utility.Log(string.Format("gen={0:N0} bestFitness={1:N6}",
            _ea.CurrentGeneration, _ea.Statistics._maxFitness));

        Fitness = _ea.Statistics._maxFitness;
        Generation = _ea.CurrentGeneration;


        //    Utility.Log(string.Format("Moving average: {0}, N: {1}", _ea.Statistics._bestFitnessMA.Mean, _ea.Statistics._bestFitnessMA.Length));
    }

    void ea_PauseEvent(object sender, EventArgs e)
    {
        Time.timeScale = 1;
        Utility.Log("Done ea'ing (and neat'ing)");

        // TODO: Transmit the current state after pausing

        XmlWriterSettings _xwSettings = new XmlWriterSettings();
        _xwSettings.Indent = true;
        // Save genomes to xml file.        
        DirectoryInfo dirInf = new DirectoryInfo(Application.persistentDataPath);
        if (!dirInf.Exists)
        {
            Debug.Log("Creating subdirectory");
            dirInf.Create();
        }
        PopulationPersistenceService.Persist(_ea.GenomeList.ToList());
        GenomePersistenceService.Persist(_ea.CurrentChampGenome);

        DateTime endTime = DateTime.Now;
        Utility.Log("Total time elapsed: " + (endTime - startTime));

        EARunning = false;
    }

    public void StopEA()
    {
        if (_ea != null && _ea.RunState == SharpNeat.Core.RunState.Running)
        {
            _ea.Stop();
        }
    }

    public void Evaluate(IBlackBox box)
    {
        GameObject obj = Instantiate(Unit, Unit.transform.position, Unit.transform.rotation) as GameObject;
        UnitController controller = obj.GetComponent<UnitController>();

        ControllerMap.Add(box, controller);

        controller.Activate(box);
    }

    public void StopEvaluation(IBlackBox box)
    {
        UnitController ct = ControllerMap[box];

        Destroy(ct.gameObject);
    }

    public void RunBest()
    {
        Time.timeScale = 1;

        NeatGenome genome = null;

        // Try to load the genome from the XML document.
        try
        {
            genome = GenomePersistenceService.Get();
        }
        catch (Exception e1)
        {
            return;
        }

        // Get a genome decoder that can convert genomes to phenomes.
        var genomeDecoder = Experiment.CreateGenomeDecoder();

        // Decode the genome into a phenome (neural network).
        var phenome = genomeDecoder.Decode(genome);

        GameObject obj = Instantiate(Unit, Unit.transform.position, Unit.transform.rotation) as GameObject;
        UnitController controller = obj.GetComponent<UnitController>();

        ControllerMap.Add(phenome, controller);

        controller.Activate(phenome);
    }

    public void Reset()
    {
        StopEA();
        File.Delete(Application.persistentDataPath + "/" + "champ");
        File.Delete(Application.persistentDataPath + "/" + "population");
    }

    public float GetFitness(IBlackBox box)
    {
        if (ControllerMap.ContainsKey(box))
        {
            return ControllerMap[box].GetFitness();
        }
        return 0;
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 40), "Start EA"))
        {
            StartEA();
        }
        if (GUI.Button(new Rect(10, 60, 100, 40), "Stop EA"))
        {
            StopEA();
        }
        if (GUI.Button(new Rect(10, 110, 100, 40), "Run best"))
        {
            RunBest();
        }
        if (GUI.Button(new Rect(10, 160, 100, 40), "Run best"))
        {
            Reset();
        }

        GUI.Button(new Rect(10, Screen.height - 70, 100, 60),
            string.Format("Generation: {0}\nFitness: {1:0.00000000}", Generation, Fitness));
    }

    public override NeatGenome Get()
    {
        return EvolutionAlgorithm != null ? EvolutionAlgorithm.CurrentChampGenome : null;
    }
}