using UnityEngine;


public enum lampBehaviour{
idle,
exp
}
public class Lamp : MonoCache
{
    [Header("Lamp options")]
    [SerializeField] private float _timeToExp = 1f;
    private float _timeStart;
    private Animator anim;
    private lampBehaviour behaviour;
    private AudioSource _audioSource;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }
    protected override void OnEnable()
    {
        behaviour = lampBehaviour.idle;
        _timeStart = Time.time;
        base.OnEnable();
    }
    public override void OnTick()
    {
        if (_timeStart + _timeToExp < Time.time &&behaviour!=lampBehaviour.exp) {
            _audioSource.Play();
            behaviour = lampBehaviour.exp;
            anim.CrossFade("Exp", 1);
        }
    }

    public void EndExp() => this.gameObject.SetActive(false);
}
