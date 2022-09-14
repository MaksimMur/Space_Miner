using UnityEngine;
using UnityEngine.UI;

public class InventoryInformation : MonoBehaviour
{
    public Text tAmountBlocks;
    public BlockType type;

    public void IntitializeInventoryInforamtion(Sprite s, int n)
    {

        transform.Find("SpriteImage").GetComponent<Image>().sprite = s;
        tAmountBlocks= transform.Find("T_AmountBlocks").GetComponent<Text>();
        type = n + BlockType.FirstOre;
    }
}