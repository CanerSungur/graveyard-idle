using UnityEngine;
using DG.Tweening;
using System;
using ZestGames;
using ZestCore.Utility;

namespace GraveyardIdle
{
    public class Log : MonoBehaviour
    {
        private CoffinMachine _coffinMachine;

        private Rigidbody _rigidbody;
        private Vector3 _defaultPosition;

        #region SEQUENCE
        private Sequence _moveSequence;
        private Guid _moveSequenceID;
        #endregion

        public void Init(CoffinMachine coffinMachine)
        {
            if (!_rigidbody)
            {
                _coffinMachine = coffinMachine;
                _rigidbody = GetComponent<Rigidbody>();
                _defaultPosition = transform.localPosition;
            }

            transform.localPosition = _defaultPosition;

            //_coffinMachine.OnStartMachine += StartMoveSequence;
            CoffinMachineEvents.OnStartMakingACoffin += StartMoveSequence;
        }

        private void OnDisable()
        {
            if (!_coffinMachine) return;

            CoffinMachineEvents.OnStartMakingACoffin -= StartMoveSequence;
        }

        #region DOTWEEN FUNCTIONS
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

                _moveSequence.Append(transform.DOLocalMove(_coffinMachine.MiddlePoint.localPosition, _coffinMachine.CoffinMakeTime * 0.5f)).OnComplete(() => {
                    DeleteMoveSequence();
                    gameObject.SetActive(false);
                    _coffinMachine.SpawnEmptyCoffin();
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
