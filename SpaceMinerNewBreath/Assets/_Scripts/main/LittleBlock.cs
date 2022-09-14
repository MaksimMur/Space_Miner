using UnityEngine;
using System;
using System.Collections.Generic;
public class LittleBlock : MonoBehaviour
{
    private BlockType _littleBlockType;
    public void SetSprite(Sprite s) {
        GetComponent<SpriteRenderer>().sprite = s;
    }
    public void TakenByMagnite() {
        MachineInterface.S.FillOnePlaceInInventory(_littleBlockType);
        this.gameObject.SetActive(false);
    }
    public BlockType type {
        get => _littleBlockType;
        set {
            if (value < BlockType.UpperOre && value > BlockType.EighthOre) {
                throw new ArgumentException("It's not ore");
            }
            _littleBlockType = value;
        }
    }
    public sbyte IndexOfOre => (sbyte)(_littleBlockType - BlockType.FirstOre);
}
