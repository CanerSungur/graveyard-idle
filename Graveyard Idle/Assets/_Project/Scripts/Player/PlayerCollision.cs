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
                if (GraveManager.CanCoffinBeingCarried && CoffinAreaStackHandler.CoffinsInArea.Count > 0)
                    _player.TimerHandler.StartFilling(() => takeCoffinArea.TakeCoffin(_player));
                else if (CoffinAreaStackHandler.CoffinsInArea.Count <= 0)
                {
                    UiEvents.OnPopupText?.Invoke(_player.transform.position + (Vector3.up * 3f), "No Coffin");
                    return;
                }
                else if (!GraveManager.CanCoffinBeingCarried)
                {
                    UiEvents.OnPopupText?.Invoke(_player.transform.position + (Vector3.up * 3f), "No Grave");
                    return;
                }
            }

            if (other.TryGetComponent(out DigArea digArea) && !_player.IsCarryingCoffin)
                digArea.StartFilling();

            if (other.TryGetComponent(out InteractableGround interactableGround))
            {
                _player.EnteredInteractableGround = interactableGround;

                if (_player.IsCarryingCoffin && interactableGround.CanBeThrownCoffin && !interactableGround.Grave.CarriersAssigned)
                {
                    _player.TimerHandler.StartFilling(() => {
                        PlayerEvents.OnDropCoffin?.Invoke(_player.CoffinCarryingNow, interactableGround);
                        interactableGround.HasCoffin = true;
                        interactableGround.CanBeThrownCoffin = false;
                        GraveManagerEvents.OnCoffinThrownToGrave?.Invoke();
                        GraveManager.RemoveEmptyGrave(interactableGround.Grave);
                    });
                }
            }

            if (other.TryGetComponent(out InteractableGroundCanvas interactableGroundCanvas) && !interactableGroundCanvas.PlayerIsInArea && _player.MoneyHandler.CanSpendMoney)
            {
                interactableGroundCanvas.PlayerIsInArea = true;
                _player.MoneyHandler.StartSpending(interactableGroundCanvas);

            }

            if (other.gameObject.layer == LayerMask.NameToLayer("GraveUpgradeArea"))
            {
                GraveUpgradeHandler graveUpgradeHandler = other.GetComponentInParent<GraveUpgradeHandler>();
                if (graveUpgradeHandler != null && !graveUpgradeHandler.Grave.PlayerIsInUpgradeArea && !graveUpgradeHandler.Grave.PlayerIsInMaintenanceArea && _player.MoneyHandler.CanSpendMoney)
                {
                    graveUpgradeHandler.PlayerStartedUpgrading();
                    _player.MoneyHandler.StartSpending(graveUpgradeHandler);
                }
            }

            if (other.TryGetComponent(out BuyCoffinArea buyCoffinArea) && !buyCoffinArea.PlayerIsInArea)
            {
                buyCoffinArea.PlayerIsInArea = true;

                if (buyCoffinArea.CoffinMachine.TimerHandler.IsMakingCoffin)
                {
                    UiEvents.OnPopupText?.Invoke(_player.transform.position + (Vector3.up * 3f), "He's Busy");
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

            if (other.TryGetComponent(out Grave grave) && grave.SpoilHandler.IsSpoiling && !grave.PlayerIsInMaintenanceArea && !Player.IsMaintenancing && !_player.IsCarryingCoffin)
            {
                PlayerEvents.OnStartedMaintenance?.Invoke();
                WateringCanEvents.OnStartedWatering?.Invoke();
                grave.PlayerIsInMaintenanceArea = true;
                _player.MaintenanceHandler.StartMaintenance(grave);
            }
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

            if (other.TryGetComponent(out InteractableGround interactableGround))
            {
                _player.EnteredInteractableGround = null;

                if (_player.TimerHandler.IsFilling)
                    _player.TimerHandler.StopFilling();
            }

            if (other.TryGetComponent(out InteractableGroundCanvas interactableGroundCanvas) && interactableGroundCanvas.PlayerIsInArea)
            {
                interactableGroundCanvas.PlayerIsInArea = false;
                _player.MoneyHandler.StopSpending();
            }

            if (other.gameObject.layer == LayerMask.NameToLayer("GraveUpgradeArea"))
            {
                GraveUpgradeHandler graveUpgradeHandler = other.GetComponentInParent<GraveUpgradeHandler>();
                if (graveUpgradeHandler != null && graveUpgradeHandler.Grave.PlayerIsInUpgradeArea)
                {
                    graveUpgradeHandler.PlayerStoppedUpgrading();
                    _player.MoneyHandler.StopSpending();
                }
            }

            if (other.TryGetComponent(out BuyCoffinArea buyCoffinArea) && buyCoffinArea.PlayerIsInArea)
            {
                buyCoffinArea.PlayerIsInArea = false;
                _player.MoneyHandler.StopSpending();
            }

            if (other.TryGetComponent(out Grave grave) && grave.SpoilHandler.CanSpoil && grave.PlayerIsInMaintenanceArea)
            {
                PlayerEvents.OnStoppedMaintenance?.Invoke();
                WateringCanEvents.OnStoppedWatering?.Invoke();
                grave.PlayerIsInMaintenanceArea = false;
                //grave.OnStartSpoiling?.Invoke();
            }
        }
    }
}
