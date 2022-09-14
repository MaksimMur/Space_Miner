﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralax : MonoBehaviour
{
    [Header("Set in Inspector")]
    public GameObject poi;//Корабль игркоа
    public GameObject[] panels;//Прокручиваемыепанели переднего плана
    public float scrollSpeed = -30f;
    //Определяет степень реакции панелей на перемещение корабля игрока
    public float motionMult = 0.25f;

    private float panelHt;//высота каждой панели
    private float depth;//Глубина панелей
    void Start()
    {
        panelHt = panels[0].transform.localScale.y;
        depth = panels[0].transform.position.z;

        //Установить панели в начальные позиции
        panels[0].transform.position = new Vector3(0, 0, depth);
        panels[1].transform.position = new Vector3(0, panelHt, depth);
    }

    // Update is called once per frame
    void Update()
    {
        float tY, tX = 0;
        tY = Time.time * scrollSpeed % panelHt + (panelHt * 0.5f);

        if (poi != null) {
            tX = -poi.transform.position.x * motionMult;
        }

        //Сместить панель panel[0]
        panels[0].transform.position = new Vector3(tX, tY, depth);

        //Сместить панель panel[1], чтобы создать эффект непрерывности звездного поля
        if (tY >= 0)
        {
            panels[1].transform.position = new Vector3(tX, tY - panelHt, depth);
        }
        else {
            panels[1].transform.position = new Vector3(tX, tY + panelHt, depth);
        }
    }
}
