using UnityEngine;
using ZestGames;
using TMPro;

namespace GraveyardIdle
{
    public class InteractableGroundCanvas : MonoBehaviour
    {
        //private InteractableGround _interactableGround;

        //[SerializeField] private TextMeshProUGUI remainingMoneyText;

        //private readonly int _coreActivationMoney = 20;
        //private int _consumedMoney;

        //public bool PlayerIsInArea { get; set; }
        //public int RequiredMoney => _coreActivationMoney + GraveManager.AvailableGraveCount;
        //public bool MoneyCanBeSpent => DataManager.TotalMoney > 0 && _consumedMoney < RequiredMoney;

        //public void Init(InteractableGround interactableGround)
        //{
        //    _interactableGround = interactableGround;
        //    PlayerIsInArea = false;

        //    LoadData();
        //    UpdateRemainingMoneyText();

        //    GraveEvents.OnUpdateGraveActivationRequiredMoney += UpdateRemainingMoneyText;
        //}

        //private void OnDisable()
        //{
        //    if (!_interactableGround) return;

        //    GraveEvents.OnUpdateGraveActivationRequiredMoney -= UpdateRemainingMoneyText;

        //    SaveData();
        //}

        //private void UpdateRemainingMoneyText() => remainingMoneyText.text = (RequiredMoney - _consumedMoney).ToString();

        //#region PUBLICS
        //public void ConsumeMoney(int amount)
        //{
        //    if (amount > (RequiredMoney - _consumedMoney))
        //    {
        //        if (amount > DataManager.TotalMoney)
        //        {
        //            _consumedMoney += DataManager.TotalMoney;
        //            CollectableEvents.OnConsume?.Invoke(DataManager.TotalMoney);
        //        }
        //        else
        //        {
        //            CollectableEvents.OnConsume?.Invoke(RequiredMoney - _consumedMoney);
        //            _consumedMoney = RequiredMoney;
        //        }
        //    }
        //    else
        //    {
        //        if (amount > DataManager.TotalMoney)
        //        {
        //            _consumedMoney += DataManager.TotalMoney;
        //            CollectableEvents.OnConsume?.Invoke(DataManager.TotalMoney);
        //        }
        //        else
        //        {
        //            CollectableEvents.OnConsume?.Invoke(amount);
        //            _consumedMoney += amount;
        //        }
        //    }

        //    UpdateRemainingMoneyText();

        //    if (_consumedMoney >= RequiredMoney)
        //    {
        //        PlayerEvents.OnStopSpendingMoney?.Invoke();
        //        MoneyCanvas.Instance.StopSpendingMoney();
        //        AudioHandler.PlayAudio(Enums.AudioType.GraveBuilt);
        //        _interactableGround.ActivateGrave();
        //    }
        //}
        //#endregion

        //#region SAVE-LOAD
        //private void LoadData()
        //{
        //    _consumedMoney = PlayerPrefs.GetInt($"Interactable-{_interactableGround.ID}-Consume", 0);

        //    if (RequiredMoney - _consumedMoney <= 0)
        //        _interactableGround.ActivateGrave();
        //}
        //private void SaveData()
        //{
        //    PlayerPrefs.SetInt($"Interactable-{_interactableGround.ID}-Consume", _consumedMoney);
        //    PlayerPrefs.Save();
        //}
        //private void OnApplicationQuit()
        //{
        //    if (!_interactableGround) return;
        //    SaveData();
        //}
        //private void OnApplicationPause(bool pause)
        //{
        //    if (!_interactableGround) return;
        //    SaveData();
        //}
        //#endregion
    }
}
