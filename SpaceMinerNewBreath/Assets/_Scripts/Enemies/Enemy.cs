using UnityEngine;
using System.Collections.Generic;
public class Enemy : MonoCache, IPausedController
{
    [Header("Set in Inspector: Enemy options")]
    [SerializeField] protected float _maxHealth = 100;
    [SerializeField] protected float _damage = 2;
    [SerializeField] private float _timeOfIndicatorDamage = .25f;
    [SerializeField] private string _nameOfDeathAnimation = "dead";
    [SerializeField] private string _enemyName;
    public string Name { get => _enemyName; private set => _enemyName = value; }
    public static List<Enemy> enemiesList = new List<Enemy>();
    protected Animator animator;
    protected Rigidbody2D rigid;
    protected SpriteRenderer sp;
    private float _currentHealth;
    [SerializeField] protected GameObject HPSprite;
    private float hpSpriteX = 0.8f, hpSpriteY = 0.8f;


    protected virtual void Awake()
    {
        enemiesList.Add(this);
        _currentHealth = _maxHealth;
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
    }
    private void Start() {
        PauseManager.S.Register(this);
    }
    protected bool IsDead = false;
    protected bool getDamage = false;
    private float _timeGetDamage = -2f;
    protected bool _machineInEnemyRadius = false;
    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        Idamageable idamageable = collision.gameObject.GetComponent<Idamageable>();
        if (idamageable != null && !IsDead) {
            GetDamage(idamageable.Damage);
        }
    }
    public void GetDamage(float damage) {
        _currentHealth -= damage;
        HPSprite.transform.localScale = new Vector3(Mathf.Max(0, _currentHealth / _maxHealth * hpSpriteX), hpSpriteY, 1);
        if (_currentHealth <= 0)
        {
            animator.enabled = true;
            animator.speed = 1;
            animator.CrossFade(_nameOfDeathAnimation, 1);
            IsDead = true;
            enemiesList.Remove(this);
            Statistics.S.EnemyKilled();
            QuestManager.S.DestroyEnemy(Name);
            return;
        }
        if (!getDamage)
        {
            _timeGetDamage = Time.time;
            sp.color = Color.red;
        }
        getDamage = true;
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Machine>())
        {
            _machineInEnemyRadius = true;

        }
    }
    public virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Machine>())
        {
            _machineInEnemyRadius = false;
        }
    }

    public override void OnTick()
    {
        if (_isPaused) return;
        if (getDamage && _timeGetDamage + _timeOfIndicatorDamage < Time.time) {
            getDamage = false;
            sp.color = Color.white;
        }
    }
    public void Death() =>
        Destroy(this.gameObject);
   
    protected Vector2 Pos {
        get => transform.position;
        set => transform.position = value;
    }
    private void OnDestroy()
    {
        PauseManager.S.UnRegister(this);
        Statistics.S.EnemyKilled();
    }
    protected bool _isPaused=false;
    public virtual void SetPaused(bool isPaused) {
        _isPaused = isPaused;
    }

}
