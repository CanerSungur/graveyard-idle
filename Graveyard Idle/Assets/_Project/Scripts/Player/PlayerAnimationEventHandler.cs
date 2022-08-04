using UnityEngine;
using ZestGames;
using ZestCore.Utility;

namespace GraveyardIdle
{
    public class PlayerAnimationEventHandler : MonoBehaviour
    {
        private Player _player;
        private PlayerAnimationController _animationController;

        #region DIGGING ANIMATION TIME SECTION
        // all of these are in seconds
        private readonly float _digHappeningTime = 0.19f;
        private readonly float _timeThrowingSoilShouldHappen = 1.22f;
        private float _defaultTimeDelayForThrowingSoil;
        private float _throwSoilTriggerDelay;
        #endregion

        public void Init(PlayerAnimationController animationController, Player player)
        {
            _player = player;
            _animationController = animationController;

            _defaultTimeDelayForThrowingSoil = _timeThrowingSoilShouldHappen - _digHappeningTime;
            UpdateThrowSoilDelay();

            PlayerEvents.OnSetCurrentDigSpeed += UpdateThrowSoilDelay;
        }

        private void OnDisable()
        {
            if (!_player) return;

            PlayerEvents.OnSetCurrentDigSpeed -= UpdateThrowSoilDelay;
        }

        private void UpdateThrowSoilDelay()
        {
            _throwSoilTriggerDelay = _defaultTimeDelayForThrowingSoil / DataManager.DigSpeed;
        }

        #region ANIMATION EVENT FUNCTIONS
        public void CanDig()
        {
            ShovelEvents.OnCanDig?.Invoke();
            if (_player.IsDigging)
            {
                ShovelEvents.OnDigHappened?.Invoke();
                //Delayer.DoActionAfterDelay(this, 1f, () => ShovelEvents.OnThrowSoilToPile?.Invoke());
                Delayer.DoActionAfterDelay(this, _throwSoilTriggerDelay, () => ShovelEvents.OnPlaySoilFX?.Invoke(Enums.SoilThrowTarget.Pile));
            }
            else if (_player.IsFilling)
            {
                ShovelEvents.OnFillHappened?.Invoke();
                //Delayer.DoActionAfterDelay(this, 1f, () => ShovelEvents.OnThrowSoilToGrave?.Invoke());
                Delayer.DoActionAfterDelay(this, _throwSoilTriggerDelay, () => ShovelEvents.OnPlaySoilFX?.Invoke(Enums.SoilThrowTarget.Grave));
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
        public void PutDownShovel()
        {
            if (_animationController.Player.Shovel.PutItDown)
            {
                PlayerEvents.OnPutDownShovel?.Invoke();

            }
        }
        public void EnableShovelMesh()
        {
            ShovelEvents.OnEnableMesh?.Invoke();
        }
        public void DisableShovelMesh()
        {
            ShovelEvents.OnDisableMesh?.Invoke();
        }
        #endregion
    }
}
