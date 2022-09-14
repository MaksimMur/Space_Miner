using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITransferInventoryShopInfo {
    public List<byte> GetAmountOre();

    public int GetMoney();
    public sbyte IndexOre(BlockType type);
    
}
