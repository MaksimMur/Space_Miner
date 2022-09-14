using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S; // Одиночка

    [Header("Set in Inspector")]
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float gameRestartDelay = 2f;
    public GameObject projectilePrefab;
    public GameObject rocketPrefab;
    public float projectileSpeed = 40;
    public Weapon[] weapons;

    [Header("Set Dynamically")]
    public float _shieldLevel = 4;
    private GameObject lastTrigger = null;

    public delegate void WeaponFireDelegate();
    public WeaponFireDelegate fireDelegate;

    private void Start()
    {
        if (S == null)
        {
            S = this;
        }
        ClearWeapons();
        weapons[0].SetType(WeaponType.blaster);

    }

    public float shieldLevel
    {
        get { return (_shieldLevel); }
        set
        {
            _shieldLevel = Mathf.Min(value, 4);
            if (value < 0)
            {
                Destroy(this.gameObject);
          
                Main.S.DelayedRestart(gameRestartDelay);
            }
        }

    }

    void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        if (go == lastTrigger) return;
        lastTrigger = go;
        if (go.tag == "Enemy")
        {
            shieldLevel--;
            Main.Delete(this.gameObject);
            Destroy(go);
        }
        if (go.tag == "ProjectileEnemy") {
            shieldLevel--;
            Destroy(go);
        }
        else if (go.tag == "PowerUp") {
            AbsorbPowerUp(go);
        }
    }

    void Update()
    {
        // реализация движения корабля
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;
        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);
        if (Input.GetAxis("Jump") == 1 && fireDelegate != null) {
            fireDelegate();
        }
       
    }
    void TempFire() {
        GameObject proGo = Instantiate<GameObject>(projectilePrefab);
        proGo.transform.position = transform.position;
        Rigidbody rigidB = proGo.GetComponent<Rigidbody>();
        Projectile proj = proGo.GetComponent<Projectile>();
        proj.type = WeaponType.blaster;
        float tSpeed = Main.GetWeaponDefenition(proj.type).velocity;
        rigidB.velocity = Vector3.up * tSpeed;
    }

    public void AbsorbPowerUp(GameObject go) {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type) {
            case WeaponType.shield:
                shieldLevel++;
                break;
            default:
                if (pu.type == WeaponType.missil) {
                    Weapon w = GetEmpryWeaponSlot();
                    if (w != null)
                    {
                        w.SetType(pu.type);
                        break;
                    }
                   
                    else
                    {
                        foreach (Weapon wt in weapons)
                        {
                            if (wt.type == WeaponType.blaster|| wt.type==WeaponType.spread)
                            {
                                wt.SetType(pu.type);
                                break;
                            }
                        }
                    }
                }
                if (pu.type == WeaponType.spread)
                {
                    Weapon w = GetEmpryWeaponSlot();
                    if (w != null)
                    {
                        w.SetType(pu.type);
                    }
                    else
                    {
                        foreach (Weapon wt in weapons)
                        {
                            if (wt.type == WeaponType.blaster)
                            {
                                wt.SetType(pu.type);
                                break;
                            }
                        }
                    }
                }
                else {
                    Weapon w = GetEmpryWeaponSlot();
                    if (w != null)
                    {
                        w.SetType(pu.type);
                    }
                    
                }
                /*if (pu.type == weapons[0].type)//если оружие того же типа
                {
                    Weapon w = GetEmpryWeaponSlot();
                    if (w != null)
                    {
                        w.SetType(pu.type);
                    }
                }
                else
                {//если оружие другого типа
                    ClearWeapons();
                    weapons[0].SetType(pu.type);
                }*/
                break;
        }
        pu.AbsorbedBy(this.gameObject);
    }
    Weapon GetEmpryWeaponSlot()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].type == WeaponType.none) return (weapons[i]);
        }
        return (null);
    }
    void ClearWeapons()
    {
        foreach (Weapon w in weapons)
        {
            w.SetType(WeaponType.none);
        }
    }
}
