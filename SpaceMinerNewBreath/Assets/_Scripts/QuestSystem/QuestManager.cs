using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Quests {
    public Quest quest;
    public QuestInfo questInfo;
    public GameObject missionCompleteIcon;
    public Quests(QuestInfo qI, GameObject mC, Quest q) => (questInfo, missionCompleteIcon, quest) = (qI, mC, q);
}
public class QuestManager : MonoBehaviour
{
    public static QuestManager S;
    [Header("Set in Inspector: Quest Manager Options")]
    [SerializeField] private GameObject questPrefab;
    [SerializeField] private Transform questContent;

    [Header("Window options")]
    [SerializeField] private QuestInfo goalPrefab;
    [SerializeField] private Transform goalsContent;
    private AudioSource _audioSource;

   
    private List<Quests> _questsList= new List<Quests>();

    public List<Quest> CurrentQuests;

    private byte _takedMissions=0;
    private byte _currentMissionAmount = 0;
    private byte _maxMissionAmount = 3;

    private void Awake()
    {
        S = this;
        _audioSource = GetComponent<AudioSource>();
        _questsList = new List<Quests>();

        for (int i = 0; i < _maxMissionAmount; i++)
        {
            GameObject questObj = Instantiate(questPrefab);
            questObj.transform.SetParent(questContent, false);
            questObj.transform.Find("Icon").GetComponent<Image>().enabled = false;

            QuestInfo questInfo = Instantiate<QuestInfo>(goalPrefab);
            questInfo.transform.SetParent(goalsContent, false);
            _questsList.Add(new Quests(questInfo, questObj, null));

            questInfo.gameObject.SetActive(false);
            questObj.SetActive(false);
        }
        GetFreeMissions();
    }

    public void GetMission(Quest quest)
    {
        if (_questsList.Any(x => x.quest == null))
        {
            byte index = 0;
            for (byte i = 0; i < _questsList.Count; i++) {
                if (_questsList[i].quest == null) {
                    index = i;
                    break;
                }
            }
            quest.Initializate();
            quest.QuestCompleted.AddListener(OnQuestCompleted);
            _questsList[index].quest = quest;
            _questsList[index].missionCompleteIcon.SetActive(true);
            _questsList[index].missionCompleteIcon.transform.Find("Icon").GetComponent<Image>().enabled = false;
            InitializeGoal(quest, index);
            _currentMissionAmount++;
        }
    }


    public void ResetInfo()
    {
        for (int i = 0; i < _questsList.Count; i++) {
            if (_questsList[i].quest != null)
            {
                _questsList[i].questInfo.Count.text = _questsList[i].quest.Goal.CurrentAmount + "/" + _questsList[i].quest.Goal.RequiredAmount;
            } 
        }
    }
    public void GetFreeMissions()
    {
        for (int i = _currentMissionAmount; i < _maxMissionAmount; i++) {
            if (_takedMissions < CurrentQuests.Count) {
                GetMission(CurrentQuests[_takedMissions]);
                _takedMissions++;
            }
        }
    }

    public void Mine(BlockType type) {
       EventManager.Instance.QueueEvent(new MiningGameEvent(type));
    }

    public void Building(string nameBuilding)
    {
        EventManager.Instance.QueueEvent(new BuildingGameEvent(nameBuilding));
    }

    public void UseItem(ItemType itemType) {
        EventManager.Instance.QueueEvent(new UsingItemsGameEvent(itemType));
    }
    public void DestroyEnemy(string enemyName)
    {
        EventManager.Instance.QueueEvent(new DestroyedEnemiesEvent(enemyName));
    }
    private void OnQuestCompleted(Quest quest) {
        foreach (Quests qs in  _questsList) {
            if (qs.quest == quest) {
                try
                {
                    FlyText t = UIManager.S.MessageForUser(new List<Vector2>() {
                    new Vector2(Machine.S.machinePos.x, Machine.S.machinePos.y),
                    new Vector2(Machine.S.machinePos.x, Machine.S.machinePos.y+0.6f) },
                        Easing.Out, Time.time, 3f, default, default, true, "MissionComplete");
                    t.SetRectSize(new List<Vector2>() { new Vector2(8f, 0.2f) });
                    t.SetColorChange(new List<Color>() { Color.green });
                } 
                catch (System.NullReferenceException) { }
                qs.missionCompleteIcon.transform.Find("Icon").GetComponent<Image>().enabled=true;
                qs.questInfo.ButtonReward.gameObject.SetActive(true);
                Button missionComplete = qs.missionCompleteIcon.GetComponentInChildren<Button>();
                missionComplete.onClick.AddListener(delegate {
                    qs.questInfo.gameObject.SetActive(false);
                    qs.missionCompleteIcon.gameObject.SetActive(false);
                    MachineInterface.S.SetBalance(qs.quest.Reward.Currency);
                    qs.quest = null;
                    missionComplete.onClick.RemoveAllListeners();
                    qs.questInfo.ButtonReward.onClick.RemoveAllListeners();
                    Statistics.S.MissionCompletes();
                    _currentMissionAmount--;
                    _audioSource.Play();
                    GetFreeMissions();
                });
                qs.questInfo.ButtonReward.onClick.AddListener(delegate
                {
                    qs.questInfo.gameObject.SetActive(false);
                    qs.missionCompleteIcon.gameObject.SetActive(false);
                    missionComplete.onClick.RemoveAllListeners();
                    MachineInterface.S.SetBalance(qs.quest.Reward.Currency);
                    qs.quest = null;
                    qs.questInfo.ButtonReward.onClick.RemoveAllListeners();
                    Statistics.S.MissionCompletes();
                    _currentMissionAmount--;
                    _audioSource.Play();
                    GetFreeMissions();
                });
              
            }
        }
    }
   

    public void InitializeGoal(Quest quest, byte currentMission)
    {
        _questsList[currentMission].questInfo.Count.gameObject.SetActive(true);
        _questsList[currentMission].questInfo.ButtonReward.gameObject.SetActive(false);
        _questsList[currentMission].questInfo.Title.GetComponent<LocalizedText>().SetKey(quest.Information.LocalizedTitleKey);
        _questsList[currentMission].questInfo.Description.GetComponent<LocalizedText>().SetKey(quest.Goal.GetDescriptionLocalizedKey()); 
        _questsList[currentMission].questInfo.Count.text = quest.Goal.CurrentAmount + "/" + quest.Goal.RequiredAmount;


        _questsList[currentMission].questInfo.Reward.text = quest.Reward.Currency.ToString()+"$";
        _questsList[currentMission].questInfo.gameObject.SetActive(true);
    }
}
