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
        public static float MaintenanceSpeed { get; private set; }
        #endregion

        #region UPGRADE LEVELS
        public static int DigSpeedLevel { get; private set; }
        public static int MaintenanceSpeedLevel { get; private set; }
        #endregion

        #region UPGRADE COSTS
        public static int DigSpeedCost => (int)(_upgradeCost * Mathf.Pow(_upgradeCostIncreaseRate, DigSpeedLevel));
        public static int MaintenanceSpeedCost => (int)(_upgradeCost * Mathf.Pow(_upgradeCostIncreaseRate, MaintenanceSpeedLevel));
        #endregion

        #region UPGRADE COST DATA
        private static readonly int _upgradeCost = 30;
        private static readonly float _upgradeCostIncreaseRate = 1.2f;
        #endregion

        #region CORE DATA
        private readonly float _coreDigSpeed = 1.5f;
        private readonly float _coreMaintenanceSpeed = 0.5f;
        #endregion

        #region INCREMENT DATA
        private readonly float _digSpeedIncrement = 0.1f;
        private readonly float _maintenanceSpeedIncrement = 0.05f;
        #endregion

        public void Init(GameManager gameManager)
        {
            MoneyValue = 1;

            LoadData();

            UpdateDigSpeed();
            UpdateMaintenanceSpeed();

            PlayerUpgradeEvents.OnUpgradeDigSpeed += DigSpeedUpgrade;
            PlayerUpgradeEvents.OnUpgradeMaintenanceSpeed += MaintenanceSpeedUpgrade;

            CollectableEvents.OnCollect += IncreaseTotalMoney;
            CollectableEvents.OnConsume += DecreaseTotalMoney;
        }

        private void OnDisable()
        {
            PlayerUpgradeEvents.OnUpgradeDigSpeed -= DigSpeedUpgrade;
            PlayerUpgradeEvents.OnUpgradeMaintenanceSpeed -= MaintenanceSpeedUpgrade;

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
        private void MaintenanceSpeedUpgrade()
        {
            IncreaseMaintenanceSpeedLevel();
            UpdateMaintenanceSpeed();
        }
        private void UpdateDigSpeed()
        {
            DigSpeed = _coreDigSpeed + _digSpeedIncrement * (DigSpeedLevel - 1);
            PlayerEvents.OnSetCurrentDigSpeed?.Invoke();
        }
        private void UpdateMaintenanceSpeed()
        {
            MaintenanceSpeed = _coreMaintenanceSpeed + _maintenanceSpeedIncrement * (MaintenanceSpeedLevel - 1);
            PlayerEvents.OnSetCurrentMaintenanceSpeed?.Invoke();
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
        private void IncreaseMaintenanceSpeedLevel()
        {
            if (TotalMoney >= MaintenanceSpeedCost)
            {
                DecreaseTotalMoney(MaintenanceSpeedCost);
                MaintenanceSpeedLevel++;
                PlayerUpgradeEvents.OnUpdateUpgradeTexts?.Invoke();
                UiEvents.OnUpdateCollectableText?.Invoke(0);
            }
        }
        #endregion

        #region SAVE-LOAD
        private void LoadData()
        {
            TotalMoney = PlayerPrefs.GetInt("TotalMoney", 100);
            DigSpeedLevel = PlayerPrefs.GetInt("DigSpeedLevel", 1);
            MaintenanceSpeedLevel = PlayerPrefs.GetInt("MaintenanceSpeedLevel", 1);
        }
        private void SaveData()
        {
            PlayerPrefs.SetInt("TotalMoney", TotalMoney);
            PlayerPrefs.SetInt("DigSpeedLevel", DigSpeedLevel);
            PlayerPrefs.SetInt("MaintenanceSpeedLevel", MaintenanceSpeedLevel);
            PlayerPrefs.Save();
        }
        private void OnApplicationPause(bool pause) => SaveData();
        private void OnApplicationQuit() => SaveData();
        #endregion
    }
}
