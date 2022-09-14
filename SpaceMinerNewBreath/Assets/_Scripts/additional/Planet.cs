using UnityEngine;
[System.Serializable]
public class Planet 
{
    [Header("Set in Inspector: Blocks info for Planet")]
    public LevelBlock[] levelsBlockChanceSpawn;
    public Block[] blockPrefabs;
    public string[] _blocksNameTransalaterKey;
    public string[] _blocksQuestNameTranslateKey;
    public GameObject backGround_Prefab;
    public Sprite[] littleBlocks;
    [Header("Set other")]
    public Color planetColor;
    [Header("Items planet")]
    public ItemType[] items;
    [Header("shop info for planet")]
    public short[] BlocksPrice;
    [Header("Spawn enemy info")]
    public EnemySpawnInfo[] _enemySpawnInfo;

}

