using TMPro;
using UnityEngine;
using ZestCore.Utility;
using ZestGames;

namespace GraveyardIdle.GraveSystem
{
    public class FinishedGraveUpgradeHandler : MonoBehaviour
    {
        [Header("-- GENERAL SETUP --")]
        [SerializeField] private GameObject upgradeAreaObj;

        [Header("-- MONEY CONSUME SETUP --")]
        [SerializeField] private TextMeshProUGUI remainingMoneyText;
        [SerializeField] private Transform moneyImageTransform;

        private FinishedGrave _finishedGrave;

        #region MONEY SECTION
        private const int CORE_REQUIRED_MONEY = 50;
        private int _requiredMoney => CORE_REQUIRED_MONEY * _finishedGrave.CurrentLevel;
        private int _consumedMoney;
        #endregion

        #region PROPERTIES
        public bool MoneyCanBeSpent => DataManager.TotalMoney > 0 && _consumedMoney < _requiredMoney;
        public Transform MoneyImageTransform => moneyImageTransform;
        #endregion

        public void Init(FinishedGrave finishedGrave)
        {
            if (_finishedGrave == null)
                _finishedGrave = finishedGrave;

            Load();
        }

        private void OnDisable()
        {
            if (_finishedGrave == null) return;

            Save();
        }

        #region PUBLICS
        public void EnableUpgradeArea()
        {
            upgradeAreaObj.SetActive(true);
            UpdateRemainingMoneyText();
        }
        public void DisableUpgradeArea() => upgradeAreaObj.SetActive(false);
        public void ConsumeMoney(int amount)
        {
            if (amount > (_requiredMoney - _consumedMoney))
            {
                if (amount > DataManager.TotalMoney)
                {
                    CollectableEvents.OnConsume?.Invoke(DataManager.TotalMoney);
                    _consumedMoney += DataManager.TotalMoney;
                }
                else
                {
                    CollectableEvents.OnConsume?.Invoke(_requiredMoney - _consumedMoney);
                    _consumedMoney = _requiredMoney;
                }
            }
            else
            {
                if (amount > DataManager.TotalMoney)
                {
                    CollectableEvents.OnConsume?.Invoke(DataManager.TotalMoney);
                    _consumedMoney += DataManager.TotalMoney;
                }
                else
                {
                    CollectableEvents.OnConsume?.Invoke(amount);
                    _consumedMoney += amount;
                }
            }

            UpdateRemainingMoneyText();

            if (_consumedMoney >= _requiredMoney)
            {
                PlayerEvents.OnStopSpendingMoney?.Invoke();
                MoneyCanvas.Instance.StopSpendingMoney();
                AudioHandler.PlayAudio(Enums.AudioType.GraveBuilt);

                DisableUpgradeArea();
                _finishedGrave.UpgradeLevel();

                _consumedMoney = 0;
            }
        }
        #endregion

        #region HELPERS
        private void UpdateRemainingMoneyText() => remainingMoneyText.text = (_requiredMoney - _consumedMoney).ToString();
        #endregion

        #region SAVE-LOAD
        private void Save()
        {
            PlayerPrefs.SetInt($"Grave-{_finishedGrave.Grave.ID}-Finished-ConsumedMoney", _consumedMoney);
            PlayerPrefs.Save();
        }
        private void Load()
        {
            _consumedMoney = PlayerPrefs.GetInt($"Grave-{_finishedGrave.Grave.ID}-Finished-ConsumedMoney", 0);
            UpdateRemainingMoneyText();
        }
        #endregion
    }
}
