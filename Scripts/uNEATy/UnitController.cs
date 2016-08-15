using UnityEngine;
using SharpNeat.Phenomes;

public abstract class UnitController : MonoBehaviour
{
    public bool IsRunning { get; private set; }
    public IBlackBox Box { get; private set; }

    public virtual void Activate(IBlackBox box)
    {
        Box = box;
        IsRunning = true;
    }

    public virtual void Stop()
    {
        IsRunning = false;
    }

    public abstract float GetFitness();
}