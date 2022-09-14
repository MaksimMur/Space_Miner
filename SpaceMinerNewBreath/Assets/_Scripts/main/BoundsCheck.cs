using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsCheck : MonoCache
{
    [SerializeField]private float _yMaxLength=10;
    Vector2 pos {
        get => transform.position;
        set => transform.position=value;
    }
    private float _xMaxLength;
    private void Start()
    {
        _xMaxLength = Mathf.Floor(BlockManager.S.LengthMap / 2);
    }
    protected override void OnEnable()
    {
       AddLateUpadte();
    }
    protected override void OnDisable()
    {
        RemoveLateUpdate();
    }
    public override void OnLateTick()
    {
        if (pos.x <= -_xMaxLength) pos = new Vector2(-_xMaxLength, pos.y);
        if (pos.x >= _xMaxLength) pos = new Vector2(_xMaxLength, pos.y);
        if (pos.y >= _yMaxLength) pos = new Vector2(pos.x, _yMaxLength);
    }
}
