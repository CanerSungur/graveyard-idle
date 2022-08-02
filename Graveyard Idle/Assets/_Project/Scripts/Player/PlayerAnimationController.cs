using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class PlayerAnimationController : MonoBehaviour
    {
        private Player _player;

        private Animator _animator;
        private PlayerHandsIKController _handsIKController;
        private PlayerAnimationEventHandler _animationEventHandler;

        #region ANIMATION PARAMETERS
        private readonly int _runID = Animator.StringToHash("Move");
        private readonly int _runSpeedID = Animator.StringToHash("MoveSpeed");
        private readonly int _carryingCoffinID = Animator.StringToHash("CarryingCoffin");
        private readonly int _isInDiggingZoneID = Animator.StringToHash("InDiggingZone");
        private readonly int _digSpeedID = Animator.StringToHash("DigSpeed");
        private readonly int _pullOutShovelID = Animator.StringToHash("PullOutShovel");
        private readonly int _putDownShovelID = Animator.StringToHash("PutDownShovel");
        #endregion

        #region GETTERS
        public Animator Animator => _animator;
        #endregion

        public void Init(Player player)
        {
            _player = player;

            _animator = GetComponentInChildren<Animator>();
            _animationEventHandler = GetComponentInChildren<PlayerAnimationEventHandler>();
            _animationEventHandler.Init(this, _player);
            _handsIKController = GetComponentInChildren<PlayerHandsIKController>();
            _handsIKController.Init(this);

            _animator.SetFloat(_runSpeedID, 1f);
            _animator.SetBool(_carryingCoffinID, false);

            PlayerEvents.OnIdle += Idle;
            PlayerEvents.OnMove += Run;
            PlayerEvents.OnTakeACoffin += StartCarryingCoffin;
            PlayerEvents.OnDropCoffin += StopCarryingCoffin;
            PlayerEvents.OnEnteredDigZone += StartDigging;
            PlayerEvents.OnExitedDigZone += StopDigging;
            PlayerEvents.OnEnteredFillZone += StartDigging;
            PlayerEvents.OnExitedFillZone += StopDigging;
            //PlayerEvents.OnStopDigging += StopDigging;

            PlayerEvents.OnEnteredDigZone += PullOutShovel;
            PlayerEvents.OnExitedDigZone += PutDownShovel;
        }

        private void OnDisable()
        {
            PlayerEvents.OnIdle -= Idle;
            PlayerEvents.OnMove -= Run;
            PlayerEvents.OnTakeACoffin -= StartCarryingCoffin;
            PlayerEvents.OnDropCoffin -= StopCarryingCoffin;
            PlayerEvents.OnEnteredDigZone -= StartDigging;
            PlayerEvents.OnExitedDigZone -= StopDigging;
            PlayerEvents.OnEnteredFillZone -= StartDigging;
            PlayerEvents.OnExitedFillZone -= StopDigging;
            //PlayerEvents.OnStopDigging -= StopDigging;

            PlayerEvents.OnEnteredDigZone -= PullOutShovel;
            PlayerEvents.OnExitedDigZone -= PutDownShovel;
        }

        private void Run() => _animator.SetBool(_runID, true);
        private void Idle() => _animator.SetBool(_runID, false);
        private void StartCarryingCoffin()
        {
            _animator.SetFloat(_runSpeedID, 0.5f);
            _animator.SetBool(_carryingCoffinID, true);
        }
        private void StopCarryingCoffin(Coffin ignore, InteractableGround ignoreAlso)
        {
            _animator.SetFloat(_runSpeedID, 1f);
            _animator.SetBool(_carryingCoffinID, false);
        }
        private void StartDigging()
        {
            _animator.SetBool(_isInDiggingZoneID, true);
        }
        private void StopDigging() =>_animator.SetBool(_isInDiggingZoneID, false);
        private void PullOutShovel() => _animator.SetTrigger(_pullOutShovelID);
        private void PutDownShovel() => _animator.SetTrigger(_putDownShovelID);

        #region PUBLICS
        public void SetCoffinIK(Coffin coffin)
        {
            _handsIKController.SetIKHandObjects(coffin.LeftHandObjectIK, coffin.RightHandObjectIK);
        }
        #endregion
    }
}
