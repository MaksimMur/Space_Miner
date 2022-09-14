using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class SelectedPlanet : MonoBehaviour
{

    public static SelectedPlanet S;
    [Header("Set in Inspector: PlanetOptions")]
    [SerializeField] private Planet _Moon;
    [SerializeField] private Planet _Mars;
    [SerializeField] private Planet _Venera;
    [SerializeField] private Transform _background;
    private Planet _selectedPlanet;
    private void Awake()
    {
        if (S == null) S = this;
        else Debug.Log("SelectedPlanet.Awake(): was try to connect in second time");
        _selectedPlanet = _Mars;
        SpriteRenderer[] backSprites =_background.GetComponentsInChildren<SpriteRenderer>();
        for (byte i = 0; i < backSprites.Length; i++) {
            backSprites[i].color = _selectedPlanet.planetColor;
        }
    }
    public Planet selectedPlanet => _selectedPlanet;
}
