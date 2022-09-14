using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;

public class Shop : MonoBehaviour,ITransferInventoryShopInfo
{
    public static Shop S;
    [Header("Set in Inspector: shop options")]
    [SerializeField] private GameObject _shop;
    [SerializeField] private Text _tBalance;
    [SerializeField] private AudioClip _buttonClickSound;
    [SerializeField] private AudioClip _buttonClickCloseSound;
    [SerializeField] private AudioClip _actionImpossibleSound;
    [SerializeField] private AudioClip _sellAnythingSound;
    [SerializeField] private AudioClip _setGreadSound;
    [SerializeField] private AudioClip _butItemsSound;
    [SerializeField] private GameObject[] _hidenUIObjects;
    [SerializeField] private LocalizationManager _localizationManager;
    private ITransferInventoryShopInfo ImachineInerface;
    private IMachineCharacteristic Imachine;
    public GameObject[] _shopPanels;
    

    [Header("Set in Inspcetor: first panel options")]
    [SerializeField] private ShopOreInformation _shopOreInfoPrefab;
    [SerializeField] private Button _bSellAllOre;
    [SerializeField] private Transform _ShopOreAnchor;
    private List<ShopOreInformation> _listShopOre;
    private byte _amountOre = 0;



    [Header("Set in Inspector: second panel options")]
    [SerializeField] private ShopModelsInformation _shopModelInfoPrefab;
    [SerializeField] private Transform _ShopModelsAnchor;
    [SerializeField] private string[] _modelsNameTranslaterKey;
    private List<ShopModelsInformation> _listShopModels;


    [Header("Set in Inspector: third panel options")]
    [SerializeField] private ShopItemsInformation _shopItemInfoPrefab;
    [SerializeField] private Transform _ShopItemsAnchor;
    [SerializeField] private Text _tAmountOfItems;
    [SerializeField] Text _tAmountOfItemsTranslate;
    private List<ShopItemsInformation> _listShopItems;
    private AudioSource _audioSource;
    private int _balance=0;

    [Header("Set in Inspector: fourth panel options")]
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private Slider _fuelSlider;
    [SerializeField] private Text _tTextTopItTranslater;
    [SerializeField] private Text _tTextNotHaveMoneyTranslater;
    [SerializeField] private Text _tTextTopItHP;
    [SerializeField] private Text _tTextTopItFuel;
    [SerializeField] private byte costTopIt = 5;
    [SerializeField] private AudioClip _repairSound;
    [SerializeField] private AudioClip _fillFuelSound;
    [Header("statistics text refers")]
    [SerializeField] private Text _tTextMaxDepth;
    [SerializeField] private Text _tTextMinedBlocks;
    [SerializeField] private Text _tTextDestroyedEnemies;
    [SerializeField] private Text _tTextUsedItems;
    [SerializeField] private Text _tTextCompletedMissions;
    [Header("characteristics text refers")]
    [SerializeField] private Text _tTextMaxOreToDestroy;
    [SerializeField] private Text _tTextMaxDepthWithoutDamage;
    [SerializeField] private Text _tTextArmor;
    [SerializeField] private Text _tTextGunFireRate;
    [SerializeField] private Text _tTextGunDamage;
    [Header("info about upgreads")]
    [SerializeField] private Text _tTextAmountOre;
    [SerializeField] private Text _tTextAmountItems;
    private int _fullCostHealth, _fullCostFuel;
    private byte _amountHealthTopIt, _amountFuelTopIt;
    private void Awake()
    {
        S = this;
        _listShopModels = new List<ShopModelsInformation>();
        _listShopOre = new List<ShopOreInformation>();
        _listShopItems = new List<ShopItemsInformation>();
        _audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        Imachine = Machine.S.GetComponent<IMachineCharacteristic>();
        ImachineInerface = MachineInterface.S.GetComponent<ITransferInventoryShopInfo>();
    }
    //////////////////////////////////////////////
    //FirstPanell
    /// <summary>
    /// This method add little blocks sprites and image's blocks and prices in shop
    /// </summary>
    /// <param name="selectedPlanet"></param>
    public void SetShopOreInforamtions(Planet selectedPlanet)
    {
        for (byte i = 0; i < selectedPlanet.littleBlocks.Length; i++)
        {
            //spawn shop infromation objects about ore
            ShopOreInformation go = Instantiate(_shopOreInfoPrefab);
            go.transform.SetParent(_ShopOreAnchor, false);
            _listShopOre.Add(go);
            _listShopOre[i].SetType(BlockType.FirstOre+i);
            _listShopOre[i].SetSprite(selectedPlanet.littleBlocks[i]);
            _listShopOre[i].localizedText.SetKey(selectedPlanet._blocksNameTransalaterKey[i]);
            //Take Price for block
            try
            {
                _listShopOre[i].tBlockPrice.text = selectedPlanet.BlocksPrice[i].ToString() + "$";
                _listShopOre[i].price = selectedPlanet.BlocksPrice[i];
            }
            catch
            {
                Debug.LogError($"Check massives and how yo fill them in script SelectedPlanet ");
            }
        }
    }
    
    /// <summary>
    /// This method add method in dynamic created buttons Ore
    /// </summary>
    public void DeclareButtonsEventsForOre(Action<BlockType> funcbSellOne, Action<BlockType> funcbSellAll) {
        for (byte i = 0; i < _listShopOre.Count; i++) {
            byte i1 = i;
            _listShopOre[i].bSellOneOre.onClick.AddListener(()=> funcbSellOne(_listShopOre[i1].type));
            _listShopOre[i].bSellAllOre.onClick.AddListener(() => funcbSellAll(_listShopOre[i1].type));
        }
    }
    /// <summary>
    /// set amount ore and buttonColors;
    /// </summary>
    public void DeclareAmountOreToTextAndButtonColor() {
        _amountOre = 0;
        List<byte> l = ImachineInerface.GetAmountOre();
        for (byte i = 0; i < _listShopOre.Count; i++) {
            _amountOre += l[i];
            if (l[i] > 0) SetButtonOreColor(Color.green, i);
            _listShopOre[i].tAmountBlocks.text = l[i].ToString("00");
        }
        if (_amountOre > 0) _bSellAllOre.GetComponent<Image>().color = Color.green;
    }
    public void SetButtonOreColor(Color r, int indexOre) {
        _listShopOre[indexOre].bSellOneOre.GetComponent<Image>().color = r;
        _listShopOre[indexOre].bSellAllOre.GetComponent<Image>().color = r;
    }  
    /// <summary>
    /// event for button to seel sellected type of ore in amount equal one
    /// </summary>
    /// <param name="t">selected block type</param>
    public void SellOneOre(BlockType t) {
        sbyte index = IndexOre(t);
        if (index!=-1 &&AmountOreBiggerThanOne(t)) {
            _audioSource.PlayOneShot(_sellAnythingSound);
            SetBalance(_listShopOre[index].price);
            _amountOre -= 1;
            _listShopOre[index].tAmountBlocks.text = (byte.Parse(_listShopOre[index].tAmountBlocks.text) - 1).ToString("00");
            //makes button red color if amount of ore less than one
            AmountOreBiggerThanOne(t);
            if (_amountOre <= 0) _bSellAllOre.GetComponent<Image>().color = Color.red;
            return;
        }
        _audioSource.PlayOneShot(_actionImpossibleSound);  
    }

    public void SellAllOreOneType(BlockType t)
    {
        sbyte index = IndexOre(t);
        if (index != -1 && AmountOreBiggerThanOne(t))
        {
            _audioSource.PlayOneShot(_sellAnythingSound);
            _amountOre -= byte.Parse(_listShopOre[index].tAmountBlocks.text);
            SetBalance(int.Parse(_listShopOre[index].tAmountBlocks.text)*_listShopOre[index].price);
            _listShopOre[index].tAmountBlocks.text = "00";
            //makes button red color if amount of ore less than one
            AmountOreBiggerThanOne(t);
            if (_amountOre <= 0) _bSellAllOre.GetComponent<Image>().color = Color.red;
            return;
        }
        _audioSource.PlayOneShot(_actionImpossibleSound);
    }

    public void SellAllOre() {
        if (_amountOre > 0) _audioSource.PlayOneShot(_sellAnythingSound);
        else {
            _audioSource.PlayOneShot(_actionImpossibleSound);
        }
        for (byte i = 0; i < _listShopOre.Count; i++) {
            
            SetBalance(int.Parse(_listShopOre[i].tAmountBlocks.text) * _listShopOre[i].price);
            _listShopOre[i].tAmountBlocks.text = "00";
            //makes button red color if amount of ore less than one
            AmountOreBiggerThanOne(_listShopOre[i].type);
        }
        _amountOre = 0;
        _bSellAllOre.GetComponent<Image>().color = Color.red;
    }
    /// <summary>
    /// This methid return true if selected ore has amount bigger than one
    /// </summary>
    /// <param name="indexOre">index ore[0,7)</param>
    /// <returns></returns>
    public bool AmountOreBiggerThanOne(BlockType type) {
        sbyte indexOre = IndexOre(type);
        if (indexOre == -1) throw new Exception("Uncorrect ore type");
        if (byte.Parse(_listShopOre[indexOre].tAmountBlocks.text) > 0) return true;
        SetButtonOreColor(Color.red, indexOre);
        return false;
    }
    
    //FirstPanell
    //////////////////////////////////////////////


    //////////////////////////////////////////////
    //SecondPanel
    public void SetShopModelsInforamtion() {
        for (byte i = 0; i < Models.dictModelsSprites.Count; i++)
        {

            //spawn shop models Gread
            ShopModelsInformation go = Instantiate(_shopModelInfoPrefab);
            go.transform.SetParent(_ShopModelsAnchor, false);
            _listShopModels.Add(go);
            _listShopModels[i].SetType((ModelType)i);
            _listShopModels[i].SetSprite(Models.dictModelsSprites[(ModelType)i]);
            _listShopModels[i].localizedText.SetKey(_modelsNameTranslaterKey[i]);
            _listShopModels[i].tDescription.SetKey(_modelsNameTranslaterKey[i] + "Description");
            //_listShopModels;
            //TakePrice for gread
                _listShopModels[i].tBlockPrice.text = Models.dictModelsPrice[ModelType.Bur + i].ToString() + "$";
                _listShopModels[i].price = Models.dictModelsPrice[ModelType.Bur + i];
                _listShopModels[i].improvePrice= Models.dictModelsImprovePrice[ModelType.Bur + i];
        }
    }
    /// <summary>
    /// This method add method in dynamic created buttons models
    /// </summary>
    public void DeclareButtonsEventsForModels(Action<ModelType> funcbSellOne)
    {
        for (byte i = 0; i < _listShopModels.Count; i++)
        {
            byte i1 = i;
            _listShopModels[i].bUpgread.onClick.AddListener(() => funcbSellOne(_listShopModels[i1].type));
        }
    }
    public void BuyUpgread(ModelType t) {
        byte index = (byte)t;
        if (_listShopModels[index].LevelIsMax || _balance < _listShopModels[index].price)
        {
            _audioSource.PlayOneShot(_actionImpossibleSound);
            return;
        }
        SetBalance(-_listShopModels[index].price);
        _listShopModels[index].price += _listShopModels[index].improvePrice;
        _listShopModels[index].tBlockPrice.text = $"{_listShopModels[index].price}$";
        _listShopModels[index].GreadLevel();
        SetButtonColorForModels();
        Models.SetImprove(t);
        _audioSource.PlayOneShot(_setGreadSound);
    }
    public void SetButtonColorForModels() {
        for (byte i = 0; i < _listShopModels.Count; i++)
        {
            if (_listShopModels[i].LevelIsMax) continue;
            if (_listShopModels[i].price <= _balance) {
                _listShopModels[i].bUpgread.GetComponent<Image>().color = Color.green;
                continue;
            }
            _listShopModels[i].bUpgread.GetComponent<Image>().color = Color.red;
        }
    }

    //SecondPanel
    //////////////////////////////////////////////


    //////////////////////////////////////////////
    //ThirdPanel

    /// <summary>
    /// set blocks with items in shop
    /// </summary>
    /// <param name="planet">This type define items that will consist her</param>
    public void SetShopItemsInforamtion(Planet planet)
    {
        for (byte i = 0; i < planet.items.Length; i++)
        {
            ItemType type = planet.items[i];
            ShopItemsInformation go = Instantiate(_shopItemInfoPrefab);
            go.transform.SetParent(_ShopItemsAnchor, false);
            _listShopItems.Add(go);
            _listShopItems[i].SetType(type);
            _listShopItems[i].SetSprite(Items.dictItemsSprite[type], Items.dictItemsSpriteRectSize[type]);
            //_listShopItems;
            _listShopItems[i].tItemPrice.text = Items.dictItemsPrice[type].ToString() + "$";
            _listShopItems[i].price = Items.dictItemsPrice[type];
            _listShopItems[i].localizedText.SetKey(Items.dictItemsNameTranslaterKey[type]);
            _listShopItems[i].tDescription.SetKey(Items.dictItemsNameTranslaterKey[type] + "Description");
        }

        
    }
    /// <summary>
    /// This method add method in dynamic created buttons items
    /// </summary>
    public void DeclareButtonsEventsForItems(Action<ItemType> funcbSellOne)
    {
        for (byte i = 0; i < _listShopItems.Count; i++)
        {
            byte i1 = i;
            _listShopItems[i].bBuyItem.onClick.AddListener(() => funcbSellOne(_listShopItems[i1].type));
        }
    }
    public void BuyItem(ItemType type) {
        if (!Items.ENOUGH_PLACE) {
            _audioSource.PlayOneShot(_actionImpossibleSound); 
            return;
        }
        if (Items.dictItemsPrice[type] <= _balance) {
            _audioSource.PlayOneShot(_butItemsSound);
            byte index = (byte)Enumerable.Range(0,_listShopItems.Count).Where(x=>_listShopItems[x].type==type).Single();
            _listShopItems[index].tAmountItems.text = (short.Parse(_listShopItems[index].tAmountItems.text) + 1).ToString("00");
            SetBalance(-Items.dictItemsPrice[type]);
            Items.IncreaseIvnentoryAmountOfItemsByOne(type);
            SetButtonsColorWithbalanceForItems();
            ChangeTotalAmountOfItemsText();
            return;
        }
        _audioSource.PlayOneShot(_actionImpossibleSound);
    }
    public void ChangeTotalAmountOfItemsText() {
        _tAmountOfItems.text = $"{_tAmountOfItemsTranslate.text} {Items.CURRENT_AMOUNT_ITEMS}/{Items.CAPACITY}";
    }
    public void GetAmountItems() {
        for (byte i = 0; i < _listShopItems.Count; i++) {
            _listShopItems[i].tAmountItems.text = Items.dictItems[(ItemType)i].ToString("00");
        }

    }
    public void SetButtonsColorWithbalanceForItems() {
        for (byte i = 0; i < _listShopItems.Count; i++)
        {
            if (!Items.ENOUGH_PLACE) {
                _listShopItems[i].bBuyItem.GetComponent<Image>().color=Color.red;
                continue;
            }
            if (_listShopItems[i].price<= _balance) {
                _listShopItems[i].bBuyItem.GetComponent<Image>().color = Color.green;
                continue;
            }
            _listShopItems[i].bBuyItem.GetComponent<Image>().color = Color.red;
        }
    }
    //ThirdPanel
    //////////////////////////////////////////////


    //////////////////////////////////////////////
    //FourthPanel

    public void SetInfoForFourthPanel() {
        _hpSlider.value =  Imachine.Health / Imachine.MaxHealth;
        _fuelSlider.value = Imachine.Fuel / Imachine.MaxFuel;
        SetTopItInfoFourthPanel();
        SetStatisticsFourthPanel();
        SetUpgreadInfoFourthPanel();
        SetCharacteristicsFourthPanel();
    }
    private void SetUpgreadInfoFourthPanel() {
        _tTextAmountOre.text = GetAmountOre().Sum(x=>x).ToString()+"/"+MachineInterface.S.InventoryCapacity;
        _tTextAmountItems.text = Items.CURRENT_AMOUNT_ITEMS + "/" + Items.CAPACITY;
    }
    private void SetStatisticsFourthPanel() {
        _tTextMaxDepth.text = (-Statistics.S.MaxDepth).ToString();
        _tTextMinedBlocks.text = Statistics.S.MinedBlocksAmount.ToString();
        _tTextDestroyedEnemies.text = Statistics.S.DestroyedEnemiesAmount.ToString();
        _tTextCompletedMissions.text = Statistics.S.CompletedMisionsAmount.ToString();
        _tTextUsedItems.text = Statistics.S.UsedItemsAmount.ToString();
    }
    private void SetCharacteristicsFourthPanel() {
        //info about bur level
        byte[] levelOfBlocks = BlockManager.S.GetOreLevelsToDestroy();
        byte machineLevelOfBur = Imachine.LevelOfBur;
        byte indexOfMaxOreToDestroy = 0;
        for (byte i = 0; i < levelOfBlocks.Length; i++) {
            if (machineLevelOfBur < levelOfBlocks[i])break;
            indexOfMaxOreToDestroy = i;
        }
        _tTextMaxOreToDestroy.GetComponent<LocalizedText>().SetKey(SelectedPlanet.S.selectedPlanet._blocksNameTransalaterKey[indexOfMaxOreToDestroy]);
        //info about max depth that machine can reach without damage
        _tTextMaxDepthWithoutDamage.text = (-Imachine.MaxDepthToReach).ToString();
        //info about armor' potential
        _tTextArmor.text = (Imachine.ArmorKof*100).ToString() + "%";
        //info about gun
        _tTextGunFireRate.text = $"{Imachine.GunFireRate} {_localizationManager.GetLocalizedValue("Seconds")}";
        _tTextGunDamage.text = $"{Imachine.GunDamage}";
    }
    private void SetTopItInfoFourthPanel() {
        TopItInfo(Imachine.Health, Imachine.MaxHealth, out _fullCostHealth, _tTextTopItHP, out _amountHealthTopIt);
        TopItInfo(Imachine.Fuel, Imachine.MaxFuel, out _fullCostFuel, _tTextTopItFuel, out _amountFuelTopIt);
    }
    private void TopItInfo(float currentValue, float maxValue, out int fullCost, Text _tTopIt,out byte amountToTopItResourses) {
        fullCost = (int)(Mathf.Ceil(maxValue - currentValue) * costTopIt);
        amountToTopItResourses = (byte)(fullCost / costTopIt);
        if (fullCost == 0)
        {
            _tTopIt.text = "MAX";
            return;
        }
        if (_balance < costTopIt) {
            fullCost = 0;
            amountToTopItResourses = 0;
            _tTopIt.text = _tTextNotHaveMoneyTranslater.text;
            return;
        }
        if (fullCost > _balance)
        {
            fullCost = _balance - _balance % costTopIt;
            amountToTopItResourses = (byte)(fullCost / costTopIt);
        }
        _tTopIt.text = _tTextTopItTranslater.text + " " + fullCost;
        
    }
    public void TopItFuel() {
        if (Imachine.Fuel == Imachine.MaxFuel || _balance < costTopIt) _audioSource.PlayOneShot(_actionImpossibleSound);
        else _audioSource.PlayOneShot(_fillFuelSound);
        SetBalance(-_fullCostFuel);
        Imachine.Fuel = Mathf.Min(Imachine.MaxFuel, Imachine.Fuel + _amountFuelTopIt);
        _fuelSlider.value = Imachine.Fuel / Imachine.MaxFuel;
        SetTopItInfoFourthPanel();
    }

    public void TopItHealth() {
        if (Imachine.Health == Imachine.MaxHealth || _balance < costTopIt) _audioSource.PlayOneShot(_actionImpossibleSound);
        else _audioSource.PlayOneShot(_repairSound);
        SetBalance(-_fullCostHealth);
        Imachine.Health = Mathf.Min(Imachine.MaxHealth, Imachine.Health + _amountHealthTopIt);
        _hpSlider.value = Imachine.Health / Imachine.MaxHealth;
        SetTopItInfoFourthPanel();
    }
    //FourthPanel
    //////////////////////////////////////////////





    bool _shopZone = false;
    bool _firstVisiting = false;
    public void VisitShopZone(bool visit) => _shopZone = visit;
    //common methods
    private void VisitShopInFirstTime() {
        //get ITransferInventoryShopInfo Interface from MAchineInterface
        ImachineInerface = MachineInterface.S.GetComponent<ITransferInventoryShopInfo>();
        //instaniate blocks with informations aboout ore
        SetShopOreInforamtions(SelectedPlanet.S.selectedPlanet);
        //buttones get evenents
        DeclareButtonsEventsForOre(SellOneOre, SellAllOreOneType);

        // for second panel
        //instance blocks with gread info
        SetShopModelsInforamtion();
        //buttones get evenents
        DeclareButtonsEventsForModels(BuyUpgread);

        //for third panel
        SetShopItemsInforamtion(SelectedPlanet.S.selectedPlanet);
        //buttones get evenents
        DeclareButtonsEventsForItems(BuyItem);
        _firstVisiting = true;
    }

    public void OpenShopPanel(int numPanel) {
        _audioSource.PlayOneShot(_buttonClickSound);
        for (int i = 0; i < _shopPanels.Length; i++) { 
            if(i==numPanel-1)_shopPanels[i].SetActive(true);
            else _shopPanels[i].SetActive(false);
        }
        if (numPanel == 2) {
            SetButtonColorForModels();
        }
        if (numPanel == 3) {
            SetButtonsColorWithbalanceForItems();
            ChangeTotalAmountOfItemsText();
        }
        if (numPanel == 4) {
            SetInfoForFourthPanel();
        }
    }
    public void OpenShop() {
        if (!_shopZone) return;
        for (byte i = 0; i < _hidenUIObjects.Length; i++) _hidenUIObjects[i].SetActive(false);
        _audioSource.PlayOneShot(_buttonClickSound);
        _shop.SetActive(true);
        if (!_firstVisiting) VisitShopInFirstTime();
        //set amount ore to text in block infromation about ore and buttons color
        DeclareAmountOreToTextAndButtonColor();
        //get amount of items 
        GetAmountItems();
        //set balance 
        SetBalance();
        //place for sound
    }
    public void CloseShop() {
        for (byte i = 0; i < _hidenUIObjects.Length; i++) _hidenUIObjects[i].SetActive(true);
        OpenShopPanel(1);
        _audioSource.PlayOneShot(_buttonClickCloseSound);
        _shop.SetActive(false);
        //reset inventoryOptions
        MachineInterface.S.ExitShop();
        //place for sound
    }
    
    private void SetBalance(int money=0) {
        _balance += money;
        _tBalance.text = _balance.ToString() + "$";

    }

    //interface methods
    public List<byte> GetAmountOre() {
        List<byte> list = new List<byte>();
        foreach (ShopOreInformation i in _listShopOre) {
            list.Add(byte.Parse(i.tAmountBlocks.text));
        }
        return list;
    }

    public int GetMoney() {
        return _balance;
    }
    public sbyte IndexOre(BlockType type) { 
    return (sbyte)(type<BlockType.UpperOre ||type>BlockType.EighthOre? -1 : type == BlockType.UpperOre ? 0 : type - BlockType.FirstOre);
    }
}
