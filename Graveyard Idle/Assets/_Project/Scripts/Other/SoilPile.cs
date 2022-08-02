using UnityEngine;
using System;
using DG.Tweening;
using ZestGames;

namespace GraveyardIdle
{
    public class SoilPile : MonoBehaviour
    {
        private bool _initialized = false;
        private InteractableGround _interactableGround;

        private SkinnedMeshRenderer _skinnedMeshRenderer;
        private Mesh _changedMesh;
        private MeshCollider _meshCollider;

        private bool _playerIsInArea = false;
        public bool PlayerIsInArea => _playerIsInArea;
        private int _currentBlendWeightIndex = -1;

        #region FILLING
        private int _fillCount = 5;
        private int _currentFillCount = 0;
        private readonly float _getFilledDuration = 1f;
        #endregion

        #region PILED VALUES
        private int _pileCount = 5;
        private int _currentPileCount = 0;
        private readonly float _getPiledDuration = 1f;
        #endregion

        #region SEQUENCE
        private Sequence _getPiledSequence, _getFilledSequence;
        private Guid _getPiledSequenceID, _getFilledSequenceID;
        #endregion

        public void Init(InteractableGround interactableGround)
        {
            _initialized = true;
            _interactableGround = interactableGround;

            _skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
            _skinnedMeshRenderer.SetBlendShapeWeight(0, 0f);
            _meshCollider = GetComponent<MeshCollider>();

            _playerIsInArea = false;

            ShovelEvents.OnFillHappened += GetFilled;
            ShovelEvents.OnThrowSoilToPile += GetPiled;
        }

        private void OnDisable()
        {
            if (!_initialized) return;

            ShovelEvents.OnFillHappened -= GetFilled;
            ShovelEvents.OnThrowSoilToPile -= GetPiled;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player) && !_playerIsInArea && _interactableGround.CanBeFilled)
            {
                _playerIsInArea = true;
                PlayerEvents.OnEnteredFillZone?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Player player) && _playerIsInArea)
            {
                _playerIsInArea = false;
                PlayerEvents.OnExitedFillZone?.Invoke();
            }
        }

        private void GetPiled()
        {
            if (!_interactableGround.DiggableSoil.PlayerIsInArea) return;

            _currentPileCount++;
            _currentBlendWeightIndex = _pileCount - _currentPileCount;

            StartGetPiledSequence(_currentBlendWeightIndex - 1);

            if (_currentPileCount == _pileCount)
            {
                PlayerEvents.OnExitedDigZone?.Invoke();
                PlayerEvents.OnStopDigging?.Invoke();
            }
        }
        private void GetFilled()
        {
            if (!_playerIsInArea || !_interactableGround.CanBeFilled) return;

            _currentBlendWeightIndex = _currentFillCount;
            _currentFillCount++;

            StartGetFilledSequence(_currentBlendWeightIndex - 1);

            //if (_currentFillCount == _fillCount)
            //{
            //    _interactableGround.CanBeFilled = false;
            //}
        }
        private void UpdateMeshCollider()
        {
            _changedMesh = new Mesh();
            _skinnedMeshRenderer.BakeMesh(_changedMesh);
            _meshCollider.sharedMesh = null;
            _meshCollider.sharedMesh = _changedMesh;
        }

        #region DOTWEEN FUNCTIONS
        private void StartGetFilledSequence(int index)
        {
            _getFilledSequence.Pause();
            DeleteGetFilledSequence();
            CreateGetFilledSequence(index);
            _getFilledSequence.Play();

            //_interactableGround.DiggableSoil.GetFilled();
        }
        private void CreateGetFilledSequence(int index)
        {
            if (_getFilledSequence == null)
            {
                _getFilledSequence = DOTween.Sequence();
                _getFilledSequenceID = Guid.NewGuid();
                _getFilledSequence.id = _getFilledSequenceID;

                _getFilledSequence.Append(DOVirtual.Float(100f, 0f, _getFilledDuration, r =>
                {

                    if (_currentBlendWeightIndex == 0) // meaning first iteration
                    {
                        if (r < 50)
                            r = 50;
                        _skinnedMeshRenderer.SetBlendShapeWeight(4, r);
                    }
                    else if (_currentBlendWeightIndex == _fillCount - 1) // meaning last iteration
                    {
                        _skinnedMeshRenderer.SetBlendShapeWeight(index, r);

                        if (r > 50)
                            r = 50;
                        _skinnedMeshRenderer.SetBlendShapeWeight(4, r);
                    }
                    else
                        _skinnedMeshRenderer.SetBlendShapeWeight(index, r);

                    UpdateMeshCollider();
                })).OnComplete(() =>
                {
                    DeleteGetFilledSequence();
                });
            }
        }
        private void DeleteGetFilledSequence()
        {
            DOTween.Kill(_getFilledSequenceID);
            _getFilledSequence = null;
        }

        private void StartGetPiledSequence(int index)
        {
            _getPiledSequence.Pause();
            DeleteGetPiledSequence();
            CreateGetPiledSequence(index);
            _getPiledSequence.Play();
        }
        private void CreateGetPiledSequence(int index)
        {
            if (_getPiledSequence == null)
            {
                _getPiledSequence = DOTween.Sequence();
                _getPiledSequenceID = Guid.NewGuid();
                _getPiledSequence.id = _getPiledSequenceID;

                _getPiledSequence.Append(DOVirtual.Float(0f, 100f, _getPiledDuration, r =>
                {

                    if (_currentBlendWeightIndex == _pileCount - 1) // meaning the first iteration
                    {
                        _skinnedMeshRenderer.SetBlendShapeWeight(index, r); // one before last

                        if (r > 50)
                            r = 50;
                        _skinnedMeshRenderer.SetBlendShapeWeight(index + 1, r); // last blend shape
                    }
                    else if (_currentBlendWeightIndex == 0) // meaning the last iteration
                    {
                        if (r <= 50)
                            r = 50;
                        _skinnedMeshRenderer.SetBlendShapeWeight(4, r);
                    }
                    else
                        _skinnedMeshRenderer.SetBlendShapeWeight(index, r);

                    UpdateMeshCollider();
                })).OnComplete(() =>
                {
                    DeleteGetPiledSequence();
                });
            }
        }
        private void DeleteGetPiledSequence()
        {
            DOTween.Kill(_getPiledSequenceID);
            _getPiledSequence = null;
        }
        #endregion
    }
}
