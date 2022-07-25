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
            if (other.gameObject.layer == LayerMask.NameToLayer("CoffinTakeArea") && !_player.IsCarryingCoffin)
            {
                PlayerEvents.OnTakeACoffin?.Invoke();
                Debug.Log("Take a COFFIN!");
            }

            if (other.gameObject.layer == LayerMask.NameToLayer("CoffinDropArea") && _player.IsCarryingCoffin)
            {
                PlayerEvents.OnDropCoffin?.Invoke();
            }
        }
    }
}
