using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class PlayerCoffinThrower : MonoBehaviour
    {
        private Player _player;

        [SerializeField] private Transform throwTransform;

        public void Init(Player player)
        {
            _player = player;

            PlayerEvents.OnDropCoffin += ThrowCoffin;
        }

        private void OnDisable()
        {
            if (!_player) return;

            PlayerEvents.OnDropCoffin -= ThrowCoffin;
        }

        private void ThrowCoffin(Coffin carriedCoffin, InteractableGround triggeredInteractableGround)
        {
            carriedCoffin.GetThrownToGrave(triggeredInteractableGround.transform);
        }
    }
}
