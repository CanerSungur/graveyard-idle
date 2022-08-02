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
            if (other.TryGetComponent(out TakeCoffinArea takeCoffinArea) && !takeCoffinArea.PlayerIsInArea)
            {
                //takeCoffinArea.StartOpening();
                takeCoffinArea.TakeCoffin(_player);
            }
                

            if (other.TryGetComponent(out DigArea digArea) && !_player.IsCarryingCoffin)
                digArea.StartFilling();

            //if (other.gameObject.layer == LayerMask.NameToLayer("CoffinDropArea") && _player.IsCarryingCoffin)
            //{
            //    PlayerEvents.OnDropCoffin?.Invoke();
            //    // start drop trigger timer.
            //}
            if (other.TryGetComponent(out InteractableGround interactableGround) && _player.IsCarryingCoffin && interactableGround.CanBeThrownCoffin)
            {
                PlayerEvents.OnDropCoffin?.Invoke(_player.CoffinCarryingNow, interactableGround);
                interactableGround.HasCoffin = true;
                interactableGround.CanBeThrownCoffin = false;
                GraveManagerEvents.OnCoffinThrownToGrave?.Invoke();
                // start drop trigger timer.
            }

            if (other.TryGetComponent(out InteractableGroundCanvas interactableGroundCanvas) && !interactableGroundCanvas.PlayerIsInArea)
                interactableGroundCanvas.StartOpening();
        }

        private void OnTriggerExit(Collider other)
        {
            //if (other.TryGetComponent(out TakeCoffinArea takeCoffinArea) && takeCoffinArea.PlayerIsInArea)
            //    takeCoffinArea.StopOpening();

            if (other.TryGetComponent(out TakeCoffinArea takeCoffinArea) && takeCoffinArea.PlayerIsInArea)
            {
                //takeCoffinArea.StartOpening();
                takeCoffinArea.PlayerExitArea();
            }

            if (other.TryGetComponent(out DigArea digArea))
                digArea.StopFilling();

            if (other.TryGetComponent(out InteractableGroundCanvas interactableGroundCanvas) && interactableGroundCanvas.PlayerIsInArea)
                interactableGroundCanvas.StopOpening();
        }
    }
}
