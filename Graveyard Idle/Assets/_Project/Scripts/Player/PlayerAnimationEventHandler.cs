using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class PlayerAnimationEventHandler : MonoBehaviour
    {
        private PlayerAnimationController _animationController;

        public void Init(PlayerAnimationController animationController)
        {
            _animationController = animationController;
        }

        #region ANIMATION EVENT FUNCTIONS
        public void CanDig()
        {
            ShovelEvents.OnCanDig?.Invoke();
            ShovelEvents.OnDigHappened?.Invoke();
        }
        public void CantDig()
        {
            ShovelEvents.OnCantDig?.Invoke();
        }
        #endregion
    }
}
