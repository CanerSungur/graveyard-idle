using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class PlayerAnimationController : MonoBehaviour
    {
        private Animator _animator;
        private PlayerHandsIKController _handsIKController;

        #region ANIMATION PARAMETERS
        private readonly int _runID = Animator.StringToHash("Run");
        private readonly int _runSpeedID = Animator.StringToHash("RunSpeed");
        private readonly int _carryingCoffinID = Animator.StringToHash("CarryingCoffin");
        #endregion

        #region GETTERS
        public Animator Animator => _animator;
        #endregion

        public void Init(Player player)
        {
            _animator = GetComponentInChildren<Animator>();
            _handsIKController = GetComponentInChildren<PlayerHandsIKController>();
            _handsIKController.Init(this);
            StopCarryingCoffin();

            PlayerEvents.OnIdle += Idle;
            PlayerEvents.OnMove += Run;
            PlayerEvents.OnTakeACoffin += StartCarryingCoffin;
            PlayerEvents.OnDropCoffin += StopCarryingCoffin;
        }

        private void OnDisable()
        {
            PlayerEvents.OnIdle -= Idle;
            PlayerEvents.OnMove -= Run;
            PlayerEvents.OnTakeACoffin -= StartCarryingCoffin;
            PlayerEvents.OnDropCoffin -= StopCarryingCoffin;
        }

        private void Run()
        {
            _animator.SetBool(_runID, true);
        }
        private void Idle()
        {
            _animator.SetBool(_runID, false);
        }
        private void StartCarryingCoffin()
        {
            _animator.SetFloat(_runSpeedID, 0.5f);
            _animator.SetBool(_carryingCoffinID, true);
        }
        private void StopCarryingCoffin()
        {
            _animator.SetFloat(_runSpeedID, 1f);
            _animator.SetBool(_carryingCoffinID, false);
        }

        #region PUBLICS
        public void SetCoffinIK(Coffin coffin)
        {
            _handsIKController.SetIKHandObjects(coffin.LeftHandObjectIK, coffin.RightHandObjectIK);
        }
        #endregion
    }
}
