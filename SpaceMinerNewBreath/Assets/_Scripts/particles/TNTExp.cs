using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNTExp : LightExp
{
    protected override void OnEnable()
    {
        base.OnEnable();
        _damageByExpWasGiven = false;
        _machineInZoneOfDamage = false;
        _gasVacuumAffect = false;
        Scale = Vector2.one;
        _timeStart = Time.time;
        ChangeLightIntencityValue(_lightIntensity);
    }

    [System.Obsolete]
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        GasHealth g = collision.GetComponent<GasHealth>();
        if (g != null && !g.isExp)
        {
            g.ExploseGas();
        }
    }
    public override void OnTick()
    {
        if (_IsPaused || _gasVacuumAffect) return;
        float u = (Time.time - _timeStart) / _lifeTime;
        ReduceLightIntencityWithTime(u);
        if (u >= 1) this.gameObject.SetActive(false);
        if (!_damageByExpWasGiven)
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
    }
    
}
