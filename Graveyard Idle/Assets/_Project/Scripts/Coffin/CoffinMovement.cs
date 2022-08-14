using UnityEngine;
using ZestCore.Ai;
using DG.Tweening;
using System;
using ZestGames;
using ZestCore.Utility;

namespace GraveyardIdle
{
    public class CoffinMovement : MonoBehaviour
    {
        private Coffin _coffin;

        private bool _targetReached, _startMovingToAssignedGrave;
        private Grave _graveToBeDropped;

        #region SEQUENCE
        private Sequence _getThrownSequence, _rotateSequence;
        private Guid _getThrownSequenceID, _rotateSequenceID;
        #endregion

        public void Init(Coffin coffin)
        {
            _coffin = coffin;
            _targetReached = false;
            _graveToBeDropped = null;

            _coffin.OnMoveToAssignedGrave += StartMovingToAssignedGrave;
        }

        private void OnDisable()
        {
            if (!_coffin) return;

            _coffin.OnMoveToAssignedGrave -= StartMovingToAssignedGrave;
        }

        private void Update()
        {
            if (!_startMovingToAssignedGrave) return;

            if (!_targetReached)
            {
                if (Operation.IsTargetReached(transform, _graveToBeDropped.CarrierDropPoint.position, .75f))
                {
                    _targetReached = true;
                    //action?.Invoke();
                    Debug.Log("Throw coffin to grave");
                    StartRotateSequence();
                }
                else
                {
                    Navigation.MoveTransform(transform, _graveToBeDropped.CarrierDropPoint.position, 1f);
                    Navigation.LookAtTarget(transform, _graveToBeDropped.CarrierDropPoint.position);
                }
            }
        }

        private void StartMovingToAssignedGrave()
        {
            _targetReached = false;
            _startMovingToAssignedGrave = true;

            _graveToBeDropped = GraveManager.EmptyGraves[GraveManager.EmptyGraves.Count - 1];
            _graveToBeDropped.CarriersAssigned = true;
        }

        #region DOTWEEN FUNCTIONS
        private void StartGetThrownSequence()
        {
            CreateGetThrownSequence();
            _getThrownSequence.Play();
        }
        private void CreateGetThrownSequence()
        {
            if (_getThrownSequence == null)
            {
                _getThrownSequence = DOTween.Sequence();
                _getThrownSequenceID = Guid.NewGuid();
                _getThrownSequence.id = _getThrownSequenceID;

                _getThrownSequence.Append(transform.DOMove(transform.position - (transform.forward * 0.7f), 1f)).OnComplete(() =>
                    {
                        DeleteGetThrownSequence();
                        Delayer.DoActionAfterDelay(this, 1f, () => CoffinCarrierEvents.OnReturnToWaitingPosition?.Invoke());
                        _coffin.GetThrownToGraveByCarriers(_graveToBeDropped.transform);

                        _graveToBeDropped.InteractableGround.HasCoffin = true;
                        _graveToBeDropped.InteractableGround.CanBeThrownCoffin = false;
                        GraveManagerEvents.OnCoffinThrownToGrave?.Invoke();
                        GraveManager.RemoveEmptyGrave(_graveToBeDropped);
                    });
            }
        }
        private void DeleteGetThrownSequence()
        {
            DOTween.Kill(_getThrownSequenceID);
            _getThrownSequence = null;
        }

        private void StartRotateSequence()
        {
            CreateRotateSequence();
            _rotateSequence.Play();
        }
        private void CreateRotateSequence()
        {
            if (_rotateSequence == null)
            {
                _rotateSequence = DOTween.Sequence();
                _rotateSequenceID = Guid.NewGuid();
                _rotateSequence.id = _rotateSequenceID;

                _rotateSequence.Append(transform.DORotate(new Vector3(0f, 180f, 0f), 1f)).OnComplete(() => {
                    DeleteRotateSequence();
                    CoffinCarrierEvents.OnLeaveCoffin?.Invoke();
                    StartGetThrownSequence();
                });
            }
        }
        private void DeleteRotateSequence() 
        {
            DOTween.Kill(_rotateSequenceID);
            _rotateSequence = null;
        }
        #endregion
    }
}
