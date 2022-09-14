using UnityEngine;

public class ParallaxStars : MonoCache
{
    [Header("Set In Inspector: parallax stars options")]
    public GameObject fStarsGroup;
    public GameObject sStarsGroup;
    public float scrollRespawnPosX=35;
    public float scrollingSpeed=.2f;
    private Transform _trStars1, _trStars2;
    private float _x1, _x2, _y1, _y2;
    private void Awake()
    {
        _trStars1 = fStarsGroup.GetComponent<Transform>();
        _trStars2 = sStarsGroup.GetComponent<Transform>();
        _y1 = _trStars1.position.y;
        _y2 = _trStars2.position.y;
    }
    public override void OnTick()
    {
        _x1 = _trStars1.position.x - scrollingSpeed * Time.deltaTime;
        _x2 = _trStars2.position.x - scrollingSpeed * Time.deltaTime;
        if (_x1 <= -scrollRespawnPosX) { _trStars1.position = new Vector2(scrollRespawnPosX, _y1); }
        else _trStars1.position = new Vector2(_x1, _y1);
        if (_x2 <= -scrollRespawnPosX) _trStars2.position = new Vector2(scrollRespawnPosX, _y2);
        else _trStars2.position = new Vector2(_x2, _y2);
    }
}
