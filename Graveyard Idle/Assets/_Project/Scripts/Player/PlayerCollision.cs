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
            if (other.TryGetComponent(out TakeCoffinArea takeCoffinArea))
                takeCoffinArea.StartOpening();

            if (other.TryGetComponent(out DigArea digArea) && !_player.IsCarryingCoffin)
                digArea.StartFilling();

            if (other.gameObject.layer == LayerMask.NameToLayer("CoffinDropArea") && _player.IsCarryingCoffin)
                PlayerEvents.OnDropCoffin?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out TakeCoffinArea takeCoffinArea))
                takeCoffinArea.StopOpening();

            if (other.TryGetComponent(out DigArea digArea))
                digArea.StopFilling();
        }
    }
}
