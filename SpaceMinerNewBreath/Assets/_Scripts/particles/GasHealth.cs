using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class GasHealth : LightExp
{
    [Header("Set in Ispector: GasHealth options")]
    [SerializeField] private float _timeDuration=3;
    [SerializeField] private float _damageByGasPerSecond=20;
    [SerializeField] private Sprite _gasSprite;
    [SerializeField] private Sprite _expGas;
    [SerializeField] private AudioClip _gasSound;
    [SerializeField] private AudioClip _expSound;

    [Header("Set in Inspector: Hints options")]
    [SerializeField] private RadiusHint _hintF;
    [SerializeField] private RadiusHint _hintS;
    //other
    private ParticleSystem _particleSystem;
    protected override void Awake()
    {
        _particleSystem = this.GetComponent<ParticleSystem>();
        base.Awake();
    }
   
    public void ExploseGas() {
        _hintF.BlockHints(true);
        _hintS.BlockHints(true);
        _particleSystem.textureSheetAnimation.SetSprite(0, _expGas);
        isExp = true;
        if (IsLight) _light2D.enabled = true;
        SetAudioClip(_expSound, true);
    }
    public void SprayGas() {
        _particleSystem.textureSheetAnimation.SetSprite(0, _gasSprite);
        isExp = false;
        _light2D.enabled = false;
        SetAudioClip(_gasSound, true);
    }
    [System.Obsolete]
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (_gasVacuumAffect) return;
        if (collision.GetComponent<Machine>())
        {
            _machineInZoneOfDamage = true;
        }
    }
    [System.Obsolete]
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isExp)
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
            return;
        }
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        _hintF.BlockHints(false);
        _hintS.BlockHints(false);
        _damageByExpWasGiven = false;
        _machineInZoneOfDamage = false;
        _gasVacuumAffect = false;
        isExp = false;
        _timeStart = Time.time;
        Scale = Vector2.one;
        ChangeLightIntencityValue(_lightIntensity);

    }

    [System.Obsolete]
    public override void OnTick()
    {

        if (_gasVacuumAffect  ||_IsPaused) return;
        float u = (Time.time - _timeStart) / _lifeTime;
        if (u >= 1) this.gameObject.SetActive(false);
        if (!_damageByExpWasGiven) {
            if (isExp)
            {
                for (byte i = 0; i < Enemy.enemiesList.Count; i++)
                {
                    if (Enemy.enemiesList[i] == null) continue;
                    if (Vector2.Distance(Enemy.enemiesList[i].transform.position, transform.position) > _distanceToGetDamage) continue;
                    Enemy.enemiesList[i].GetDamage(DamageByExp(Enemy.enemiesList[i].transform.position, true));
                }
                if (Vector2.Distance(Machine.S.machinePos, transform.position) < _distanceToGetDamage)
                {
                    float damage = DamageByExp(Machine.S.machinePos, false);
                    Machine.S.GetDamage(damage);
                    Machine.S.GetDamageText(damage);
                }
                _damageByExpWasGiven = true;
                return;
            } 
            if (_machineInZoneOfDamage)
            {
                Machine.S.GetDamage(_damageByGasPerSecond * Time.deltaTime);
            }
        }
    }
  
   
    public override void GasPull()
    {
        _hintF.BlockHints(true);
        _hintS.BlockHints(true);
        base.GasPull();
    }
 
 
}
