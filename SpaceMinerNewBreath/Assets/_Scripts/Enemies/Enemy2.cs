using UnityEngine;

public class Enemy2 : Enemy
{
    [Header("Set in Inspector")]
    [SerializeField] private Transform _launchPoint;
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private float _delayBeetweenAttack = .75f;
    [SerializeField] private float _projectileSpeed = 3;
    [SerializeField] private float _radiusMovement = 5;
    [SerializeField] private float _speedDuration = 3;
    //dynamically
    private static PoolMono<Projectile> _poolProjectile;
    private float _xMovement, _yMovement;
    private float u = 1;
    private float _timeStartMoving = -2;
    private Vector2 pos;
    private float _timeLaunch = -2;
    protected override void Awake()
    {
        base.Awake();
        if (_poolProjectile == null)
        {
            _poolProjectile = new PoolMono<Projectile>(_projectilePrefab, 5, GameObject.Find("ENEMY2_PROJECTIELS").GetComponent<Transform>());
            _poolProjectile.autoExpand = true;
        }
    }
    public override void OnTick()
    {
        if (IsDead ||_isPaused) return;
        if (!_machineInEnemyRadius) return;
        Fire();
        if (_machineInEnemyRadius && u >= 1)
        {
            pos = Pos;
            u = 0;
            animator.enabled = false;
            TakeMovementPoint();
            _timeStartMoving = Time.time;
            return;
        }
        u = (Time.time - _timeStartMoving) / _speedDuration;
        Pos = Vector2.Lerp(pos, new Vector2(_xMovement, _yMovement), u);
        base.OnTick();

    }
    private void TakeMovementPoint() {
        Vector2 machinePos = Machine.S.machinePos;
        if (machinePos.x < Pos.x)
        {
            _xMovement = Random.Range(machinePos.x, machinePos.x + _radiusMovement);
            sp.flipX = true;
        }
        else {
            _xMovement = Random.Range(machinePos.x - _radiusMovement, machinePos.x);
            sp.flipX = false;
        }
        if (machinePos.y < Pos.y) _yMovement = Random.Range(machinePos.y - _radiusMovement, machinePos.y);
        else _yMovement =  Random.Range(machinePos.y, machinePos.y + _radiusMovement);
    }
    public void Fire() {
        if (_timeLaunch + _delayBeetweenAttack<Time.time && !IsDead) {
            _timeLaunch = Time.time;
            Projectile go = _poolProjectile.GetFreeElemet();
            go.transform.position = _launchPoint.position;
            go.LaunchProjectile(_damage, _projectileSpeed, (Machine.S.machinePos - Pos).normalized);
        }
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Magnite>()) _machineInEnemyRadius = true;
    }
    public override void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Magnite>())
        {
            u = 1;
            _machineInEnemyRadius = false;
        }
    }
}
