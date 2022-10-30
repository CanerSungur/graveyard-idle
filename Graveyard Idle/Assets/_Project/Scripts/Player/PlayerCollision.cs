using GraveyardIdle.GraveSystem;
using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class PlayerCollision : MonoBehaviour
    {
        private Player _player;

        public void Init(Player player)
        {
            _player = player;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out TakeCoffinArea takeCoffinArea) && !takeCoffinArea.PlayerIsInArea && !_player.IsCarryingCoffin)
            {
                takeCoffinArea.PlayerIsInArea = true;
                if (GraveSystem.GraveManager.Instance.CanCoffinBeCarried && CoffinAreaStackHandler.CoffinsInArea.Count > 0)
                    _player.TimerHandler.StartFilling(() => takeCoffinArea.TakeCoffin(_player));
                else if (CoffinAreaStackHandler.CoffinsInArea.Count <= 0)
                {
                    UiEvents.OnPopupText?.Invoke(_player.transform.position + (Vector3.up * 3f), "No Coffin");
                    return;
                }
                else if (!GraveSystem.GraveManager.Instance.CanCoffinBeCarried)
                {
                    UiEvents.OnPopupText?.Invoke(_player.transform.position + (Vector3.up * 3f), "No Grave");
                    return;
                }
            }

            if (other.TryGetComponent(out DigArea digArea) && !_player.IsCarryingCoffin)
                digArea.StartFilling();

            if (other.TryGetComponent(out CoffinCarriersUnlocker coffinCarrierUnlocker) && !coffinCarrierUnlocker.PlayerIsInArea && _player.MoneyHandler.CanSpendMoney)
            {
                coffinCarrierUnlocker.PlayerIsInArea = true;
                _player.MoneyHandler.StartSpending(coffinCarrierUnlocker);
            }

            if (other.TryGetComponent(out BuyCoffinArea buyCoffinArea) && !buyCoffinArea.PlayerIsInArea)
            {
                buyCoffinArea.PlayerIsInArea = true;

                if (buyCoffinArea.CoffinMachine.TimerHandler.IsMakingCoffin)
                {
                    UiEvents.OnPopupText?.Invoke(_player.transform.position + (Vector3.up * 3f), "He's Busy");
                    return;
                }
                else if (DataManager.TotalMoney == 0)
                {
                    UiEvents.OnPopupText?.Invoke(_player.transform.position + (Vector3.up * 3f), "No Money");
                    return;
                }
                else if (!buyCoffinArea.CoffinMachine.CanMakeCoffin)
                {
                    UiEvents.OnPopupText?.Invoke(_player.transform.position + (Vector3.up * 3f), "No Space");
                    return;
                }
                else if (buyCoffinArea.CoffinMachine.CanMakeCoffin)
                {
                    _player.MoneyHandler.StartSpending(buyCoffinArea);
                }
            }

            #region NEW GRAVE SYSTEM
            if (other.TryGetComponent(out GraveGround graveGround) && !graveGround.PlayerIsInArea)
            {
                graveGround.PlayerGetsIn();

                if (DataManager.TotalMoney == 0)
                {
                    UiEvents.OnPopupText?.Invoke(_player.transform.position + (Vector3.up * 3f), "No Money");
                    return;
                }
                else if (_player.IsCarryingCoffin)
                {
                    UiEvents.OnPopupText?.Invoke(_player.transform.position + (Vector3.up * 3f), "You're Carrying");
                    return;
                }

                if (_player.MoneyHandler.CanSpendMoney)
                    _player.MoneyHandler.StartSpending(graveGround);
            }
            if (other.gameObject.layer == LayerMask.NameToLayer("GraveUpgradeArea"))
            {
                FinishedGrave finishedGrave = other.GetComponentInParent<FinishedGrave>();
                if (!finishedGrave.PlayerIsInUpgradeArea && !finishedGrave.PlayerIsInMaintenanceArea && _player.MoneyHandler.CanSpendMoney)
                {
                    finishedGrave.EnteredUpgradeArea();
                    //finishedGrave.UpgradeLevel();
                    _player.MoneyHandler.StartSpending(finishedGrave);
                }
            }
            if (other.TryGetComponent(out FinishedGrave finishedGraveForMaintenance) && finishedGraveForMaintenance.SpoilHandler.IsSpoiling && !finishedGraveForMaintenance.PlayerIsInMaintenanceArea && !Player.IsMaintenancing && !_player.IsCarryingCoffin)
            {
                PlayerEvents.OnStartedMaintenance?.Invoke();
                WateringCanEvents.OnStartedWatering?.Invoke();
                finishedGraveForMaintenance.EnteredMaintenanceArea();
                _player.MaintenanceHandler.StartMaintenance(finishedGraveForMaintenance);
            }
            #endregion
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out TakeCoffinArea takeCoffinArea) && takeCoffinArea.PlayerIsInArea)
            {
                takeCoffinArea.PlayerIsInArea = false;
                _player.TimerHandler.StopFilling();
            }

            if (other.TryGetComponent(out DigArea digArea))
                digArea.StopFilling();

            if (other.TryGetComponent(out CoffinCarriersUnlocker coffinCarriersUnlocker) && coffinCarriersUnlocker.PlayerIsInArea)
            {
                coffinCarriersUnlocker.PlayerIsInArea = false;
                _player.MoneyHandler.StopSpending();
            }

            if (other.TryGetComponent(out BuyCoffinArea buyCoffinArea) && buyCoffinArea.PlayerIsInArea)
            {
                buyCoffinArea.PlayerIsInArea = false;
                _player.MoneyHandler.StopSpending();
            }

            #region NEW GRAVE SYSTEM
            if (other.TryGetComponent(out GraveGround graveGround) && graveGround.PlayerIsInArea)
            {
                graveGround.PlayerGetsOut();
                _player.MoneyHandler.StopSpending();
            }
            if (other.gameObject.layer == LayerMask.NameToLayer("GraveUpgradeArea"))
            {
                FinishedGrave finishedGrave = other.GetComponentInParent<FinishedGrave>();
                if (finishedGrave.PlayerIsInUpgradeArea)
                {
                    finishedGrave.ExitedUpgradeArea();
                    _player.MoneyHandler.StopSpending();
                }
            }
            if (other.TryGetComponent(out FinishedGrave finishedGraveForMaintenance) && finishedGraveForMaintenance.PlayerIsInMaintenanceArea)
            {
                PlayerEvents.OnStoppedMaintenance?.Invoke();
                WateringCanEvents.OnStoppedWatering?.Invoke();
                finishedGraveForMaintenance.ExitedMaintenanceArea();

                if (!finishedGraveForMaintenance.SpoilHandler.MaintenanceIsSuccessfull)
                    finishedGraveForMaintenance.OnContinueSpoiling?.Invoke();
            }
            #endregion
        }
    }
}
