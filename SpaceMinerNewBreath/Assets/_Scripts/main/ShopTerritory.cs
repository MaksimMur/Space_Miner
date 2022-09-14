using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTerritory : MonoBehaviour
{
    // Start is called before the first frame update
    private bool _machineInShopZone = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Machine>()!=null) {
            Shop.S.VisitShopZone(true);
            _machineInShopZone = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Machine>()!=null)
        {
            Shop.S.VisitShopZone(false);
            _machineInShopZone = false;
        }
    }
    private void OnMouseDown()
    {
        if (_machineInShopZone) {
            Shop.S.OpenShop();
        }
    }
}
