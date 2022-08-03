using UnityEngine;
using System.Collections;
using ZestGames;

namespace GraveyardIdle
{
    public class PlayerMoneyHandler : MonoBehaviour
    {
        private Player _player;

        #region SPEND MONEY SECTION
        private float _currentSpendMoneyDelay;
        private readonly float _startingSpendMoneyDelay = 0.25f;
        private readonly float _delayDecreaseRate = 0.05f;
        private readonly float _minDelay = 0.001f;
        private IEnumerator _spendMoneyEnum;
        // Spend value will increase by 10 in every 5 spend counts to shorten spending time immensely.
        private int _currentMoneySpendValue, _moneySpendingCount;
        private readonly int _moneyValueMultiplier = 5;
        #endregion

        public void Init(Player player)
        {
            _player = player;

            _currentSpendMoneyDelay = _startingSpendMoneyDelay;
            _currentMoneySpendValue = DataManager.MoneyValue;
            _moneySpendingCount = 0;
        }

        private IEnumerator SpendMoney(GraveUpgradeHandler graveUpgradeHandler)
        {
            while (graveUpgradeHandler.MoneyCanBeSpent)
            {
                graveUpgradeHandler.ConsumeMoney(_currentMoneySpendValue);
                yield return new WaitForSeconds(_currentSpendMoneyDelay);
                //DecreaseMoneyDelay();
                UpdateMoneyValue();
            }
        }
        private void UpdateMoneyValue()
        {
            _moneySpendingCount++;
            if (_moneySpendingCount != 0 && _moneySpendingCount % 5 == 0)
            {
                _currentMoneySpendValue *= _moneyValueMultiplier;
                //Debug.Log(_currentMoneySpendValue);
            }
        }

        #region PUBLICS
        public void StartSpending(GraveUpgradeHandler graveUpgradeHandler)
        {
            _spendMoneyEnum = SpendMoney(graveUpgradeHandler);
            _currentSpendMoneyDelay = _startingSpendMoneyDelay;
            _currentMoneySpendValue = DataManager.MoneyValue;
            _moneySpendingCount = 0;
            StartCoroutine(_spendMoneyEnum);

            // Start throwing money
            if (graveUpgradeHandler.MoneyCanBeSpent)
                MoneyCanvas.Instance.StartSpendingMoney(graveUpgradeHandler);
        }
        public void StopSpending(GraveUpgradeHandler graveUpgradeHandler)
        {
            StopCoroutine(_spendMoneyEnum);

            // Stop throwing money
            MoneyCanvas.Instance.StopSpendingMoney();
        }
        #endregion
    }
}
