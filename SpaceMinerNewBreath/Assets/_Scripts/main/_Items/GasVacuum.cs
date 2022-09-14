using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasVacuum : MonoCache, IPausedController
{
    [Header("Set in Inpector: gas vacuum options")]
    [SerializeField] private float _timeToDestroyGas = 0.5f;
    [SerializeField] private float _timeDuration=2;

    private float _timeStart=-2;
    private AudioSource _audioSource;
    private List<IGasVacuumAffect>  _listGas= new List<IGasVacuumAffect>();
    private List<IGasVacuumAffect> _listDestroyGas = new List<IGasVacuumAffect>();
    private Vector2 _vacuumPos;
    private void Awake()
    {
        PauseManager.S.Register(this);
        _audioSource = GetComponent<AudioSource>();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        _timeStart = Time.time;
        _vacuumPos = this.transform.position;
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        IGasVacuumAffect gas = collision.GetComponent<IGasVacuumAffect>();
        if (gas!=null)
        {
            gas.GasPull();
            _listGas.Add(gas);
        }
    }
    public override void OnTick()
    {
        if (_IsPaused) return;
        if ((Time.time - _timeStart) / _timeDuration > 1 && _listGas.Count == 0) {
            this.gameObject.SetActive(false);
        }
        for (int i = 0; i < _listGas.Count; i++) {
            _listGas[i].ApproachGas(_timeToDestroyGas, _vacuumPos);
            if (_listGas[i].Scale.x < 0.2f) _listDestroyGas.Add(_listGas[i]);
        }
        for (int i = 0; i < _listDestroyGas.Count; i++) {
            _listGas.Remove(_listDestroyGas[i]);
            _listDestroyGas[i].DestroyGasByGasVacuum();
        }
        _listDestroyGas.Clear();
    }
    //pause logic
    private bool _IsPaused = false;
    private float _timeLifeInDuringPause;
    public void SetPaused(bool ispaused) {
        _IsPaused = ispaused;
        if (ispaused)
        {
            _timeLifeInDuringPause = Time.time - _timeStart;
            return;
        }
        _timeStart = Time.time - _timeLifeInDuringPause;
    }
}
