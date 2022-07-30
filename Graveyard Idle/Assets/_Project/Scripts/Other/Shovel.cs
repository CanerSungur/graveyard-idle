using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class Shovel : MonoBehaviour
    {
        [Header("-- DIG SETUP --")]
        [SerializeField] private LayerMask soilMask;

        private Player _player;

        private Collider _collider;
        private MeshRenderer _meshRenderer;
        
        private Transform _digPoint;
        private bool _canDig = false;
        private Ray _ray;
        private RaycastHit _hit;

        public void Init(Player player)
        {
            _player = player;

            _collider = GetComponent<Collider>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _digPoint = transform.GetChild(0);
            Disable();

            PlayerEvents.OnStartDigging += Enable;
            PlayerEvents.OnStopDigging += Disable;
            ShovelEvents.OnCanDig += CanDig;
            ShovelEvents.OnCantDig += CantDig;

            //ShovelEvents.OnDigHappened += ShootRay;
        }

        private void OnDisable()
        {
            PlayerEvents.OnStartDigging -= Enable;
            PlayerEvents.OnStopDigging -= Disable;
            ShovelEvents.OnCanDig -= CanDig;
            ShovelEvents.OnCantDig -= CantDig;

            //ShovelEvents.OnDigHappened -= ShootRay;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out SoilDeform soilDeform) && _canDig)
                soilDeform.DeformThis(_digPoint.position);
        }

        private void ShootRay()
        {
            if (Physics.Raycast(_player.transform.position, Vector3.down, out _hit, 2f, soilMask))
            {
                SoilDeform soilDeform = _hit.transform.GetComponent<SoilDeform>();
                soilDeform.DeformThis(_hit.point);
            }
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
