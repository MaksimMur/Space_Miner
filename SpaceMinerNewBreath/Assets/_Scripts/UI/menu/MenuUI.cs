using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Text;

public class MenuUI : MonoBehaviour
{
    [Header("Set in Inpsector: Menu UI options")]
    public GameObject mainElements;
    public GameObject planets;
    public GameObject optionsPanel;
    public GameObject recordsPanel;
    public AudioClip buttonClick;
    public AudioClip buttonBackClick;
    public Sprite[] spriteScins;
    public SkinBlockInformation scinPrefab;
    public Transform skinsAnchor;
    private AudioSource _audioSource;
    private List<SkinBlockInformation> _listSkins = new List<SkinBlockInformation>();
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private delegate void ActionSkinButton();
    private ActionSkinButton GetAction(string type) {
        switch (type) {
            case "take":
            case "taken":
            case "buy":
                return delegate { print("a"); };
            default: throw new System.Exception("uncorrectType");
        }
    }
    private bool _planetIsOpen=false;

    public void OpenPlanets() {
        if (_planetIsOpen) return;
        _planetIsOpen = !_planetIsOpen;
        _audioSource.PlayOneShot(buttonClick);
        planets.SetActive(true);
        GameObject.Find("mainMenu").GetComponent<Animator>().SetTrigger("openPlanets");
    }
    public void ClosePlanets() {
        if (!_planetIsOpen) return;
        _planetIsOpen = !_planetIsOpen;
        _audioSource.PlayOneShot(buttonClick);
        GameObject.Find("mainMenu").GetComponent<Animator>().SetTrigger("closePlanets");
    }
    public void Back() {
        _audioSource.PlayOneShot(buttonBackClick);
        mainElements.SetActive(true);
        planets.SetActive(false);
    }

    private bool _optionsPanelIsOpen = false;
    public void ClickOptions() {
        if (!_optionsPanelIsOpen)
        {
            optionsPanel.SetActive(true);
            optionsPanel.GetComponent<Animator>().SetTrigger("open");
            _audioSource.PlayOneShot(buttonClick);
        }
        else {
            optionsPanel.GetComponent<Animator>().SetTrigger("close");
            _audioSource.PlayOneShot(buttonBackClick);
        }
        _optionsPanelIsOpen = !_optionsPanelIsOpen;
    }

    private bool _recordsPanelIsOpen = false;
    public void ClickRecords()
    {
        if (!_recordsPanelIsOpen)
        {
            recordsPanel.SetActive(true);
            recordsPanel.GetComponent<Animator>().SetTrigger("open");
            _audioSource.PlayOneShot(buttonClick);
        }
        else
        {
            recordsPanel.GetComponent<Animator>().SetTrigger("close");
            _audioSource.PlayOneShot(buttonBackClick);
        }
        _recordsPanelIsOpen = !_recordsPanelIsOpen;
    }
    public void SetScins() {
        for (byte i = 0; i < spriteScins.Length; i++) {
            SkinBlockInformation go = Instantiate<SkinBlockInformation>(scinPrefab);
        }
    }
}
