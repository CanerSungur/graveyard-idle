using UnityEngine;
using ZestCore.Ai;
using ZestGames;
using ZestCore.Utility;

namespace GraveyardIdle
{
    public class CoffinCarrierMovement : MonoBehaviour
    {
        private CoffinCarrier _coffinCarrier;

        private bool _targetReached, _startMovingToCoffinArea, _startMovingToAssignedHandle, _startMovingToWaitingPosition;
        private Coffin _assignedCoffin;

        private readonly float _delay = 2f;
        private float _timer;

        public void Init(CoffinCarrier coffinCarrier)
        {
            _coffinCarrier = coffinCarrier;
            _targetReached = _startMovingToCoffinArea = _startMovingToAssignedHandle = _startMovingToWaitingPosition = false;
            _assignedCoffin = null;
            _timer = _delay;

            _coffinCarrier.OnMoveToCoffinArea += StartMovingToCoffinArea;
            CoffinCarrierEvents.OnSendCarriersToHandles += StartMovingToAssignedCoffinHandle;
            CoffinCarrierEvents.OnReturnToWaitingPosition += StartMovingToWaitingPosition;
        }

        private void OnDisable()
        {
            if (!_coffinCarrier) return;

            _coffinCarrier.OnMoveToCoffinArea -= StartMovingToCoffinArea;
            CoffinCarrierEvents.OnSendCarriersToHandles -= StartMovingToAssignedCoffinHandle;
            CoffinCarrierEvents.OnReturnToWaitingPosition -= StartMovingToWaitingPosition;
        }

        private void Update()
        {
            MoveToCoffinArea();
            MoveToAssignedCoffinHandle();
            MoveToWaitingPosition();
        }

        private void MoveToCoffinArea()
        {
            if (!_startMovingToCoffinArea) return;

            if (!_targetReached)
            {
                if (Operation.IsTargetReached(transform, CoffinArea.CarrierTakeTransforms[_coffinCarrier.Number].position, 0.5f))
                {
                    _targetReached = true;
                    _coffinCarrier.OnIdle?.Invoke();
                    if (!CoffinCarrierManager.CoffinTakeTriggered)
                    {
                        CoffinCarrierManager.CoffinTakeTriggered = true;

                        Delayer.DoActionAfterDelay(this, _delay, () => {
                            // give a coffin and move carrier to its handle position.
                            Coffin coffin = CoffinAreaStackHandler.CoffinsInArea[CoffinAreaStackHandler.CoffinsInArea.Count - 1];
                            coffin.GetThrownToCarriers();
                            CoffinAreaStackHandler.RemoveCoffin(coffin);
                            //CoffinCarrierEvents.OnSendCarriersToHandles?.Invoke(coffin);
                        });
                    }
                }
                else
                {
                    Navigation.MoveTransform(transform, CoffinArea.CarrierTakeTransforms[_coffinCarrier.Number].position);
                    Navigation.LookAtTarget(transform, CoffinArea.CarrierTakeTransforms[_coffinCarrier.Number].position);
                }
            }
        }
        private void MoveToAssignedCoffinHandle()
        {
            if (!_startMovingToAssignedHandle) return;

            if (!_targetReached)
            {
                if (Operation.IsTargetReached(transform, _assignedCoffin.CarrierHandles[_coffinCarrier.Number].transform.position, 0.25f))
                {
                    _targetReached = true;
                    transform.SetParent(_assignedCoffin.CarrierHandles[_coffinCarrier.Number].transform);
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                    _assignedCoffin.OnMoveToAssignedGrave?.Invoke();
                    _coffinCarrier.OnTakeCoffin?.Invoke();
                }
                else
                {
                    Navigation.MoveTransform(transform, _assignedCoffin.CarrierHandles[_coffinCarrier.Number].transform.position);
                    Navigation.LookAtTarget(transform, _assignedCoffin.CarrierHandles[_coffinCarrier.Number].transform.position);
                }
            }
        }
        private void MoveToWaitingPosition()
        {
            if (!_startMovingToWaitingPosition) return;

            if (!_targetReached)
            {
                if (Operation.IsTargetReached(transform, CoffinCarrierManager.WaitTransforms[_coffinCarrier.Number].transform.position, 0.15f))
                {
                    _targetReached = true;
                    _coffinCarrier.OnIdle?.Invoke();
                    CoffinCarrierEvents.OnReadyForDuty?.Invoke();
                    transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                    //if (!CoffinCarrierManager.CoffinThrowTriggered)
                    //{
                    //    CoffinCarrierManager.CoffinThrowTriggered = true;

                    //    Delayer.DoActionAfterDelay(this, _delay, () => {
                    //        // make carriers not busy again.
                            
                    //    });
                    //}
                }
                else
                {
                    Navigation.MoveTransform(transform, CoffinCarrierManager.WaitTransforms[_coffinCarrier.Number].transform.position);
                    Navigation.LookAtTarget(transform, CoffinCarrierManager.WaitTransforms[_coffinCarrier.Number].transform.position);
                }
            }
        }
        private void StartMovingToCoffinArea()
        {
            _targetReached = _startMovingToAssignedHandle = _startMovingToWaitingPosition = false;
            _startMovingToCoffinArea = true;
            _coffinCarrier.OnMove?.Invoke();
        }

        private void StartMovingToAssignedCoffinHandle(Coffin coffin)
        {
            _assignedCoffin = coffin;
            _targetReached = _startMovingToCoffinArea = _startMovingToWaitingPosition = false;
            _startMovingToAssignedHandle = true;
            _coffinCarrier.OnMove?.Invoke();
        }
        private void StartMovingToWaitingPosition()
        {
            _assignedCoffin = null;
            _targetReached = _startMovingToCoffinArea = _startMovingToAssignedHandle = false;
            _startMovingToWaitingPosition = true;
            _coffinCarrier.OnMove?.Invoke();
        }
    }
}
