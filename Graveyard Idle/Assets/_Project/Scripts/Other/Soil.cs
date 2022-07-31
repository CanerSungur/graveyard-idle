using UnityEngine;
using System;
using DG.Tweening;
using ZestGames;

namespace GraveyardIdle
{
    public class Soil : MonoBehaviour
    {
        private bool _initialized = false;
        private InteractableGround _interactableGround;

        private SkinnedMeshRenderer _skinnedMeshRenderer;
        private Mesh _changedMesh;
        private MeshCollider _meshCollider;

        private bool _playerIsInArea = false;
        private int _digCount = 5;
        private int _currentDigCount = 0;
        private readonly float _getDiggedDuration = 1f;

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

        #region POSITION
        private readonly float _defaultHeight = 0f;
        private readonly float _diggedHeight = -0.2f;
        #endregion

        #region SEQUENCE
        private Sequence _getDiggedSequence, _getFilledSequence;
        private Guid _getDiggedSequenceID, _getFilledSequenceID;
        #endregion

        public void Init(InteractableGround interactableGround)
        {
            _initialized = true;
            _interactableGround = interactableGround;

            _skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
            _skinnedMeshRenderer.SetBlendShapeWeight(0, 0f);
            _meshCollider = GetComponent<MeshCollider>();

            _playerIsInArea = false;
            transform.localPosition = Vector3.zero;

            ShovelEvents.OnDigHappened += GetDigged;
        }

        private void OnDisable()
        {
            if (!_initialized) return;
            ShovelEvents.OnDigHappened -= GetDigged;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player) && !_playerIsInArea && _currentDigCount < _digCount && _interactableGround.CanBeDigged)
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

        //private void OnCollisionEnter(Collision collision)
        //{
        //    if (collision.gameObject.TryGetComponent(out Player player) && !_playerIsInArea && _currentDigCount < _digCount)
        //    {
        //        _playerIsInArea = true;
        //        PlayerEvents.OnEnteredDigZone?.Invoke();
        //    }
        //}

        //private void OnCollisionExit(Collision collision)
        //{
        //    if (collision.gameObject.TryGetComponent(out Player player) && _playerIsInArea)
        //    {
        //        _playerIsInArea = false;
        //        PlayerEvents.OnExitedDigZone?.Invoke();
        //    }
        //}

        private void GetDigged()
        {
            if (!_playerIsInArea || !_interactableGround.CanBeDigged) return;

            _currentBlendWeightIndex = _currentDigCount;
            _currentDigCount++;

            StartGetDiggedSequence(_currentBlendWeightIndex);

            if (_currentDigCount == _digCount)
            {
                PlayerEvents.OnExitedDigZone?.Invoke();
                PlayerEvents.OnStopDigging?.Invoke();
                _interactableGround.CanBeDigged = false;
                _interactableGround.CanBeFilled = true;
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
        private void StartGetDiggedSequence(int index)
        {
            _getDiggedSequence.Pause();
            DeleteGetDiggedSequence();
            CreateGetDiggedSequence(index);
            _getDiggedSequence.Play();

            _interactableGround.SoilPile.GetPiled();
        }
        private void CreateGetDiggedSequence(int index)
        {
            if (_getDiggedSequence == null)
            {
                _getDiggedSequence = DOTween.Sequence();
                _getDiggedSequenceID = Guid.NewGuid();
                _getDiggedSequence.id = _getDiggedSequenceID;

                _getDiggedSequence.Append(DOVirtual.Float(0f, 100f, _getDiggedDuration,r => {
                    _skinnedMeshRenderer.SetBlendShapeWeight(index, r);
                    UpdateMeshCollider();
                }))
                    .Join(DOVirtual.Float(_defaultHeight, _diggedHeight, _getDiggedDuration, r => {
                        if (_currentBlendWeightIndex == _digCount - 1)
                            transform.localPosition = new Vector3(0f, r, 0f);
                    }))
                    .OnComplete(() => {
                    DeleteGetDiggedSequence();
                });
            }
        }
        private void DeleteGetDiggedSequence()
        {
            DOTween.Kill(_getDiggedSequenceID);
            _getDiggedSequence = null;
        }

        private void StartGetFilledSequence(int index)
        {
            _getFilledSequence.Pause();
            DeleteGetFilledSequence();
            CreateGetFilledSequence(index);
            _getFilledSequence.Play();
        }
        private void CreateGetFilledSequence(int index)
        {
            if (_getFilledSequence == null)
            {
                _getFilledSequence = DOTween.Sequence();
                _getFilledSequenceID = Guid.NewGuid();
                _getFilledSequence.id = _getFilledSequenceID;

                _getFilledSequence.Append(DOVirtual.Float(100f, 0f, _getFilledDuration, r => {
                    _skinnedMeshRenderer.SetBlendShapeWeight(index, r);
                    UpdateMeshCollider();
                }))
                    .Join(DOVirtual.Float(_diggedHeight, _defaultHeight, _getFilledDuration, r =>
                    {
                        if (_currentBlendWeightIndex == _fillCount - 1)
                            transform.localPosition = new Vector3(0f, r, 0f);
                    }))
                    .OnComplete(() => {
                        DeleteGetFilledSequence();
                    });
            }
        }
        private void DeleteGetFilledSequence()
        {
            DOTween.Kill(_getFilledSequenceID);
            _getFilledSequence = null;
        }
        #endregion

        #region PUBLICS
        public void GetFilled()
        {
            _currentFillCount++;
            _currentBlendWeightIndex = _fillCount - _currentFillCount;

            StartGetFilledSequence(_currentBlendWeightIndex);
        }
        #endregion
    }
}
