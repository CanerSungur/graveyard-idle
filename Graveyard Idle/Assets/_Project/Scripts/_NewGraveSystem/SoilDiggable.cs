using UnityEngine;
using ZestGames;
using DG.Tweening;
using System;
using ZestCore.Utility;

namespace GraveyardIdle.GraveSystem
{
    public class SoilDiggable : MonoBehaviour
    {
        #region COMPONENTS
        private Grave _grave;
        private SkinnedMeshRenderer _skinnedMeshRenderer;
        private Mesh _changedMesh;
        private MeshCollider _meshCollider;
        private SoilPile _soilPile;
        #endregion

        #region DIG COUNT
        private const int TOTAL_DIG_COUNT = 5;
        private int _currentDigCount;
        #endregion

        #region SEQUENCE
        private Sequence _digSequence;
        private Guid _digSequenceID;
        public readonly float DigDuration = 0.5f;
        private const float HEIGHT_DECREASE_VALUE = -0.11f;
        #endregion

        private bool _playerIsInArea;

        public Grave Grave => _grave;

        public void Init(Grave grave)
        {
            if (_grave == null)
            {
                _grave = grave;
                _skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
                _meshCollider = GetComponent<MeshCollider>();
                _soilPile = GetComponentInChildren<SoilPile>();
            }

            _soilPile.Init(this);
            _playerIsInArea = false;

            Load();

            ShovelEvents.OnDigHappened += GetDigged;
        }

        #region MONO FUNCTIONS
        private void OnDisable()
        {
            if (_grave == null) return;

            ShovelEvents.OnDigHappened -= GetDigged;

            Save();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player) && !_playerIsInArea && _currentDigCount < TOTAL_DIG_COUNT)
            {
                _playerIsInArea = true;
                PlayerEvents.OnStartDigging?.Invoke();
                PlayerEvents.OnEnteredDigZone?.Invoke();
                PlayerEvents.OnPullOutShovel?.Invoke();
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Player player) && _playerIsInArea)
            {
                _playerIsInArea = false;
                PlayerEvents.OnStopDigging?.Invoke();
                PlayerEvents.OnExitedDigZone?.Invoke();
                PlayerEvents.OnPutDownShovel?.Invoke();
            }
        }
        #endregion

        #region EVENT HANDLER FUNCTIONS
        private void GetDigged()
        {
            if (!_playerIsInArea || _currentDigCount >= TOTAL_DIG_COUNT) return;

            StartDigSequence(_currentDigCount);
            _soilPile.UpdateForDigging(_currentDigCount);
            _currentDigCount++;
            //UpdateSoilShape();
            //UpdateMeshCollider();

            if (_currentDigCount == TOTAL_DIG_COUNT)
            {
                PlayerEvents.OnExitedDigZone?.Invoke();
                PlayerEvents.OnStopDigging?.Invoke();
                PlayerEvents.OnPullOutShovel?.Invoke();

                Delayer.DoActionAfterDelay(this, _grave.PileUpdateDelay, () => _grave.DiggingIsComplete());
            }
        }
        #endregion

        #region UPDATER FUNCTIONS
        private void UpdateShape()
        {
            for (int i = 0; i < 5; i++)
            {
                if (i <= _currentDigCount - 1)
                    _skinnedMeshRenderer.SetBlendShapeWeight(i, 100f);
                else
                    _skinnedMeshRenderer.SetBlendShapeWeight(i, 0f);
            }
            transform.localPosition = new Vector3(transform.localPosition.x, HEIGHT_DECREASE_VALUE * (_currentDigCount), transform.localPosition.z);

            _soilPile.UpdateShapeForDig(_currentDigCount);

            if (_currentDigCount == TOTAL_DIG_COUNT)
                _grave.DiggingIsComplete();

            UpdateMeshCollider();
        }

        private void UpdateMeshCollider()
        {
            _changedMesh = new Mesh();
            _skinnedMeshRenderer.BakeMesh(_changedMesh);
            _meshCollider.sharedMesh = null;
            _meshCollider.sharedMesh = _changedMesh;
        }
        #endregion

        #region DOTWEEN FUNCTIONS
        private void StartDigSequence(int digCount)
        {
            CreateDigSequence(digCount);
            _digSequence.Play();
        }
        private void CreateDigSequence(int digCount)
        {
            if (_digSequence == null)
            {
                _digSequence = DOTween.Sequence();
                _digSequenceID = Guid.NewGuid();
                _digSequence.id = _digSequenceID;

                _digSequence.Append(DOVirtual.Float(0f, 100f, DigDuration, r => {
                    _skinnedMeshRenderer.SetBlendShapeWeight(digCount, r);
                    UpdateMeshCollider();
                }))
                    .Join(transform.DOLocalMoveY(HEIGHT_DECREASE_VALUE * (digCount + 1), DigDuration)).OnComplete(() => DeleteDigSequence());
            }
        }
        private void DeleteDigSequence()
        {
            DOTween.Kill(_digSequenceID);
            _digSequence = null;
        }
        #endregion

        #region SAVE-LOAD FUNCTIONS
        private void Save()
        {
            PlayerPrefs.SetInt($"Grave-{_grave.ID}-DigCount", _currentDigCount);
            PlayerPrefs.Save();
        }
        private void Load()
        {
            _currentDigCount = PlayerPrefs.GetInt($"Grave-{_grave.ID}-DigCount", 0);

            UpdateShape();
        }
        #endregion
    }
}
