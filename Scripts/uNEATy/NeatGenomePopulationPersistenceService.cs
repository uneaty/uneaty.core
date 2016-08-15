using System.Collections.Generic;
using System.Xml;
using SharpNeat.Genomes.Neat;

namespace uNEATy.core.uNEATy
{
    class NeatGenomePopulationPersistenceService : AbstractGenomePersistenceService<string, List<NeatGenome>>
    {
        public NeatGenomePopulationPersistenceService(string filename, ExperimentConfig config) : base(filename, config)
        {
        }

        public override void Persist(List<NeatGenome> t)
        {
            using (XmlWriter xw = XmlWriter.Create(Filename, _xwSettings))
            {
                NeatGenomeXmlIO.WriteComplete(xw, t, false);
            }
        }

        public override List<NeatGenome> Get(string k)
        {
            return Get();
        }

        public override List<NeatGenome> Get()
        {
            using (XmlReader xr = XmlReader.Create(Filename))
            {
                return NeatGenomeXmlIO.ReadCompleteGenomeList(xr, false, (NeatGenomeFactory) CreateGenomeFactory());
            }
        }
    }
}