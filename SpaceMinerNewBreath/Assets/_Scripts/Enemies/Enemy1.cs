using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : Enemy
{
    [Header("Set in Inspector: Enemy1 options")]
    [SerializeField] private float _impluesJump = 5;
    [SerializeField] private float _timeBetweenJumps = .25f;
    [SerializeField] private float _delayBeetweenAttack = .75f;
    private float _jumpStart = -2f;
    public void EnemyMove() {
        rigid.velocity=new Vector2((Machine.S.machinePos-Pos).x,_impluesJump);
        _jumpStart = Time.time;
    }

    public override void OnTick()
    {
        if (IsDead) return;
        if (_machineInEnemyRadius)
        {
            if (_jumpStart + _timeBetweenJumps < Time.time)
            {
                EnemyMove();
            }
        }
        base.OnTick();
    }
    private float _timeStartAttack = -2f;
    public override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if (collision.gameObject.GetComponent<Machine>()) {
            if ( _timeStartAttack + _delayBeetweenAttack > Time.time) return;
                Machine.S.GetDamage(_damage);
                _timeStartAttack = Time.time;
        }
    }
   
}
