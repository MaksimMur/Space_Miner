using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasFuelBlock : Block
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
        
        GasFuel gasFuel = BlockManager.S.gasFuelPool.GetFreeElemet();
        gasFuel.transform.position = this.transform.position;
       
        //statictics count destroyed block
        Statistics.S.BlockDestroyed();
        Destroy(this.gameObject);
    }
  
}
