using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class TruckAnimationController : MonoBehaviour
    {
        private Truck _truck;
        private Animator _animator;

        #region ANIMATION PARAMETERS
        private readonly int _moveID = Animator.StringToHash("Move");
        private readonly int _dropID = Animator.StringToHash("Drop");
        private readonly int _turnAroundID = Animator.StringToHash("TurnAround");
        #endregion

        public void Init(Truck truck)
        {
            _truck = truck;
            _animator = GetComponent<Animator>();
            _animator.Rebind();

            _truck.OnMove += Move;
            _truck.OnIdle += Idle;
            _truck.OnDrop += Drop;
        }

        private void OnDisable()
        {
            if (!_truck) return;

            _truck.OnMove -= Move;
            _truck.OnIdle -= Idle;
            _truck.OnDrop -= Drop;
        }

        private void Move() => _animator.SetBool(_moveID, true);
        private void Idle() => _animator.SetBool(_moveID, false);
        private void Drop() => _animator.SetTrigger(_dropID);
        private void TurnAround() => _animator.SetTrigger(_turnAroundID);

        #region ANIMATION EVENT FUNCTIONS
        public void ThrowCoffin()
        {
            _truck.DropCarriedCoffin();
        }
        public void DisableTruck()
        {
            TruckEvents.OnDisableTruck?.Invoke();
        }
        #endregion
    }
}
