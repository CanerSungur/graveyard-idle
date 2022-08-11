using UnityEngine;
using ZestCore.Utility;
using ZestGames;

namespace GraveyardIdle
{
    public class SellTruckAnimationController : MonoBehaviour
    {
        private SellTruck _sellTruck;
        private Animator _animator;

        #region ANIMATION PARAMETERS
        private readonly int _moveID = Animator.StringToHash("Move");
        #endregion

        public void Init(SellTruck sellTruck)
        {
            _sellTruck = sellTruck;
            _animator = GetComponent<Animator>();
            _animator.Rebind();
            Idle();

            _sellTruck.OnMove += Move;
            _sellTruck.OnIdle += Idle;
        }

        private void OnDisable()
        {
            if (!_sellTruck) return;

            _sellTruck.OnMove -= Move;
            _sellTruck.OnIdle -= Idle;
        }

        private void Move() => _animator.SetBool(_moveID, true);
        private void Idle() => _animator.SetBool(_moveID, false);

        #region ANIMATION EVENT FUNCTIONS
        public void DisableTruck()
        {
            //TruckEvents.OnEnableSellTruck?.Invoke(_sellTruck.ActivatorCoffinMachine);
            //TruckEvents.OnDisableSellTruck?.Invoke();
            Delayer.DoActionAfterDelay(this, 5f, () => TruckEvents.OnEnableSellTruck?.Invoke(_sellTruck.ActivatorCoffinMachine));
        }
        #endregion
    }
}
