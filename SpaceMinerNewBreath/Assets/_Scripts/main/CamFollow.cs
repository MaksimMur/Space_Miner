using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoCache
{
    public static Transform POI;
    [Range(0,1)]
    public float dynamicKoff;
    private float _yCamBoarder,_xCamBoarder;
    private float _xMaxLength;
    private float _yMaxLength = 10;
    public void Awake()
    {
        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0)); // bottom-left corner
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1)); // top-right corner
        _xCamBoarder = (max.x - min.x)/2;
        _yCamBoarder = (max.y - min.y)/2;
        POI = GameObject.Find("Machine").GetComponent<Transform>();
    }
    public void Start()
    {
        _xMaxLength =1f*BlockManager.S.LengthMap/2;

    }
    Vector3 pos {
        get => transform.position;
        set => transform.position = value;
    }
    public void Update()
    {
        Vector3 target = new Vector3(
        POI.position.x,
        POI.position.y,
        POI.position.z-10
        );
        
        pos= Vector3.Lerp(POI.transform.position, target, dynamicKoff);
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
        if (pos.x - _xCamBoarder <= -_xMaxLength) pos = new Vector3(-_xMaxLength+_xCamBoarder, pos.y, pos.z);
        if (pos.x + _xCamBoarder >= _xMaxLength) pos = new Vector3(_xMaxLength - _xCamBoarder, pos.y, pos.z);
        if (pos.y+_yCamBoarder-1 >= _yMaxLength) pos = new Vector3(pos.x, _yMaxLength-_yCamBoarder+1,pos.z);
    }
}
