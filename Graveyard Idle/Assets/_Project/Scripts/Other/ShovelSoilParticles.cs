using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class ShovelSoilParticles : MonoBehaviour
    {
        private Shovel _shovel;

        private bool _canTrigger;
        private Enums.SoilThrowTarget _currentThrowTarget;
        private ParticleSystem _particleSystem;
        
        public void Init(Shovel shovel)
        {
            _shovel = shovel;
            _particleSystem = GetComponent<ParticleSystem>();
            _canTrigger = false;
        }

        private void OnParticleCollision(GameObject other)
        {
            if (!_canTrigger) return;
            Debug.Log("particle collided");

            if (_currentThrowTarget == Enums.SoilThrowTarget.Grave)
                ShovelEvents.OnThrowSoilToGrave?.Invoke();
            else if (_currentThrowTarget == Enums.SoilThrowTarget.Pile)
                ShovelEvents.OnThrowSoilToPile?.Invoke();
            _canTrigger = false;
        }

        #region PUBLICS
        public void ActivateSoilParticle(Enums.SoilThrowTarget soilThrowTarget)
        {
            _canTrigger = true;
            _currentThrowTarget = soilThrowTarget;
            _particleSystem.Play();
        }
        #endregion
    }
}
