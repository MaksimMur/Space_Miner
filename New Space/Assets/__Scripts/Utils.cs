using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    //================ Функция для работы с материалом ===============\\

    //возвращает список всех материалов в жанном игровом объекте
    // и его дочерних объектов
    static public Material[] GetAllMaterials(GameObject go) {
        Renderer[] rends = go.GetComponentsInChildren<Renderer>();

        List<Material> mats = new List<Material>();
        foreach (Renderer rend in rends) {
            mats.Add(rend.material);
            }
        return (mats.ToArray());
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
