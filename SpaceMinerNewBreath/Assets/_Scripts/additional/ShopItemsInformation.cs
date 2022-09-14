using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShopItemsInformation : MonoBehaviour
{
    [HideInInspector]public Text tAmountItems;
    [HideInInspector]public Text tItemPrice;
    public ItemType type { get; private set; }
    [HideInInspector]public Button bBuyItem;
    [HideInInspector]public short price;
    public LocalizedText localizedText;
    public LocalizedText tDescription;
    private void Awake()
    {
        tAmountItems = transform.Find("T_AmountItems").GetComponent<Text>();
        tItemPrice = transform.Find("T_Price").GetComponent<Text>();
        bBuyItem = transform.Find("B_BuyItem").GetComponent<Button>();
    }
    public void SetSprite(Sprite s, Rect sRect)
    {
        transform.Find("SpriteImage").GetComponent<Image>().sprite = s;
        transform.Find("SpriteImage").GetComponent<RectTransform>().sizeDelta = sRect.size;
        transform.Find("SpriteImage").GetComponent<RectTransform>().localPosition = sRect.position;
    }
    public void SetType(ItemType t)
    {
        type = t;
    }
}
