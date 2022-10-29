using System.Collections;
using UnityEngine;

namespace GraveyardIdle.GraveSystem
{
    public class FinishedGraveMoneyHandler : MonoBehaviour
    {
        private FinishedGrave _finishedGrave;

        private int _value = 20;
        private readonly WaitForSeconds _spawnMoneyDelay = new WaitForSeconds(0.05f);

        public void Init(FinishedGrave finishedGrave)
        {
            if (_finishedGrave == null)
            {
                _finishedGrave = finishedGrave;
            }
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
