using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameEvent {
    public string EventDescription;
}

public class DestroyedEnemiesEvent : GameEvent {
    public string Name;
    public DestroyedEnemiesEvent(string name) => (Name) = (name);
}
public class UsingItemsGameEvent : GameEvent { 
    public ItemType type;
    public UsingItemsGameEvent(ItemType type) => (this.type) = (type); 
}
public class BuildingGameEvent : GameEvent
{
    public string BuildingName;
    public BuildingGameEvent(string name)
    {
        BuildingName = name;
    }
}

public class MiningGameEvent : GameEvent
{
    public BlockType type;
    public MiningGameEvent(BlockType t)
    {
        type = t;
    }
}