using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class Shovel : MonoBehaviour
    {
        private Collider _collider;
        private MeshRenderer _meshRenderer;
        private Transform _digPoint;

        private bool _canDig = false;

        public void Init(Player player)
        {
            _collider = GetComponent<Collider>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _digPoint = transform.GetChild(0);
            Disable();

            PlayerEvents.OnStartDigging += Enable;
            PlayerEvents.OnStopDigging += Disable;
            ShovelEvents.OnCanDig += CanDig;
            ShovelEvents.OnCantDig += CantDig;
        }

        private void OnDisable()
        {
            PlayerEvents.OnStartDigging -= Enable;
            PlayerEvents.OnStopDigging -= Disable;
            ShovelEvents.OnCanDig -= CanDig;
            ShovelEvents.OnCantDig -= CantDig;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out SoilDeform soilDeform) && _canDig)
                soilDeform.DeformThis(_digPoint.position);
        }

        private void Enable()
        {
            _collider.enabled = _meshRenderer.enabled = true;
        }
        private void Disable()
        {
            _collider.enabled = _meshRenderer.enabled = false;
            _canDig = false;
        }
        private void CanDig() => _canDig = true;
        private void CantDig() => _canDig = false;
    }
}
