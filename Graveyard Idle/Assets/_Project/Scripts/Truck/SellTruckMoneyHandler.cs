using System.Collections;
using UnityEngine;

namespace GraveyardIdle
{
    public class SellTruckMoneyHandler : MonoBehaviour
    {
        private int _value = 15;
        private readonly WaitForSeconds _spawnMoneyDelay = new WaitForSeconds(0.05f);

        public void Init(SellTruck sellTruck)
        {

        }

        private IEnumerator SpawnMoney()
        {
            int currentCount = 0;
            while (currentCount < _value)
            {
                MoneyCanvas.Instance.SpawnCollectMoney(transform);
                currentCount++;

                yield return _spawnMoneyDelay;
            }
        }

        #region PUBLICS
        public void StartSpawningMoney() => StartCoroutine(SpawnMoney());
        #endregion   
    }
}
