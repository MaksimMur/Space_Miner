using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Joystick jFire;
    [Header("Set in Inpsector: gun's options")]
    [SerializeField] private float _posXGunOnTheMachine = .18f;
    [SerializeField] private float _posXLaunchPointOnTheGun = .14f;
    [SerializeField] private float _damage = 2;
    [SerializeField] private float _speedProjectile = 10;
    [SerializeField] private float _fireRate = .5f;
    [SerializeField] private byte _capacityPoolProjectiles = 5;
    [SerializeField] private Projectile _projectile;
    [SerializeField] private Transform _launchPoint;
    [SerializeField] private Transform _projectilesAnchor;
    [SerializeField] private AudioClip _shootSound;
    [SerializeField] private AudioClip _notEnoughAmmoSound;
    private float _timeStartShot = -2f;
    private SpriteRenderer _spRend;
    private Transform _gunTransform;
    private PoolMono<Projectile> _projectilePool;
    private AudioSource _audioSource;
    private void Awake()
    {
        _gunTransform = GetComponent<Transform>();
        _spRend = GetComponent<SpriteRenderer>();
        _audioSource=GetComponent<AudioSource>();
    }
    private void Start()
    {
        _projectilePool = new PoolMono<Projectile>(_projectile, _capacityPoolProjectiles, _projectilesAnchor);
        _projectilePool.autoExpand = true;
    }
    private float _x, _y;
    private void Update()
    {
        _x = jFire.Horizontal;
        _y = jFire.Vertical;
        RotateGun(_x, _y);
        if (DistanceBeetweenPointAndOrigin(_x, _y) >= 0.95f && _timeStartShot + _fireRate < Time.time ) {
            _timeStartShot = Time.time;
            if (Items.dictItems[ItemType.MagniteImpulse] > 0)
            {
                _audioSource.PlayOneShot(_shootSound);
                Fire();
            }
            else {
                _audioSource.PlayOneShot(_notEnoughAmmoSound);
            }
        }
    }
    private float DistanceBeetweenPointAndOrigin(float x, float y) {
        return Mathf.Sqrt(x * x + y * y);
    }
    public void Fire() {
        Projectile go = _projectilePool.GetFreeElemet();
        go.transform.position = _launchPoint.position;
        Vector2 machineVelocity= Machine.S.Rigid.velocity;
        Items.ReduceIvnentoryAmountOfItemsByOne(ItemType.MagniteImpulse);
        go.GetComponent<Projectile>().LaunchProjectile(_damage, _speedProjectile +Mathf.Sqrt(Mathf.Abs(machineVelocity.x)+Mathf.Abs(machineVelocity.y)), new Vector2(jFire.Horizontal, jFire.Vertical));
    }
    
    public void RotateGun(float x, float y) {
        if (x != 0 && y != 0) {
            int flip = _spRend.flipX ? -1 : 1;
            _gunTransform.rotation = Quaternion.LookRotation(Vector3.forward, flip * (Vector3.up * x + Vector3.left * y));
        }
    } 
    public void FlipXGunWithMachine(bool flipX) {
        transform.localRotation = Quaternion.identity;
        if (!flipX)
        {
            this.transform.localPosition = new Vector3(_posXGunOnTheMachine,0,0);
            _launchPoint.localPosition= new Vector3(-_posXLaunchPointOnTheGun, 0, 0);
            _spRend.flipX = true;
            return;
        }
        _launchPoint.localPosition = new Vector3(_posXLaunchPointOnTheGun, 0, 0);
        this.transform.localPosition = new Vector3(-_posXGunOnTheMachine, 0, 0);
        _spRend.flipX = false;
    }
    /// <summary>
    /// This method imporve gun (increase damage and firerate) 
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="fireRate"></param>
    public void ImproveGun(float damage, float firerate) {
        _damage += damage;
        _fireRate -= Mathf.Abs(firerate);
    }
    public float Damage
        => _damage;
    public float FireRate
        => _fireRate;
}
