using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class AudioManager : MonoCache
{
    [Header("Set in Inspector: AudioManager")]
    [SerializeField] private Slider _soundSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private AudioMixerGroup _mixerGroup;
    [SerializeField] private AudioClip[] _music;
    private AudioSource _musicSource;
    private void Awake()
    {
        _musicSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        if (PlayerPrefs.HasKey("music"))
        {
            _musicSlider.value = PlayerPrefs.GetFloat("music");
            _mixerGroup.audioMixer.SetFloat("music", Mathf.Lerp(-80, 0, PlayerPrefs.GetFloat("music")));
        }
        if (PlayerPrefs.HasKey("sounds"))
        {
            _soundSlider.value = PlayerPrefs.GetFloat("sounds");
            _mixerGroup.audioMixer.SetFloat("sounds", Mathf.Lerp(-80, 0, PlayerPrefs.GetFloat("sounds")));
        }
    }
    public void SetMusicVolume() {
        _mixerGroup.audioMixer.SetFloat("music", Mathf.Lerp(-80, 0, _musicSlider.value));
        PlayerPrefs.SetFloat("music", _musicSlider.value);
    }
    public void SetSoundsVolume() {
        _mixerGroup.audioMixer.SetFloat("sounds", Mathf.Lerp(-80, 0, _soundSlider.value));
        PlayerPrefs.SetFloat("sounds", _soundSlider.value);
    }
    public override void OnTick()
    {
        if (_musicSource.isPlaying) return;
        _musicSource.clip = _music[Random.Range(0, _music.Length)];
        _musicSource.Play();
    }
}
