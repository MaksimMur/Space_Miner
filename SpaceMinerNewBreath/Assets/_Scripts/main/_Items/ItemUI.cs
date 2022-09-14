using UnityEngine;
using UnityEngine.UI;
public class ItemUI : MonoBehaviour
{
    [Header("Set In Inspector: item ui options")]
    [SerializeField] private ItemType _itemType;
    [HideInInspector] public Text T_Amount;
    private void Awake()
    {
        T_Amount = transform.Find("T_amount").GetComponent<Text>();
    }
    public ItemType Type { get => _itemType; }
}
