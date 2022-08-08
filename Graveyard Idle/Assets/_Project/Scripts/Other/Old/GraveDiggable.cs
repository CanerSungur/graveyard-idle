using UnityEngine;
using ZestGames;
using ZestCore.Utility;

namespace GraveyardIdle
{
    public class GraveDiggable : MonoBehaviour
    {
        private SoilDeform _soil;
        private bool _isDug = false;

        private void OnEnable()
        {
            _isDug = false;
            _soil = GetComponentInChildren<SoilDeform>();
            Delayer.DoActionAfterDelay(this, 2f, () => _soil.Init(this));

            GraveEvents.OnAGraveIsDug += GraveIsDug;
        }

        private void OnDisable()
        {
            GraveEvents.OnAGraveIsDug -= GraveIsDug;
        }

        private void GraveIsDug(GraveDiggable grave)
        {
            if (grave != this) return;
            _isDug = true;
            PlayerEvents.OnExitedDigZone?.Invoke();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player) && !player.IsInDigZone && !_isDug)
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
