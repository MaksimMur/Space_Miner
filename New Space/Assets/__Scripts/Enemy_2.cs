using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : Enemy
{
    [Header("Set in Inspector: Enemy_2")]
    //определяет насколько ярко будет выражен синусоидный характер движения
    public float sinEccentricity = 0.6f;
    public float lifeTIme = 10f;

    [Header("Set Dynamically: Enemy_3")]
    //используется линейная интерполяция между двумя точками,
    //изменяя результат по синусоиде
    public Vector3 p0;
    public Vector3 p1;
    public float birthTime;
    
    void Start()
    {
        //выбрать случайную точку на левой границе экрана
        p0 = Vector3.zero;
        p0.x = -bndCheck.camWidth - bndCheck.radius;
        p0.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        //выбрать случайную точку на правой границе экрана
        p1 = Vector3.zero;
        p1.x = bndCheck.camWidth + bndCheck.radius;
        p1.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        //Случайно поменять начальную и конечную точку местами
        if (Random.value > 0.5f) {
            // Изменение знака .х каждой точки
            // Переносит ее на другой край экрана
            p0.x *= -1;
            p1.x *= -1;
        }
        birthTime = Time.time;
    }
    public override void Move()
    {
        //Кривые Безье вычисляются на основе значения u между 0 и 1
        float u = (Time.time - birthTime) / lifeTIme;

        //Если u>1, значит, корабль существует дальше, чем lifeTIme
        if (u > 1) {
            //экземпляр завершил свой жизненный цикл
            Main.Delete(this.gameObject);
            Destroy(this.gameObject);
            return;
        }

        // скоректировать u добавлением значения кривой, именяющейся по синусоиде
        u = u + sinEccentricity * (Mathf.Sin(Mathf.PI * u * 2));

        //интерпольровать между двумя точками
        pos = (1 - u) * p0+u*p1;

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
}
