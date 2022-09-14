using UnityEngine;
public class Block : MonoCache
{
    [Header("Block options")]
    [SerializeField] private float _health;
    [SerializeField] protected BlockType _blockType;
    [SerializeField] private byte _levelToDestroy;
    [SerializeField] protected bool _possibilityToDestroy = true;
    [SerializeField] private Color _colorParticles = new Color(1, 1, 1, .5f);
    public bool IsBlockIsOre=> _blockType < BlockType.UpperOre || _blockType > BlockType.EighthOre ? false : true;
    public sbyte IndexOfOre=> (sbyte)(!IsBlockIsOre ? -1 : _blockType == BlockType.UpperOre ? 0 : _blockType - BlockType.FirstOre);

    /// <summary>
    /// This method called when machine mining block 
    /// </summary>
    /// <param name="damage">damage that block get</param>
    /// <param name="levelBur">level of machine bur</param>
    [System.Obsolete]
    public void GetDamage(float damage, byte levelBur) {
        if (!_possibilityToDestroy) {
            return;
        }
        if (levelBur >= _levelToDestroy) {
            _health -= damage;
            if (_health <= 0) {
                Destroyed();
            }
            return;
        }
    }
    /// <summary>
    /// This method calle when health of block less or equal 0 and execute som instructions
    /// </summary>
    /// <param name="destroyedByTNT">flag that indicate source of destroyiding block</param>
    [System.Obsolete]
    public virtual void Destroyed(bool destroyedByTNT=false) {
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
            if (MachineInterface.S.EnoughInvenoryPlace) MachineInterface.S.FillOnePlaceInInventory(_blockType);
            else BlockManager.S.SpawnLittleBlockAfterDestroyBlock(IndexOfOre, transform.position);
            BlockManager.S.GetParticle(_blockType, transform.position, _colorParticles);
        }
        else {
            BlockManager.S.SpawnLittleBlockAfterDestroyBlock(IndexOfOre, transform.position);
        }
        //statictics count destroyed block
        Statistics.S.BlockDestroyed();
        Destroy(this.gameObject);
    }
    public float Y => transform.position.y;
    public float X => transform.position.x;
    public bool PossibillityToDestroy => _possibilityToDestroy;
    public byte LevelOfBurToDestroy => _levelToDestroy;
    public BlockType blockType => _blockType;
}
