using UnityEngine;
using System;
using TMPro;
using ZestGames;

namespace GraveyardIdle.GraveSystem
{
    public class GraveGround : MonoBehaviour
    {
        // saves if grave is activated or not
        // saves spent money into this grave space
        // loads and sets spent money ui

        private Grave _grave;

        #region MONEY DATA
        private const int CORE_ACTIVATION_MONEY = 20;
        private int _consumedMoney;
        private int _requiredMoney;

        private TextMeshProUGUI _requiredMoneyText;
        #endregion

        #region MONEY SPEND SECTION
        public bool PlayerIsInArea { get; private set; }
        public bool MoneyCanBeSpent => DataManager.TotalMoney > 0 && _consumedMoney < _requiredMoney;
        #endregion

        public void Init(Grave grave)
        {
            if (_grave == null)
            {
                // First initialization
                _grave = grave;

                _requiredMoneyText = GetComponentInChildren<TextMeshProUGUI>();
                _requiredMoney = CORE_ACTIVATION_MONEY;
                _consumedMoney = 0;

                PlayerIsInArea = false;
            }

            Load();

            UpdateRemainingMoneyText();
        }

        private void OnDisable()
        {
            if (_grave == null) return;
            Save();
        }

        private void UpdateRemainingMoneyText() => _requiredMoneyText.text = (_requiredMoney - _consumedMoney).ToString();

        #region PUBLICS
        public void PlayerGetsIn()
        {
            PlayerIsInArea = true;
        }
        public void PlayerGetsOut()
        {
            PlayerIsInArea = false;
        }
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

                _grave.ActivateGraveGround(this);
            }
        }
        #endregion

        #region SAVE-LOAD FUNCTIONS
        private void Save()
        {
            PlayerPrefs.SetInt($"graveGround-{_grave.ID}-ConsumedMoney", _consumedMoney);
            PlayerPrefs.Save();
        }
        private void Load()
        {
            _consumedMoney = PlayerPrefs.GetInt($"graveGround-{_grave.ID}-ConsumedMoney", 0);
        }
        #endregion
    }
}
