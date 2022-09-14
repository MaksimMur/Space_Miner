using UnityEngine;
using UnityEngine.UI;

public class ShopModelsInformation : MonoBehaviour
{
    public Text tBlockPrice;
    public LocalizedText tDescription;
    public ModelType type { get; private set; }
    [HideInInspector]
    public Button bUpgread;
    public short price;
    public short improvePrice;
    [HideInInspector]public Image[] improveImages;
    [HideInInspector]public short level = 0;
    private short _maxLevel = 6;
    public LocalizedText localizedText;
    private void Awake()
    {
        bUpgread = transform.Find("B_Upgread").GetComponent<Button>();
        improveImages = transform.Find("UpgreadImages").GetComponentsInChildren<Image>();
    }
    public bool LevelIsMax => level == _maxLevel;
    public void SetType(ModelType m) {
        type = m;
    }
    public void SetSprite(Sprite sp) {
        transform.Find("SpriteImage").GetComponent<Image>().sprite = sp;
    }
    private void AchieveMaxLevel() {
        tBlockPrice.text = "MAX";
        bUpgread.GetComponentInChildren<Text>().text = "MAX";
        bUpgread.GetComponent<Image>().color = Color.green;
    }
    public void GreadLevel() {
        if (LevelIsMax) return;
        improveImages[level].color = Color.yellow;
        ++level;
        if (LevelIsMax) AchieveMaxLevel();
        
    }
}