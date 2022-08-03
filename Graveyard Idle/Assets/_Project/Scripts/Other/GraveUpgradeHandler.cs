using UnityEngine;
using ZestGames;
using TMPro;
using ZestCore.Utility;

namespace GraveyardIdle
{
    public class GraveUpgradeHandler : MonoBehaviour
    {
        private Grave _grave;

        [Header("-- SETUP --")]
        [SerializeField] private GameObject upgradeArea;

        [Header("-- MONEY CONSUME SETUP --")]
        [SerializeField] private TextMeshProUGUI remainingMoneyText;
        [SerializeField] private Transform moneyImageTransform;

        #region PROPERTIES
        public bool IsActivated { get; private set; }
        public Grave Grave => _grave;
        public bool MoneyCanBeSpent => DataManager.TotalMoney > 0 && _consumedMoney < _requiredMoney;
        public Transform MoneyImageTransform => moneyImageTransform;
        #endregion

        #region FIRST ACTIVATION
        private readonly int _coreRequiredMoney = 100;
        #endregion

        #region UPGRADE
        private readonly int _coreUpgradeCost = 200;
        #endregion

        private int _consumedMoney;
        private int _requiredMoney;

        public void Init(Grave grave)
        {
            _grave = grave;
            IsActivated = false;
            _requiredMoney = _coreUpgradeCost;
            _consumedMoney = 0;
            UpdateRemainingMoneyText();
            upgradeArea.SetActive(false);

            //_grave.InteractableGround.OnGraveUpgraded += UpdateMoneyRequirements;
        }

        private void OnDisable()
        {
            if (!_grave) return;
            //_grave.InteractableGround.OnGraveUpgraded -= UpdateMoneyRequirements;
        }

        private void UpdateRemainingMoneyText() => remainingMoneyText.text = (_requiredMoney - _consumedMoney).ToString();
        private void UpdateMoneyRequirements()
        {
            _requiredMoney = (int)(_coreRequiredMoney *_grave.Level * 0.5f);
            _consumedMoney = 0;
            UpdateRemainingMoneyText();
        }

        #region PUBLICS
        public void ActivateUpgradeArea() => upgradeArea.SetActive(true);
        public void DeActivateUpgradeArea() => upgradeArea.SetActive(false);
        public void PlayerStartedUpgrading()
        {
            _grave.PlayerIsInUpgradeArea = true;
        }
        public void PlayerStoppedUpgrading()
        {
            _grave.PlayerIsInUpgradeArea = false;
        }
        public void ConsumeMoney(int amount)
        {
            if (amount > (_requiredMoney - _consumedMoney))
            {
                CollectableEvents.OnConsume?.Invoke(_requiredMoney - _consumedMoney);
                _consumedMoney = _requiredMoney;
            }
            else
            {
                if (amount > DataManager.TotalMoney)
                {
                    _consumedMoney += DataManager.TotalMoney;
                    CollectableEvents.OnConsume?.Invoke(DataManager.TotalMoney);
                }
                else
                {
                    CollectableEvents.OnConsume?.Invoke(amount);
                    _consumedMoney += amount;
                }
            }

            UpdateRemainingMoneyText();

            if (_consumedMoney == _requiredMoney)
            {
                if (!upgradeArea.activeSelf) return;
                MoneyCanvas.Instance.StopSpendingMoney();
                //AudioHandler.PlayAudio(Enums.AudioType.PhoneBoothUnlock);
                //Activate();
                Debug.Log("upgrade");
                _grave.InteractableGround.OnGraveUpgraded?.Invoke();
                upgradeArea.SetActive(false);
                _grave.PlayerIsInUpgradeArea = false;
                if (_grave.Level < _grave.MaxLevel)
                    Delayer.DoActionAfterDelay(this, 3f, () =>
                    {
                        upgradeArea.SetActive(true);
                        UpdateMoneyRequirements();
                    });
            }
        }
        #endregion
    }
}
