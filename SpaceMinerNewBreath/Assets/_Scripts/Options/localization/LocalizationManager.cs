using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LocalizationManager : MonoBehaviour
{
    private string currentLanguage;
    private Dictionary<string, string> localizedText;
    public static bool isReady = false;

    public delegate void ChangeLangText();
    public event ChangeLangText OnLanguageChanged;
    [Obsolete]
    void Awake()
    {
        if (!PlayerPrefs.HasKey("Language"))
        {
            if (Application.systemLanguage == SystemLanguage.Russian || Application.systemLanguage == SystemLanguage.Ukrainian || Application.systemLanguage == SystemLanguage.Belarusian)
            {
                PlayerPrefs.SetString("Language", "ru_RU");
            }
            else
            {
                PlayerPrefs.SetString("Language", "en_US");
            }
        }
        currentLanguage = PlayerPrefs.GetString("Language");
        LoadLocalizedText(currentLanguage);
    }

    [Obsolete]
    public void LoadLocalizedText(string langName)
    {
        string path = Application.streamingAssetsPath + "/Languages/" + langName + ".json";

        string dataAsJson;

        if (Application.platform == RuntimePlatform.Android)
        {
            WWW reader = new WWW(path);
            while (!reader.isDone) { };
            dataAsJson = reader.text;
        }
        else
        {
            dataAsJson = File.ReadAllText(path);
        }

        LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

        localizedText = new Dictionary<string, string>();
        for (int i = 0; i < loadedData.items.Length; i++)
        {
            localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
        }

        PlayerPrefs.SetString("Language", langName);
        isReady = true;
        OnLanguageChanged?.Invoke();
    }

    public string GetLocalizedValue(string key)
    {
        if (localizedText.ContainsKey(key))
        {
            return localizedText[key];
        }
        else
        {
            throw new Exception("Localized text with key \"" + key + "\" not found");
        }
    }

    [Obsolete]
    public string CurrentLanguage
    {
        get
        {
            return currentLanguage;
        }
        set
        {

            PlayerPrefs.SetString("Language", value);
            currentLanguage = PlayerPrefs.GetString("Language");
            LoadLocalizedText(currentLanguage);
        }
    }
    public bool IsReady
    {
        get
        {
            return isReady;
        }
    }
    public void Restart() {
        OnLanguageChanged?.Invoke();
    }
}