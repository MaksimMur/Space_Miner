using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;
public class Building : MonoBehaviour
{
    [Header("Set building options")]
    [SerializeField] private GameObject _building;
    private List<GameObject> _buildingParts;
    private byte _currentPart;
    private byte _maxCount;
    [SerializeField] private GameObject _buildUI;
    [SerializeField] private Text _tAmountOre;
    [SerializeField] private Image _spriteBlock;
    [SerializeField] private GameObject _buidlingParticles;
    [SerializeField] private AudioClip _addOreSound;
    [SerializeField] private AudioClip _notEnoughOreSound;
    private AudioSource _audioSource;
    private byte _currentOreAmount = 0;
    private byte _maxOreAmount = 10;
    private void Awake()
    {
        _buildingParts = _building.GetComponentsInChildren<SpriteRenderer>().Select(x => x.gameObject).ToList();
        _maxCount = (byte)_buildingParts.Count;
        for (byte i = 0; i < _buildingParts.Count; i++) _buildingParts[i].SetActive(false);
        GetPartBuilding();
        _audioSource = GetComponent<AudioSource>();
    }

    private void  GetPartBuilding() {
        _spriteBlock.sprite = SelectedPlanet.S.selectedPlanet.littleBlocks[_currentPart];
        _tAmountOre.text = _currentOreAmount + "/" + _maxOreAmount;
    }
    public void BuildingZone(bool visit) {
        _buildingParts[_currentPart].SetActive(visit);
        _buildUI.SetActive(visit);
    }
    public void TopIt() {
        byte neededAmountOre = (byte)(_maxOreAmount - _currentOreAmount);
        if (MachineInterface.S.OreTypeEnough(BlockType.FirstOre+_currentPart)) {
            byte amountOreInInvetory = MachineInterface.S.GetAmountTypeOre(BlockType.FirstOre+_currentPart);
            if (neededAmountOre > amountOreInInvetory)
            {
                _currentOreAmount += amountOreInInvetory;
                TopItOre(amountOreInInvetory);
            }
            else {
                _currentOreAmount += neededAmountOre;
                TopItOre(neededAmountOre);
            }
            if (_currentOreAmount == _maxOreAmount) {
                _buildingParts[_currentPart].GetComponent<SpriteRenderer>().color = Color.white;
                _currentOreAmount = 0;

                GameObject go = Instantiate(_buidlingParticles);
                go.transform.position = _buildingParts[_currentPart].transform.position;

               _currentPart++;
                if (_currentPart >= _buildingParts.Count) {
                    GameEnd.GameIsEnd(GameEndState.Win);
                    return;
                }
               _buildingParts[_currentPart].SetActive(true);
                GetPartBuilding();
            }
            _tAmountOre.text = _currentOreAmount + "/" + _maxOreAmount;
            _audioSource.PlayOneShot(_addOreSound);
            return;
        }
        _audioSource.PlayOneShot(_notEnoughOreSound);
    }
    public void TopItOre(byte amount) {
        for (byte i = 0; i < amount; i++) {
            MachineInterface.S.TopItOneOre(BlockType.FirstOre+_currentPart);
        }
    }
}
