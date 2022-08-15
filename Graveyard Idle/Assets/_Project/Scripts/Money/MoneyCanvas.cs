using System.Collections;
using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class MoneyCanvas : MonoBehaviour
    {
        public RectTransform MiddlePointRectTransform { get; private set; }
        private WaitForSeconds _waitforSpendMoneyDelay = new WaitForSeconds(0.1f);
        private IEnumerator _spendMoneyEnum;
        public bool SpendMoneyEnumIsPlaying { get; private set; }

        #region SINGLETON
        public static MoneyCanvas Instance;
        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            MiddlePointRectTransform = transform.GetChild(0).GetComponent<RectTransform>();
            SpendMoneyEnumIsPlaying = false;
        }
        #endregion

        public void SpawnCollectMoney(Transform spawnTransform)
        {
            CollectMoney money = ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.CollectMoney, Vector3.zero, Quaternion.identity, transform).GetComponent<CollectMoney>();
            money.Init(this, spawnTransform);
        }
        public void SpawnSpendMoney(GraveUpgradeHandler graveUpgradeHandler)
        {
            SpendMoney money = ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.SpendMoney, Vector3.zero, Quaternion.identity, transform).GetComponent<SpendMoney>();
            money.Init(this, graveUpgradeHandler);
        }
        public void SpawnSpendMoney(InteractableGroundCanvas interactableGroundCanvas)
        {
            SpendMoney money = ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.SpendMoney, Vector3.zero, Quaternion.identity, transform).GetComponent<SpendMoney>();
            money.Init(this, interactableGroundCanvas);
        }
        public void SpawnSpendMoney(BuyCoffinArea buyCoffinArea)
        {
            SpendMoney money = ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.SpendMoney, Vector3.zero, Quaternion.identity, transform).GetComponent<SpendMoney>();
            money.Init(this, buyCoffinArea);
        }
        public void SpawnSpendMoney(CoffinCarriersUnlocker coffinCarriersUnlocker)
        {
            SpendMoney money = ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.SpendMoney, Vector3.zero, Quaternion.identity, transform).GetComponent<SpendMoney>();
            money.Init(this, coffinCarriersUnlocker);
        }
        //public void SpawnSpendMoney(PhoneBooth phoneBooth)
        //{
        //    SpendMoney money = ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.SpendMoney, Vector3.zero, Quaternion.identity, transform).GetComponent<SpendMoney>();
        //    money.Init(this, phoneBooth);
        //}
        //public void StartSpendingMoney(PhaseUnlocker phaseUnlocker)
        //{
        //    SpendMoneyEnumIsPlaying = true;
        //    _spendMoneyEnum = SpendMoney(phaseUnlocker);
        //    StartCoroutine(_spendMoneyEnum);
        //}
        public void StartSpendingMoney(GraveUpgradeHandler graveUpgradeHandler)
        {
            SpendMoneyEnumIsPlaying = true;
            _spendMoneyEnum = SpendMoney(graveUpgradeHandler);
            StartCoroutine(_spendMoneyEnum);
        }
        public void StartSpendingMoney(InteractableGroundCanvas interactableGroundCanvas)
        {
            SpendMoneyEnumIsPlaying = true;
            _spendMoneyEnum = SpendMoney(interactableGroundCanvas);
            StartCoroutine(_spendMoneyEnum);
        }
        public void StartSpendingMoney(BuyCoffinArea buyCoffinArea)
        {
            SpendMoneyEnumIsPlaying = true;
            _spendMoneyEnum = SpendMoney(buyCoffinArea);
            StartCoroutine(_spendMoneyEnum);
        }
        public void StartSpendingMoney(CoffinCarriersUnlocker coffinCarriersUnlocker)
        {
            SpendMoneyEnumIsPlaying = true;
            _spendMoneyEnum = SpendMoney(coffinCarriersUnlocker);
            StartCoroutine(_spendMoneyEnum);
        }
        public void StopSpendingMoney()
        {
            if (SpendMoneyEnumIsPlaying)
            {
                StopCoroutine(_spendMoneyEnum);
                SpendMoneyEnumIsPlaying = false;
            }
        }

        private IEnumerator SpendMoney(GraveUpgradeHandler graveUpgradeHandler)
        {
            while (DataManager.TotalMoney > 0)
            {
                SpawnSpendMoney(graveUpgradeHandler);
                AudioEvents.OnPlaySpendMoney?.Invoke();
                yield return _waitforSpendMoneyDelay;
            }
        }
        private IEnumerator SpendMoney(InteractableGroundCanvas interactableGroundCanvas)
        {
            while (DataManager.TotalMoney > 0)
            {
                SpawnSpendMoney(interactableGroundCanvas);
                AudioEvents.OnPlaySpendMoney?.Invoke();
                yield return _waitforSpendMoneyDelay;
            }
        }
        private IEnumerator SpendMoney(BuyCoffinArea buyCoffinArea)
        {
            while (DataManager.TotalMoney > 0)
            {
                SpawnSpendMoney(buyCoffinArea);
                AudioEvents.OnPlaySpendMoney?.Invoke();
                yield return _waitforSpendMoneyDelay;
            }
        }
        private IEnumerator SpendMoney(CoffinCarriersUnlocker coffinCarriersUnlocker)
        {
            while (DataManager.TotalMoney > 0)
            {
                SpawnSpendMoney(coffinCarriersUnlocker);
                AudioEvents.OnPlaySpendMoney?.Invoke();
                yield return _waitforSpendMoneyDelay;
            }
        }
    }
}
