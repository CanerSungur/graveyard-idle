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
        public Grave Grave => _grave;
        public bool MoneyCanBeSpent => DataManager.TotalMoney > 0 && _consumedMoney < RequiredMoney;
        public Transform MoneyImageTransform => moneyImageTransform;
        public int RequiredMoney => (int)(_coreUpgradeCost * _grave.Level * 0.5f);
        public GameObject UpgradeArea => upgradeArea;
        #endregion

        #region UPGRADE
        private readonly int _coreUpgradeCost = 50;
        #endregion

        private int _consumedMoney;

        public void Init(Grave grave)
        {
            _grave = grave;

            LoadData();

            UpdateRemainingMoneyText();
            //upgradeArea.SetActive(false);

            //_grave.InteractableGround.OnGraveUpgraded += UpdateMoneyRequirements;
        }

        private void OnDisable()
        {
            if (!_grave) return;
            //_grave.InteractableGround.OnGraveUpgraded -= UpdateMoneyRequirements;

            SaveData();
        }


        #region PUBLICS
        public void UpdateRemainingMoneyText() => remainingMoneyText.text = (RequiredMoney - _consumedMoney).ToString();
        public void ActivateUpgradeArea()
        {
            upgradeArea.SetActive(true);
            UpdateRemainingMoneyText();
        }
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
            if (amount > (RequiredMoney - _consumedMoney))
            {
                if (amount > DataManager.TotalMoney)
                {
                    _consumedMoney += DataManager.TotalMoney;
                    CollectableEvents.OnConsume?.Invoke(DataManager.TotalMoney);
                }
                else
                {
                    CollectableEvents.OnConsume?.Invoke(RequiredMoney - _consumedMoney);
                    _consumedMoney = RequiredMoney;
                }
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

            if (_consumedMoney >= RequiredMoney)
            {
                PlayerEvents.OnStopSpendingMoney?.Invoke();
                MoneyCanvas.Instance.StopSpendingMoney();
                AudioHandler.PlayAudio(Enums.AudioType.GraveBuilt);
                //Activate();
                Debug.Log("upgrade");
                _grave.InteractableGround.OnGraveUpgraded?.Invoke();
                upgradeArea.SetActive(false);
                _grave.PlayerIsInUpgradeArea = false;
                if (_grave.Level < _grave.MaxLevel)
                    Delayer.DoActionAfterDelay(this, 3f, () =>
                    {
                        upgradeArea.SetActive(true);
                        _consumedMoney = 0;
                        UpdateRemainingMoneyText();
                    });
            }
        }
        #endregion

        #region SAVE-LOAD
        private void LoadData()
        {
            _consumedMoney = PlayerPrefs.GetInt($"Grave-{_grave.InteractableGround.ID}-ConsumedMoney", 0);

            if (_grave.IsBuilt && _grave.Level < _grave.MaxLevel)
                ActivateUpgradeArea();
            else
                DeActivateUpgradeArea();
        }
        private void SaveData()
        {
            PlayerPrefs.SetInt($"Grave-{_grave.InteractableGround.ID}-ConsumedMoney", _consumedMoney);
            PlayerPrefs.Save();
        }
        private void OnApplicationQuit()
        {
            if (!_grave) return;
            SaveData();
        }
        private void OnApplicationPause(bool pause)
        {
            if (!_grave) return;
            SaveData();
        }
        #endregion
    }
}
