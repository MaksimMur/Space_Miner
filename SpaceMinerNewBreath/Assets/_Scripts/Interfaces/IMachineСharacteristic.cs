using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IMachineCharacteristic { 
    public float PosY { get; }
    public float Health { get; set; }
    public float Fuel { get; set; }
    public float MaxHealth { get; }
    public float MaxFuel { get; }
    public float ArmorKof { get; }
    public byte LevelOfBur { get; set; }
    public float GunDamage { get; }
    public float GunFireRate { get; }

    public byte MaxDepthToReach { get;}
    public void AddDamageForBur(float damage);
    public void AddSpeedForFlying(float speed);
    public void AddSpeedForRiding(float speed);
    public void AddArmor(float armor, byte amountBlocksWithNormalPresure,byte health);
    public void AddBlocksForImproveSystemCold(byte amountBlocksWithNormalTemperature);
    public void AddGunPower(float damage, float delayBetweenShot = 0.05f);
    public void ImproveFuelRates(float redComp,byte fuelCapacity);
}
