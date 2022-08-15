using UnityEngine;
using System;
using ZestGames;

namespace GraveyardIdle
{
    public class CoffinCarrier : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private int number = 1;

        public int Number => number;

        #region SCRIPT REFERENCES
        private CoffinCarrierAnimationController _animationController;
        public CoffinCarrierAnimationController AnimationController => _animationController == null ? _animationController = GetComponent<CoffinCarrierAnimationController>() : _animationController;
        private CoffinCarrierMovement _movementHandler;
        public CoffinCarrierMovement MovementHandler => _movementHandler == null ? _movementHandler = GetComponent<CoffinCarrierMovement>() : _movementHandler;
        #endregion

        #region EVENTS
        public Action OnMoveToCoffinArea, OnMove, OnIdle, OnTakeCoffin, OnWaitForDuty;
        #endregion

        private void OnEnable()
        {
            MovementHandler.Init(this);
            AnimationController.Init(this);

            CoffinCarrierEvents.OnLeaveCoffin += HandleLeaveCoffin;
        }

        private void OnDisable()
        {
            CoffinCarrierEvents.OnLeaveCoffin -= HandleLeaveCoffin;
        }

        private void HandleLeaveCoffin()
        {
            transform.SetParent(null);
        }
    }
}
