using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckFallen : MonoBehaviour
{
    private Machine _machine;
    private bool getDamage=false;
    private AudioSource _audioSource;
    private void Awake()
    {
        _machine = GetComponentInParent<Machine>();
        _audioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.GetComponent<Block>() != null || collision.GetComponent<Plate>() != null) && !getDamage)
        {
            if (_machine.MachineFallen())
            {
                _audioSource.Play();
                getDamage = true;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        getDamage = false;
    }
}
