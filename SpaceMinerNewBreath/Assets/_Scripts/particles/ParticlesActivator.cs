using UnityEngine;
public class ParticlesActivator : MonoCache
{
    [SerializeField] private float _timeActivationDuration=0.3f;
    private float _timeOfActivation = -2;
    protected override void OnEnable()
    {
        _timeOfActivation = Time.time;
        base.OnEnable();
    }
    public override void OnTick()
    {
        if (_timeOfActivation + _timeActivationDuration < Time.time) gameObject.SetActive(false);
    }
}
