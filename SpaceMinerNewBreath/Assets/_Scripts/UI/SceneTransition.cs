using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SceneTransition : MonoBehaviour
{
    //public Text LoadingPercentage;
    //public Image LoadingProgressBar;

    private static SceneTransition instance;
    private static bool shouldPlayClosingAnimation = false;

    private Animator componentAnimator;
    private AsyncOperation loadingSceneOperation;

   private bool isClick = false;
    public static void SwitchToScene(string sceneName)
    {
        Options.S.SaveGraphicsOption();
        Options.S.SaveOtherOptions();
        if (instance.isClick) return;
        instance.isClick = true;
        instance.loadingSceneOperation = SceneManager.LoadSceneAsync(sceneName);
        // Чтобы сцена не начала переключаться пока играет анимация closing:
        instance.loadingSceneOperation.allowSceneActivation = false;
    }

    private void Start()
    {
        instance = this;

        componentAnimator = GetComponent<Animator>();

        if (shouldPlayClosingAnimation)
        {
            switch (PlayerPrefs.GetString("LoadedPlanetName")){
                case "Moon":
                    componentAnimator.SetTrigger("moonClosing");
                    break;
                case "Mars":
                    componentAnimator.SetTrigger("marsClosing");
                    break;
                case "Venera":
                    componentAnimator.SetTrigger("veneraClosing");
                    break;
                default:
                    componentAnimator.SetTrigger("menuClosing");
                    break;
            }
            // Чтобы если следующий переход будет обычным SceneManager.LoadScene, не проигрывать анимацию opening:
            shouldPlayClosingAnimation = false;
        }
    }



    public void OnAnimationOver()
    {
        // Чтобы при открытии сцены, куда мы переключаемся, проигралась анимация opening:
        shouldPlayClosingAnimation = true;

        loadingSceneOperation.allowSceneActivation = true;
    }
    public void OpenMenu() {
        instance.componentAnimator.SetTrigger("menuOpening");
        PlayerPrefs.SetString("LoadedPlanetName", "Nothing");
        SwitchToScene("Menu");
       
    }
    public void OpenPlanet(string name)
    {
        switch (name)
        {
            case "Moon":
                instance.componentAnimator.SetTrigger("moonOpening");
                PlayerPrefs.SetString("LoadedPlanetName", "Moon");
                SwitchToScene("SampleScene");
                break;
            case "Mars":
                instance.componentAnimator.SetTrigger("marsOpening");
                PlayerPrefs.SetString("LoadedPlanetName", "Mars");
                SwitchToScene("SampleScene");
                break;
            case "Venera":
                instance.componentAnimator.SetTrigger("veneraOpening");
                PlayerPrefs.SetString("LoadedPlanetName", "Venera");
                SwitchToScene("SampleScene");
                break;
            default:
                break;
        }

    }

}