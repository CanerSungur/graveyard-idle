using UnityEngine;
using TMPro;
using ZestGames;

namespace GraveyardIdle
{
    public class CoffinCarriersUnlocker : MonoBehaviour
    {
        private Collider _collider;

        [Header("-- MONEY CONSUME SETUP --")]
        [SerializeField] private TextMeshProUGUI remainingMoneyText;
        [SerializeField] private Transform moneyImageTransform;
        private GameObject _canvas;

        private int _consumedMoney;
        private readonly int _requiredMoney = 1000;

        public bool MoneyCanBeSpent => DataManager.TotalMoney > 0 && _consumedMoney < _requiredMoney;
        public bool PlayerIsInArea { get; set; }

        private void Start()
        {
            _collider = GetComponent<Collider>();
            _canvas = transform.GetChild(0).gameObject;
            PlayerIsInArea = false;

            LoadData();
            UpdateRemainingMoneyText();
        }

        private void OnDisable()
        {
            SaveData();
        }

        private void EnableArea()
        {
            _collider.enabled = true;
            _canvas.SetActive(true);
        }
        private void DisableArea()
        {
            _collider.enabled = false;
            _canvas.SetActive(false);
        }
        private void UpdateRemainingMoneyText() => remainingMoneyText.text = (_requiredMoney - _consumedMoney).ToString();
        public void ConsumeMoney(int amount)
        {
            if (amount > (_requiredMoney - _consumedMoney))
            {
                if (amount > DataManager.TotalMoney)
                {
                    _consumedMoney += DataManager.TotalMoney;
                    CollectableEvents.OnConsume?.Invoke(DataManager.TotalMoney);
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

            if (_consumedMoney >= _requiredMoney)
            {
                MoneyCanvas.Instance.StopSpendingMoney();
                AudioHandler.PlayAudio(Enums.AudioType.GraveBuilt);
                DisableArea();
                PlayerIsInArea = false;
                CoffinCarrierEvents.OnActivatedCarriers?.Invoke();
            }
        }

        #region SAVE-LOAD
        private void LoadData()
        {
            _consumedMoney = PlayerPrefs.GetInt("CoffinCarriersConsumedMoney", 0);
            if (_requiredMoney - _consumedMoney <= 0)
                DisableArea();
            else
                EnableArea();
        }
        private void SaveData()
        {
            PlayerPrefs.SetInt("CoffinCarriersConsumedMoney", _consumedMoney);
            PlayerPrefs.Save();
        }
        //private void OnApplicationQuit()
        //{
        //    SaveData();
        //}
        //private void OnApplicationPause(bool pause)
        //{
        //    SaveData();
        //}
        #endregion
    }
}
