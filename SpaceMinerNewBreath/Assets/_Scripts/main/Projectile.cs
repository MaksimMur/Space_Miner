using UnityEngine;

public class Projectile : MonoCache, Idamageable,IPausedController
{
    private float _damage = 0;
    private Rigidbody2D _rigid;
    [SerializeField] private short _timeLife = 2;
    [SerializeField] private bool _isHeroProjectile=false; 
    private float _timeStart = 0;

    protected override void OnEnable() {
        _timeStart = Time.time;
        base.OnEnable();
    }
    private void Awake()
    {
        PauseManager.S.Register(this);
        _rigid = GetComponent<Rigidbody2D>();
    }
    /// <summary>
    /// This Method launch Projectile from gun
    /// </summary>
    /// <param name="damage">damage of projectile</param>
    /// <param name="speed">speed of projectile</param>
    /// <param name="direction">Direction of projectile</param>
    public void LaunchProjectile(float damage, float speed, Vector3 direction)
    {
        _damage = damage;
        _rigid.velocity = direction * speed;
    }
    public float Damage {
        get => _damage;
        set => _damage = value;
    }

    public override void OnTick()
    {
        if (_IsPaused) return;
        if (_timeStart + _timeLife < Time.time) this.gameObject.SetActive(false);
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        this.gameObject.SetActive(false);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(!_isHeroProjectile)this.gameObject.SetActive(false);
    }

    //paused logic 
    private float _timeLifeInDuringPause;
    private bool _IsPaused=false;
    private Vector2 _velocityBeforePaused;
    public void SetPaused(bool ispause) {
        _IsPaused = ispause;
        if (ispause) {
            _timeLifeInDuringPause = (_timeStart + _timeLife)-Time.time;
            _velocityBeforePaused = _rigid.velocity;
            _rigid.bodyType = RigidbodyType2D.Kinematic;
            _rigid.Sleep();
            return;
        }
        _rigid.WakeUp();
        _rigid.velocity = _velocityBeforePaused;
        _rigid.bodyType = RigidbodyType2D.Dynamic;
        _timeStart = Time.time - _timeLifeInDuringPause;
    }
}
