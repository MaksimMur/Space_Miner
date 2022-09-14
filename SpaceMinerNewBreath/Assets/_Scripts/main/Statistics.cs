using UnityEngine;
public class Statistics : MonoBehaviour
{
    public static Statistics S;
    private short _minedBlocks = 0;
    private short _usedItems = 0;
    private byte _completedMissions=0;
    private byte _destroyedEnemies = 0;
    private byte _maxDepth = 0;
    public delegate void DestroyedBlocks();

    public void Awake()
    {
        S = this;
    }
    public void BlockDestroyed()=>_minedBlocks++;
    public void MissionCompletes() => _completedMissions++;
    public void ItemUsed() => _usedItems++;
    public void EnemyKilled() => _destroyedEnemies++;
    public void AchieveNewDepth() => _maxDepth++;
    public short MinedBlocksAmount => _minedBlocks;
    public short UsedItemsAmount => _usedItems;
    public byte CompletedMisionsAmount => _completedMissions;
    public byte DestroyedEnemiesAmount => _destroyedEnemies;

    public byte MaxDepth => _maxDepth;
}
