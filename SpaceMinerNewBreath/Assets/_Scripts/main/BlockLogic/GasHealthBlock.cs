using System.Collections.Generic;
using UnityEngine;

public class GasHealthBlock : Block
{
    [System.Obsolete]
    public override void Destroyed(bool destroyedByTNT = false)
    {
        QuestManager.S.Mine(_blockType);
        //spawn new layer
        if (BlockManager.S.DepthLevelWithDiffToSpawn <= Mathf.Abs(transform.position.y))
        {
            //check max depth achived by machine for statistics
            Statistics.S.AchieveNewDepth();
            BlockManager.S.SpawnBlocks();
        }
        if (!destroyedByTNT)
        {
            GasHealth gasHealth = BlockManager.S.gasHealthPool.GetFreeElemet();
            gasHealth.transform.position = this.transform.position;
            gasHealth.SprayGas();
            
        }
        else
        {
            GasHealth gasHealth = BlockManager.S.gasHealthPool.GetFreeElemet();
            gasHealth.transform.position = this.transform.position;
            gasHealth.ExploseGas();
        }
        //statictics count destroyed block
        Statistics.S.BlockDestroyed();
        Destroy(this.gameObject);
    }
 
}
