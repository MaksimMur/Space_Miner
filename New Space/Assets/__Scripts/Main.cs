using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;// для загрузки и перезагрузки сцен 
using UnityEngine.UI;
public class Main : MonoBehaviour
{
    static public Main S;
    static Dictionary<WeaponType, WeaponDefenition> WEAP_DICT; //словарь для упрощения достпуа к оружию
    static public List<GameObject> enemies;
    [Header("Set in Inspector")]
    public GameObject[] prefabEnemies;
    public float enemySpawnPerSecond = 0.5f;// появление кораблей в секунду
    public float enemyDefaultPadding = 1.5f;// отступ для позиционирования
    public WeaponDefenition[] weaponDefenitions;
    public GameObject prefabPowerUp;
    public WeaponType[] powerUpFrequency = new WeaponType[] { WeaponType.blaster, WeaponType.blaster, WeaponType.spread, WeaponType.shield, WeaponType.missil };
    static public int Score;

    private BoundsCheck bndCheck;
    void Awake()
    {
        Score = 0;
        S = this;
        bndCheck = GetComponent<BoundsCheck>();
        enemies = new List<GameObject>();
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);

        //Словарь с клчюами WeaponType
        WEAP_DICT = new Dictionary<WeaponType, WeaponDefenition>();
        foreach (WeaponDefenition def in weaponDefenitions) {
            WEAP_DICT[def.type] = def;
        }
    }
    public void SpawnEnemy() {
        // выбор префаба 1 из 5 кораблей
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);
        //размещение корабля над экраном в любой точки х
        float enemyPadding = enemyDefaultPadding;
        if (go.GetComponent<BoundsCheck>() != null) {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;
        enemies.Add(go);
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);
    }

    public void ShipDestroyed(Enemy e) {
        //Генерация бонусов с заднанной вероятностью
        if (Random.value <= e.powerUpDropChance) {
            //Выбрать тип бонуса
            //Выбрать один из элеементов powerUpFrequency
            int ndx = Random.Range(0, powerUpFrequency.Length);
            WeaponType puType = powerUpFrequency[ndx];

            //create object PowerUp
            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            //Установить соответсвтующий тип WeaponType
            pu.SetType(puType);

            //Поместить в место, где находитлся разрушенный корабль
            pu.transform.position = e.transform.position;
        }
    }
    void Start()
    {
    }
    public Text score;
    void OnGUI()
    {
        score.text = "Score:" + Score;
        //GUI.Label(new Rect(320, 20, 100, 100),"Score:"+Score);
        
    }

    public void AddScore(int a) {
        Score += a;
    }
    void Update()
    {
        
    }
    public void DelayedRestart(float delay) {
        Invoke("Restart", delay);
    }
    public void Restart() {
        SceneManager.LoadScene("_Scene_0");
    }
    ///<summary>
    ///Статическая функция, возвращающая WeaponDefenition из статического защищенного
    ///поля WEAP_DICT класса Main
    ///</summary>
    ///<returns>Экземпляр WeaponDefenition или, если 
    ///нет такого опредления для указанного WeaponType, возвращает новый экземпляр WeaponDefenition
    ///с типом none.</returns>
    ///<param name="wt">Тип оружия  WeaponType, для которого трбуется получить WeaponDefenition
    ///</param>
    static public WeaponDefenition GetWeaponDefenition(WeaponType wt) {
        //Проверить наличие указанного ключа в словаре
        //Попытка извлечь значение по отсутствующему ключу вызовет ошибку,
        //поэтому следующая инструкция играет роль.
        if (WEAP_DICT.ContainsKey(wt)) return WEAP_DICT[wt];
        //Следующая инструкция возвращает новый экземляр WeaponDefenition
        //с типом оружия WeaponTypeб.none, что означает неудачную попытку 
        //найти требуемой определение WeaponDefenition
        return new WeaponDefenition();
    }

    //Возвращение листа врагов
    static public List<GameObject> ReturnList()
    {
        return enemies;
    }
    //удаление врага из списка при его уничтожении
    static public void Delete(GameObject g)
    {
        enemies.Remove(g);
    }

}
