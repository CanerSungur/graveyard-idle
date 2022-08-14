using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class CoffinCarrierAnimationController : MonoBehaviour
    {
        private CoffinCarrier _coffinCarrier;
        private Animator _animator;

        #region ANIMATION VARIABLES
        private readonly int _moveID = Animator.StringToHash("Move");
        private readonly int _carryID = Animator.StringToHash("Carry");
        private readonly int _throwID = Animator.StringToHash("Throw");
        private readonly int _sideIndexID = Animator.StringToHash("SideIndex");

        private readonly int _leftSideIndex = 1;
        private readonly int _rightSideIndex = 2;
        private readonly int _carryLayer = 1;
        #endregion

        public void Init(CoffinCarrier coffinCarrier)
        {
            if (!_animator)
            {
                _animator = GetComponent<Animator>();
                _coffinCarrier = coffinCarrier;
            }

            _animator.SetLayerWeight(_carryLayer, 1f);
            SetSideIndex();

            _coffinCarrier.OnMove += Move;
            _coffinCarrier.OnIdle += Idle;
            _coffinCarrier.OnThrow += Throw;
            _coffinCarrier.OnTakeCoffin += StartCarrying;
            CoffinCarrierEvents.OnLeaveCoffin += Throw;
        }

        private void OnDisable()
        {
            if (!_coffinCarrier) return;

            _coffinCarrier.OnMove -= Move;
            _coffinCarrier.OnIdle -= Idle;
            _coffinCarrier.OnThrow -= Throw;
            _coffinCarrier.OnTakeCoffin -= StartCarrying;
            CoffinCarrierEvents.OnLeaveCoffin -= Throw;
        }

        private void SetSideIndex()
        {
            if (_coffinCarrier.Number == 0 || _coffinCarrier.Number == 1)
                _animator.SetInteger(_sideIndexID, _rightSideIndex);
            else
                _animator.SetInteger(_sideIndexID, _leftSideIndex);
        }
        private void Move()
        {
            _animator.SetBool(_moveID, true);
        }
        private void Idle()
        {
            _animator.SetBool(_moveID, false);
        }
        private void StartCarrying()
        {
            _animator.SetLayerWeight(_carryLayer, 1f);
            _animator.SetBool(_carryID, true);
        }
        private void Throw()
        {
            _animator.SetLayerWeight(_carryLayer, 0f);
            _animator.SetBool(_carryID, false);
            _animator.SetTrigger(_throwID);
        }
    }
}
