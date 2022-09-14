using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Расширение класс Enemy
public class Enemy_1 : Enemy {
    [Header("Set in Inspetor: Enemy_1")]
    // число секунд полного цикла синусоиды
    public float waveFrequency = 2f;
    // ширина синусоиды в метрах
    public float waveWidth = 4f;
    public float waveRotY=45f;

    private float x0;//Начльное значение координаты Х
    private float birthTime;
    void Start()
    {
        // Установить начальную координату Х объекта Enemy_1
           
        x0 = pos.x;
        birthTime = Time.time;
     
    }

    public override void Move()
    {
        // pos - свойтво Enemmy, которое не позволяет изменить pos.x;
        // То используется VEctor3 temPos    
        Vector3 temPos = pos;
        //Значение theta изменяется с течением времени
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        temPos.x = x0 + waveWidth * sin;
        pos = temPos;

        // повернуть немного относительно оси Y
        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);
        //base.Move() обрабатыввает движение вниз, вдоль оси Y

        base.Move();
        //print(bndCheck.isOnScreen);
    }
    void Update()
    {
        Move();
        if (bndCheck.offDown) {
            Main.Delete(this.gameObject);
            Destroy(this.gameObject);
        }
        if (showingDamage && Time.time > damageDoneTime)
        {
            UnShowDamage();
        }
    }
    
}
