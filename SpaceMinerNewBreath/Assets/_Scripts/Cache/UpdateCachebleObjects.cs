using UnityEngine;
public class UpdateCachebleObjects : MonoBehaviour
{
    void Update() {
        for (int i = 0; i < MonoCache.allUpdates.Count; i++) {
            MonoCache.allUpdates[i].Tick();
        }
    }
    private void LateUpdate()
    {
        for (int i = 0; i < MonoCache.allLateUpdates.Count; i++) {
            MonoCache.allLateUpdates[i].LateTick();
        }
    }
}
