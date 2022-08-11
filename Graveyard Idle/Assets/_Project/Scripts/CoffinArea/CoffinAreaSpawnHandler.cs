using System.Collections;
using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class CoffinAreaSpawnHandler : MonoBehaviour
    {
        private CoffinArea _coffinArea;
        private Truck _activatedTruck = null;
        public Truck ActivatedTruck => _activatedTruck;

        [Header("-- SETUP --")]
        [SerializeField] private Transform spawnTransform;

        private float _spawnDelay = 1f;
        private WaitForSeconds _waitForSpawnDelay;

        public void Init(CoffinArea coffinArea)
        {
            _coffinArea = coffinArea;
            _activatedTruck = null;
            _waitForSpawnDelay = new WaitForSeconds(_spawnDelay);

            StartCoroutine(SpawnCoffin());

            CoffinAreaEvents.OnAssignTruck += AssignTruck;
        }

        private void OnDisable()
        {
            CoffinAreaEvents.OnAssignTruck -= AssignTruck;
        }

        private void AssignTruck(Truck truck)
        {
            _activatedTruck = truck;
        }
        private IEnumerator SpawnCoffin()
        {
            while (true)
            {
                if (_coffinArea.CanSpawn)
                {
                    TruckEvents.OnEnableTruck?.Invoke(_coffinArea);
                    GraveManagerEvents.OnCoffinSpawned?.Invoke();
                    //Coffin coffin = ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.Coffin, spawnTransform.position, Quaternion.Euler(0f, 90f, 0f)).GetComponent<Coffin>();
                    //coffin.Init(this);
                    //coffin.GetStacked(_coffinArea.StackHandler.TargetStackPosition, _coffinArea.StackHandler.StackContainer);
                    //CoffinAreaStackHandler.AddCoffin(coffin);
                }

                yield return _waitForSpawnDelay;
            }
        }
    }
}
