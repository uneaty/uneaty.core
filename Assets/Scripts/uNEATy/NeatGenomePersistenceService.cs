using System.Xml;
using SharpNeat.Genomes.Neat;

namespace uNEATy.core.uNEATy
{
    class NeatGenomePersistenceService : AbstractGenomePersistenceService<string, NeatGenome>
    {
        public NeatGenomePersistenceService(string filename, ExperimentConfig config) : base(filename, config)
        {
        }

        public override void Persist(NeatGenome t)
        {
            using (XmlWriter xw = XmlWriter.Create(Filename, _xwSettings))
            {
                NeatGenomeXmlIO.WriteComplete(xw, new NeatGenome[] {t}, false);
            }
        }

        public override NeatGenome Get(string k)
        {
            return Get();
        }

        public override NeatGenome Get()
        {
            using (XmlReader xr = XmlReader.Create(Filename))
            {
                return NeatGenomeXmlIO.ReadCompleteGenomeList(xr, false, (NeatGenomeFactory) CreateGenomeFactory())[0];
            }
        }
    }
}