using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sand : Block,IPausedController
{
    [Header("set sand options")]
    [SerializeField] private float _speedOFFalling=2f;
    [SerializeField] private float _damageForMachine = 30f;
    GameObject _boarder;

    private void Awake()
    {
        GetComponent<SpriteRenderer>().color = SelectedPlanet.S.selectedPlanet.planetColor;
        PauseManager.S.Register(this);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Block>() || collision.gameObject.GetComponent<Plate>()) {
            Vector2 pos = collision.gameObject.transform.position;
            if (pos.y < Y && pos.x + .5f > X && pos.x - .5f < X) { 
                _boarder = collision.gameObject;
                transform.position = new Vector2(pos.x, pos.y + 1);
            }
        }
    }
    private void OnDestroy()
    {
        PauseManager.S.UnRegister(this);
    }
    [System.Obsolete]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Machine>()) {
            Machine.S.GetDamage(_damageForMachine);
            Destroyed();
      
        }
    }
    override public void OnTick() {

        if (_isPaused) return;
        if (_boarder != null|| BlockManager.S.DepthLevelWithDiffToSpawn <= Mathf.Abs(transform.position.y)) return;
        transform.position += Vector3.down * _speedOFFalling * Time.deltaTime;
    }
    private bool _isPaused = false;
    public void SetPaused(bool isPaused) {
        _isPaused = isPaused;
    }
}
