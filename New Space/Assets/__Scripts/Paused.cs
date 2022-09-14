using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Paused : MonoBehaviour
{
    [SerializeField]
    GameObject pause;
    void Start()
    {
        startGame.SetActive(true);
        Time.timeScale = 0;
        pause.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && startGame.activeSelf==false)
        {
            pause.SetActive(true);
            Time.timeScale = 0;

        }
    }
    public void PauseOff() {
        pause.SetActive(false);
        Time.timeScale = 1;
    }
    public GameObject startGame;
    public void StartGamePauseOff()
    {
        startGame.SetActive(false);
        Time.timeScale = 1;
    }
    public void Exit() {
        Application.Quit();
    }
    public void Menu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }
}
