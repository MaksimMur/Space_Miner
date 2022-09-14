using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
public class TNT : MonoCache, IHints,IPausedController
{
    [Header("Set In Inspector: TNT options")]
    [SerializeField] private float _timeToExplosion=2;    
    [SerializeField] private float _distanceForDamage = 5f;
    [SerializeField] private float _damageForMachine= 45f;
    [SerializeField] private float _damageForEnemies=15f;

    [Header("Set in Inspector: Hints options")]
    [SerializeField] private Transform _littleCircleHint;
    [SerializeField] private Transform _bigCircleHint;
    private bool _showHints = true;
    private float _timeSetTNTStart = 2;
    private void Awake()
    {
        PauseManager.S.Register(this);
        Options.S.RegisterHint(this);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        _timeSetTNTStart = Time.time;
    }

    public void RotateCircle(Transform circle,float speed) {
        circle.rotation = Quaternion.Euler(0, 0, circle.eulerAngles.z + speed *Time.deltaTime);
    }

    public override void OnTick()
    {
        if (_IsPaused) return;
        if(_showHints)RotateCircle(_littleCircleHint, 40);
        if(_showHints)RotateCircle(_bigCircleHint, -35f);
        if (_timeSetTNTStart + _timeToExplosion < Time.time) {
            Items.SetTNTExp(transform.position);
            this.gameObject.SetActive(false);
        }
    }
    public void SetHints(bool show) {
        _showHints = show;
        _littleCircleHint.gameObject.SetActive(show);
        _bigCircleHint.gameObject.SetActive(show);
    }
    //paused logic 
    private float _timeLifeInDuringPause;
    private bool _IsPaused = false;
    public void SetPaused(bool ispause)
    {
        _IsPaused = ispause;
        if (ispause)
        {
            _timeLifeInDuringPause = Time.time- _timeSetTNTStart;
            return;
        }
        _timeSetTNTStart = Time.time - _timeLifeInDuringPause;
    }
}
