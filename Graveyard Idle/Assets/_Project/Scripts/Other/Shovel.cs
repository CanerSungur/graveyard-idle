using UnityEngine;
using ZestGames;
using DG.Tweening;
using System;

namespace GraveyardIdle
{
    public class Shovel : MonoBehaviour
    {
        [Header("-- DIG SETUP --")]
        [SerializeField] private Transform throwSoilPoint;
        [SerializeField] private LayerMask soilMask;
        [SerializeField] private Transform soil;
        //private ShovelSoilParticles _soilParticle;

        private Player _player;

        private Collider _collider;
        private MeshRenderer _meshRenderer;
        
        private Transform _digPoint;
        private bool _canDig = false;
        private Ray _ray;
        private RaycastHit _hit;

        public bool ItCanDig => _canDig;
        public bool PutItDown { get; private set; }

        #region SEQUENCE
        private Sequence _changeSoilScaleSequence;
        private Guid _changeSoilScaleSequenceID;
        #endregion

        public void Init(Player player)
        {
            _player = player;
            //_soilParticle = GetComponentInChildren<ShovelSoilParticles>();
            //_soilParticle.Init(this);
            soil.localScale = Vector3.zero;

            _collider = GetComponent<Collider>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _digPoint = transform.GetChild(0);

            PutDown();
            DisableMesh();

            PlayerEvents.OnEnteredDigZone += PullOut;
            PlayerEvents.OnExitedDigZone += PutDown;
            PlayerEvents.OnEnteredFillZone += PullOut;
            PlayerEvents.OnExitedFillZone += PutDown;

            ShovelEvents.OnEnableMesh += EnableMesh;
            ShovelEvents.OnDisableMesh += DisableMesh;
            ShovelEvents.OnCanDig += CanDig;
            ShovelEvents.OnCantDig += CantDig;
            ShovelEvents.OnPlaySoilFX += PlaySoilFX;
            ShovelEvents.OnDigHappened += FillSoil;
            ShovelEvents.OnFillHappened += FillSoil;
        }

        private void OnDisable()
        {
            PlayerEvents.OnEnteredDigZone -= PullOut;
            PlayerEvents.OnExitedDigZone -= PutDown;
            PlayerEvents.OnEnteredFillZone -= PullOut;
            PlayerEvents.OnExitedFillZone -= PutDown;

            ShovelEvents.OnEnableMesh -= EnableMesh;
            ShovelEvents.OnDisableMesh -= DisableMesh;
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
            ShovelSoilParticles shovelSoilParticle = ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.ThrowSoilPS, throwSoilPoint.position, Quaternion.identity, throwSoilPoint).GetComponent<ShovelSoilParticles>();
            shovelSoilParticle.transform.localPosition = Vector3.zero;
            shovelSoilParticle.transform.localRotation = Quaternion.identity;
            shovelSoilParticle.Init(this, soilThrowTarget);

            //_soilParticle.ActivateSoilParticle(soilThrowTarget);
            EmptySoil();
        }
        private void EnableMesh() => _collider.enabled = _meshRenderer.enabled = true;
        private void DisableMesh() => _collider.enabled = _meshRenderer.enabled = false;
        private void PullOut()
        {
            PutItDown = false;
        }
        private void PutDown()
        {
            _canDig = false;
            PutItDown = true;
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
