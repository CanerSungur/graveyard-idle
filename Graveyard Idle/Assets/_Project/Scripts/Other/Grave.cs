using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class Grave : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player) && !player.IsInDigZone)
            {
                PlayerEvents.OnEnteredDigZone?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Player player) && player.IsInDigZone)
            {
                PlayerEvents.OnExitedDigZone?.Invoke();
            }
        }
    }
}
