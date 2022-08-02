using UnityEngine;
using ZestGames;
using ZestCore.Utility;

namespace GraveyardIdle
{
    public class PlayerAnimationEventHandler : MonoBehaviour
    {
        private Player _player;
        private PlayerAnimationController _animationController;

        public void Init(PlayerAnimationController animationController, Player player)
        {
            _player = player;
            _animationController = animationController;
        }

        #region ANIMATION EVENT FUNCTIONS
        public void CanDig()
        {
            ShovelEvents.OnCanDig?.Invoke();
            if (_player.IsDigging)
            {
                ShovelEvents.OnDigHappened?.Invoke();
                //Delayer.DoActionAfterDelay(this, 1f, () => ShovelEvents.OnThrowSoilToPile?.Invoke());
                Delayer.DoActionAfterDelay(this, 1f, () => ShovelEvents.OnPlaySoilFX?.Invoke(Enums.SoilThrowTarget.Pile));
            }
            else if (_player.IsFilling)
            {
                ShovelEvents.OnFillHappened?.Invoke();
                //Delayer.DoActionAfterDelay(this, 1f, () => ShovelEvents.OnThrowSoilToGrave?.Invoke());
                Delayer.DoActionAfterDelay(this, 1f, () => ShovelEvents.OnPlaySoilFX?.Invoke(Enums.SoilThrowTarget.Grave));
            }
        }
        public void CantDig()
        {
            ShovelEvents.OnCantDig?.Invoke();
        }
        public void ThrowSoil()
        {
            //ShovelEvents.OnPlaySoilFX?.Invoke();
            //if (_player.IsDigging)
            //    ShovelEvents.OnThrowSoilToPile?.Invoke();
            //else if (_player.IsFilling)
            //    ShovelEvents.OnThrowSoilToGrave?.Invoke();
        }
        #endregion
    }
}
