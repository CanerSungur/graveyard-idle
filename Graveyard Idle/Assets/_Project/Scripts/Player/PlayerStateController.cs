using UnityEngine;
using ZestGames;
using DG.Tweening;
using System;

namespace GraveyardIdle
{
    public class PlayerStateController : MonoBehaviour
    {
        private Transform _meshTransform;
        private readonly Vector3 _defaultRotation = Vector3.zero;
        private readonly Vector3 _carryRotation = new Vector3(0f, 180f, 0f);
        private readonly float _rotationTime = 2f;

        #region SEQUENCE
        private Sequence _changeRotationSequence;
        private Guid _changeRotationSequenceID;
        #endregion

        public void Init(Player player)
        {
            _meshTransform = transform.GetChild(0);
            _meshTransform.localRotation = Quaternion.identity;

            PlayerEvents.OnTakeACoffin += StartCarryingCoffin;
            PlayerEvents.OnDropCoffin += StopCarryingCoffin;
        }

        private void OnDisable()
        {
            PlayerEvents.OnTakeACoffin -= StartCarryingCoffin;
            PlayerEvents.OnDropCoffin -= StopCarryingCoffin;
        }

        private void StartCarryingCoffin()
        {
            //_meshTransform.localRotation = _carryRot;
            StartChangeRotationSequence(_carryRotation);
        }
        private void StopCarryingCoffin()
        {
            StartChangeRotationSequence(_defaultRotation);
            //_meshTransform.localRotation = _defaultRot;
        }
        private void StartChangeRotationSequence(Vector3 rotation)
        {
            _changeRotationSequence.Pause();
            DeleteChangeRotationSequence();
            CreateChangeRotationSequence(rotation);
            _changeRotationSequence.Play();
        }

        #region DOTWEEN FUNCTIONS
        private void CreateChangeRotationSequence(Vector3 rotation)
        {
            if (_changeRotationSequence == null)
            {
                _changeRotationSequence = DOTween.Sequence();
                _changeRotationSequenceID = Guid.NewGuid();
                _changeRotationSequence.id = _changeRotationSequenceID;

                _changeRotationSequence.Append(_meshTransform.DOLocalRotate(rotation, _rotationTime))
                    .OnComplete(DeleteChangeRotationSequence);
            }
        }
        private void DeleteChangeRotationSequence()
        {
            DOTween.Kill(_changeRotationSequenceID);
            _changeRotationSequence = null;
        }
        #endregion
    }
}
