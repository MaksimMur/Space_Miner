using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enenmy_4 создается за верхней границей экрана, выбирает случайную точку на экране
/// и перемещается к ней. Добравшиь до мкста, выбирает другую случайную точку
/// и продолжает двигаться, пока игрок не уничтожите го
/// </summary>


///<summary>
///Part - еще один сериализуемый класс предназначенный для храннения данных
///</summary>
[System.Serializable]
public class Part
{
    //Значения трех полей должны определяться  в инспекторе
    public string name;//имя части
    public float health;//Степень стойкости этой части
    public string[] protectedBy;//Другие части, защищающие эту



    //Эти два поля автоматически инициализируются в Start();
    //Кэшированеи, как здесь ускоряет получение необъодимых данных
    [HideInInspector]
    public GameObject go;//игровой объект этой части
    [HideInInspector]
    public Material mat;//Материал для отображения повреждений
}
public class Enemy_4 : Enemy
{
    [Header("Set in Inspector: Enemy_4")]
    public Part[] parts;//Массив частей, составляющих корабля
    public GameObject projectilePrefab;

    private Vector3 p0, p1;//две точки для интерполяции
    private float timeStart;//Время создания этого корабля
    private float duration = 4;// Продолжительность перемещения

    // Start is called before the first frame update
    void Start()
    {
        p0 = p1 = pos;
        InitMovement();
        //Записать в кэш игровой объект и материал каждой части в parts
        Transform t;
        foreach (Part prt in parts) {
            t = transform.Find(prt.name);
            if (t != null) {
                prt.go = t.gameObject;
                prt.mat = prt.go.GetComponent<Renderer>().material;
            }
        }

    }
    void InitMovement() {
        p0 = p1;

        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);

        //Сбросить время
        timeStart = Time.time;
        
    }
    public override void Move()
    {
        float u = (Time.time - timeStart) / duration;
        if (u > 1) {
            InitMovement();
                u = 0;
        }
        u = 1 - Mathf.Pow(1 - u, 2);
        pos = (1 - u) * p0 + u * p1;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        if (showingDamage && Time.time > damageDoneTime)
        {
            UnShowDamage();
        }

    }
    //Эти две функции выполняют поиск части в массиве parts по
    //имени или по ссылке на игрвоой объект
    Part FindPart(string n)
    {
        foreach (Part prt in parts) {
            if (prt.name == n) {
                return (prt);
            }
        }
        return (null);
    }
    Part FindPart(GameObject go) {
        foreach (Part prt in parts) {
            if (prt.go == go) {
                return (prt);
            }
        }
        return (null);
    }

    //Эти функции возвращают true, если данная часть уничтожена
    bool Destroyed(GameObject go) {
        return (Destroyed(FindPart(go)));
    }
    bool Destroyed(string n) {
        return (Destroyed(FindPart(n)));
    }
    bool Destroyed(Part prt) {
        if (prt == null) {//Если ссылка на часть не была передана
            return (true);//вернуть правду(то есть, да, была уничтожена)
        }
        //Вернуть результат сравнения: prt.health<=0
        //ЕСли prt.health<=0, вернуть правду(да было уничтожена)
        return (prt.health <= 0);
    }

    //окрашивает в красный только одну часть, а не весь корабль
    void ShowLocalizedDamage(Material m) {
        m.color = Color.red;
        damageDoneTime = Time.time + showFamageDuration;
        showingDamage = true;
    }

    //Переопределяет метод OnCollisionEnter из сценария Enemy.cs
    void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        switch (other.tag) {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                //Если корабль за границами экрана, не повреждать его
                if (!bndCheck.isOnScreen) {
                    Destroy(other);
                    break;
                }

                //Поразить вражейский корабль
                GameObject goHit = collision.contacts[0].thisCollider.gameObject;
                Part prtHit = FindPart(goHit);
                if (prtHit == null) {
                    goHit = collision.contacts[0].otherCollider.gameObject;
                    prtHit = FindPart(goHit);
                }

                //Проверить защищена ли еще эта часть
                if (prtHit.protectedBy != null) {
                    foreach (string s in prtHit.protectedBy) {
                        //Если хотя бы одна часть не разрушена 
                        if (!Destroyed(s)) {
                            //Не наносить вопреждения этой части
                            Destroy(other);//уничтожить снаряд
                            return;//выйти, не повреждая Enemy_4
                        }
                    }
                }

                //Эта часть не защищена, наносиь ей повреждение
                //Получать разрушающую силу из Projectile.type и Main.WEAP_DICT 
                prtHit.health -= Main.GetWeaponDefenition(p.type).damaegOnHit;

                //Показать эффект попадания в часть
                Material m = prtHit.mat;
                    ShowLocalizedDamage(prtHit.mat);
                


                if (prtHit.health <= 0) {
                    //Вместо разрушения всего корабля 
                    //Деактивировать уничтоженную часть
                    MakeProjectile();
                    prtHit.go.SetActive(false);
                    Main.S.AddScore(25);
                }

                //Проверить, был ли корабль полностью разрушен
                bool allDestroyed = true;//Предположить, что разрушен
                foreach (Part prt in parts) {
                    if (!Destroyed(prt)) {
                        allDestroyed = false;
                        break;
                    }
                }
                if (allDestroyed) {
                    Main.S.ShipDestroyed(this);
                    Main.S.AddScore(score);
                    Main.Delete(this.gameObject);
                    Destroy(this.gameObject);
                }
                Destroy(other);
                break;


        }
    }
    public void MakeProjectile()
    {
        for (int i = 0; i < 8; i++)
        {
            GameObject go = Instantiate<GameObject>(projectilePrefab);
            Projectile t = go.GetComponent<Projectile>();
            Vector3 vel = Vector3.up * 20;
            t.transform.rotation = Quaternion.AngleAxis(i*35, Vector3.back);
            t.rigid.velocity = t.transform.rotation*vel;
            go.transform.position = this.gameObject.transform.position;
        }
       

    }

}
