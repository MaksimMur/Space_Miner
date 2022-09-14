using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
[System.Serializable]
public class Quest 
{   
    [System.Serializable]
    public struct Info {
        public string LocalizedTitleKey;
        public Sprite icon;
        public string LocalizedTitleDescription;
    }
    [Header("Info")]
    public Info Information;

    [System.Serializable]
    public struct Stat {
        public int Currency;
    }
    [Header("Reward")] public Stat Reward = new Stat{Currency=1000};

    public bool Completed { get; protected set; }
    public QuestCompleteEvent QuestCompleted;

    public abstract class QuestGoal : MonoBehaviour
    {
        protected string LocalizedTitleDescription;
        public int CurrentAmount { get; protected set; }
        public int RequiredAmount=1;
        public bool Completed { get; protected set; }
        [HideInInspector] public UnityEvent GoalCompleted;

        public virtual string GetDescriptionLocalizedKey() {
            return LocalizedTitleDescription;
        }

        public virtual void Initializate()
        {
            Completed = false;
            CurrentAmount = 0;
            GoalCompleted = new UnityEvent();
        }

        protected void Evaluate() {
            QuestManager.S.ResetInfo();
            if (CurrentAmount >= RequiredAmount && !Completed) {
                Complete();
                return;
            }
           
        }
        private void Complete() {
            Completed = true;
            GoalCompleted.Invoke();
            GoalCompleted.RemoveAllListeners();
        }

        public void Skip() {
            Complete();
        }


       
    }
    public QuestGoal Goal;
    public void Initializate() {
        Completed = true;
        QuestCompleted = new QuestCompleteEvent();
        Goal.Initializate();
        Goal.GoalCompleted.AddListener(delegate { CheckGoals(); });
        
    }
    private void CheckGoals() {
        Completed = Goal.Completed;
        if (Completed) {
            QuestCompleted.Invoke(this);
            QuestCompleted.RemoveAllListeners();
        }
    }
}

public class QuestCompleteEvent : UnityEvent<Quest> { }

