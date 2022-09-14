using UnityEngine;

public class BuildingZone : MonoBehaviour
{
    private Building _building;
    private void Start()
    {
        _building = GetComponentInParent<Building>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Machine>()) {
            _building.BuildingZone(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Machine>())
        {
            _building.BuildingZone(false);
        }
    }
}
