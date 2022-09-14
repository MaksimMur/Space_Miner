using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiusHint : MonoBehaviour,IHints
{
    [SerializeField] private GameObject _hints;
    private bool _showHints=false;
    private bool _blockHints = false;
    void Awake() {
        Options.S.RegisterHint(this);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Machine>() && _showHints && !_blockHints)
        {
            _hints.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Machine>() && _showHints)
        {
            _hints.SetActive(false);
        }
    }
    public void BlockHints(bool blockHints) {
        _blockHints = blockHints;
        _hints.SetActive(!_blockHints);
    }
    public void SetHints(bool show)
    {
        _showHints = show;
        if (!show) _hints.SetActive(false);
    }
}
