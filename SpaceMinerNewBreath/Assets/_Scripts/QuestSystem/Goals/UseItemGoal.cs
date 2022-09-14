using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItemGoal : Quest.QuestGoal 
{
    public ItemType itemType;

    public string LocalizedTextKey;
    public override string GetDescriptionLocalizedKey()
    {
        return LocalizedTextKey;
    }
    public override void Initializate()
    {
        base.Initializate();
        EventManager.Instance.AddListener<UsingItemsGameEvent>(OnUsing);
    }
    public void OnUsing(UsingItemsGameEvent evenInfo)
    {
        if (evenInfo.type == itemType)
        {
            CurrentAmount++;
            Evaluate();
        }
    }
}
