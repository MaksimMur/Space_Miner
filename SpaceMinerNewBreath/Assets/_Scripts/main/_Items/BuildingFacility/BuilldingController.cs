using UnityEngine;
public class BuilldingController : MonoBehaviour
{
    [Header("Set in Inspector: building Controler options")]
    public Joystick j;
    [SerializeField] private ItemPointer _buildingPointerPrefab;
    [SerializeField] private ItemBuilding _buildingPrefab;
    [SerializeField] private byte _buildingRangeInBlocks=4;
    [SerializeField] private AudioClip _setBuildingSound;
    private ItemPointer _buildingPointer;
    private SpriteRenderer _spPointer;
    private AudioSource _auduoSource;
    private void Awake()
    {
        _auduoSource = GetComponent<AudioSource>();
        _buildingPointer = Instantiate(_buildingPointerPrefab);
        _buildingPointer.SetSprite(_buildingPrefab.GetComponent<SpriteRenderer>().sprite);
    }
    private void OnEnable()
    {
        _buildingPointer.gameObject.SetActive(true);
    }
    private void OnDisable()
    {
        try
        {
            _lastPointerPos = Vector2.zero;
            _buildingPointer.gameObject.SetActive(false);
        }
        catch (MissingReferenceException) {
            return;
        }
    }
    private Vector2 GetRangeWithJoysticInBlocks
    {
        get {
            return new Vector2(Mathf.Round(j.Horizontal*_buildingRangeInBlocks), Mathf.Round(j.Vertical * _buildingRangeInBlocks));
        }
    }
    private Vector2 PointerPos => _buildingPointer.Pos;
    private Vector2 _lastPointerPos = Vector2.zero;
    private void Update()
    {
        try
        {
            if (Items.dictItems[_buildingPointer.Type] == 0)
            {
                _buildingPointer.gameObject.SetActive(false);
                return;
            }
            if (Items.dictItems[_buildingPointer.Type] > 0)
            {
                if(!_buildingPointer.gameObject.activeSelf) _buildingPointer.gameObject.SetActive(true);
                if (j.Horizontal == 0 && j.Vertical == 0)
                {
                    _buildingPointer.transform.position = Machine.S.MachinePosInBlocks + _lastPointerPos;
                    return;
                }
                _buildingPointer.transform.position = Machine.S.MachinePosInBlocks + GetRangeWithJoysticInBlocks;
                _lastPointerPos = GetRangeWithJoysticInBlocks;
            }
            else return;
        }
        catch (MissingReferenceException)
        {
            Debug.Log("Pointer was trust");
            return;
        }
    }
    public void SetBuilding() {

        if (_buildingPointer.PossibleToBuild && _buildingPointer.gameObject.activeSelf)
        {
            _auduoSource.PlayOneShot(_setBuildingSound);
            Items.ReduceIvnentoryAmountOfItemsByOne(_buildingPointer.Type);
            ItemBuilding go = Instantiate(_buildingPrefab);
            QuestManager.S.Building(go.NameBuilding);
            go.transform.position = PointerPos;
        }
    }
    public void SetPointerColor(Color r)
    {
        _spPointer.color = r;
    }
}
