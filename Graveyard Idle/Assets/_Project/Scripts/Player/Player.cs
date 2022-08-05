using System;
using DG.Tweening;
using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class Player : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private Transform carryTransform;

        #region CARRY TRANSFORM RELATED
        private readonly Vector3 _carryTransformDefaultPosition = new Vector3(0.03f, 0.617f, -1.54f);
        private readonly Vector3 _carryTransformCloserPosition = new Vector3(0.03f, 0.617f, -1.41f);
        private readonly float _carryTransformPositionChangeSpeed = 1f;
        private Sequence _changeCarryTransformSequence;
        private Guid _changeCarryTransformSequenceID;
        #endregion

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
        private Shovel _shovel;
        public Shovel Shovel => _shovel == null ? _shovel = GetComponentInChildren<Shovel>() : _shovel;
        private PlayerCoffinThrower _coffinThrower;
        public PlayerCoffinThrower CoffinThrower => _coffinThrower == null ? _coffinThrower = GetComponent<PlayerCoffinThrower>() : _coffinThrower;
        private PlayerMoneyHandler _moneyHandler;
        public PlayerMoneyHandler MoneyHandler => _moneyHandler == null ? _moneyHandler = GetComponent<PlayerMoneyHandler>() : _moneyHandler;
        private PlayerTimerHandler _timerHandler;
        public PlayerTimerHandler TimerHandler => _timerHandler == null ? _timerHandler = GetComponent<PlayerTimerHandler>() : _timerHandler;
        #endregion

        #region PROPERTIES
        public bool IsCarryingCoffin { get; private set; }
        public bool IsDigging { get; private set; }
        public bool IsFilling { get; private set; }
        public bool IsInDigZone { get; private set; }
        public bool IsInFillZone { get; private set; }
        public Coffin CoffinCarryingNow { get; private set; }
        public InteractableGround EnteredInteractableGround { get; set; }
        #endregion

        #region GETTERS
        public Transform CarryTransform => carryTransform;
        #endregion

        private void Start()
        {
            IsCarryingCoffin = IsDigging = IsFilling = IsInDigZone = IsInFillZone = false;
            CoffinCarryingNow = null;
            EnteredInteractableGround = null;

            InputHandler.Init(this);
            Movement.Init(this);
            AnimationController.Init(this);
            CollisionHandler.Init(this);
            StateController.Init(this);
            Shovel.Init(this);
            CoffinThrower.Init(this);
            MoneyHandler.Init(this);
            TimerHandler.Init(this);

            PlayerEvents.OnMove += TakeCoffinFurther;
            PlayerEvents.OnIdle += TakeCoffinCloser;
            PlayerEvents.OnTakeACoffin += TakeACoffin;
            PlayerEvents.OnDropCoffin += DropCoffin;
            PlayerEvents.OnSetCarryingCoffin += SetCarryingCoffin;
            PlayerEvents.OnStartDigging += StartDigging;
            PlayerEvents.OnStopDigging += StopDigging;
            PlayerEvents.OnStartFilling += StartFilling;
            PlayerEvents.OnStopFilling += StopFilling;
            PlayerEvents.OnEnteredDigZone += EnteredDigZone;
            PlayerEvents.OnExitedDigZone += ExitedDigZone;
            PlayerEvents.OnEnteredFillZone += EnteredFillZone;
            PlayerEvents.OnExitedFillZone += ExitedFillZone;
        }

        private void OnDisable()
        {
            PlayerEvents.OnMove -= TakeCoffinFurther;
            PlayerEvents.OnIdle -= TakeCoffinCloser;
            PlayerEvents.OnTakeACoffin -= TakeACoffin;
            PlayerEvents.OnDropCoffin -= DropCoffin;
            PlayerEvents.OnSetCarryingCoffin += SetCarryingCoffin;
            PlayerEvents.OnStartDigging -= StartDigging;
            PlayerEvents.OnStopDigging -= StopDigging;
            PlayerEvents.OnStartFilling -= StartFilling;
            PlayerEvents.OnStopFilling -= StopFilling;
            PlayerEvents.OnEnteredDigZone -= EnteredDigZone;
            PlayerEvents.OnExitedDigZone -= ExitedDigZone;
            PlayerEvents.OnEnteredFillZone -= EnteredFillZone;
            PlayerEvents.OnExitedFillZone -= ExitedFillZone;
        }

        private void EnteredDigZone() => IsInDigZone = true;
        private void ExitedDigZone() => IsInDigZone = false;
        private void EnteredFillZone() => IsInFillZone = true;
        private void ExitedFillZone() => IsInFillZone = false;
        private void StartDigging() => IsDigging = true;
        private void StopDigging() => IsDigging = false;
        private void StartFilling() => IsFilling = true;
        private void StopFilling() => IsFilling = false;

        #region COFFIN RELATED FUNCTIONS
        private void TakeCoffinFurther()
        {
            _changeCarryTransformSequence.Pause();
            DeleteChangeCarryTransformSequence();
            StartChangingCarryTransform(_carryTransformDefaultPosition);
        }
        private void TakeCoffinCloser()
        {
            _changeCarryTransformSequence.Pause();
            DeleteChangeCarryTransformSequence();
            StartChangingCarryTransform(_carryTransformCloserPosition);
        }
        private void TakeACoffin()
        {
            IsCarryingCoffin = true;
        }
        private void DropCoffin(Coffin ignore, InteractableGround ignoreAlso)
        {
            IsCarryingCoffin = false;
            //CoffinCarryingNow.GetDropped();
        }
        private void SetCarryingCoffin(Coffin coffin)
        {
            CoffinCarryingNow = coffin;
            AnimationController.SetCoffinIK(CoffinCarryingNow);
        }

        #region DOTWEEN FUNCTIONS
        private void StartChangingCarryTransform(Vector3 position)
        {
            CreateChangeCarryTransformSequence(position);
            _changeCarryTransformSequence.Play();
        }
        private void CreateChangeCarryTransformSequence(Vector3 position)
        {
            if (_changeCarryTransformSequence == null)
            {
                _changeCarryTransformSequence = DOTween.Sequence();
                _changeCarryTransformSequenceID = Guid.NewGuid();
                _changeCarryTransformSequence.id = _changeCarryTransformSequenceID;

                _changeCarryTransformSequence.Append(DOVirtual.Vector3(carryTransform.localPosition, position, _carryTransformPositionChangeSpeed, r => {
                    carryTransform.localPosition = r;
                })).OnComplete(() => {
                    DeleteChangeCarryTransformSequence();
                });
            }
        }
        private void DeleteChangeCarryTransformSequence() 
        {
            DOTween.Kill(_changeCarryTransformSequenceID);
            _changeCarryTransformSequence = null;
        }
        #endregion

        #endregion
    }
}
