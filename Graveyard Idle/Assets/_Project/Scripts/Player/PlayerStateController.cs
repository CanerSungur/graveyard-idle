using UnityEngine;
using ZestGames;
using DG.Tweening;
using System;

namespace GraveyardIdle
{
    public class PlayerStateController : MonoBehaviour
    {
        private Player _player;

        private Transform _meshTransform;
        private readonly Vector3 _defaultRotation = Vector3.zero;
        private readonly Vector3 _carryRotation = new Vector3(0f, 180f, 0f);
        private readonly Vector3 _digRotation = Vector3.zero;
        private readonly Vector3 _fillRotation = new Vector3(0f, 180f, 0f);
        private readonly float _rotationTime = 2f;

        #region SEQUENCE
        private Sequence _changeRotationSequence;
        private Guid _changeRotationSequenceID;
        #endregion

        private bool _activateRotation = false;

        public void Init(Player player)
        {
            _player = player;

            _meshTransform = transform.GetChild(0);
            _meshTransform.localRotation = Quaternion.identity;
            _activateRotation = false;

            PlayerEvents.OnTakeACoffin += StartCarryingCoffin;
            PlayerEvents.OnDropCoffin += StopCarryingCoffin;
            PlayerEvents.OnStartDigging += StartedDigging;
            PlayerEvents.OnStopDigging += StoppedDigging;
            PlayerEvents.OnStartFilling += StartedFilling;
            PlayerEvents.OnStopFilling += StoppedFilling;
        }

        private void OnDisable()
        {
            PlayerEvents.OnTakeACoffin -= StartCarryingCoffin;
            PlayerEvents.OnDropCoffin -= StopCarryingCoffin;
            PlayerEvents.OnStartDigging -= StartedDigging;
            PlayerEvents.OnStopDigging -= StoppedDigging;
            PlayerEvents.OnStartFilling -= StartedFilling;
            PlayerEvents.OnStopFilling -= StoppedFilling;
        }

        private void Update()
        {
            if (!_activateRotation && !_player.IsCarryingCoffin)
            {
                _meshTransform.localRotation = Quaternion.identity;
                return;
            }

            if (_player.IsDigging)
                _meshTransform.rotation = Quaternion.Lerp(_meshTransform.rotation, Quaternion.identity, 2f * Time.deltaTime);
            else if (_player.IsFilling)
                _meshTransform.rotation = Quaternion.Lerp(_meshTransform.rotation, Quaternion.Euler(0f, 180f, 0f), 2f * Time.deltaTime);
        }

        private void StartedDigging() => _activateRotation = true;
        private void StoppedDigging() => _activateRotation = false;
        private void StartedFilling() => _activateRotation = true;
        private void StoppedFilling() => _activateRotation = false;
        private void StartCarryingCoffin() => StartChangeRotationSequence(_carryRotation);
        private void StopCarryingCoffin(Coffin ignore, InteractableGround ignoreAlso) => StartChangeRotationSequence(_defaultRotation);
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
