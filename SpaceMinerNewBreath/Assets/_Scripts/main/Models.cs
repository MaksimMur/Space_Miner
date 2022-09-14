using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Models : MonoBehaviour
{
    private static Models S;
    [Header("Set in Inspector: upgread machineOptions")]
    [SerializeField] private Sprite[] _spritesModels;
    
    [Header("influence on the block")]
    [SerializeField][Range(0,1)]private float _imporveDamageBur = 0.5f;
    
    [Header("moving")]
    [SerializeField][Range(0, 1)] private float _impoveSpeedMoving = 0.4f;
    [SerializeField][Range(0, 1)] private float _improveSpeedFlying = 0.45f;

    [Header("armor")]
    [SerializeField] [Range(0, 0.1f)] private float _improveArmor=0.05f;
    [SerializeField] private byte _improveAmountBlocksWithNormalPreasure = 15;
    [SerializeField] private byte _improveHealthCapacity = 10;

    [Header("systen cold")] 
    [SerializeField] byte _improveAmountBlocksWithNormalTemperature = 14;

    [Header("fuel")]
    [SerializeField][Range(0, 0.08f)] private float _imporveReducingfuelConsumption = 0.03f;
    [SerializeField] private byte _improveFuelCapacity = 20;

    [Header("amount ore in inventory")]
    [SerializeField] private byte _imporveAmountOreInInventory = 5;

    [Header("amount items in inventory")]
    [SerializeField] private byte _imporveAmountItemsInInventory = 5;

    [Header("Electormagnite gun")]
    [SerializeField] private float _improveDamage=1;
    [SerializeField] [Range(0, 0.06f)] private float _improveFireRate = 0.05f;

    private IMachineCharacteristic Imachine;

    public static Dictionary<ModelType, Sprite> dictModelsSprites = new Dictionary<ModelType, Sprite>();
    public static Dictionary<ModelType, short> dictModelsPrice = new Dictionary<ModelType, short>() { 
        { ModelType.Bur, 1500 }, { ModelType.MotorX, 1200 }, { ModelType.MotorY, 1150 }, 
        { ModelType.ElecttroMagniteGun, 1150 },{ ModelType.Armor, 1000 },{ ModelType.SystemFreeze,600 },
        { ModelType.Fuelbarrels, 800 },{ ModelType.BoxOre, 1000 },{ ModelType.BoxItems, 1000 }
    };
    public static Dictionary<ModelType, short> dictModelsImprovePrice = new Dictionary<ModelType, short>() {
        { ModelType.Bur, 1200 }, { ModelType.MotorX, 1000 }, { ModelType.MotorY, 1000 },
        { ModelType.ElecttroMagniteGun, 1100 },{ ModelType.Armor, 800},{ ModelType.SystemFreeze,600 },
        { ModelType.Fuelbarrels, 800 },{ ModelType.BoxOre, 1000 },{ ModelType.BoxItems, 1000 }
    };
    private void Awake()
    {
       S = this;
       for (int i = 0; i < _spritesModels.Length; i++) {
            if (dictModelsSprites.ContainsKey(ModelType.Bur + i)) continue;
           dictModelsSprites.Add(ModelType.Bur + i, _spritesModels[i]);
       }
    }
    private void Start()
    {
        Imachine = Machine.S.GetComponent<IMachineCharacteristic>();
    }

    /// <summary>
    /// Take model's type and for seleceted type improve characteristics machine and other models
    /// </summary>
    /// <param name="m"> </param>
    public static void SetImprove(ModelType m) {
        switch (m) {
            case ModelType.Bur:
                S.Imachine.AddDamageForBur(S._imporveDamageBur);
                break;
            case ModelType.MotorX:
                S.Imachine.AddSpeedForRiding(S._impoveSpeedMoving);
                break;
            case ModelType.MotorY:
                S.Imachine.AddSpeedForFlying(S._improveSpeedFlying);
                break;
            case ModelType.ElecttroMagniteGun:
                Machine.S.AddGunPower(S._improveDamage, S._improveFireRate);
                break;
            case ModelType.Armor:
                S.Imachine.AddArmor(S._improveArmor,S._improveAmountBlocksWithNormalPreasure,S._improveHealthCapacity);
                break;
            case ModelType.SystemFreeze:
                S.Imachine.AddBlocksForImproveSystemCold(S._improveAmountBlocksWithNormalTemperature);
                break;
            case ModelType.Fuelbarrels:
                S.Imachine.ImproveFuelRates(S._imporveReducingfuelConsumption,S._improveFuelCapacity);
                break;
            case ModelType.BoxOre:
                MachineInterface.S.DeclareIndicatorInInterface(S._imporveAmountOreInInventory,true);
                break;
            case ModelType.BoxItems:
                Items.ImproveCapacity(S._imporveAmountItemsInInventory);
                break;
        }
    }
    
}
