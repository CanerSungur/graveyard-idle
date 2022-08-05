using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class ShovelSoilParticles : MonoBehaviour
    {
        private Shovel _shovel;

        private bool _triggered;
        private Enums.SoilThrowTarget _currentThrowTarget;
        private ParticleSystem _particleSystem;

        public void Init(Shovel shovel, Enums.SoilThrowTarget soilThrowTarget)
        {
            _shovel = shovel;
            _particleSystem = GetComponent<ParticleSystem>();
            _triggered = false;

            ActivateSoilParticle(soilThrowTarget);
        }

        private void OnParticleCollision(GameObject other)
        {
            if (_triggered) return;

            if (_currentThrowTarget == Enums.SoilThrowTarget.Grave)
                ShovelEvents.OnThrowSoilToGrave?.Invoke();
            else if (_currentThrowTarget == Enums.SoilThrowTarget.Pile)
                ShovelEvents.OnThrowSoilToPile?.Invoke();
            _triggered = true;
        }

        private void ActivateSoilParticle(Enums.SoilThrowTarget soilThrowTarget)
        {
            _currentThrowTarget = soilThrowTarget;
            _particleSystem.Play();
        }
    }
}
