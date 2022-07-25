using UnityEngine;
using DG.Tweening;
using ZestGames;

namespace GraveyardIdle
{
    public class Coffin : MonoBehaviour
    {
        #region STACK DATA
        private bool _stacked = false;
        private readonly float _stackJumpHeight = 1.5f;
        private readonly float _animationTime = 0.5f;
        #endregion

        #region PROPERTIES
        public bool CanBeCarried => _stacked;
        #endregion

        #region PUBLICS
        public void GetStacked(Vector3 position, Transform parent)
        {
            transform.DOKill();
            transform.SetParent(parent);

            transform.DOLocalJump(position, _stackJumpHeight, 1, _animationTime);
            transform.DOLocalRotate(Vector3.zero, _animationTime);
            transform.DOShakeScale(_animationTime, 0.5f);

            _stacked = true;

            CoffinAreaEvents.OnStackedCoffin?.Invoke();
        }
        public void GoToPlayer(Transform parent)
        {
            transform.DOKill();
            transform.SetParent(parent);

            transform.DOLocalJump(Vector3.zero, _stackJumpHeight, 1, _animationTime);
            transform.DOLocalRotate(new Vector3(23f, 0f, 0f), _animationTime);
            transform.DOShakeScale(_animationTime, 0.5f);

            CoffinAreaEvents.OnUnStackedCoffin?.Invoke();
        }
        #endregion
    }
}
