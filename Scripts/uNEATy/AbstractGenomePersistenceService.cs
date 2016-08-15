using System.IO;
using System.Xml;
using knife.sharpener;
using SharpNeat.Core;
using SharpNeat.Genomes.Neat;
using UnityEngine;

namespace uNEATy.core.uNEATy
{
    abstract class AbstractGenomePersistenceService<K, T> : IPersistenceService<K, T>
    {
        public string Filename;
        public ExperimentConfig ExperimentConfig;

        protected XmlWriterSettings _xwSettings;

        public AbstractGenomePersistenceService(string filename, ExperimentConfig config)
        {
            Filename = Application.persistentDataPath + "/" + filename;
            ExperimentConfig = config;

            _xwSettings = new XmlWriterSettings();
            _xwSettings.Indent = true;

            // Create directory if it doesn't exist
            DirectoryInfo dirInf = new DirectoryInfo(Application.persistentDataPath);
            if (!dirInf.Exists)
            {
                Debug.Log("Creating subdirectory");
                dirInf.Create();
            }
        }

        protected IGenomeFactory<NeatGenome> CreateGenomeFactory()
        {
            return new NeatGenomeFactory(ExperimentConfig.InputNodes, ExperimentConfig.OutputNodes,
                ExperimentConfig.GenomeParameters);
        }

        public abstract void Persist(T t);
        public abstract T Get(K k);
        public abstract T Get();
    }
}