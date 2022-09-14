using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGoal : Quest.QuestGoal
{
    public string nameBuilding;
    public string LocalizedTextKey;
    public override string GetDescriptionLocalizedKey()
    {
        return LocalizedTextKey;
    }
    public override void Initializate()
    {
        base.Initializate();
        EventManager.Instance.AddListener<BuildingGameEvent>(OnBuilding);
    }
    public void OnBuilding(BuildingGameEvent evenInfo) {
        if (evenInfo.BuildingName == nameBuilding) {
            CurrentAmount++;
            Evaluate();
        }
    }

}
