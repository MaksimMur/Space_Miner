using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameOver : MonoBehaviour
{
    static public GameOver S;
    [Header("Set in Inspector")]
    public GameObject checkGameOver;

    private void Awake()
    {
        checkGameOver = GetComponent<GameObject>();
        checkGameOver.SetActive(false);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
