using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour,IHints
{
    public static UIManager S;
    [Header("Set in Inspector: UIManager options")]
    [SerializeField] private Transform _flyTextsAnchor;
    [SerializeField] private short _poolCountFlyTexts=5;
    [SerializeField] private FlyText _flyTextPrefab;

    private AudioSource _audioSource;
    private PoolMono<FlyText> _poolFlyTexts;
    public void Awake()
    {
        S = this;
        _audioSource = GetComponent<AudioSource>();
        _poolFlyTexts = new PoolMono<FlyText>(_flyTextPrefab, _poolCountFlyTexts, _flyTextsAnchor);
        _poolFlyTexts.autoExpand = true;
        Options.S.RegisterHint(this);
    }
    private bool _showHint;
    public void SetHints(bool showHint)
    {
        _showHint = showHint;
    }
    public FlyText MessageForUser(List<Vector2> ePts, string curve, float eTimeS = 0, float eTimeD = 1, GameObject finsihTo = null,string finishNameFunc="",bool isLocalizedText=false,string localizedTextKey="Default") {
        if (!_showHint) return null;
        FlyText t = _poolFlyTexts.GetFreeElemet();
        t.Init(ePts,curve,eTimeS,eTimeD,finsihTo,finishNameFunc,isLocalizedText,localizedTextKey);
        return t;
    }
    public void PlaySound(AudioClip clip) {
        _audioSource.PlayOneShot(clip);
    }
}
