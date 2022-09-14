using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Это перечисление всех возможных типов оружия
/// Также включает тип shield, чтобы дать возможность совершнествовать защиту
/// </summary>
public enum WeaponType
{
    none, //нет оружия
    blaster, // простой бласте
    spread,//Веерная пушка
    phaser,
    missil,
    laser,
    shield// увеличивает мощность щита

}
/// <summary>
/// Класс WeaponDefinition позволяет настраивать свойства конкретного вида оружия в инспекторе. Для этого класс Main
/// будет хранить массив элементов типа WeaponDefenition
/// </summary>
[System.Serializable]
public class WeaponDefenition{
    public WeaponType type = WeaponType.none;
    public string letter; //Буква на кубике, отображающая бонус

    public Color color = Color.white;// Цвет ствола оружия и кубика бонуса
    public GameObject projectilePrefab; // шаблон снаряда
    public Color projectielColor = Color.white; //Цвет снаряда
    public float damaegOnHit = 0;// Разрушительная мощность
    public float continuousDamage = 0;//Степень разрушения в секунду

    public float delayBetweenShots = 0; // Задеркжа между выстрелами
    public float velocity = 20f; //скорость полета снарядов

}
public class Weapon : MonoBehaviour { 
    static public Transform PROJECTILE_ANCHOR;

    [Header("Set Dynamically")]
    [SerializeField]
    private WeaponType _type = WeaponType.none;
    public WeaponDefenition def;
    public GameObject collar;
    public float lastShotTime;//время последнего выстрела
    private Renderer collarRend;
    void Start()
    {
        collar=transform.Find("Collar").gameObject;
        collarRend = collar.GetComponent<Renderer>();

        //Вызвать SetType(), чтобы заменить тип оружия по умолчанию WeaponType.non
        SetType(_type);

        //Динамически создать точку привязки снарядлов
        if (PROJECTILE_ANCHOR != null) {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }
        //Найти fireDelegate в корневом игровом объекте
        GameObject rootGo = transform.root.gameObject;
        if (rootGo.GetComponent<Hero>() != null) {
            rootGo.GetComponent<Hero>().fireDelegate += Fire;
        }
        
    }
    public WeaponType type
    {
        get { return (_type); }
        set { SetType(value); }
    }
    public void SetType(WeaponType wt) {
        _type = wt;
        if (type == WeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else this.gameObject.SetActive(true);
        def = Main.GetWeaponDefenition(_type);
        collarRend.material.color = def.color;
        lastShotTime = 0;//сразу полсе устанвки _type можно выстрелить
    }
    public void Fire() {
        //Если this.gameObject не активен выйти
        if (!gameObject.activeInHierarchy) return;
        //Если между выстраелами прошло много времени выйти
        if (Time.time - lastShotTime < def.delayBetweenShots) {
            return;
        }
        Projectile p;
        Vector3 vel = Vector3.up * def.velocity;
        if (transform.up.y < 0) {
            vel.y = -vel.y;
        }
        switch (type) {
            case WeaponType.blaster:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                break;
            case WeaponType.spread:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.rigid.velocity = p.transform.rotation*vel;
                break;
            case WeaponType.missil:
                p = MakeProjectile();
                transform.LookAt(this.transform.position + p.rigid.velocity);
                break;
            
        }
    }
    public Projectile MakeProjectile() {
        GameObject  go = Instantiate<GameObject>(def.projectilePrefab);
     
        if (transform.parent.gameObject.tag == "Hero") {
            go.tag = "ProjectileHero";
            go.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        go.transform.position = collar.transform.position;
        go.transform.SetParent(PROJECTILE_ANCHOR, true);
        Projectile p = go.GetComponent<Projectile>();
        p.type=type;
        lastShotTime = Time.time;
        return (p);
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
