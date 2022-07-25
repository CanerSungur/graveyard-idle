using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class CoffinAnimationController : MonoBehaviour
    {
        private Animator _animator;
        private Coffin _coffin;

        #region ANIMATION PAREMETERS
        private readonly int _movingID = Animator.StringToHash("Moving");
        #endregion

        public void Init(Coffin coffin)
        {
            _animator = GetComponent<Animator>();
            _coffin = coffin;

            PlayerEvents.OnMove += Move;
            PlayerEvents.OnIdle += Stop;
        }

        private void OnDisable()
        {
            PlayerEvents.OnMove -= Move;
            PlayerEvents.OnIdle -= Stop;
        }

        private void Move()
        {
            if (_coffin.IsBeingCarried)
                _animator.SetBool(_movingID, true);
        }
        private void Stop()
        {
            if (_coffin.IsBeingCarried)
                _animator.SetBool(_movingID, false);
        }

        #region PUBLICS
        public void StopMovement()
        {
            _animator.SetBool(_movingID, false);
        }
        #endregion
    }
}
