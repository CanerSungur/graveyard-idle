using UnityEngine;
using System.Collections;
using ZestGames;
using GraveyardIdle.GraveSystem;

namespace GraveyardIdle
{
    public class PlayerMoneyHandler : MonoBehaviour
    {
        private Player _player;

        #region SPEND MONEY SECTION
        private float _currentSpendMoneyDelay;
        private readonly float _startingSpendMoneyDelay = 0.25f;
        private IEnumerator _spendMoneyEnum;
        // Spend value will increase by 10 in every 5 spend counts to shorten spending time immensely.
        private int _currentMoneySpendValue, _moneySpendingCount;
        private readonly int _moneyValueMultiplier = 5;
        #endregion

        public bool CanSpendMoney => !_player.IsCarryingCoffin && !_player.IsInDigZone && !_player.IsInFillZone && !Player.IsMaintenancing && DataManager.TotalMoney > 0;

        public void Init(Player player)
        {
            _player = player;

            _currentSpendMoneyDelay = _startingSpendMoneyDelay;
            _currentMoneySpendValue = DataManager.MoneyValue;
            _moneySpendingCount = 0;

            PlayerEvents.OnStopSpendingMoney += StopSpending;
        }

        private void OnDisable()
        {
            if (!_player) return;
            PlayerEvents.OnStopSpendingMoney -= StopSpending;
        }

        private IEnumerator SpendMoney(GraveGround graveGround)
        {
            while (graveGround.MoneyCanBeSpent)
            {
                graveGround.ConsumeMoney(_currentMoneySpendValue);
                yield return new WaitForSeconds(_currentSpendMoneyDelay);
                //DecreaseMoneyDelay();
                UpdateMoneyValue();
            }
        }
        private IEnumerator SpendMoney(FinishedGrave finishedGrave)
        {
            while (finishedGrave.UpgradeHandler.MoneyCanBeSpent)
            {
                finishedGrave.UpgradeHandler.ConsumeMoney(_currentMoneySpendValue);
                yield return new WaitForSeconds(_currentSpendMoneyDelay);
                UpdateMoneyValue();
            }
        }

        //private IEnumerator SpendMoney(GraveUpgradeHandler graveUpgradeHandler)
        //{
        //    while (graveUpgradeHandler.MoneyCanBeSpent)
        //    {
        //        graveUpgradeHandler.ConsumeMoney(_currentMoneySpendValue);
        //        yield return new WaitForSeconds(_currentSpendMoneyDelay);
        //        //DecreaseMoneyDelay();
        //        UpdateMoneyValue();
        //    }
        //}
        //private IEnumerator SpendMoney(InteractableGroundCanvas interactableGroundCanvas)
        //{
        //    while (interactableGroundCanvas.MoneyCanBeSpent)
        //    {
        //        interactableGroundCanvas.ConsumeMoney(_currentMoneySpendValue);
        //        yield return new WaitForSeconds(_currentSpendMoneyDelay);
        //        //DecreaseMoneyDelay();
        //        UpdateMoneyValue();
        //    }
        //}
        private IEnumerator SpendMoney(BuyCoffinArea buyCoffinArea)
        {
            while (buyCoffinArea.MoneyCanBeSpent)
            {
                buyCoffinArea.ConsumeMoney(_currentMoneySpendValue);
                yield return new WaitForSeconds(_currentSpendMoneyDelay);
                //DecreaseMoneyDelay();
                UpdateMoneyValue();
            }
        }
        private IEnumerator SpendMoney(CoffinCarriersUnlocker coffinCarriersUnlocker)
        {
            while (coffinCarriersUnlocker.MoneyCanBeSpent)
            {
                coffinCarriersUnlocker.ConsumeMoney(_currentMoneySpendValue);
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
        public void StartSpending(GraveGround graveGround)
        {
            _spendMoneyEnum = SpendMoney(graveGround);
            _currentSpendMoneyDelay = _startingSpendMoneyDelay;
            _currentMoneySpendValue = DataManager.MoneyValue;
            _moneySpendingCount = 0;
            StartCoroutine(_spendMoneyEnum);

            // Start throwing money
            if (graveGround.MoneyCanBeSpent)
                MoneyCanvas.Instance.StartSpendingMoney(graveGround);
        }
        public void StartSpending(FinishedGrave finishedGrave)
        {
            _spendMoneyEnum = SpendMoney(finishedGrave);
            _currentSpendMoneyDelay = _startingSpendMoneyDelay;
            _currentMoneySpendValue = DataManager.MoneyValue;
            _moneySpendingCount = 0;
            StartCoroutine(_spendMoneyEnum);

            if (finishedGrave.UpgradeHandler.MoneyCanBeSpent)
                MoneyCanvas.Instance.StartSpendingMoney(finishedGrave);
        }

        //public void StartSpending(GraveUpgradeHandler graveUpgradeHandler)
        //{
        //    _spendMoneyEnum = SpendMoney(graveUpgradeHandler);
        //    _currentSpendMoneyDelay = _startingSpendMoneyDelay;
        //    _currentMoneySpendValue = DataManager.MoneyValue;
        //    _moneySpendingCount = 0;
        //    StartCoroutine(_spendMoneyEnum);

        //    // Start throwing money
        //    if (graveUpgradeHandler.MoneyCanBeSpent)
        //        MoneyCanvas.Instance.StartSpendingMoney(graveUpgradeHandler);
        //}
        public void StopSpending()
        {
            StopCoroutine(_spendMoneyEnum);

            // Stop throwing money
            MoneyCanvas.Instance.StopSpendingMoney();
        }
        //public void StartSpending(InteractableGroundCanvas interactableGroundCanvas)
        //{
        //    _spendMoneyEnum = SpendMoney(interactableGroundCanvas);
        //    _currentSpendMoneyDelay = _startingSpendMoneyDelay;
        //    _currentMoneySpendValue = DataManager.MoneyValue;
        //    _moneySpendingCount = 0;
        //    StartCoroutine(_spendMoneyEnum);

        //    // Start throwing money
        //    if (interactableGroundCanvas.MoneyCanBeSpent)
        //        MoneyCanvas.Instance.StartSpendingMoney(interactableGroundCanvas);
        //}
        //public void StopSpending()
        //{
        //    StopCoroutine(_spendMoneyEnum);

        //    // Stop throwing money
        //    MoneyCanvas.Instance.StopSpendingMoney();
        //}
        public void StartSpending(BuyCoffinArea buyCoffinArea)
        {
            _spendMoneyEnum = SpendMoney(buyCoffinArea);
            _currentSpendMoneyDelay = _startingSpendMoneyDelay;
            _currentMoneySpendValue = DataManager.MoneyValue;
            _moneySpendingCount = 0;
            StartCoroutine(_spendMoneyEnum);

            // Start throwing money
            if (buyCoffinArea.MoneyCanBeSpent)
                MoneyCanvas.Instance.StartSpendingMoney(buyCoffinArea);
        }
        //public void StopSpending()
        //{
        //    StopCoroutine(_spendMoneyEnum);

        //    // Stop throwing money
        //    MoneyCanvas.Instance.StopSpendingMoney();
        //}
        public void StartSpending(CoffinCarriersUnlocker coffinCarriersUnlocker)
        {
            _spendMoneyEnum = SpendMoney(coffinCarriersUnlocker);
            _currentSpendMoneyDelay = _startingSpendMoneyDelay;
            _currentMoneySpendValue = DataManager.MoneyValue;
            _moneySpendingCount = 0;
            StartCoroutine(_spendMoneyEnum);

            // Start throwing money
            if (coffinCarriersUnlocker.MoneyCanBeSpent)
                MoneyCanvas.Instance.StartSpendingMoney(coffinCarriersUnlocker);
        }
        #endregion
    }
}
