using UnityEngine;
using System.Collections;

namespace GraveyardIdle
{
    public class GraveMoneyHandler : MonoBehaviour
    {
        private int _value = 20;
        private readonly WaitForSeconds _spawnMoneyDelay = new WaitForSeconds(0.05f);

        public void Init(Grave grave)
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
