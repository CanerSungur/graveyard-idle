using System.Collections;
using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class CoffinAreaSpawnHandler : MonoBehaviour
    {
        private CoffinArea _coffinArea;

        [Header("-- SETUP --")]
        [SerializeField] private Transform spawnTransform;

        private float _spawnDelay = 1f;
        private WaitForSeconds _waitForSpawnDelay;

        public void Init(CoffinArea coffinArea)
        {
            _coffinArea = coffinArea;
            _waitForSpawnDelay = new WaitForSeconds(_spawnDelay);

            StartCoroutine(SpawnCoffin());
        }

        private IEnumerator SpawnCoffin()
        {
            while (true)
            {
                if (_coffinArea.CanSpawn)
                {
                    Coffin coffin = ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.Coffin, spawnTransform.position, Quaternion.Euler(0f, 90f, 0f)).GetComponent<Coffin>();
                    coffin.GetStacked(_coffinArea.StackHandler.TargetStackPosition, _coffinArea.StackHandler.StackContainer);
                    CoffinAreaStackHandler.AddCoffin(coffin);
                }

                yield return _waitForSpawnDelay;
            }
        }
    }
}
