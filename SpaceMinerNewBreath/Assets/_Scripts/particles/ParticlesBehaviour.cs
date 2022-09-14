using UnityEngine;

public class ParticlesBehaviour : MonoCache, IPausedController
{
    [SerializeField] protected float _lifeTime = 1.5f;
    [SerializeField] private bool destroyedAfterCreate = false;
    protected float _timeStart = -2;
    protected AudioSource _audioSource;
    protected bool _IsPaused;

    public void SetAudioClip(AudioClip clip, bool playClip)
    {
        _audioSource.clip = clip;
        if (playClip) _audioSource.Play();
    }
    protected virtual void Awake()
    {
        _timeStart = Time.time;
        PauseManager.S.Register(this);
        _audioSource = GetComponent<AudioSource>();
    }
    protected override void OnEnable()
    {
        _timeStart = Time.time;
        if (_audioSource!=null &&_audioSource.clip != null) _audioSource.Play();
        base.OnEnable();
    }
    public override void OnTick()
    {
        if (_IsPaused) return;
        if (_timeStart + _lifeTime < Time.time) {
            if (destroyedAfterCreate) Destroy(this.gameObject);
            this.gameObject.SetActive(false);
        }
    }
    [System.Obsolete]
    public void SetColor(Color color) {
        GetComponent<ParticleSystem>().startColor = color;
    }
    //pauseLogic
    protected float _timeLifeInDuringPause;
    public virtual void SetPaused(bool ispaused)
    {
        _IsPaused = ispaused;
        if (ispaused)
        {
            GetComponent<ParticleSystem>().Pause();
            _timeLifeInDuringPause = Time.time-_timeStart;
            return;
        }
        _timeStart = Time.time - _timeLifeInDuringPause;
        GetComponent<ParticleSystem>().Play();
    }
    
}
