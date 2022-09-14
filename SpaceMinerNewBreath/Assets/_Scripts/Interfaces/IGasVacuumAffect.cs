using UnityEngine;
public interface IGasVacuumAffect 
{
    public void GasPull() { }
    public void DestroyGasByGasVacuum() { }
    public void ApproachGas(float _timeToApproach, Vector2 transformVacuum) { }
    public Vector2 Scale { get;}
    public Vector2 Pos { get; }
}
