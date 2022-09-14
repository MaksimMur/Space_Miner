using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
public class Options : MonoBehaviour, IHints, ILightExp
{
    public static Options S;
   
    private readonly List<IHints> _listIHints = new List<IHints>();
    private readonly List<ILightExp> _listLights = new List<ILightExp>();
    public Renderer2DData data;
    public UniversalRenderPipelineAsset asset;
    [Header("Set in Inspector: Options")]
    [SerializeField] private GameObject[] _optionsSections;
    [SerializeField] private GameObject _defaultOptionsSection;
    [SerializeField] private AudioClip _openSectionSound;
    [SerializeField] private AudioClip _closeSectionSound;
    [Header("Set in Inpsector: graphicsOptions")]
    [SerializeField] private Slider _sliderHDR;
    [SerializeField] private LocalizedText _textHDR;
    [SerializeField] private Slider _sliderMSAA;
    [SerializeField] private Text _textMSAA;
    [SerializeField] private Slider _sliderRS;
    [SerializeField] private Text _textRS;
    [SerializeField] private Slider _sliderLE;
    [SerializeField] private LocalizedText _textLE;
    [SerializeField] private Slider _sliderLB;
    [SerializeField] private LocalizedText _textLB;

    [Header("Set in Inpsector: otherOptions")]
    [SerializeField] private Slider _sliderHints;
    [SerializeField] private LocalizedText _textHints;
    [SerializeField] private Slider _sliderJoysticSide;
    [SerializeField] private LocalizedText _textJoysticSide;
    [SerializeField] private RectTransform _joysticMovemnet;
    [SerializeField] private RectTransform _itemsAnchor;

    //tune buildings light
    [SerializeField] private Material _lit;
    [SerializeField] private Material _unLit;
    [SerializeField] private SpriteRenderer[] _buildingsSP;
    [SerializeField] private GameObject[] _buildingsLight;

    private AudioSource _audioSource;
    //save options
    private string _saveGraphicsOptionsPath;
    private string _saveOtherOptionsPath;
    private void Awake()
    {
        S = this;
        _saveGraphicsOptionsPath = Application.persistentDataPath + "/saveGraphicsOptions.gameOptionsSave";
        _saveOtherOptionsPath = Application.persistentDataPath + "/saveOtherOptions.gameOptionsSave";
        _audioSource = GetComponent<AudioSource>();
    }
    public void Start()
    {
        LoadGraphicsOption();
        LoadOtherOptions();
    }
    //other
    public void RegisterHint(IHints hint)
    {
        _listIHints.Add(hint);
        hint.SetHints(ShowHints);
    }
    public void UnRegisterHint(IHints hint) => _listIHints.Remove(hint);
    public void SetHint(bool show) {
        ShowHints = _sliderHints.value==1 ||show;
        _textHints.SetKey(_sliderHints.value==1?"Yes":"No");
        for (int i = 0; i < _listIHints.Count; i++) {
            _listIHints[i].SetHints(ShowHints);
        }
    }
    public void SetJoysticsSides() {
        
        if (_sliderJoysticSide.value==0)
        {
            _textJoysticSide.SetKey("No");
            IsLeft = false;
            if (_joysticMovemnet == null || _itemsAnchor==null) return;
            _joysticMovemnet.anchorMin = new Vector2(0.764f, 0);
            _joysticMovemnet.anchorMax = new Vector2(0.764f, 0);

            _itemsAnchor.anchorMin = new Vector2(0.585f, 0);
            _itemsAnchor.anchorMax = new Vector2(0, 0);

        }
        else {
            _textJoysticSide.SetKey("Yes");
            IsLeft = true;
            if (_joysticMovemnet == null || _itemsAnchor == null) return;
            _joysticMovemnet.anchorMin = new Vector2(0, 0);
            _joysticMovemnet.anchorMax = new Vector2(0, 0);

            _itemsAnchor.anchorMin = new Vector2(2f, 0);
            _itemsAnchor.anchorMax = new Vector2(0, 0);
        }
    }

    public void SaveOtherOptions()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = new FileStream(_saveOtherOptionsPath, FileMode.OpenOrCreate);
        SaveOtherOptions save = new SaveOtherOptions();
        //action save
        save.JoystickIsLeft = IsLeft;
        save.ShowHint = ShowHints;
        bf.Serialize(fs, save);
        fs.Close();
    }
    public void LoadOtherOptions()
    {
        if (File.Exists(_saveOtherOptionsPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(_saveOtherOptionsPath, FileMode.Open);
            SaveOtherOptions save = new SaveOtherOptions();
            save = (SaveOtherOptions)bf.Deserialize(fs);
            //set Hints;
            _sliderHints.value = save.ShowHint ? 1 : 0;
            SetHint(save.ShowHint);
            //set joystic pos
            _sliderJoysticSide.value = save.JoystickIsLeft ? 1 : 0;
            SetJoysticsSides();
            fs.Close();
            return;
        }
        SetOtherOptionsDefault();
        SaveOtherOptions();
    }
    private void SetOtherOptionsDefault()
    {
        _sliderHints.value = 1;
        _sliderJoysticSide.value = 1;
        SetJoysticsSides();
        SetHint(true);
    }
        //graphics
    public void RegisterLightExp(ILightExp exp)
    {
        _listLights.Add(exp);
        exp.SetLightExp(IsLightExp);
    }
    public void UnRegisLightExp(ILightExp exp) => _listLights.Remove(exp);
    public void SetHDR() {

        if (_sliderHDR.value == 0)
        {
            asset.supportsHDR = false;
            _textHDR.SetKey("Off");
            return;
        }
        asset.supportsHDR = true;
        _textHDR.SetKey("On");
    }
    public void SetMSAA() {
        asset.msaaSampleCount = (int)Mathf.Pow(2, _sliderMSAA.value);
        _textMSAA.text = _sliderMSAA.value==0?"0x":((int)Mathf.Pow(2, _sliderMSAA.value)).ToString("0x");
    }
    public void SetRendererScale()
    {
        asset.renderScale = (float)System.Math.Round(_sliderRS.value,2);
        _textRS.text = _sliderRS.value.ToString("0.00");
    }

    public void SetBuildingLight() {
        IsLightBuilding = _sliderLB.value == 1 ? true : false;
        _textLB.SetKey(_sliderLB.value==1 ? "On" : "Off");
        if (_buildingsSP.Length == 0 || _buildingsLight.Length == 0) return;
        for (int i = 0; i < _buildingsSP.Length; i++) _buildingsSP[i].material = IsLightBuilding ? _lit : _unLit;
        for (int i = 0; i < _buildingsLight.Length; i++) _buildingsLight[i].SetActive(IsLightBuilding);
    }
    private void SetGraphiscOptionsDefault() {
        //set hdr
        _sliderHDR.value = 1;
        SetHDR();
        //set MSAA
        _sliderMSAA.value = 0;
        SetMSAA();
        //set rend scale
        _sliderRS.value = 1;
        SetRendererScale();
        //set Light
        _sliderLE.value = 1;
        SetLightExp(true);
        _sliderLB.value = 1;
        SetBuildingLight();
    }

    public void SetLightExp(bool setLightExp)
    {
        if (_sliderLE.value == 0 && !setLightExp)
        {
            IsLightExp = false;
            for (int i = 0; i < _listLights.Count; i++) _listLights[i].SetLightExp(false);
            _textLE.SetKey("Off");
            return;
        }
        IsLightExp = true;
        _textLE.SetKey("On");
        for (int i = 0; i < _listLights.Count; i++) _listLights[i].SetLightExp(true);
    }
    public void SaveGraphicsOption()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = new FileStream(_saveGraphicsOptionsPath, FileMode.OpenOrCreate);
        //action Save
        SaveGraphicsOptions saveGraphicsOptions = new SaveGraphicsOptions();
        saveGraphicsOptions.HDR = asset.supportsHDR;
        saveGraphicsOptions.MSAA = _sliderMSAA.value;
        saveGraphicsOptions.RendererScale = asset.renderScale;
        saveGraphicsOptions.IsExpLightEnabled = IsLightExp;
        saveGraphicsOptions.NaturalBuildingLight = IsLightBuilding;
        bf.Serialize(fs, saveGraphicsOptions);
        fs.Close();
    }
   
    public void LoadGraphicsOption() {
        if (File.Exists(_saveGraphicsOptionsPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(_saveGraphicsOptionsPath, FileMode.Open);
            SaveGraphicsOptions saveGraphicsOptions = new SaveGraphicsOptions();
            saveGraphicsOptions=(SaveGraphicsOptions)bf.Deserialize(fs);
            //set HDR
            _sliderHDR.value = saveGraphicsOptions.HDR?1:0;
            SetHDR();
            //set MSAA
            _sliderMSAA.value = saveGraphicsOptions.MSAA;
            SetMSAA();
            //set Renderer Scale
            _sliderRS.value = saveGraphicsOptions.RendererScale;
            SetRendererScale();
            //set LigthExp
            _sliderLE.value = saveGraphicsOptions.IsExpLightEnabled?1:0;
            SetLightExp(saveGraphicsOptions.IsExpLightEnabled);
            //set Builidng Light
            _sliderLB.value = saveGraphicsOptions.NaturalBuildingLight ? 1 : 0;
            SetBuildingLight();
            fs.Close();
            return;
        }
        SetGraphiscOptionsDefault();
        SaveGraphicsOption();
    }

    public void OpenOptionsSection(GameObject section) {
            
        for (byte i = 0; i < _optionsSections.Length; i++) {
            _optionsSections[i].SetActive(false);
        }
        _audioSource.PlayOneShot(_openSectionSound);
        section.SetActive(true);
    }
    public void ReturnToDefaultOptionsSection() {
        for (byte i = 0; i < _optionsSections.Length; i++)
        {
            _optionsSections[i].SetActive(false);
        }
        _audioSource.PlayOneShot(_openSectionSound);
        _defaultOptionsSection.SetActive(true);
    }
    private void OnApplicationQuit()
    {
        SaveGraphicsOption();
        SaveOtherOptions();
    }
    public bool ShowHints { private set; get; }
    public bool IsLeft { private set; get; }
    public bool IsLightExp { get; private set; }
    public bool IsLightBuilding { get; private set; }
}

[System.Serializable] 
public class SaveGraphicsOptions {
    public bool HDR;
    public float MSAA;
    public float RendererScale;
    public bool IsExpLightEnabled;
    public bool NaturalBuildingLight;
}
[System.Serializable]
public class SaveOtherOptions {
    public bool JoystickIsLeft;
    public bool ShowHint;
}
