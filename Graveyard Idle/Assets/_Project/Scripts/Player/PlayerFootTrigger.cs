using UnityEngine;

namespace GraveyardIdle
{
    public class PlayerFootTrigger : MonoBehaviour
    {
        private bool _isTriggered;
        private ParticleSystem _dirtParticle;

        private void Start()
        {
            _dirtParticle = GetComponentInChildren<ParticleSystem>();
            _isTriggered = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Ground") && !_isTriggered)
            {
                _isTriggered = true;
                _dirtParticle.Play();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Ground") && _isTriggered)
            {
                _isTriggered = false;
            }
        }
    }
}
