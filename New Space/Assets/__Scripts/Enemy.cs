using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Set in Inspector")]
    public float speed = 10f;
    public float fireRate = 0.3f;
    public float health = 10;
    public int score = 100;
    public float powerUpDropChance = 1f;
    public float showFamageDuration = 0.1f;//длительность эффекта попадания в секундах
    protected BoundsCheck bndCheck;

    [Header("Set Dynamically: Enemy")]
    public Color[] originalColors;
    public Material[] materials;//все маетриалы игрового объекта и его потомков
    public bool showingDamage = false;
    public float damageDoneTime; //Время прекращения отображения эффекта
    public bool notifiedOfDestruction = false;


    
    public Vector3 pos {
        get { return (this.transform.position); }
        set { this.transform.position = value; }
    }
    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for (int i = 0; i < originalColors.Length; i++) {
            originalColors[i] = materials[i].color;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        if (bndCheck != null && bndCheck.offDown) {
            Main.Delete(this.gameObject);
            Destroy(gameObject); 

        }
        if (showingDamage && Time.time > damageDoneTime) {
            UnShowDamage();
        }
    }
    public virtual void Move() {
        Vector3 tempPos = transform.position;
        tempPos.y -= speed * Time.deltaTime;
        transform.position = tempPos;
    }
    void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        switch (other.tag) {
            case "ProjectileHero":
                //если вражеский корабль за границей нельзя наносить ему урон
                Projectile p = other.GetComponent<Projectile>();
                if (!bndCheck.isOnScreen)
                {
                    Destroy(other);
                    break;
                }
                //Поразить вражеский корабль
                ShowDamage();
                health -= Main.GetWeaponDefenition(p.type).damaegOnHit;
                if (health <= 0)
                {
                    //ссообщение о уничтожении корабля
                    if (!notifiedOfDestruction) {
                        Main.S.ShipDestroyed(this);
                        
                    }
                    Main.S.AddScore(score);
                    Main.Delete(this.gameObject);
                    Destroy(this.gameObject);
                    


                }
                Destroy(other);
                break;
            default:
                    print("Enemy hit by non-projectile" + other.name);
                break;

        }
    }
    void ShowDamage() {
        foreach (Material m in materials) {
            m.color = Color.red;
        }
        showingDamage = true;
        damageDoneTime = Time.time + showFamageDuration;
    }
  public  void UnShowDamage() {
        for (int i = 0; i < materials.Length; i++) {
            materials[i].color = originalColors[i];
        }
        showingDamage = false;
  }
}
