using knife.sharpener;
using SharpNeat.Genomes.Neat;
using UnityEngine;

public abstract class GenomeProvider : MonoBehaviour, IProvider<NeatGenome>
{
    private NeatGenome _genome;

    protected void SetGenome(NeatGenome gen)
    {
        _genome = gen;
    }

    protected NeatGenome GetGenome()
    {
        return _genome;
    }

    public abstract NeatGenome Get();
}