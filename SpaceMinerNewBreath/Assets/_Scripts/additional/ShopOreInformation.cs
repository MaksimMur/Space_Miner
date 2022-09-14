using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShopOreInformation : MonoBehaviour
{
    public Text tAmountBlocks;
    public Text tBlockPrice;
    public BlockType type { get; private set; }
    public Button bSellOneOre;
    public Button bSellAllOre;
    public LocalizedText localizedText;
    public short price;
    private void Awake()
    {
        type = BlockType.None;
        tAmountBlocks = transform.Find("T_AmountBlocks").GetComponent<Text>();
        tBlockPrice = transform.Find("T_Price").GetComponent<Text>();
        bSellOneOre = transform.Find("B_SellOneOre").GetComponent<Button>();
        bSellAllOre = transform.Find("B_SellAllOre").GetComponent<Button>();
        localizedText = transform.Find("T_BlockName").GetComponent<LocalizedText>();
    }
    public void SetSprite(Sprite s) {
        transform.Find("SpriteImage").GetComponent<Image>().sprite= s;
    }
    public void SetType(BlockType t) {
        type = t;
    }
}