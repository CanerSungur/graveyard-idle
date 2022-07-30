using UnityEngine;
using ZestCore.Ai;
using System;
using ZestGames;

namespace GraveyardIdle
{
    public class Truck : MonoBehaviour
    {
        [Header("-- POINT SETUP --")]
        [SerializeField] private Transform startPoint;
        [SerializeField] private Transform dropPoint;
        [SerializeField] private Transform coffinStackPoint;

        private Coffin _carriedCoffin = null;
        private CoffinArea _activatorCoffinArea = null;

        #region SCRIPT REFERENCES
        private TruckAnimationController _animationController;
        public TruckAnimationController AnimationController => _animationController == null ? _animationController = GetComponent<TruckAnimationController>() : _animationController;
        #endregion

        #region FLAGS
        private bool _initialized,_targetReached, _moving, _hasCoffin = false;
        #endregion

        #region PROPERTIES

        #endregion

        #region EVENTS
        public Action OnMove, OnIdle, OnDrop, OnTurnAround;
        #endregion

        public void Init(CoffinArea coffinArea)
        {
            _initialized = true;
            _activatorCoffinArea = coffinArea;
            transform.position = startPoint.position;

            _targetReached = _moving = _hasCoffin = false;

            AnimationController.Init(this);

            SpawnCoffin();
        }

        private void OnEnable()
        {
            _initialized = false;
        }

        private void Update()
        {
            if (!_targetReached && _initialized)
            {
                MoveToDropPoint();
            }
        }

        private void MoveToDropPoint()
        {
            Navigation.MoveTransform(transform, dropPoint.position, 5f);
            Navigation.LookAtTarget(transform, dropPoint.position);

            if (!_moving)
            {
                _moving = true;
                OnMove?.Invoke();
            }

            if (Operation.IsTargetReached(transform, dropPoint.position))
            {
                _targetReached = true;
                _moving = false;
                OnIdle?.Invoke();
                OnDrop?.Invoke();
            }
        }
        private void TurnAround()
        {

        }
        private void SpawnCoffin()
        {
            if (_hasCoffin) return;

            _carriedCoffin = ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.Coffin, coffinStackPoint.position, Quaternion.identity).GetComponent<Coffin>();
            _carriedCoffin.Init(this);
            _carriedCoffin.transform.SetParent(coffinStackPoint);
            _carriedCoffin.transform.localPosition = Vector3.zero;
            _carriedCoffin.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            _hasCoffin = true;
        }

        #region PUBLICS
        public void DropCarriedCoffin()
        {
            if (!_carriedCoffin || !_activatorCoffinArea) return;

            _carriedCoffin.GetStacked(_activatorCoffinArea.StackHandler.TargetStackPosition, _activatorCoffinArea.StackHandler.StackContainer);
            CoffinAreaStackHandler.AddCoffin(_carriedCoffin);
            _hasCoffin = false;
            _activatorCoffinArea = null;
            _carriedCoffin = null;
        }
        #endregion
    }
}
