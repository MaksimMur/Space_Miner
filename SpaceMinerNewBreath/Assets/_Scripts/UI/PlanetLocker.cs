using UnityEngine;

public class PlanetLocker : MonoBehaviour
{
   public string planetName;
   private AudioSource _audioSource;
    void Awake() {
        if (PlayerPrefs.HasKey(planetName + "isOpen"))
        {
            if (PlayerPrefs.GetInt(planetName + "isOpen") == 1)
            {
                Destroy(this.gameObject);
            }
        }
        else PlayerPrefs.SetInt(planetName + "isOpen", 0);
        _audioSource = GetComponent<AudioSource>();
    }

   public void Close() {
        _audioSource.Play();
   }
}
