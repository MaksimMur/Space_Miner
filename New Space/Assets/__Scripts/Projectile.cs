using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private BoundsCheck bndCheck;
    private Renderer rend;


    [Header("Set Dynamically")]
    public List<GameObject> enemyList;
    public Vector3 pos;
    public Rigidbody rigid;
    [SerializeField]
    private WeaponType _type;

    //Это общедоступное свойство масскирует поле _type и обрабатывает
    // операции присваивания ему нового значения
    public WeaponType type
    {
        get { return (_type); }
        set { SetType(value); }
    }
    public void findEnemy()
    {
       
        GameObject aim = null;
        float min = 100;
        if (enemyList.Count == 0)
        {
            this.transform.position -= new Vector3(0, -0.2f, 0);
            this.transform.position += new Vector3(Random.Range(-0.1f, 0.1f), 0,0);
        }
        foreach (GameObject g in enemyList)
        {
            if (g != null)
            {
                if (Vector3.Distance(pos, g.transform.position) < min)
                {
                    min = Vector3.Distance(pos, g.transform.position);
                    aim = g;
                }
            }
        }
        if (aim != null)
        {
            transform.position = Vector3.Lerp(this.transform.position, aim.transform.position, 0.03f);
            transform.LookAt(this.transform.position);

        }
        else {

            this.transform.position -= new Vector3(0, -0.2f, 0);
            this.transform.position += new Vector3(Random.Range(-0.1f, 0.1f), 0, 0);
        }


    }

    void Awake()
    {
        enemyList = Main.ReturnList();
        bndCheck = GetComponent<BoundsCheck>();
        rend = GetComponent<Renderer>();
        rigid = GetComponent<Rigidbody>();
    }
    void Update()
    {

        if (bndCheck.offUp ||bndCheck.offDown || bndCheck.offLeft ||bndCheck.offRight) {
            Destroy(gameObject);
        }
        WeaponType t= type;
        if (WeaponType.missil == t) {
            findEnemy();
        }
    }
    ///<summary>
    ///Изменяет скрытое поле _type т устанавливает цвет этого снаряда,
    ///как определено в WeaponDefenition
    ///</summary>
    ///<param name="eType">Тип WeaponType используемого оружия.</param>
    public void SetType(WeaponType eType)
    {
        //Установить _type
        _type = eType;
        WeaponDefenition def = Main.GetWeaponDefenition(_type);
        rend.material.color = def.projectielColor;
    }
  

}
