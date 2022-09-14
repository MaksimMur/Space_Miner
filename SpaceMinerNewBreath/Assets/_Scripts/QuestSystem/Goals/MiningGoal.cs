using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class MiningGoal : Quest.QuestGoal
{
    public BlockType OreType;
    public override string GetDescriptionLocalizedKey()
    {
        if (OreType < BlockType.FirstOre || OreType > BlockType.EighthOre) return "Default";
        return SelectedPlanet.S.selectedPlanet._blocksQuestNameTranslateKey[OreType - BlockType.FirstOre];
    }
    public override void Initializate()
    {
        base.Initializate();
        EventManager.Instance.AddListener<MiningGameEvent>(OnMinning);
    }

    private void OnMinning(MiningGameEvent eventInfo)
    {
        if (eventInfo.type == OreType || (OreType == BlockType.FirstOre && eventInfo.type==BlockType.UpperOre))
        {
            CurrentAmount++;
            Evaluate();
        }
    }


}



