using UnityEngine;
using UnityEngine.UI;

public enum GameEndState { 
    fuelIsEnd,
    HeatlhIsEnd,
    Win
}
public class GameEnd : MonoBehaviour
{
    [Header("Set in Inspector: GameEnd options")]
    [SerializeField]private Text _tTextAmountMinedBlocks;
    [SerializeField] private Text _tTextAmountDestroyedEnemies;
    [SerializeField] private Text _tTextMaxDepth;
    [SerializeField] private Text _tTextAmountAchievedMissions;
    [SerializeField] private Text _tTextAmountUsedItems;
    private Animator _animatorGameEnd;
    private static GameEnd S;
    private void Awake()
    {
        S = this;
        _animatorGameEnd = GetComponent<Animator>();
    }
    public static void GameIsEnd(GameEndState state) {
        switch (state){
            case GameEndState.fuelIsEnd:
                S.GameEndWithFuelLack();
                break;
            case GameEndState.HeatlhIsEnd:
                S.GameEndWithHp();
                break;
            case GameEndState.Win:
                S.GameEndWithWin();
                break;
        }
    }
    public void GameEndWithFuelLack() {
        S._animatorGameEnd.SetTrigger("LackOfFuel");
    }
    public void GameEndWithHp() {
        S._animatorGameEnd.SetTrigger("LackOfHP");
    }
    private void GetStatistics() {
        _tTextAmountMinedBlocks.text += $" {Statistics.S.MinedBlocksAmount}";
        _tTextAmountDestroyedEnemies.text += $" {Statistics.S.DestroyedEnemiesAmount}";
        _tTextAmountUsedItems.text += $" {Statistics.S.UsedItemsAmount}";
        _tTextMaxDepth.text += $" {Statistics.S.MaxDepth}";
        _tTextAmountAchievedMissions.text += $" {Statistics.S.CompletedMisionsAmount}";
    }
    public void GameEndWithWin() {
        S._animatorGameEnd.SetTrigger("GameWin");
    }
}
