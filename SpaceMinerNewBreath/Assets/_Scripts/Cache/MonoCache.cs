using System.Collections.Generic;
using UnityEngine;
public class MonoCache : MonoBehaviour
{
    public static List<MonoCache> allUpdates= new List<MonoCache>(1001);
    public static List<MonoCache> allLateUpdates = new List<MonoCache>(1001);
    protected virtual void OnEnable() => allUpdates.Add(this);
    protected virtual void OnDisable() => allUpdates.Remove(this);
    private void OnDestroy() => allUpdates.Remove(this);
    protected void AddLateUpadte() => allLateUpdates.Add(this);
    protected void RemoveLateUpdate() => allLateUpdates.Remove(this);
    public void LateTick() => OnLateTick();
    public virtual void OnLateTick() { }
    public void Tick() => OnTick();
    public virtual void OnTick() { }
}
