using UnityEngine;
using ZestGames;
using DG.Tweening;
using System;
using ZestCore.Utility;

namespace GraveyardIdle.GraveSystem
{
    public class SoilFillable : MonoBehaviour
    {
        #region COMPONENTS
        private Grave _grave;
        private SkinnedMeshRenderer _skinnedMeshRenderer;
        private MeshCollider _meshCollider;
        private Mesh _changedMesh;
        private SoilPile _soilPile;
        #endregion

        #region FILL COUNT
        private const int TOTAL_FILL_COUNT = 5;
        private int _currentFillCount;
        #endregion

        #region SEQUENCE
        private Sequence _fillSequence;
        private Guid _fillSequenceID;
        public readonly float FillDuration = 0.5f;
        #endregion

        private bool _playerIsInArea;

        public Grave Grave => _grave;

        public void Init(Grave grave)
        {
            if (_grave == null)
            {
                _grave = grave;

                _soilPile = GetComponentInChildren<SoilPile>();
                _skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
                _meshCollider = GetComponent<MeshCollider>();
            }

            _soilPile.Init(this);
            _playerIsInArea = false;

            Load();

            ShovelEvents.OnFillHappened += GetFilled;
        }

        #region MONO FUNCTIONS
        private void OnDisable()
        {
            if (_grave == null) return;

            ShovelEvents.OnFillHappened -= GetFilled;

            Save();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player) && !_playerIsInArea && _currentFillCount < TOTAL_FILL_COUNT && _grave.HasCoffin)
            {
                _playerIsInArea = true;
                PlayerEvents.OnStartFilling?.Invoke();
                PlayerEvents.OnEnteredFillZone?.Invoke();
                PlayerEvents.OnPullOutShovel?.Invoke();
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Player player) && _playerIsInArea)
            {
                _playerIsInArea = false;
                PlayerEvents.OnStopFilling?.Invoke();
                PlayerEvents.OnExitedFillZone?.Invoke();
                PlayerEvents.OnPutDownShovel?.Invoke();
            }
        }
        #endregion

        #region EVENT HANDLER FUNCTIONS
        private void GetFilled()
        {
            if (!_playerIsInArea || _currentFillCount >= TOTAL_FILL_COUNT) return;

            StartFillSequence(_currentFillCount);
            _soilPile.UpdateForFilling(_currentFillCount);
            _currentFillCount++;
            //UpdateShape();
            //UpdateMeshCollider();

            if (_currentFillCount == TOTAL_FILL_COUNT)
            {
                PlayerEvents.OnExitedFillZone?.Invoke();
                PlayerEvents.OnStopFilling?.Invoke();
                PlayerEvents.OnPutDownShovel?.Invoke();

                Delayer.DoActionAfterDelay(this, _grave.PileUpdateDelay, () => _grave.FillingIsCompleted());
            }
        }
        #endregion

        #region UPDATER FUNCTIONS
        private void UpdateShape()
        {
            for (int i = 0; i < 5; i++)
            {
                if (i <= _currentFillCount - 1)
                    _skinnedMeshRenderer.SetBlendShapeWeight(TOTAL_FILL_COUNT - i - 1, 0f);
                else
                    _skinnedMeshRenderer.SetBlendShapeWeight(TOTAL_FILL_COUNT - i - 1, 100f);
            }

            _soilPile.UpdateShapeForFill(_currentFillCount);

            if (_currentFillCount == TOTAL_FILL_COUNT)
                _grave.FillingIsCompleted();

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
        private void StartFillSequence(int fillCount)
        {
            CreateFillSequence(fillCount);
            _fillSequence.Play();
        }
        private void CreateFillSequence(int fillCount)
        {
            if (_fillSequence == null)
            {
                _fillSequence = DOTween.Sequence();
                _fillSequenceID = Guid.NewGuid();
                _fillSequence.id = _fillSequenceID;

                _fillSequence.Append(DOVirtual.Float(100f, 0f, FillDuration, r => {
                    _skinnedMeshRenderer.SetBlendShapeWeight(TOTAL_FILL_COUNT - fillCount - 1, r);
                    UpdateMeshCollider();
                })).OnComplete(() => DeleteFillSequence());
            }
        }
        private void DeleteFillSequence()
        {
            DOTween.Kill(_fillSequenceID);
            _fillSequence = null;
        }
        #endregion

        #region SAVE-LOAD FUNCTIONS
        private void Save()
        {
            PlayerPrefs.SetInt($"Grave-{_grave.ID}-FillCount", _currentFillCount);
            PlayerPrefs.Save();
        }
        private void Load()
        {
            _currentFillCount = PlayerPrefs.GetInt($"Grave-{_grave.ID}-FillCount", 0);

            UpdateShape();
        }
        #endregion
    }
}
