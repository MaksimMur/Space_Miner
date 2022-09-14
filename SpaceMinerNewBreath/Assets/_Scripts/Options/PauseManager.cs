using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour, IPausedController
{
    public static PauseManager S;
    public void Awake()
    {
        S = this;
    }
    private readonly List<IPausedController> _controllers = new List<IPausedController>();
    public bool isPaused { get; private set; }
    public void Register(IPausedController controller) => _controllers.Add(controller);
    public void UnRegister(IPausedController controller) => _controllers.Remove(controller);
    public void SetPaused(bool isPaused) {
        this.isPaused = isPaused;
        for (int i = 0; i < _controllers.Count; i++) {
            _controllers[i].SetPaused(isPaused);
        }
    }
}
