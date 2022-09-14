using System.Collections.Generic;
using UnityEngine;

public class Plate : ItemBuilding
{
    private BoxCollider2D _col;
    [SerializeField] [Range(.5f, 1)] private float _effortToPassPlate = .85f;
    public static Dictionary<Vector2, bool> platesPosition =new Dictionary<Vector2, bool>();
    private void Awake()
    {
        _col = GetComponent<BoxCollider2D>();
    }
    private void Start()
    {
        platesPosition.Add(this.transform.position, true);
    }
    bool isActive = true;
    public override void OnTick()
    {
        if (Mathf.Abs(Machine.S.joystic.Vertical) > _effortToPassPlate)
        {
            _col.enabled = false;
            isActive = false;
            return;
        }
        if (!isActive) { 
            _col.enabled = true;
            isActive = true;    
        }
        
    }
}
