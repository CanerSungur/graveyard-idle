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
        private int _fillCount = 5;
        private int _currentFillCount = 0;
        private readonly float _getFilledDuration = 1f;

        #region BLEND SHAPE INDEX
        private readonly int _blendWeightIndex_1 = 0;
        private readonly int _blendWeightIndex_2 = 1;
        private readonly int _blendWeightIndex_3 = 2;
        private readonly int _blendWeightIndex_4 = 3;
        private readonly int _blendWeightIndex_5 = 4;
        private int _currentBlendWeightIndex = -1;
        #endregion

        #region PILED VALUES
        private int _pileCount = 5;
        private int _currentPileCount = 0;
        private readonly float _getPiledDuration = 1f;
        #endregion

        #region POSITION
        private readonly float _defaultHeight = 0f;
        private readonly float _diggedHeight = -0.2f;
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

            ShovelEvents.OnDigHappened += GetFilled;
        }

        private void OnDisable()
        {
            if (!_initialized) return;

            ShovelEvents.OnDigHappened += GetFilled;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player) && !_playerIsInArea && _interactableGround.CanBeFilled)
            {
                _playerIsInArea = true;
                PlayerEvents.OnEnteredDigZone?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Player player) && _playerIsInArea)
            {
                _playerIsInArea = false;
                PlayerEvents.OnExitedDigZone?.Invoke();
            }
        }

        private void GetFilled()
        {
            if (!_playerIsInArea || !_interactableGround.CanBeFilled) return;

            _currentBlendWeightIndex = _currentFillCount;
            _currentFillCount++;

            StartGetFilledSequence(_currentBlendWeightIndex -1);

            if (_currentFillCount == _fillCount)
            {
                PlayerEvents.OnExitedDigZone?.Invoke();
                PlayerEvents.OnStopDigging?.Invoke();
                _interactableGround.CanBeFilled = false;

                Debug.Log("GRAVE IS FINISHED");
                _interactableGround.OnGraveBuilt?.Invoke();
            }
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

            _interactableGround.DiggableSoil.GetFilled();
        }
        private void CreateGetFilledSequence(int index)
        {
            if (_getFilledSequence == null)
            {
                _getFilledSequence = DOTween.Sequence();
                _getFilledSequenceID = Guid.NewGuid();
                _getFilledSequence.id = _getFilledSequenceID;

                _getFilledSequence.Append(DOVirtual.Float(100f, 0f, _getFilledDuration, r => {

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
                })).OnComplete(() => {
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

                _getPiledSequence.Append(DOVirtual.Float(0f, 100f, _getPiledDuration, r => {
                    
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
                })).OnComplete(() => {
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

        #region PUBLICS
        public void GetPiled()
        {
            _currentPileCount++;
            _currentBlendWeightIndex = _pileCount - _currentPileCount;

            StartGetPiledSequence(_currentBlendWeightIndex - 1);
        }
        #endregion
    }
}
