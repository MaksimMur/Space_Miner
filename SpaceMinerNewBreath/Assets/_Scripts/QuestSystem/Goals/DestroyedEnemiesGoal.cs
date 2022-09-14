using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedEnemiesGoal : Quest.QuestGoal
{
    public string enemyName;
    public string LocalizedTextKey;
    public override string GetDescriptionLocalizedKey()
    {
        return LocalizedTextKey;
    }
    public override void Initializate()
    {
        base.Initializate();
        EventManager.Instance.AddListener<DestroyedEnemiesEvent>(OnDestroyed);
    }
    public void OnDestroyed(DestroyedEnemiesEvent evenInfo)
    {
        if (evenInfo.Name == enemyName)
        {
            CurrentAmount++;
            Evaluate();
        }
    }
}
