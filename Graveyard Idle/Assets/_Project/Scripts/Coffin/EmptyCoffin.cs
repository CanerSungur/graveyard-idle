using UnityEngine;
using DG.Tweening;
using System;
using ZestGames;

namespace GraveyardIdle
{
    public class EmptyCoffin : MonoBehaviour
    {
        private CoffinMachine _coffinMachine;

        #region STACK DATA
        private bool _stacked = false;
        private readonly float _stackJumpHeight = 4f;
        private readonly float _graveJumpHeight = 2f;
        private readonly float _animationTime = 1.5f;
        #endregion

        #region SEQUENCE
        private Sequence _jumpSequence, _moveSequence;
        private Guid _jumpSequenceID, _moveSequenceID;
        #endregion

        public void Init(CoffinMachine coffinMachine)
        {
            // init for first spawn
            _coffinMachine = coffinMachine;
            transform.position = _coffinMachine.MiddlePoint.position;
            StartMoveSequence();
        }

        #region PUBLICS
        public void GetStacked(Vector3 position, Transform parent)
        {
            CoffinMachineStackHandler.AddEmptyCoffin(this);
            transform.SetParent(parent);

            StartJumpSequence(position, new Vector3(0f, 180f, 0f), _stackJumpHeight, _animationTime);

            _stacked = true;

            CoffinMachineEvents.OnStackedEmptyCoffin?.Invoke();
        }
        public void GoToTruck(Transform parent)
        {
            //IsBeingCarried = true;
            transform.SetParent(parent);

            StartJumpSequence(Vector3.zero, new Vector3(0f, 180f, 0f), _stackJumpHeight, _animationTime * 0.5f);

            CoffinMachineEvents.OnUnStackedEmptyCoffin?.Invoke();
            //GraveManagerEvents.OnCoffinPickedUp?.Invoke();
        }
        #endregion

        #region DOTWEEN FUNCTIONS
        private void StartJumpSequence(Vector3 jumpPosition, Vector3 rotation, float jumpHeight, float animationTime, Action action = null)
        {
            _jumpSequence.Pause();
            DeleteJumpSequence();
            CreateJumpSequence(jumpPosition, rotation, jumpHeight, animationTime, action);
            _jumpSequence.Play();
        }
        private void CreateJumpSequence(Vector3 jumpPosition, Vector3 rotation, float jumpHeight, float animationTime, Action action = null)
        {
            if (_jumpSequence == null)
            {
                _jumpSequence = DOTween.Sequence();
                _jumpSequenceID = Guid.NewGuid();
                _jumpSequence.id = _jumpSequenceID;

                _jumpSequence.Append(transform.DOLocalJump(jumpPosition, jumpHeight, 1, animationTime))
                    .Join(transform.DOLocalRotate(rotation, _animationTime))
                    .Join(transform.DOShakeScale(_animationTime, 0.5f))
                    .OnComplete(() =>
                    {
                        DeleteJumpSequence();
                        action?.Invoke();
                    });
            }
        }
        private void DeleteJumpSequence()
        {
            DOTween.Kill(_jumpSequenceID);
            _jumpSequence = null;
        }

        private void StartMoveSequence()
        {
            CreateMoveSequence();
            _moveSequence.Play();
        }
        private void CreateMoveSequence()
        {
            if (_moveSequence == null)
            {
                _moveSequence = DOTween.Sequence();
                _moveSequenceID = Guid.NewGuid();
                _moveSequence.id = _moveSequenceID;

                _moveSequence.Append(transform.DOMove(_coffinMachine.EndPoint.position, _coffinMachine.CoffinMakeTime * 0.5f)).OnComplete(() => {
                    DeleteMoveSequence();
                    GetStacked(_coffinMachine.StackHandler.TargetStackPosition, _coffinMachine.StackHandler.StackContainer);
                });
            }
        }
        private void DeleteMoveSequence()
        {
            DOTween.Kill(_moveSequenceID);
            _moveSequence = null;
        }
        #endregion
    }
}
