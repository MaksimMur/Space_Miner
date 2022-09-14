using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasFuel : ParticlesBehaviour,IHints, IGasVacuumAffect
{
    [Header("Set in Inspector: GasFuel options")]
    [SerializeField] private float _fuelReduceForMachinePerSecond = 10;
    [Header("Set in Inspector: GasFuel options")]
    [SerializeField] private GameObject _hints;
    private bool _machineInZone = false;
    private bool _showHints;
    protected override void Awake()
    {
        Options.S.RegisterHint(this);
        base.Awake();
    }
    protected override void OnEnable()
    {
        _gasVacuumAffect = false;
        Scale = Vector2.one;
        base.OnEnable();
    }
    public override void OnTick()
    {
        if (_gasVacuumAffect ||_IsPaused) return;
        if (_timeStart + _lifeTime < Time.time)
        {
            this.gameObject.SetActive(false);
        }
        if (_machineInZone) {
            Machine.S.GetReduceFuel(_fuelReduceForMachinePerSecond * Time.deltaTime);
        }

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Machine>()) {
            _machineInZone = true;
        }
        if (collision.GetComponent<Machine>() && _showHints && !_gasVacuumAffect)
        {
            _hints.SetActive(true);
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Machine>())
        {
            _machineInZone = false;
        }
    }
    public void SetHints(bool show)
    {
        _showHints = show;
        if (!show) _hints.SetActive(false);
    }
    //IGasVacuumAffect realization
    private float _timeStartAproach = -4;
    private bool _gasVacuumAffect = false;
    private Vector2 _firstPos;
    public void GasPull()
    {
        _timeStartAproach = Time.time;
        _gasVacuumAffect = true;
        _hints.SetActive(false);
        _firstPos = Pos;
    }
    public void ApproachGas(float _timeToApproach, Vector2 posVacuum)
    {
        float u = (Time.time - _timeStartAproach) / _timeToApproach;

        Scale = Vector2.LerpUnclamped(Vector2.one, Vector2.zero, u);

        Pos = Vector2.Lerp(_firstPos, posVacuum, u);
    }
    public void DestroyGasByGasVacuum() {
        this.gameObject.SetActive(false);
    }
    public Vector2 Scale {
        get=>transform.localScale;
        private set =>transform.localScale=value;
    }
    public Vector2 Pos {
        get => transform.position;
        private set => transform.position = value;
    }

    //pauseLogic
    private float _timeAprroachDuringpause;
    public override void SetPaused(bool ispaused)
    {
        _IsPaused = ispaused;
        if (ispaused)
        {
            GetComponent<ParticleSystem>().Pause();
            _timeLifeInDuringPause = Time.time - _timeStart;
            _timeAprroachDuringpause = Time.time - _timeStartAproach;
            return;
        }
        _timeStart = Time.time - _timeLifeInDuringPause;
        _timeStartAproach = Time.time - _timeAprroachDuringpause;
        GetComponent<ParticleSystem>().Play();
    }
}
