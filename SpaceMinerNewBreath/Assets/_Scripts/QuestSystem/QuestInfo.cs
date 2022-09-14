using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class QuestInfo :MonoBehaviour
{
    public Text Title;
    public Text Description;
    public Text Count;
    public Text Reward;
    public Button ButtonReward;
    void Awake() {
        Title = transform.Find("Title").GetComponent<Text>();
        Description = transform.Find("Description").GetComponent<Text>();
        Count = transform.Find("Count").GetComponent<Text>();
        Reward = transform.Find("Reward").GetComponent<Text>();
        ButtonReward= transform.Find("Done").GetComponent<Button>();
    }
}
