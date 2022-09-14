using UnityEngine;

public class BtnSwitchLang : MonoBehaviour
{
    [SerializeField]
    private LocalizationManager localizationManager;

    [System.Obsolete]
    public void OnButtonClick()
    {
        localizationManager.CurrentLanguage = name;
    }
}
