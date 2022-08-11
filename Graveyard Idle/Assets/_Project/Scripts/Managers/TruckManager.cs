using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class TruckManager : MonoBehaviour
    {
        [Header("-- TRUCK SETUP --")]
        [SerializeField] private Truck truck;
        [SerializeField] private SellTruck sellTruck;

        public static bool ThereIsAvailableTruck { get; private set; }
        public static bool ThereIsAvailableSellTruck { get; private set; }

        private void Start()
        {
            ThereIsAvailableTruck = ThereIsAvailableSellTruck = true;

            TruckEvents.OnEnableTruck += EnableTruck;
            TruckEvents.OnDisableTruck += DisableTruck;
            TruckEvents.OnEnableSellTruck += EnableSellTruck;
            //TruckEvents.OnDisableSellTruck += DisableSellTruck;
        }

        private void OnDisable()
        {
            TruckEvents.OnEnableTruck -= EnableTruck;
            TruckEvents.OnDisableTruck -= DisableTruck;
            TruckEvents.OnEnableSellTruck -= EnableSellTruck;
            //TruckEvents.OnDisableSellTruck -= DisableSellTruck;
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
        private void EnableSellTruck(CoffinMachine coffinMachine)
        {
            ThereIsAvailableSellTruck = false;
            sellTruck.gameObject.SetActive(true);
            CoffinMachineEvents.OnAssignSellTruck?.Invoke(sellTruck);
            sellTruck.Init(coffinMachine);
        }
        private void DisableSellTruck()
        {
            sellTruck.gameObject.SetActive(false);
            ThereIsAvailableSellTruck = true;
        }
    }
}
