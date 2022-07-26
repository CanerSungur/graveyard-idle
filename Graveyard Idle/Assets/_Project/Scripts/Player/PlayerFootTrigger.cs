using UnityEngine;

namespace GraveyardIdle
{
    public class PlayerFootTrigger : MonoBehaviour
    {
        private Player _player;
        private bool _isTriggered;
        private ParticleSystem _dirtParticle;

        private void Start()
        {
            _player = GetComponentInParent<Player>();
            _dirtParticle = GetComponentInChildren<ParticleSystem>();
            _isTriggered = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Ground") && !_isTriggered && !_player.IsDigging)
            {
                _isTriggered = true;
                _dirtParticle.Play();
            }

            //if (other.TryGetComponent(out SoilDeform soilDeform))
            //{
            //    soilDeform.DeformThis(transform.position);
            //}
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
