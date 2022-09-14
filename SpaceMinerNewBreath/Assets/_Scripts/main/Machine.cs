using System.Collections.Generic;
using UnityEngine;
using System;
public enum MachineCondition
{ 
idle,
move,
fly,
drilDown,
drilLeft,
drilRight
}
public class Machine : MonoBehaviour, IMachineCharacteristic, IPausedController
{
    [Header("Set control options")]
    public static Machine S;
    [SerializeField] [Range(0, 1)] private float minjoysticEffortForMove = 0.5f;
    [SerializeField] [Range(0.35f, 1f)] private float minjoysticEffortForDril = 0.35f;
    [SerializeField] [Range(0.35f, 1f)] private float minjoysticEffortForDrilSide = 0.75f;
    [SerializeField] [Range(0.25f, -1f)] private float minjoysticEffortForFly = 0.25f;
    public Joystick joystic;
    [Header("Set sounds options")]
    [SerializeField] private AudioClip _machineMovingSound;
    [SerializeField] private AudioClip _machineFlyingSound;
    [SerializeField] private AudioClip _machineDrillingSound;
    [SerializeField] private AudioClip _machineFuelIsEndSound;
    [SerializeField] private AudioClip _machineHealthIsEndSound;
    [Header("Set simple machine's options")]

    [Header("moving")]
    [SerializeField] private float _speedMove = 2;//1 - 10 km/h
    [SerializeField] private float _speedFly = 4;//1 - 10 km/h
    [SerializeField] private float _timeFlyOverclockingDuration = 1;//1 - 10 km/h

    [Header("influnce on theblock")]
    [SerializeField] private float _burDamage = 2;//damage per 0.02 second 
    [SerializeField] private byte _burLevel;
    
    [Header("health")]
    [SerializeField]private float _healthMax = 100;
    
    [Header("fuel")]
    [SerializeField] private float _fuelMax = 100;
    [SerializeField] private float _fuelMovingConsumption = 0.5f;
    [SerializeField] private float _fuelFlyingConsumption = 0.6f;
    [SerializeField] private float _fuelDrillingConsumption = 0.4f;

    [Header("armor")]
    [SerializeField][Range(0, .25f)] private float _armor = 0.05f;
    [SerializeField] private byte _amountBlocksWithNormalPreasure = 10;
    private float _damageForPreasure=5;
    //fall damage will be implemented as _damageKofForFalling*_minFallingSpeed
    //if the player does not have time to slow down
    [SerializeField] private float _damageKofForFalling = 4;
    [SerializeField] private float _minFalingSpeed = 6;
    [SerializeField] private GameObject _badFallenParticles;

    [Header("System Cold")]
    [SerializeField] private byte _amountBlocksWithNormalTemperature = 15;
    [SerializeField] private float _damageForTemperature = 4;

    
    private float _currentFuel;
    private float _currentHealth;

    //fly dinamycoOptions
    private float _startTimefly = 0;
    private bool _machineIsFly = false;
    //cacheable components
    private Animator _animator;
    private Rigidbody2D _rigid;
    private SpriteRenderer _spriteRenderer;
    private float _defaultGravityScale;
    private Gun _machineGun;
    private AudioSource _audioSource;
    private void Awake()
    {
        S = this;
        PauseManager.S.Register(this);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _rigid = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
        _machineGun = GetComponentInChildren<Gun>();
        _defaultGravityScale = _rigid.gravityScale;
        _currentHealth = _healthMax;
        _currentFuel = _fuelMax;
    }

    [Obsolete]
    void Update()
    {
        if (_isPaused ||_lose) return;
        GetInfluenceForPreasure();
        GetInfluenceForTemperature();
        GetAction(State);
        SetPossibillityToDestroyBlocks();
        if (_getInfluence && _timeGetInfluence + _timeOfIndicatorInfluence < Time.time) {
            _spriteRenderer.color = Color.white;
            _getInfluence = false;
        }
    }
    public void CheckFuelLose() {
        if (Fuel <= 0 && !_lose)
        {
            _audioSource.Stop();
            GameEnd.GameIsEnd(GameEndState.fuelIsEnd);
            _animator.CrossFade("idle", 0);
            //  _audioSource.PlayOneShot(_machineFuelIsEndSound);
            _lose = true;
        }
    }
    public void CheckHealthLose() {
        if (Health <= 0 && !_lose)
        {
            _audioSource.Stop();
            _animator.CrossFade("idle", 0);
            GameEnd.GameIsEnd(GameEndState.HeatlhIsEnd);
            //_audioSource.PlayOneShot(_machineHealthIsEndSound);
            _lose = true;
        }
    }
    private bool _lose = false;
    private bool _movingSound = false;
    private bool _flyingSound = false;
    private bool _drillingSound = false;
    public void SetSoundsFlags(bool move = false, bool fly = false, bool drill = false) => (_movingSound,_flyingSound,_drillingSound) = (move, fly,drill);
    
    public void MoveX() {
        _machineGun.FlipXGunWithMachine(IsFlipRight);
        Fuel -= Math.Abs(joystic.Horizontal) * _fuelMovingConsumption * Time.deltaTime ;
        CheckFuelLose();
        _spriteRenderer.flipX = joystic.Horizontal < 0 ? false : true;
        _rigid.velocity = _rigid.velocity.y == 0 ? new Vector2(joystic.Horizontal * _speedMove, _rigid.velocity.y) : _rigid.velocity;

        if (!_movingSound)
        {
            _audioSource.loop = true;
            _audioSource.clip = _machineMovingSound;
            _audioSource.Play();
            SetSoundsFlags(true, false, false);
        }
        
    }
    public void MoveY(float u)
    {
        _machineGun.FlipXGunWithMachine(IsFlipRight);
        Fuel -= ((Math.Abs(joystic.Vertical) * _fuelFlyingConsumption * Time.deltaTime));
        CheckFuelLose();
        _spriteRenderer.flipX = joystic.Horizontal < 0 ? false : true;
        _rigid.velocity = Vector2.Lerp(_rigid.velocity, (joystic.Vertical * _speedFly * Vector2.up) + (joystic.Horizontal * _speedMove * Vector2.right), u);

        if (!_flyingSound)
        {
            _audioSource.loop = true;
            _audioSource.clip = _machineFlyingSound;
            _audioSource.Play();
            SetSoundsFlags(false, true, false);
        }
    }

    [Obsolete]
    public void Drill(Block b) {
        Fuel -= _fuelDrillingConsumption * Time.deltaTime;
        CheckFuelLose();
        if (!b.PossibillityToDestroy && _isBlockPossibleDestroyFinish) {
            _isBlockPossibleDestroyFinish = false;
            try { 
                FlyText t = UIManager.S.MessageForUser(new List<Vector2>() { new Vector2(machinePos.x, machinePos.y - 0.3f), new Vector2(machinePos.x, machinePos.y - 0.6f) }, Easing.Out, Time.time, 3f, this.gameObject, "FinishTextPossibleToDestroy",true, "ImpossibleToDestroyBlock");
                t.SetRectSize(new List<Vector2>() { new Vector2(8f, 0.2f) });
                t.SetColorChange(new List<Color>() { Color.red});
            }
            catch (System.NullReferenceException) {}
        }
        if (b.LevelOfBurToDestroy>_burLevel && _isLevelBurTextFinish) {
            _isLevelBurTextFinish = false;
            try { 
                FlyText t = UIManager.S.MessageForUser(new List<Vector2>() { new Vector2(machinePos.x, machinePos.y - 0.3f), new Vector2(machinePos.x, machinePos.y - 0.6f) }, Easing.Out, Time.time, 2f, this.gameObject, "FinishTextLevelBur",true, "ImproveYourBur");
                t.SetRectSize(new List<Vector2>() { new Vector2(8f, 0.2f) });
                t.SetColorChange(new List<Color>() { Color.yellow});
            }
            catch (System.NullReferenceException) {}
        }
        b.GetDamage(_burDamage*Time.deltaTime, _burLevel);
        if (!_drillingSound)
        {
            _audioSource.loop = true;
            _audioSource.clip = _machineDrillingSound;
            _audioSource.Play();
            SetSoundsFlags(false, false, true);
        }

    }
   
    public MachineCondition State {
        get
        {
            float xAxis = joystic.Horizontal, yAxis = joystic.Vertical;

            if (yAxis == 0 && xAxis == 0) return MachineCondition.idle;
            if (yAxis > minjoysticEffortForFly)
            {
                _blockDown = null;
                if (!_machineIsFly) _startTimefly = Time.time;
                _machineIsFly = true;
                _plate = false;
                return MachineCondition.fly;
            }
            if (_blockRight != null && xAxis > minjoysticEffortForDrilSide)
            {
                return MachineCondition.drilRight;
            }
            if (_blockLeft != null && xAxis < -minjoysticEffortForDrilSide)
            {
                return MachineCondition.drilLeft;
            }


            if (yAxis < -minjoysticEffortForDril && _blockDown != null) { _rigid.Sleep(); return MachineCondition.drilDown; }
            if (xAxis > minjoysticEffortForMove || xAxis < -minjoysticEffortForMove)
            {
                return MachineCondition.move;
            }
            return MachineCondition.idle;
        }
        private set =>State=value;
    }

    [Obsolete]
    public void GetAction(MachineCondition condition) {

        MachineInterface.S.ChangeJoysticWithMachineCondition(condition);
        switch (condition) {
            case MachineCondition.idle:
                _audioSource.loop = false;
                if(!_audioSource.isPlaying) _audioSource.Stop();
                SetSoundsFlags();
                _animator.CrossFade("idle", 0);
                break;
            case MachineCondition.move:
                _animator.CrossFade("move", 0);
                _blockDown = null;
                MoveX();
                break;
            case MachineCondition.drilDown:
                transform.position = new Vector2(_blockDown.X, machinePos.y);
                _animator.CrossFade("drilDown", 0);
                Drill(_blockDown);
                break;
            case MachineCondition.drilRight:
                Drill(_blockRight);
                _animator.CrossFade("drilSide", 0);
                break;
            case MachineCondition.drilLeft:
                Drill(_blockLeft);
                _animator.CrossFade("drilSide", 0);
                break;
            case MachineCondition.fly:
                _animator.CrossFade("fly", 0);
                _rigid.gravityScale = 0;
                float u = Mathf.Min(1, (Time.time - _startTimefly) / _timeFlyOverclockingDuration);
                MoveY(u);
                return;
        }
        _rigid.gravityScale = _defaultGravityScale;
        _machineIsFly = false;
    }
    private void SetPossibillityToDestroyBlocks() {
        _blockRight = _blockRight == null || (_blockDown == null &&_plate==false) || !IsFlipRight || machinePos.y + 1 < _blockRight.Y ? null : _blockRight;
        _blockLeft = _blockLeft == null  || (_blockDown == null && _plate == false) || IsFlipRight || machinePos.y + 1 < _blockLeft.Y ? null : _blockLeft;
        _blockDown = _blockDown == null|| machinePos.x+ _bDXAligment < _blockDown.X|| machinePos.x - _bDXAligment > _blockDown.X ? null : _blockDown;
    }
    /// <summary>
    /// this method take block nearly machine with collision
    /// </summary>
    /// <param name="collision"></param>
    private Block _blockDown, _blockRight, _blockLeft;
    private float _bDXAligment = 0.5f;
    private bool _plate= false;
    private void OnCollisionStay2D(Collision2D collision)
    {
        Vector2 pos = collision.gameObject.transform.position;
        if (pos.y < machinePos.y && pos.x + _bDXAligment > machinePos.x && pos.x - _bDXAligment < machinePos.x) _blockDown = collision.gameObject.GetComponent<Block>();
        if (pos.x > machinePos.x && machinePos.y < pos.y) _blockRight = collision.gameObject.GetComponent<Block>();
        if (pos.x < machinePos.x && machinePos.y < pos.y) _blockLeft = collision.gameObject.GetComponent<Block>();
        if (pos.y < machinePos.y && pos.x + _bDXAligment > machinePos.x && pos.x - _bDXAligment < machinePos.x) _plate = true;
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        Idamageable idamageable = collision.gameObject.GetComponent<Idamageable>();
        if (idamageable != null) {
            GetDamage(idamageable.Damage);
        }
    }

    /// <summary>
    /// This method is called when updating and checking 
    /// whether the machine depth and maximum depth match with normal pressure
    /// </summary>
    public void GetInfluenceForPreasure() {
        MachineInterface.S.IllustratePreasure(PreausreInfluence);
        if (PreausreInfluence < 0) {
            if (_isDamageFromPreasureFinish)
            {
                _isDamageFromPreasureFinish = false;
                try { 
                    FlyText t = UIManager.S.MessageForUser(new List<Vector2>() { new Vector2(machinePos.x, machinePos.y - 0.4f), new Vector2(machinePos.x, machinePos.y - 0.6f) }, Easing.InOut, Time.time, 3.5f, this.gameObject, "FinishDamageFromPreasure",true, "YouTakeDamageFromPreasure");
                    t.SetRectSize(new List<Vector2>() { new Vector2(8f, 0.2f) });
                    t.SetColorChange(new List<Color>() { Color.cyan });
                }
                catch (System.NullReferenceException) {}
            }
            float damage = _damageForPreasure * (1 - _armor);
            damage += Math.Abs(PreausreInfluence) * damage;
            Health -= damage * Time.deltaTime;
        }
    }

    /// <summary>
    /// This method is called when updating and checking 
    /// whether the machine depth and maximum depth match with normal pressure
    /// </summary>
    public void GetInfluenceForTemperature()
    {
        MachineInterface.S.IllustrateTemperature(TemperatureInfluence);
        if (TemperatureInfluence < 0)
        {
            if (_isDamageFromTemperatureFinish) {
                _isDamageFromTemperatureFinish = false;
                try { 
                    FlyText t = UIManager.S.MessageForUser(new List<Vector2>() { new Vector2(machinePos.x, machinePos.y - 0.2f), new Vector2(machinePos.x, machinePos.y - 0.4f) }, Easing.InOut, Time.time, 3.5f, this.gameObject, "FinishDamageFromTemperature",true, "YouTakeDamageFromTemperature");
                    t.SetRectSize(new List<Vector2>() { new Vector2(8f, 0.2f) });
                    t.SetColorChange(new List<Color>() { Color.red });
                }
                catch (System.NullReferenceException) {}
            }
            float damage = _damageForTemperature * (1 - _armor);
            damage += Math.Abs(PreausreInfluence) * damage;
            Health -= damage * Time.deltaTime;
        }
    }
    
    private float _timeGetInfluence=-2f;
    private float _timeOfIndicatorInfluence = .25f;
    private bool _getInfluence = false;
    public void GetDamage(float damage) {
        if (_isPaused) return;
        _spriteRenderer.color = Color.red;
        _timeGetInfluence = Time.time;
        _getInfluence = true;
        Health -= damage * (1 - _armor);
        CheckHealthLose();
    }
    public void GetReduceFuel(float reduceKof)
    {
        if (_isPaused) return;
        _spriteRenderer.color = new Color(0.4803311f, 0.7631885f, 0.990566f);
        _timeGetInfluence = Time.time;
        _getInfluence = true;
        Fuel -= reduceKof;
        CheckFuelLose();
    }
    /// <summary>
    /// This property return preasure kof pressure protection[0,1] (0 = 0%, 1 = 100%)
    /// if  numb<0 that it is kof of damage
    /// </summary>
    public float PreausreInfluence{
        get {
            if (machinePos.y > 0) return 1;     
            return 1-Math.Abs(machinePos.y)/ _amountBlocksWithNormalPreasure;
        }
    }
    /// <summary>
    /// This property return temperature kof temperature protection[0,1] (0 = 0%, 1 = 100%)
    /// if  numb<0 that it is kof of damage
    /// </summary>
    public float TemperatureInfluence
    {
        get
        {
            if (machinePos.y > 0) return 1;
            return 1 - Math.Abs(machinePos.y) / _amountBlocksWithNormalTemperature;
        }
    }
    /// <summary>
    /// If machine was Fallen this method check if speed was raised limet that would damage the machine
    /// </summary>
   
    public bool MachineFallen()
    {
        if (_isPaused) return false;
        float fallenSpeed = _rigid.velocity.y;
        if (fallenSpeed > 0) return false;
        if (Math.Abs(fallenSpeed) > _minFalingSpeed)
        {
            _spriteRenderer.color = Color.red;
            _timeGetInfluence = Time.time;
            _getInfluence = true;
            float damage = Math.Abs(_damageKofForFalling * (fallenSpeed / _minFalingSpeed) * (1 - _armor));
            Health -=damage;
            _badFallenParticles.SetActive(true);
            GetDamageText(damage);
            CheckHealthLose();
            return true;
        }
        return false;
    }
    public void GetDamageText(float damage) {
        //set fly text options
        try { 
            Vector2 lastTextPoint = new Vector2(machinePos.x + UnityEngine.Random.Range(-3f, 3f), machinePos.y + 2);
            FlyText fT = UIManager.S.MessageForUser(new List<Vector2>() { machinePos, lastTextPoint }, Easing.InOut, Time.time, 1.5f);
            fT.SetColorChange(new List<Color>() { new Color(1, 0, 0, 1), new Color(1, 0.634616f, 0, 1), new Color(1, 0.1836021f, 0, 0) });
            fT.SetRectSize(new List<Vector2>() { new Vector2(0.4f, 0.4f), new Vector2(0.75f, 0.75f) });
            fT.SetCommonText("-" + ((short)damage).ToString());
        }
        catch (System.NullReferenceException) {}
    }
    //info for player in texts
    private bool _isBlockPossibleDestroyFinish = true;
    private bool _isLevelBurTextFinish = true;
    private bool _isDamageFromPreasureFinish = true;
    private bool _isDamageFromTemperatureFinish = true;
    /// <summary>
    /// This method calls when imeDuration of text with info about bur is over
    /// </summary>
    public void FinishTextLevelBur() => _isLevelBurTextFinish = true;
    /// <summary>
    /// This method calls when timeDuration of text with info about possible to destroy 
    /// </summary>
    public void FinishTextPossibleToDestroy() => _isBlockPossibleDestroyFinish = true;
    public void FinishDamageFromTemperature() => _isDamageFromTemperatureFinish = true;
    public void FinishDamageFromPreasure() => _isDamageFromPreasureFinish = true;
    public Vector2 machinePos 
        => transform.position;
    public bool IsFlipRight 
        => _spriteRenderer.flipX; 
    public Rigidbody2D Rigid 
        => _rigid;
    //inteface's properties
    public float MaxHealth 
        => _healthMax;
    public float MaxFuel 
        => _fuelMax;
    
    public float Health {
        get => _currentHealth;
        set => _currentHealth = value;
    }

    public float Fuel {
        get => _currentFuel;
        set => _currentFuel = value;
    }
    public byte LevelOfBur {
        get => _burLevel;
        set => _burLevel=value;
    }
    public byte MaxDepthToReach 
        => Math.Min(_amountBlocksWithNormalPreasure,_amountBlocksWithNormalTemperature);

    public float ArmorKof
        => _armor;
    public float GunDamage
        => _machineGun.Damage;
    public float GunFireRate
        => _machineGun.FireRate;
    public float PosY 
        => machinePos.y;
    public float PosX 
        => machinePos.x;
    public Vector2 MachinePosInBlocks {
        get {
            return new Vector2(Mathf.Round(PosX), Mathf.Ceil(PosY));
        }
    }
    //interface's Methods
    public void AddDamageForBur(float damage)
    {
        _burDamage += damage;
        _burLevel++;
    }
    public void AddSpeedForFlying(float speed) =>_speedFly+=speed;
    public void AddSpeedForRiding(float speed)=>_speedMove+=speed;
    public void AddArmor(float armor, byte amountBlocksWithNormalPreasure, byte improveHealthCapacity) {
        _armor += armor;
        _amountBlocksWithNormalPreasure += amountBlocksWithNormalPreasure;
        _healthMax += improveHealthCapacity;

    }
    public void AddBlocksForImproveSystemCold(byte amountBlocksWithNormalTemperature) {
        _amountBlocksWithNormalTemperature += amountBlocksWithNormalTemperature;
    }
    public void AddGunPower(float damage, float firerate = 0.05f) {
        _machineGun.ImproveGun(damage, firerate);
    }
    public void ImproveFuelRates(float fuelCons, byte fuelCapacityImprove) {
        float r = -Mathf.Abs(fuelCons);
        _fuelMovingConsumption -= r;
        _fuelFlyingConsumption -= r;
        _fuelDrillingConsumption -= r;
        _fuelMax += fuelCapacityImprove;
    }
    private Vector2 _rigidVelocityBeforePaused = Vector2.zero;
    private bool _isPaused = false;
    public void SetPaused(bool isPaused) {
        _isPaused = isPaused;
        if (isPaused == true)
        {
            _audioSource.Stop();
            _animator.speed = 0;
            _rigidVelocityBeforePaused = _rigid.velocity;
            _rigid.Sleep();
        }
        else {
            _rigid.WakeUp();
            _animator.speed = 1;
            _rigid.velocity = _rigidVelocityBeforePaused;
        }
    }
    
}
