using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class BlockManager : MonoBehaviour
{
    public static BlockManager S;
    [Header("Set in Inspector: Blocks Options")]
    //blocks
    [SerializeField] private ParticlesBehaviour _blockDestroyedParticles;
    [SerializeField] private Transform _blocksDestroyedAnchor;
    [SerializeField] private AudioClip _destroyedSoundCoomonBlock;
    [SerializeField] private AudioClip _destroyedSoundSand;
    private PoolMono<ParticlesBehaviour> poolBlocks;
    private Block[] blockPrefabs;
    private BlockType[][] levelBlocks;
    private GameObject backgroudnPrefab;
    [Header("Set in Inspector: Little Blocks Options")]
    [SerializeField] private LittleBlock _littleBlockPrefab;
    [SerializeField] private short _littleblokcsPoolCapcity=20;
    [SerializeField] private Transform _littleBlocksAnchor;
    private PoolMono<LittleBlock> _littleBlocksPool;
    private Sprite[] _littleBlocksSprites;
    //how block will be spawn
    [Header("Set in Inspector: Blocks Options Spawn")]
    [SerializeField] private byte _amountBuildingBlocks = 20;
    [SerializeField] private byte _lengthMap = 50;
    //if distance between machine pos y and pos last block less than hiss value that sapwn new blocks layer
    [SerializeField]private byte _diffRangeToSpanwBlocksLayer = 4;
    [SerializeField]private byte _amountBlocksInLevel=10;
    private List<Block> _listBlocks;
    [Header("Set in Inpsector: Pools ")]
    [SerializeField] private GasHealth _gasHealthPrefab;
    [SerializeField] private Transform _gasHealthAnchor;
    [HideInInspector] public PoolMono<GasHealth> gasHealthPool;
    [SerializeField] private GasFuel _gasFuelPrefab;
    [SerializeField] private Transform _gasFuelAnchor;
    [HideInInspector] public PoolMono<GasFuel> gasFuelPool;
    //set dynamically
    private Transform _blocksAnchor;
    private Transform _backgroundAnchor;
    private byte depthLevel  = 0;
    public byte LengthMap => _lengthMap;
    public void Awake()
    {
        //Singlton
        S = this;
        _listBlocks = new List<Block>();
        _blocksAnchor = new GameObject("BlockAnchor").GetComponent<Transform>();
        _backgroundAnchor = new GameObject("backGroundAnchor").GetComponent<Transform>();
        _blocksAnchor.SetParent(GameObject.Find("Map").GetComponent<Transform>());
        _backgroundAnchor.SetParent(GameObject.Find("Map").GetComponent<Transform>());
        SetLevelOptionsWithSelectedPlanet(SelectedPlanet.S.selectedPlanet);
        //create pool that usin by particles
        poolBlocks = new PoolMono<ParticlesBehaviour>(this._blockDestroyedParticles, 5, this._blocksDestroyedAnchor);
        this.poolBlocks.autoExpand = true;
        //create particels that usin by little blocks
        _littleBlocksPool = new PoolMono<LittleBlock>(this._littleBlockPrefab, _littleblokcsPoolCapcity, this._littleBlocksAnchor);
        this._littleBlocksPool.autoExpand = true;
        //create particles that using by gasHealthParticles
        //if (PlayerPrefs.GetString("LoadedPlanetName") != "Mars" || PlayerPrefs.GetString("LoadedPlanetName") != "Venera") return;
        gasHealthPool = new PoolMono<GasHealth>(this._gasHealthPrefab, 5, _gasHealthAnchor);
        gasHealthPool.autoExpand = true;
        //if(PlayerPrefs.GetString("LoadedPlanetName") != "Venera") return;
        gasFuelPool = new PoolMono<GasFuel>(this._gasFuelPrefab, 5, _gasFuelAnchor);
        gasFuelPool.autoExpand = true;
    }
    private void Start()
    {
     
        //generate map
        SpawnTerrain();
        for (int i = 0; i < _diffRangeToSpanwBlocksLayer; i++) SpawnBlocks();
    }
    /// <summary>
    /// This func generate terrains' layer
    /// </summary>
    public void SpawnTerrain() {
        for (int i = 0; i < _lengthMap; i++)
        {
            //spawnback
            GameObject back = Instantiate(backgroudnPrefab);
            back.transform.position = new Vector2(-_lengthMap / 2 + i, -depthLevel);
            back.transform.SetParent(_backgroundAnchor);
            Block go;
            //spawn terrain
            if (i <_amountBuildingBlocks/2 || i >= _lengthMap-_amountBuildingBlocks/2) go = Instantiate(blockPrefabs[0]);
            //spawn defaultBlocks
            else go = Instantiate(blockPrefabs[1]);
            //set position blocks and layer order and adding them in list
            go.transform.position = new Vector2(-_lengthMap / 2 + i, -depthLevel);
            go.transform.SetParent(_blocksAnchor);
            _listBlocks.Add(go);
        }
        depthLevel++;
    }
    /// <summary>
    /// This func generate bloks' layer
    /// </summary>
    public void SpawnBlocks() {
        for (int i = 0; i < _lengthMap; i++)
        {
            GameObject back = Instantiate(backgroudnPrefab);
            back.transform.position = new Vector2(-_lengthMap/2+i, -depthLevel);
            back.transform.SetParent(_backgroundAnchor);
            Block go;
            go = Instantiate(blockPrefabs[TakeARandomBlock()]);
            //set position blocks and layer order and adding them in list
            go.transform.position = new Vector2(-_lengthMap / 2 + i, -depthLevel);
            go.transform.SetParent(_blocksAnchor);
            _listBlocks.Add(go);
        }
        depthLevel++;
    }
    /// <summary>
    /// This func pick block giving the level
    /// </summary>
    /// <returns>opicked index block</returns>
    public byte TakeARandomBlock() {
        //take a level that has different blocks that it level include
        byte level = (byte)Mathf.Min(Mathf.Floor(Mathf.Abs(depthLevel*1f/_amountBlocksInLevel)), levelBlocks.Length - 1);
        //take a random level's block 
        return (byte)levelBlocks[level][Random.Range(0, levelBlocks[level].Length)];
    }
    /// <summary>
    /// This Methood set blocks options for selected planet
    /// </summary>
    /// <param name="p"> selected planet </param>
    public void SetLevelOptionsWithSelectedPlanet(Planet p) {
        levelBlocks = new BlockType[p.levelsBlockChanceSpawn.Length][];
        for (int i = 0; i < levelBlocks.Length; i++) {
            for (int j = 0; j < p.levelsBlockChanceSpawn[i].levelBlocks.Length; j++) {
                levelBlocks[i] = p.levelsBlockChanceSpawn[i].levelBlocks.Select(x => x).ToArray();
            }
        }
        for (int i = 0; i < levelBlocks.Length; i++)
        {
            for (int j = 0; j < p.levelsBlockChanceSpawn[i].levelBlocks.Length; j++)
            {
            }
        }
        blockPrefabs = p.blockPrefabs;
        backgroudnPrefab = p.backGround_Prefab;
        _littleBlocksSprites = p.littleBlocks;
    }
    /// <summary>
    /// This method instanciate particles in indicated point
    /// </summary>
    /// <param name="type"> Type of Block</param>
    /// <param name="pos"> Position where paritcles will spawn</param>
    [System.Obsolete]
    public void GetParticle(BlockType type, Vector3 pos,Color color) {
        if (!(type == BlockType.None)) {
           var particle= this.poolBlocks.GetFreeElemet();
            particle.transform.position = pos;
            particle.SetColor(color);
            if (type == BlockType.Sand) {
                particle.SetAudioClip(_destroyedSoundSand,true);
                return;
            }
                particle.SetAudioClip(_destroyedSoundCoomonBlock,true);
        }
    }

    public byte[] GetOreLevelsToDestroy() {
        return blockPrefabs.Where(x => x.IsBlockIsOre && x.blockType!=BlockType.UpperOre).Select(x => x.LevelOfBurToDestroy).ToArray();
    }

    /// <summary>
    /// This method will call after destroyiding block and when total amount 
    /// of blocks in inventory equal with capacity of inventory
    /// </summary>
    /// <param name="indexOre"> index of ore, if index=-1 return</param>
    public void SpawnLittleBlockAfterDestroyBlock(int indexOre, Vector2 pos) {
        if (indexOre == -1) return;
        LittleBlock createdBlock = _littleBlocksPool.GetFreeElemet();
        createdBlock.transform.position = pos;
        createdBlock.type = BlockType.FirstOre + indexOre;
        createdBlock.SetSprite(_littleBlocksSprites[indexOre]);
    }
    //get depth level with diff to sapwn, that if block was destroyed spawn new layer
    public short DepthLevelWithDiffToSpawn => ((short)(depthLevel - _diffRangeToSpanwBlocksLayer));
}
