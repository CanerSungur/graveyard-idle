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
                //takeCoffinArea.StartOpening();
                //takeCoffinArea.TakeCoffin(_player);
                if (GraveManager.CanCoffinBeingCarried && CoffinAreaStackHandler.CoffinsInArea.Count > 0)
                    _player.TimerHandler.StartFilling(() => takeCoffinArea.TakeCoffin(_player));
                else if (CoffinAreaStackHandler.CoffinsInArea.Count <= 0)
                {
                    // trigger there's no coffin ui
                    UiEvents.OnPopupText?.Invoke(_player.transform.position + (Vector3.up * 3f), "No Coffin");
                    Debug.Log("no coffin");
                    return;
                }
                else if (!GraveManager.CanCoffinBeingCarried)
                {
                    // trigger there's no grave
                    UiEvents.OnPopupText?.Invoke(_player.transform.position + (Vector3.up * 3f), "No Grave");
                    Debug.Log("no grave");
                    return;
                }
            }

            if (other.TryGetComponent(out DigArea digArea) && !_player.IsCarryingCoffin)
                digArea.StartFilling();

            //if (other.gameObject.layer == LayerMask.NameToLayer("CoffinDropArea") && _player.IsCarryingCoffin)
            //{
            //    PlayerEvents.OnDropCoffin?.Invoke();
            //    // start drop trigger timer.
            //}
            if (other.TryGetComponent(out InteractableGround interactableGround))
            {
                _player.EnteredInteractableGround = interactableGround;

                if (_player.IsCarryingCoffin && interactableGround.CanBeThrownCoffin)
                {
                    _player.TimerHandler.StartFilling(() => {
                        PlayerEvents.OnDropCoffin?.Invoke(_player.CoffinCarryingNow, interactableGround);
                        interactableGround.HasCoffin = true;
                        interactableGround.CanBeThrownCoffin = false;
                        GraveManagerEvents.OnCoffinThrownToGrave?.Invoke();
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
                if (graveUpgradeHandler != null && !graveUpgradeHandler.Grave.PlayerIsInUpgradeArea && _player.MoneyHandler.CanSpendMoney)
                {
                    graveUpgradeHandler.PlayerStartedUpgrading();
                    _player.MoneyHandler.StartSpending(graveUpgradeHandler);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            //if (other.TryGetComponent(out TakeCoffinArea takeCoffinArea) && takeCoffinArea.PlayerIsInArea)
            //    takeCoffinArea.StopOpening();

            if (other.TryGetComponent(out TakeCoffinArea takeCoffinArea) && takeCoffinArea.PlayerIsInArea)
            {
                takeCoffinArea.PlayerIsInArea = false;
                //takeCoffinArea.StartOpening();
                //takeCoffinArea.PlayerExitArea();
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
                _player.MoneyHandler.StopSpending(interactableGroundCanvas);
            }

            if (other.gameObject.layer == LayerMask.NameToLayer("GraveUpgradeArea"))
            {
                GraveUpgradeHandler graveUpgradeHandler = other.GetComponentInParent<GraveUpgradeHandler>();
                if (graveUpgradeHandler != null && graveUpgradeHandler.Grave.PlayerIsInUpgradeArea)
                {
                    graveUpgradeHandler.PlayerStoppedUpgrading();
                    _player.MoneyHandler.StopSpending(graveUpgradeHandler);
                }
            }
        }
    }
}
