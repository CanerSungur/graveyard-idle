using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class TruckManager : MonoBehaviour
    {
        [Header("-- TRUCK SETUP --")]
        [SerializeField] private Truck truck;

        public static bool ThereIsAvailableTruck;

        private void Start()
        {
            ThereIsAvailableTruck = true;

            TruckEvents.OnEnableTruck += EnableTruck;
            TruckEvents.OnDisableTruck += DisableTruck;
        }

        private void OnDisable()
        {
            TruckEvents.OnEnableTruck -= EnableTruck;
            TruckEvents.OnDisableTruck -= DisableTruck;
        }

        private void EnableTruck(CoffinArea coffinArea)
        {
            ThereIsAvailableTruck = false;
            truck.gameObject.SetActive(true);
            CoffinAreaEvents.OnAssignTruck?.Invoke(truck);
            truck.Init(coffinArea);
        }
        private void DisableTruck()
        {
            truck.gameObject.SetActive(false);
            ThereIsAvailableTruck = true;
        }
    }
}
