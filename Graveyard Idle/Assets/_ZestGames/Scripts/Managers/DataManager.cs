using UnityEngine;

namespace ZestGames
{
    public class DataManager : MonoBehaviour
    {
        public bool DeleteAllData = false;

        public static int TotalMoney { get; private set; }
        public static int MoneyValue { get; private set; }

        #region UPGRADEABLE VALUES
        public static float DigSpeed { get; private set; }
        #endregion

        #region UPGRADE LEVELS
        public static int DigSpeedLevel { get; private set; }
        #endregion

        #region UPGRADE COSTS
        public static int DigSpeedCost => (int)(_upgradeCost * Mathf.Pow(_upgradeCostIncreaseRate, DigSpeedLevel));
        #endregion

        #region UPGRADE COST DATA
        private static readonly int _upgradeCost = 30;
        private static readonly float _upgradeCostIncreaseRate = 1.2f;
        #endregion

        #region CORE DATA
        private readonly float _coreDigSpeed = 1.5f;
        #endregion

        #region INCREMENT DATA
        private readonly float _digSpeedIncrement = 0.1f;
        #endregion

        public void Init(GameManager gameManager)
        {
            MoneyValue = 1;

            LoadData();

            UpdateDigSpeed();

            PlayerUpgradeEvents.OnUpgradeDigSpeed += DigSpeedUpgrade;

            CollectableEvents.OnCollect += IncreaseTotalMoney;
            CollectableEvents.OnConsume += DecreaseTotalMoney;
        }

        private void OnDisable()
        {
            PlayerUpgradeEvents.OnUpgradeDigSpeed -= DigSpeedUpgrade;

            CollectableEvents.OnCollect -= IncreaseTotalMoney;
            CollectableEvents.OnConsume -= DecreaseTotalMoney;

            SaveData();
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.M))
                CollectableEvents.OnCollect?.Invoke(1000);
#endif
        }

        #region MONEY FUNCTIONS
        private void IncreaseTotalMoney(int amount)
        {
            TotalMoney += amount;
            UiEvents.OnUpdateCollectableText?.Invoke(TotalMoney);
        }
        private void DecreaseTotalMoney(int amount)
        {
            TotalMoney -= amount;
            UiEvents.OnUpdateCollectableText?.Invoke(TotalMoney);
        }
        #endregion

        #region UPGRADE FUNCTIONS
        private void DigSpeedUpgrade()
        {
            IncreaseDigSpeedLevel();
            UpdateDigSpeed();
        }
        private void UpdateDigSpeed()
        {
            DigSpeed = _coreDigSpeed + _digSpeedIncrement * (DigSpeedLevel - 1);
            PlayerEvents.OnSetCurrentDigSpeed?.Invoke();
        }
        private void IncreaseDigSpeedLevel()
        {
            if (TotalMoney >= DigSpeedCost)
            {
                DecreaseTotalMoney(DigSpeedCost);
                DigSpeedLevel++;
                PlayerUpgradeEvents.OnUpdateUpgradeTexts?.Invoke();
                UiEvents.OnUpdateCollectableText?.Invoke(0);
            }
        }
        #endregion

        #region SAVE-LOAD
        private void LoadData()
        {
            TotalMoney = PlayerPrefs.GetInt("TotalMoney", 0);
            DigSpeedLevel = PlayerPrefs.GetInt("DigSpeedLevel", 1);
        }
        private void SaveData()
        {
            PlayerPrefs.SetInt("TotalMoney", TotalMoney);
            PlayerPrefs.SetInt("DigSpeedLevel", DigSpeedLevel);
            PlayerPrefs.Save();
        }
        private void OnApplicationPause(bool pause) => SaveData();
        private void OnApplicationQuit() => SaveData();
        #endregion
    }
}
