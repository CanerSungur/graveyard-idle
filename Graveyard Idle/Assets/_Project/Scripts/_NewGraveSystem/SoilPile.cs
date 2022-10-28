using DG.Tweening;
using UnityEngine;
using System;
using ZestCore.Utility;

namespace GraveyardIdle.GraveSystem
{
    public class SoilPile : MonoBehaviour
    {
        #region COMPONENTS
        private SoilDiggable _soilDiggable;
        private SoilFillable _soilFillable;
        private SkinnedMeshRenderer _skinnedMeshRenderer;
        private MeshCollider _meshCollider;
        private Mesh _changedMesh;
        #endregion

        #region SEQUENCE
        private Sequence _changeShapeForDigSequence, _changeShapeForFillSequence;
        private Guid _changeShapeForDigSequenceID, _changeShapeForFillSequenceID;
        private const float HEIGHT_INCREASE_VALUE_FOR_DIG = 0.11f;
        private const float DEFAULT_HEIGHT_FOR_DIG = 0.2f;
        private const float HEIGHT_INCREASE_VALUE_FOR_FILL = 0.06f;
        private const float DEFAULT_HEIGHT_FOR_FILL = -0.51f;
        #endregion

        #region FILLING
        private const int MAX_FILL_COUNT = 5;
        #endregion

        #region DIGGING

        #endregion

        public void Init(SoilDiggable soilDiggable)
        {
            if (_soilDiggable == null)
            {
                _soilDiggable = soilDiggable;
                _skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
                _meshCollider = GetComponent<MeshCollider>();
            }
        }
        public void Init(SoilFillable soilFillable)
        {
            if (_soilFillable == null)
            {
                _soilFillable = soilFillable;
                _skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
                _meshCollider = GetComponent<MeshCollider>();
            }
        }

        private void UpdateMeshCollider()
        {
            _changedMesh = new Mesh();
            _skinnedMeshRenderer.BakeMesh(_changedMesh);
            _meshCollider.sharedMesh = null;
            _meshCollider.sharedMesh = _changedMesh;
        }

        #region PUBLICS
        public void UpdateForDigging(int digCount)
        {
            Delayer.DoActionAfterDelay(this, _soilDiggable.Grave.PileUpdateDelay,() => StartChangeShapeForDigSequence(digCount));
        }
        public void UpdateForFilling(int fillCount)
        {
            Delayer.DoActionAfterDelay(this, _soilFillable.Grave.PileUpdateDelay,() => StartChangeShapeForFillSequence(fillCount));
        }
        public void UpdateShapeForDig(int digCount)
        {
            for (int i = 0; i < 5; i++)
            {
                if (i <= digCount - 1)
                    _skinnedMeshRenderer.SetBlendShapeWeight(i, 100f);
                else
                    _skinnedMeshRenderer.SetBlendShapeWeight(i, 0f);
            }

            transform.localPosition = new Vector3(transform.localPosition.x, DEFAULT_HEIGHT_FOR_DIG + HEIGHT_INCREASE_VALUE_FOR_DIG * (digCount), transform.localPosition.z);
            UpdateMeshCollider();
        }
        public void UpdateShapeForFill(int fillCount)
        {
            for (int i = 0; i < 5; i++)
            {
                if (i <= fillCount - 1)
                    _skinnedMeshRenderer.SetBlendShapeWeight(MAX_FILL_COUNT - i - 1, 0f);
                else
                    _skinnedMeshRenderer.SetBlendShapeWeight(MAX_FILL_COUNT - i - 1, 100f);
            }

            transform.localPosition = new Vector3(transform.localPosition.x, DEFAULT_HEIGHT_FOR_FILL + HEIGHT_INCREASE_VALUE_FOR_FILL * (fillCount), transform.localPosition.z);
            UpdateMeshCollider();
        }
        #endregion

        #region DOTWEEN FUNCTIONS
        private void StartChangeShapeForDigSequence(int digCount)
        {
            _changeShapeForDigSequence.Pause();
            DeleteChangeShapeForDigSequence();
            CreateChangeShapeForDigSequence(digCount);
            _changeShapeForDigSequence.Play();
        }
        private void CreateChangeShapeForDigSequence(int digCount)
        {
            if (_changeShapeForDigSequence == null)
            {
                _changeShapeForDigSequence = DOTween.Sequence();
                _changeShapeForDigSequenceID = Guid.NewGuid();
                _changeShapeForDigSequence.id = _changeShapeForDigSequenceID;

                _changeShapeForDigSequence.Append(DOVirtual.Float(0f, 100f, _soilDiggable.DigDuration, r => {
                    _skinnedMeshRenderer.SetBlendShapeWeight(digCount, r);
                    UpdateMeshCollider();
                }))
                    .Join(transform.DOLocalMoveY(DEFAULT_HEIGHT_FOR_DIG + HEIGHT_INCREASE_VALUE_FOR_DIG * (digCount + 1), _soilDiggable.DigDuration)).OnComplete(() => DeleteChangeShapeForDigSequence());
            }
        }
        private void DeleteChangeShapeForDigSequence()
        {
            DOTween.Kill(_changeShapeForDigSequenceID);
            _changeShapeForDigSequence = null;
        }
        // ########################
        private void StartChangeShapeForFillSequence(int fillCount)
        {
            _changeShapeForFillSequence.Pause();
            DeleteChangeShapeForFillSequence();
            CreateChangeShapeForFillSequence(fillCount);
            _changeShapeForFillSequence.Play();
        }
        private void CreateChangeShapeForFillSequence(int fillCount)
        {
            if (_changeShapeForFillSequence == null)
            {
                _changeShapeForFillSequence = DOTween.Sequence();
                _changeShapeForFillSequenceID = Guid.NewGuid();
                _changeShapeForFillSequence.id = _changeShapeForFillSequenceID;

                _changeShapeForFillSequence.Append(DOVirtual.Float(100f, 0f, _soilFillable.FillDuration, r => {
                    _skinnedMeshRenderer.SetBlendShapeWeight(MAX_FILL_COUNT - fillCount - 1, r);
                    UpdateMeshCollider();
                }))
                    .Join(transform.DOLocalMoveY(DEFAULT_HEIGHT_FOR_FILL + HEIGHT_INCREASE_VALUE_FOR_FILL * (fillCount + 1), _soilFillable.FillDuration)).OnComplete(() => DeleteChangeShapeForFillSequence());
            }
        }
        private void DeleteChangeShapeForFillSequence()
        {
            DOTween.Kill(_changeShapeForFillSequenceID);
            _changeShapeForFillSequence = null;
        }
        #endregion
    }
}
