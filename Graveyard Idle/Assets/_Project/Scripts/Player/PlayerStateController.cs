using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class PlayerStateController : MonoBehaviour
    {
        private Transform _meshTransform;
        private readonly Quaternion _defaultRot = Quaternion.identity;
        private readonly Quaternion _carryRot = Quaternion.Euler(0f, 180f, 0f);

        public void Init(Player player)
        {
            _meshTransform = transform.GetChild(0);
            _meshTransform.localRotation = _defaultRot;

            PlayerEvents.OnTakeACoffin += StartCarryingCoffin;
            PlayerEvents.OnDropCoffin += StopCarryingCoffin;
        }

        private void OnDisable()
        {
            PlayerEvents.OnTakeACoffin -= StartCarryingCoffin;
            PlayerEvents.OnDropCoffin -= StopCarryingCoffin;
        }

        private void StartCarryingCoffin()
        {
            _meshTransform.localRotation = _carryRot;
        }
        private void StopCarryingCoffin()
        {
            _meshTransform.localRotation = _defaultRot;
        }
    }
}
