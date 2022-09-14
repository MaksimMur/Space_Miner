using UnityEngine;

public class ItemPointer : MonoCache
{
    private SpriteRenderer _sp;
    public ItemType Type;
    private void Awake()
    {
        _sp = GetComponent<SpriteRenderer>();
    }
    public void SetColor(Color r)
    {
        _sp.color = r;
    }
    public void SetSprite(Sprite sprite) {
        _sp.sprite = sprite;
    }

    bool _triggerStay = false;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<Block>() || collision.GetComponent<ItemBuilding>())
        {
            _triggerStay = true;
        }
       
    }
    private float _timeExit = -2;
    private float _timeToBuild = 0.05f;
    private void OnTriggerExit2D(Collider2D collision)
    {
        _timeExit = Time.time;
        _triggerStay = false;
    }
    public override void OnTick()
    {
  
        if (_triggerStay || Pos.y>0 || _timeExit + _timeToBuild > Time.time || (Type==ItemType.Plate && Plate.platesPosition.ContainsKey(transform.position)) ) {
            SetColor(Color.red);
            PossibleToBuild = false;
            return;
        }
        SetColor(Color.green);
        PossibleToBuild = true;
    }
    public Vector2 Pos => transform.position;
    public bool PossibleToBuild { get; set; }
}
