using System;
using UnityEngine;
using ZestCore.Ai;
using ZestGames;
using ZestCore.Utility;

namespace GraveyardIdle
{
    public class CoffinMakerMovement : MonoBehaviour
    {
        private CoffinMaker _coffinMaker;

        [Header("-- SETUP --")]
        [SerializeField] private float speed = 3f;
        [SerializeField] private Transform machineTransform;
        [SerializeField] private Transform counterTransform;

        private bool _targetReached, _goToMachine, _goToCounter;

        public void Init(CoffinMaker coffinMaker)
        {
            _coffinMaker = coffinMaker;
            _targetReached = _goToMachine = _goToCounter = false;

            CoffinMakerEvents.OnGoToMachine += GoToMachine;
            CoffinMakerEvents.OnGoToCounter += GoToCounter;
            CoffinMakerEvents.OnStartWaiting += StartWaiting;
        }

        private void OnDisable()
        {
            if (!_coffinMaker) return;

            CoffinMakerEvents.OnGoToMachine -= GoToMachine;
            CoffinMakerEvents.OnGoToCounter -= GoToCounter;
            CoffinMakerEvents.OnStartWaiting -= StartWaiting;
        }

        private void Update()
        {
            if (_goToMachine)
                MoveToTarget(machineTransform.position, () => {
                    CoffinMakerEvents.OnPushTheButton?.Invoke();
                    CoffinMachineEvents.OnStartMakingACoffin?.Invoke();
                    Delayer.DoActionAfterDelay(this, 2f, () => CoffinMakerEvents.OnGoToCounter?.Invoke());
                });

            if (_goToCounter)
                MoveToTarget(counterTransform.position, () => CoffinMakerEvents.OnStartWaiting?.Invoke());
        }

        private void GoToMachine()
        {
            _goToMachine = true;
            _goToCounter = _targetReached = false;
        }
        private void GoToCounter()
        {
            _goToCounter = true;
            _goToMachine = _targetReached = false;
        }
        private void StartWaiting()
        {
            _goToCounter = _goToMachine = _targetReached = false;
        }
        private void MoveToTarget(Vector3 targetPosition, Action action = null)
        {
            if (!_targetReached)
            {
                if (Operation.IsTargetReached(transform, targetPosition, .75f))
                {
                    _targetReached = true;
                    action?.Invoke();
                }
                else
                {
                    Navigation.MoveTransform(transform, targetPosition, speed);
                    Navigation.LookAtTarget(transform, targetPosition);
                }
            }
        }
    }
}
