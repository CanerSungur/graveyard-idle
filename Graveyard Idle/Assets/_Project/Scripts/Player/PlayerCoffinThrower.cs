using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class PlayerCoffinThrower : MonoBehaviour
    {
        private Player _player;

        public void Init(Player player)
        {
            _player = player;

            PlayerEvents.OnDropCoffin += ThrowCoffin;
            PlayerEvents.OnThrowCoffin += ThrowCoffin;
        }

        private void OnDisable()
        {
            if (!_player) return;

            PlayerEvents.OnDropCoffin -= ThrowCoffin;
            PlayerEvents.OnThrowCoffin -= ThrowCoffin;
        }

        private void ThrowCoffin(Coffin carriedCoffin, InteractableGround triggeredInteractableGround)
        {
            carriedCoffin.GetThrownToGrave(triggeredInteractableGround.transform);
        }
        private void ThrowCoffin(Coffin carriedCoffin, GraveyardIdle.GraveSystem.Grave triggeredGrave)
        {
            carriedCoffin.GetThrownToGrave(triggeredGrave.transform);
        }
    }
}
