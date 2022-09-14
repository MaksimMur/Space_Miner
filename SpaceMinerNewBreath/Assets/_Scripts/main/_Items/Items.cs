using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
    private static Items S;
    [Header("Set items options")]
    [SerializeField] private AudioClip _immossibleActionSound;
    [SerializeField] private Sprite[] _itemsSprites;
    public static Dictionary<ItemType, Sprite> dictItemsSprite = new Dictionary<ItemType, Sprite>();
    public static Dictionary<ItemType, int> dictItems;
    public static Dictionary<ItemType, short> dictItemsPrice = new Dictionary<ItemType, short>() {
        { ItemType.TNT, 1500 }, { ItemType.Fuel, 1000 }, { ItemType.RepairKit, 750 },
        { ItemType.MagniteImpulse, 200 },{ ItemType.Lamp, 500 },{ ItemType.Plate,250 },
        { ItemType.GasCleaner,400 }  
    };
    public static Dictionary<ItemType, Rect> dictItemsSpriteRectSize = new Dictionary<ItemType, Rect>() {
            { ItemType.TNT, new Rect(new Vector2(-17.2812f,92.879f),new Vector2(166.6131f,152.96f)) }, { ItemType.Fuel, new Rect(new Vector2(-15.582f,95.759f),new Vector2(151.6323f,139.2068f)) }, { ItemType.RepairKit,new Rect(new Vector2(-19.234f,95.558f),new Vector2(178.5982f,140.1699f))  },
        { ItemType.MagniteImpulse, new Rect(new Vector2(-19.234f,95.558f),new Vector2(178.5982f,140.1699f))  },{ ItemType.Lamp,new Rect(new Vector2(-9.6141f,95.558f),new Vector2(107.5732f,140.1699f))},{ ItemType.Plate,new Rect(new Vector2(-27.459f,120.7428f),new Vector2(242.59f,51.0529f)) },
        { ItemType.GasCleaner,new Rect(new Vector2(-21.153f,96.871f),new Vector2(215.7389f,133.8949f))}

    };
    public static Dictionary<ItemType, string> dictItemsNameTranslaterKey = new Dictionary<ItemType, string>() {
        { ItemType.TNT, "Explosives"},{ ItemType.Fuel, "Fuel"},{ ItemType.RepairKit, "RepairKit"},
        { ItemType.MagniteImpulse, "MagniteImpulse"},{ ItemType.Lamp, "Lamp"},{ ItemType.Plate, "Plate"},
        { ItemType.GasCleaner, "VacuumCleaner"}
    };
    [SerializeField]private short _itemsCapacity=20;
    [SerializeField] private short _currentAmountItems = 0;

    [Header("Set TNT options")]
    [SerializeField] private AudioClip _setTNTSound;
    [SerializeField] private TNT _tntPrefab;
    [SerializeField] private PoolMono<TNT> _tntPool;
    [SerializeField] private TNTExp _tntParticlesPrefab;
    [SerializeField] private PoolMono<TNTExp> _tntParticlesPool;
    [SerializeField] private Transform _tntAnchhor;
    [SerializeField] private short _poolTNTCapacity=5;
    [Header("Set lamp Bomb options")]
    [SerializeField] private AudioClip _setLampSound;
    [SerializeField] private Lamp _lampPrefab;
    [SerializeField] private PoolMono<Lamp> _lampPool;
    [SerializeField] private Transform _lampAnchhor;
    [SerializeField] private short _poolLampCapacity = 5;


    [Header("Set Fuel options")] 
    [SerializeField] private float _amountFillingFuel = 50;
    [SerializeField] private AudioClip _fillSound;
    [Header("Set RepairKit options")]
    [SerializeField] private float _amountHealth = 50;
    [SerializeField] private AudioClip _repairSound;
    [Header("Set Vacuum options")]
    [SerializeField] private AudioClip _setGasVacuumSound;
    [SerializeField] private Lamp _gasVacuumPrefab;
    [SerializeField] private PoolMono<Lamp> _gasVacuumPool;
    [SerializeField] private Transform _gasVacuumAnchhor;
    [SerializeField] private short _poolGasVacuumCapacity = 5;


    private AudioSource _audioSource;
    private void Awake()
    {
        S = this;
        _audioSource = GetComponent<AudioSource>();
        dictItems = new Dictionary<ItemType, int>();
        for (byte i = 0; i < _itemsSprites.Length; i++) {
            if (dictItemsSprite.ContainsKey((ItemType)i)) continue;
            dictItemsSprite.Add((ItemType)i, _itemsSprites[i]);
        }
        for (byte i = 0; i < SelectedPlanet.S.selectedPlanet.items.Length; i++)
        {
            dictItems.Add(SelectedPlanet.S.selectedPlanet.items[i], 10);
        }
    }
    private void Start()
    {
        _tntPool = new PoolMono<TNT>(_tntPrefab, _poolTNTCapacity, _tntAnchhor);
        _tntParticlesPool = new PoolMono<TNTExp>(_tntParticlesPrefab, _poolTNTCapacity, _tntAnchhor);
        _lampPool = new PoolMono<Lamp>(_lampPrefab, _poolLampCapacity, _lampAnchhor);
        _tntPool.autoExpand = true;
        _tntParticlesPool.autoExpand = true;
        _lampPool.autoExpand = true;
    }
    public static bool ENOUGH_PLACE => S._currentAmountItems < S._itemsCapacity;
    public static short CURRENT_AMOUNT_ITEMS=>S._currentAmountItems;
    public static short CAPACITY => S._itemsCapacity;
    public static void IncreaseIvnentoryAmountOfItemsByOne(ItemType itemType) {
        if (ENOUGH_PLACE)
        {
            dictItems[itemType]++;
            S._currentAmountItems++;
        }
    }
    public static void ReduceIvnentoryAmountOfItemsByOne(ItemType itemType)
    {
        
        if (dictItems[itemType]>0)
        {
            MachineInterface.S.UseItem(itemType);
            Statistics.S.ItemUsed();
            dictItems[itemType]--;
            S._currentAmountItems--;
        }
    }
    //tnt 
    public void SetTNT()
    {
        if (dictItems[ItemType.TNT] > 0 && Machine.S.machinePos.y < 0)
        {
            QuestManager.S.UseItem(ItemType.TNT);
            _audioSource.PlayOneShot(_setTNTSound);
            ReduceIvnentoryAmountOfItemsByOne(ItemType.TNT);
            var tnt = _tntPool.GetFreeElemet();
            tnt.transform.position = Machine.S.machinePos;
            return;
        }
        _audioSource.PlayOneShot(_immossibleActionSound);
    }
    //lamp bomb
    public void SetLamp() {
        if (dictItems[ItemType.Lamp] > 0 && Machine.S.machinePos.y < 0)
        {
            QuestManager.S.UseItem(ItemType.Lamp);
            _audioSource.PlayOneShot(_setLampSound);
            ReduceIvnentoryAmountOfItemsByOne(ItemType.Lamp);
            var lamp = _lampPool.GetFreeElemet();
            lamp.transform.position = Machine.S.machinePos;
            return;
        }
        _audioSource.PlayOneShot(_immossibleActionSound);
    }
    public static void SetTNTExp(Vector2 tntPos) {
        var particles = S._tntParticlesPool.GetFreeElemet();
        particles.transform.position = tntPos;
    }
    //fuel
    public void FillFuel()
    {
        if (dictItems[ItemType.Fuel] > 0)
        {
            QuestManager.S.UseItem(ItemType.Fuel);
            _audioSource.PlayOneShot(_fillSound);
            ReduceIvnentoryAmountOfItemsByOne(ItemType.Fuel);
            Machine.S.Fuel = Mathf.Min(Machine.S.MaxFuel, Machine.S.Fuel + _amountFillingFuel);
            return;
        }
        _audioSource.PlayOneShot(_immossibleActionSound);
    }
    //repairkit
    public void Repair()
    {
        if (dictItems[ItemType.RepairKit] > 0)
        {
            QuestManager.S.UseItem(ItemType.RepairKit);
            _audioSource.PlayOneShot(_repairSound);
            ReduceIvnentoryAmountOfItemsByOne(ItemType.RepairKit);
            Machine.S.Health = Mathf.Min(Machine.S.MaxHealth, Machine.S.Health + _amountHealth);
            return;
        }
        _audioSource.PlayOneShot(_immossibleActionSound);
    }
    public static void ImproveCapacity(short increaseCapacituValue) {
        S._itemsCapacity += increaseCapacituValue;
    }
}
