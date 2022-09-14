using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
public class LightExp : ParticlesBehaviour,ILightExp, IPausedController, IGasVacuumAffect
{
    [Header("Set in Inspector: Light options")]
    [SerializeField] private float _damageByExpGas = 40;
    [SerializeField] private float _damageByExpGasForEnemies = 9;
    [SerializeField] protected float _distanceToGetDamage = 1.5f;
    [Range(0,1)] [SerializeField] protected float _lightIntensity = 0.4f;
    protected Light2D _light2D;
    protected bool _damageByExpWasGiven = false;
    protected bool _machineInZoneOfDamage;
    public bool IsLight { get; protected set; }
    public bool isExp { get;protected set; }
    protected override void Awake()
    {
        _light2D = GetComponent<Light2D>();
        _light2D.enabled = true;
        Options.S.RegisterLightExp(this);
        PauseManager.S.Register(this);
        base.Awake();
    }
    public void SetLightExp(bool setLightExp) {
        IsLight = setLightExp;
        _light2D.enabled = setLightExp;
    }
    protected void ReduceLightIntencityWithTime(float u) {
        if (_light2D.enabled)
        {
            _light2D.intensity = Mathf.Lerp(_lightIntensity, 0, u);
        }
    }
    protected void ChangeLightIntencityValue(float value) {
        _light2D.intensity = value;
    } 
    [System.Obsolete]
    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Block b = collision.GetComponent<Block>();
        if (b != null && b.PossibillityToDestroy)
        {
            b.Destroyed(true);
        }
        Plate p = collision.GetComponent<Plate>();
        if (p != null)
        {
            Plate.platesPosition.Remove(p.transform.position);
            Destroy(p.gameObject);
        } 
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Machine>())
        {
            _machineInZoneOfDamage = false;
        }
    }
    public float DamageByExp(Vector2 position, bool isEnemy)
    {
        float distance = Vector2.Distance(this.transform.position, position);
        float damage = Mathf.Lerp(1, isEnemy.Equals(true) ? _damageByExpGasForEnemies : _damageByExpGas, 1 - distance / _distanceToGetDamage);
        return damage;
    }
    //IGasVacuumAffect realization
    private float _timeStartAproach = -4;
    protected bool _gasVacuumAffect = false;
    private Vector2 _firstPos;
    public virtual void GasPull()
    {
        _timeStartAproach = Time.time;
        _gasVacuumAffect = true;
        _firstPos = Pos;
    }
    public void ApproachGas(float _timeToApproach, Vector2 posVacuum)
    {
        float u = (Time.time - _timeStartAproach) / _timeToApproach;

        Scale = Vector2.LerpUnclamped(Vector2.one, Vector2.zero, u);

        Pos = Vector2.Lerp(_firstPos, posVacuum, u);
    }
    public void DestroyGasByGasVacuum()
    {
        this.gameObject.SetActive(false);
    }
    public Vector2 Scale
    {
        get => transform.localScale;
        set => transform.localScale = value;
    }
    public Vector2 Pos
    {
        get => transform.position;
        set => transform.position = value;
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
