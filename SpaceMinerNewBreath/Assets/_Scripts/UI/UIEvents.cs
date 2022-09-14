using UnityEngine;

public class UIEvents : MonoBehaviour
{
    public GameObject panel;
    public void CloseGamePanelWithRemovingPause()
    {
        PauseManager.S.SetPaused(false);
        panel.SetActive(false);
    }
    public void OpenStanfartPanel() => panel.SetActive(true);
    public void OpenGamePanel(GameObject _panel) => _panel.SetActive(true);
    public void OpenGamePanelWithName(string nameOfObject)
    {
        try
        {
            GameObject.Find(nameOfObject).SetActive(true);
        }
        catch {
            throw new System.Exception($"Didn't find obejct with name: {nameOfObject}");
        }
    }
    public void CloseStandartPanel ()=>panel.SetActive(false);
    public void CloseGamePanel(GameObject _panel) => _panel.SetActive(false);
    public void CloseGamePanelWithName(string nameOfObject)
    {
        try
        {
            GameObject.Find(nameOfObject).SetActive(false);
        }
        catch
        {
            throw new System.Exception($"Didn't find obejct with name: {nameOfObject}");
        }
    }


}
