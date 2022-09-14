using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
public class MachineInterface : MonoCache, ITransferInventoryShopInfo
{
    public static MachineInterface S;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip buttonClickSoundForClose;

    [Header("Set In InspectorL: joystic options")]
    [SerializeField] private Image joysticHandle;
    [SerializeField] private Sprite machineMovementHandle;
    [SerializeField] private Sprite machineDrillingHandle;

    [Header("Set In Inspector: inventory options")]
    [SerializeField]private byte _inventoryCapacity = 20;
    [SerializeField]private Transform _inventoryAnchor;

    [Header("Set In Inspector: indicators options")]
    [SerializeField] private Image _indicator;


    [Header("Set in Inspector: balance options")]
    [SerializeField] private Text T_Balance;

    [Header("Set in Inpsector: main machine's stats")]
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _fuelSlider;
    [SerializeField] private Image I_Preasure;
    [SerializeField] private Text T_Preasure;
    [SerializeField] private Image I_Temperature;
    [SerializeField] private Text T_Temperature;
    [SerializeField] private Text T_MachineBlocksDepth;

    [Header("Set InInspector: items interface")]
    [SerializeField] private Transform _itemsAnchor;
    private List<ItemUI> _listItems;


    private byte _currentFillInventoryPlace = 0;
    private List<InventoryInformation> _listInventoryOre;
    private Transform _indicatorsAnchor;
    private AudioSource _audioSource;
    private GameObject _uiElemnets;
    List<Image> _indicatorsList;


    private int _balance = 0;
    private IMachineCharacteristic Imachine;
    private ITransferInventoryShopInfo Ishop;

    protected void Awake()
    {
        //Singlton
        if (S == null) S = this;
        else Debug.Log("MachineInterface.Awake(): was try to connect in second time");
        try
        {
            _indicatorsAnchor = GameObject.Find("Indicators").GetComponent<Transform>();
            _uiElemnets = GameObject.Find("UI_Elements").gameObject;
        }
        catch (NullReferenceException)
        {
            Debug.LogError("Some objects didn't find in Hierarchy");
        }
        _indicatorsList = new List<Image>();
        _indicator.transform.localScale = Vector3.one;
        _listInventoryOre = new List<InventoryInformation>();
        _listItems = new List<ItemUI>(_itemsAnchor.GetComponentsInChildren<ItemUI>());
        _audioSource = GetComponent<AudioSource>();
        
    }
    private void Start()
    {
        //get links on interfaces
        Imachine = Machine.S.GetComponent<IMachineCharacteristic>();
        Ishop = Shop.S.GetComponent<ITransferInventoryShopInfo>();
        //get items that can used on selected planet
        SetItems(SelectedPlanet.S.selectedPlanet);
        //declare indictators with inventory' capacity
        DeclareIndicatorInInterface(_inventoryCapacity);
        //declare information about ore (sprites) 
        DeclareBlocksInvetoryInforamtions(SelectedPlanet.S.selectedPlanet);
    }
    public override void OnTick()
    {
        IllustrateHealthAndFuelSliderInfo();
        IllustrateBlocksDepth(Imachine.PosY);
    }
    /// <summary>
    /// This method set indicator on the inventory panel
    /// </summary>
    /// <param name="n"></param>
    /// <param name="improve">if improve==true that max capacity increasing</param>
    public void DeclareIndicatorInInterface(byte n, bool improve = false)
    {
        if (improve) _inventoryCapacity += n;
        Image g;
        for (int i = 0; i < n; i++)
        {
            g = Instantiate(_indicator);
            g.transform.SetParent(_indicatorsAnchor, false);
            _indicatorsList.Add(g);
        }
    }
    /// <summary>
    /// This method add little blocks sprites and image's blocks in inventory 
    /// </summary>
    /// <param name="selectedPlanet"></param>
    public void DeclareBlocksInvetoryInforamtions(Planet selectedPlanet) {
        _listInventoryOre = _inventoryAnchor.GetComponentsInChildren<InventoryInformation>().ToList();
        for (int i = 0; i < selectedPlanet.littleBlocks.Length; i++) {
          
           _listInventoryOre[i].IntitializeInventoryInforamtion(selectedPlanet.littleBlocks[i], i);
        }
    }
    /// <summary>
    /// This Methhod fill place in inventory and called after Destroy block
    /// </summary>
    /// <param name="type"> blocktype </param>
    public void FillOnePlaceInInventory(BlockType type) {
        sbyte indexOre = IndexOre(type);
        if (indexOre == -1) return;
        _currentFillInventoryPlace++;
        SetIndicator();
        _listInventoryOre[indexOre].tAmountBlocks.text = (sbyte.Parse(_listInventoryOre[indexOre].tAmountBlocks.text) + 1).ToString("00");
    }

   
    public void SetIndicator() => _indicatorsList[_currentFillInventoryPlace - 1].color = Color.yellow;
    public void ResetIndicators() {
        for (int i = 0; i < _inventoryCapacity; i++) {
            if (i < _currentFillInventoryPlace) {
                _indicatorsList[i].color = Color.yellow;
                continue;
            }
            _indicatorsList[i].color = Color.red;
        }
    }
    public bool EnoughInvenoryPlace => _currentFillInventoryPlace != _inventoryCapacity;

    //Opening and closing inventory and missions panels and options panel

    private bool _panelIsClosed = false;
    public void OpenOptionsPanel(GameObject g)
    {
        g.SetActive(true);
        _audioSource.PlayOneShot(buttonClickSound);
        g.GetComponent<Animator>().CrossFade("game_panel_open",0);
        PauseManager.S.SetPaused(true);
        _panelIsClosed = false;
        _uiElemnets.SetActive(false);
    }
    public void CloseOptionsPanel(GameObject g)
    {
        if (_panelIsClosed) return;
        g.GetComponent<Animator>().CrossFade("game_panel_close",0);
        _panelIsClosed = true;
        _uiElemnets.SetActive(true);
        _audioSource.PlayOneShot(buttonClickSoundForClose);
    }

    /// <summary>
    /// This method return amount of ore definte type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public byte GetAmountTypeOre(BlockType type) {
        sbyte indexOre = IndexOre(type);
        if (indexOre == -1) return 0;
        return byte.Parse(_listInventoryOre[indexOre].tAmountBlocks.text);
    }

    public bool OreTypeEnough(BlockType type) {
        sbyte indexOre = IndexOre(type);
        if (indexOre == -1)return false;
        return byte.Parse(_listInventoryOre[indexOre].tAmountBlocks.text) > 0 ? true : false;
    }
    public void TopItOneOre(BlockType type) {
        if (OreTypeEnough(type)) {
            sbyte indexOre = IndexOre(type);
            if (indexOre == -1) return;
            _listInventoryOre[indexOre].tAmountBlocks.text = (byte.Parse(_listInventoryOre[indexOre].tAmountBlocks.text) - 1).ToString("00");
            //_currentFillInventoryPlace--;
            //_indicatorsList[_currentFillInventoryPlace].color = Color.red;       
        }
    }
    private void ResetIvnentoryAfterVisitingShop()
    {
        List<byte> list = Ishop.GetAmountOre();
        _currentFillInventoryPlace = (byte)list.Sum(x => x);
        for (short i = 0; i < _listInventoryOre.Count; i++)
        {
            _listInventoryOre[i].tAmountBlocks.text = list[i].ToString("00");
        }
        ResetIndicators();
    }
    public void SetBalance(int money = 0)
    {
        _balance += money;
        T_Balance.text = _balance.ToString() + "$";
    }
    private void ReSetBalance(int money = 0)
    {
        _balance = money;
        T_Balance.text = _balance.ToString() + "$";
    }

    //main stats
    private Color _normalLevelStats = new Color(0.1354f, 0.6603774f, 0);
    /// <summary>
    /// This method contol value in slideres with machin's halth and fuel
    /// </summary>
    private void IllustrateHealthAndFuelSliderInfo()
    {
        _healthSlider.value = Imachine.Health / Imachine.MaxHealth;
        _fuelSlider.value = Imachine.Fuel / Imachine.MaxFuel;
    }
    /// <summary>
    /// this method illustrate preasure's influence 
    /// </summary>
    /// <param name="f">kof of Preasure [...,1]</param>
    public void IllustratePreasure(float f) {
        f = Mathf.Max(0, f);
        if (f > 1) throw new ArgumentException("this kof can't be bigger than one");
        I_Preasure.color = Color.Lerp(Color.red, _normalLevelStats, f);
        T_Preasure.text = Math.Round(f * 100, 2) + "%";
    }

    /// <summary>
    /// this method illustrate temperatures's influence 
    /// </summary>
    /// <param name="f">kof of temprature [...,1]</param>
    public void IllustrateTemperature(float f)
    {
        f = Mathf.Max(0, f);
        if (f > 1) throw new ArgumentException("this kof can't be bigger than one");
        I_Temperature.color = Color.Lerp(Color.red, _normalLevelStats, f);
        T_Temperature.text = Math.Round(f * 100, 2) + "%";
    }
    /// <summary>
    /// Illusterate depth that machine acheved
    /// </summary>
    /// <param name="machinePosY"></param>
    public void IllustrateBlocksDepth(float machinePosY) =>
       T_MachineBlocksDepth.text = $"{Mathf.Floor(machinePosY)}";

    ////////////////////////////////////////
    //items
    private byte _pointerOnItem = 0;
    public void SetItems(Planet planet) {
        List<ItemUI> itemsOnSelecletedPlanet = new List<ItemUI>();
        for (byte i = 0; i < _listItems.Count; i++) {
            for (byte j = 0; j < planet.items.Length; j++) {
                if (_listItems[i].Type == planet.items[j])
                {
                    itemsOnSelecletedPlanet.Add(_listItems[i]);
                }
                _listItems[i].gameObject.SetActive(false);
                
            }
        }
        try
        {
            _listItems = itemsOnSelecletedPlanet;
            _listItems[0].gameObject.SetActive(true);
        }
        catch (ArgumentException) {
            return;
        }
    }
    public void ItemNext() {
        try
        {
            if (_pointerOnItem+1 == _listItems.Count)
            {
                _listItems[_pointerOnItem].gameObject.SetActive(false);
                _listItems[0].gameObject.SetActive(true);
                _pointerOnItem = 0;
            }
            else {
                _listItems[_pointerOnItem].gameObject.SetActive(false);
                _listItems[_pointerOnItem+1].gameObject.SetActive(true);
                _pointerOnItem++;
            }
        }
        catch (ArgumentException)
        {
            return;
        }

    }
    public void ItemPrev()
    {
        try
        {
            if (_pointerOnItem - 1 == -1)
            {
                _listItems[_pointerOnItem].gameObject.SetActive(false);
                _listItems[_listItems.Count-1].gameObject.SetActive(true);
                _pointerOnItem = (byte)(_listItems.Count-1);
            }
            else
            {
                _listItems[_pointerOnItem].gameObject.SetActive(false);
                _listItems[_pointerOnItem - 1].gameObject.SetActive(true);
                _pointerOnItem--;
            }
        }
        catch (ArgumentException)
        {
            return;
        }

    }

    public void UseItem(ItemType type) {
        _listItems[(byte)type].T_Amount.text = (Items.dictItems[type]-1).ToString();
    }
    public void ResetItemsCountAfterVisitingShop() {
        for (byte i = 0; i < _listItems.Count; i++) {
            _listItems[i].T_Amount.text = Items.dictItems[(ItemType)i].ToString();
        }
    }

    //common
    public void ExitShop()
    {
        ResetIvnentoryAfterVisitingShop();
        ReSetBalance(Ishop.GetMoney());
        ResetItemsCountAfterVisitingShop();
    }
    //interfaceMethods
    public List<byte> GetAmountOre()
    {
        List<byte> list = new List<byte>();
        foreach (InventoryInformation i in _listInventoryOre)
        {
            list.Add(byte.Parse(i.tAmountBlocks.text));
        }
        return list;
    }
    //change joystic sprite
    public void ChangeJoysticWithMachineCondition(MachineCondition machineCondition) {
        if (machineCondition == MachineCondition.drilDown || 
            machineCondition == MachineCondition.drilRight || 
            machineCondition == MachineCondition.drilLeft) 
        {
            joysticHandle.sprite = machineDrillingHandle;
            return;
        }
        joysticHandle.sprite = machineMovementHandle;
        
    }
    public int InventoryCapacity => _inventoryCapacity;
    public int GetMoney()
    {
        return _balance;
    }
    public sbyte IndexOre(BlockType type)
    {
        return (sbyte)(type < BlockType.UpperOre || type > BlockType.EighthOre ? -1 : type == BlockType.UpperOre ? 0 : type - BlockType.FirstOre);
    }
}
