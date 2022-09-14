using UnityEngine;
using UnityEngine.UI;
public class SkinBlockInformation : MonoBehaviour
{
    public Image skinImage;
    public Button buttonAction;
    public LocalizedText buttonLocalizedText;
    private void Awake()
    {
        skinImage = transform.Find("SkinImage").GetComponent<Image>();
        buttonAction = transform.Find("ButtonAction").GetComponent<Button>();
        buttonLocalizedText = transform.Find("ButtonLocalizedText").GetComponent<LocalizedText>();
    }
}
