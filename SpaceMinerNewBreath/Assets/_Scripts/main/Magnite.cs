using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnite : MonoBehaviour
{
    [Header("Set in Inspector: Magnite options")]
    [SerializeField][Range(0, 1)]private float _distanceBetweenMachienAndLittleBlockForTaken = .2f;
    [SerializeField][Range(0.2f, 1)]private float _magniteSpeedAttraction = .5f;
    private HashSet<LittleBlock> littleBlocksHashSet;
    private List<LittleBlock> removeList= new List<LittleBlock>();
    private void Awake()
    {
        littleBlocksHashSet = new HashSet<LittleBlock>();
    }
    /// <summary>
    /// This method Take collision and check if collision is LittleBlock then object forward to hashset and then moving to machine
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!MachineInterface.S.EnoughInvenoryPlace) return;
        LittleBlock l = collision.gameObject.GetComponent<LittleBlock>();
        if (l!=null) {
            littleBlocksHashSet.Add(l);
            foreach (LittleBlock lit in littleBlocksHashSet) {
                lit.transform.position = Vector2.Lerp(lit.transform.position, Machine.S.machinePos,_magniteSpeedAttraction);
                if (Vector2.Distance(lit.transform.position, Machine.S.machinePos) <_distanceBetweenMachienAndLittleBlockForTaken) removeList.Add(lit);
            }
            foreach (LittleBlock lit in removeList) {
                if (!MachineInterface.S.EnoughInvenoryPlace) return;
                littleBlocksHashSet.Remove(lit);
                lit.TakenByMagnite();
            }
            removeList.Clear();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        LittleBlock l = collision.gameObject.GetComponent<LittleBlock>();
        if (l != null && littleBlocksHashSet.Contains(l))
        {
            littleBlocksHashSet.Remove(l);
        }
    }

}
