using UnityEngine;

public class ParalaxBackGround : MonoCache
{
    [SerializeField] private float _speed;
    private Vector2 Pos{
        get=> transform.position;
        set => transform.position = value;
    }
    public override void OnTick() {
        float ty = 0, tx1;
        tx1 = (-Machine.S.machinePos.x) * _speed;
        Pos = new Vector2(tx1, ty);
    }
}
