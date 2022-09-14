using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasBlockLeakageRadius : MonoBehaviour,IHints
{
    [Header("Set in Inspector: GasHealthBlock options")]
    [Range(0, 1)] [SerializeField] private float _chanceToSprayingGas = 0.3f;
    private bool _showHints;
    private void Awake()
    {
        Options.S.RegisterHint(this);
    }
    [System.Obsolete]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Machine>() && Random.Range(0f, 1f) < _chanceToSprayingGas)
        {
            if (_showHints)
            {
                try
                {
                    FlyText t = UIManager.S.MessageForUser(new List<Vector2>() { Machine.S.machinePos, new Vector2(Machine.S.machinePos.x, Machine.S.machinePos.y + 1) }, Easing.Out, 0, 1.5f, null, default, true, "GasLeak");
                    t.SetRectSize(new List<Vector2>() { new Vector2(6, 0.25f) });
                    t.SetColorChange(new List<Color>() { Color.red });
                }
                catch (System.NullReferenceException) { }
            }
            this.GetComponentInParent<Block>().Destroyed(false);
        }
    }
    public void OnDestroy()
    {
        Options.S.UnRegisterHint(this);
    }

    public void SetHints(bool show)
    {
        _showHints = show;
    }
}
