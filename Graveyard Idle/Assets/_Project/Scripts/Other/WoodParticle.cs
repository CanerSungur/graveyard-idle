using UnityEngine;

namespace GraveyardIdle
{
    public class WoodParticle : MonoBehaviour
    {
        private ParticleSystem _particleSystem;

        private void Start()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Log log) && !_particleSystem.isPlaying)
            {
                _particleSystem.Play();
            }
        }
    }
}
