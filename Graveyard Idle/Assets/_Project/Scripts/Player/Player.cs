using System;
using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class Player : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private Transform carryTransform;

        #region COMPONENTS
        private Collider _collider;
        public Collider Collider => _collider == null ? _collider = GetComponent<Collider>() : _collider;
        #endregion

        #region SCRIPT REFERENCES
        private JoystickInput _inputHandler;
        public JoystickInput InputHandler => _inputHandler == null ? _inputHandler = GetComponent<JoystickInput>() : _inputHandler;
        private PlayerMovement _movement;
        public PlayerMovement Movement => _movement == null ? _movement = GetComponent<PlayerMovement>() : _movement;
        private PlayerAnimationController _animationController;
        public PlayerAnimationController AnimationController => _animationController == null ? _animationController = GetComponent<PlayerAnimationController>() : _animationController;
        private PlayerCollision _collisionHandler;
        public PlayerCollision CollisionHandler => _collisionHandler == null ? _collisionHandler = GetComponent<PlayerCollision>() : _collisionHandler;
        private PlayerStateController _stateController;
        public PlayerStateController StateController => _stateController == null ? _stateController = GetComponent<PlayerStateController>() : _stateController;
        #endregion

        #region PROPERTIES
        public bool IsCarryingCoffin { get; private set; }
        public Transform CarryTransform => carryTransform;
        #endregion

        private void Start()
        {
            IsCarryingCoffin = false;

            InputHandler.Init(this);
            Movement.Init(this);
            AnimationController.Init(this);
            CollisionHandler.Init(this);
            StateController.Init(this);

            PlayerEvents.OnTakeACoffin += TakeACoffin;
            PlayerEvents.OnDropCoffin += DropCoffin;
        }

        private void OnDisable()
        {
            PlayerEvents.OnTakeACoffin -= TakeACoffin;
            PlayerEvents.OnDropCoffin -= DropCoffin;
        }

        private void TakeACoffin()
        {
            IsCarryingCoffin = true;
        }
        private void DropCoffin()
        {
            IsCarryingCoffin = false;
        }
    }
}
