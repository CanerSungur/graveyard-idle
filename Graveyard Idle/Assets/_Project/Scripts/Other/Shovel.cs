using UnityEngine;
using ZestGames;
using DG.Tweening;
using System;

namespace GraveyardIdle
{
    public class Shovel : MonoBehaviour
    {
        [Header("-- DIG SETUP --")]
        [SerializeField] private LayerMask soilMask;
        [SerializeField] private Transform soil;
        private ShovelSoilParticles _soilParticle;

        private Player _player;

        private Collider _collider;
        private MeshRenderer _meshRenderer;
        
        private Transform _digPoint;
        private bool _canDig = false;
        private Ray _ray;
        private RaycastHit _hit;

        public bool ItCanDig => _canDig;

        #region SEQUENCE
        private Sequence _changeSoilScaleSequence;
        private Guid _changeSoilScaleSequenceID;
        #endregion

        public void Init(Player player)
        {
            _player = player;
            _soilParticle = GetComponentInChildren<ShovelSoilParticles>();
            _soilParticle.Init(this);
            soil.localScale = Vector3.zero;

            _collider = GetComponent<Collider>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _digPoint = transform.GetChild(0);
            Disable();

            PlayerEvents.OnEnteredDigZone += Enable;
            PlayerEvents.OnExitedDigZone += Disable;
            PlayerEvents.OnEnteredFillZone += Enable;
            PlayerEvents.OnExitedFillZone += Disable;
            ShovelEvents.OnCanDig += CanDig;
            ShovelEvents.OnCantDig += CantDig;
            ShovelEvents.OnPlaySoilFX += PlaySoilFX;
            ShovelEvents.OnDigHappened += FillSoil;
            ShovelEvents.OnFillHappened += FillSoil;
        }

        private void OnDisable()
        {
            PlayerEvents.OnEnteredDigZone -= Enable;
            PlayerEvents.OnExitedDigZone -= Disable;
            PlayerEvents.OnEnteredFillZone -= Enable;
            PlayerEvents.OnExitedFillZone -= Disable;
            ShovelEvents.OnCanDig -= CanDig;
            ShovelEvents.OnCantDig -= CantDig;
            ShovelEvents.OnPlaySoilFX -= PlaySoilFX;
            ShovelEvents.OnDigHappened -= FillSoil;
            ShovelEvents.OnFillHappened -= FillSoil;
        }

        private void ShootRay()
        {
            if (Physics.Raycast(_player.transform.position, Vector3.down, out _hit, 2f, soilMask))
            {
                SoilDeform soilDeform = _hit.transform.GetComponent<SoilDeform>();
                soilDeform.DeformThis(_hit.point);
            }
        }
        private void PlaySoilFX(Enums.SoilThrowTarget soilThrowTarget)
        {
            _soilParticle.ActivateSoilParticle(soilThrowTarget);
            EmptySoil();
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

        private void FillSoil()
        {
            //soil.localScale = Vector3.one * 1.2f;
            StartChangeSoilScaleSequence(Vector3.one * 1.2f);
        }
        private void EmptySoil()
        {
            //StartChangeSoilScaleSequence(Vector3.zero);
            soil.localScale = Vector3.zero;
        }

        #region DOTWEEN FUNCTIONS
        private void StartChangeSoilScaleSequence(Vector3 targetscale)
        {
            _changeSoilScaleSequence.Pause();
            DeleteChangeSoilScaleSequence();
            CreateChangeSoilScaleSequence(targetscale);
            _changeSoilScaleSequence.Play();
        }
        private void CreateChangeSoilScaleSequence(Vector3 targetScale)
        {
            if (_changeSoilScaleSequence == null)
            {
                _changeSoilScaleSequence = DOTween.Sequence();
                _changeSoilScaleSequenceID = Guid.NewGuid();
                _changeSoilScaleSequence.id = _changeSoilScaleSequenceID;

                _changeSoilScaleSequence.Append(DOVirtual.Vector3(soil.transform.localScale, targetScale, .1f, r => {
                    soil.transform.localScale = r;
                })).OnComplete(() => DeleteChangeSoilScaleSequence());
            }
        }
        private void DeleteChangeSoilScaleSequence()
        {
            DOTween.Kill(_changeSoilScaleSequenceID);
            _changeSoilScaleSequence = null;
        }
        #endregion
    }
}
