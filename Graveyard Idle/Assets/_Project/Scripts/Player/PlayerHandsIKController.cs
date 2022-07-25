using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class PlayerHandsIKController : MonoBehaviour
    {
        private PlayerAnimationController _animationController;

        [Header("-- RIGHT HAND IK SETUP --")]
        [SerializeField, Range(0f, 1f)] private float rightHandWeight;
        [SerializeField] private Transform rightHandHint;
        private Transform _rightHandObject = null;

        [Header("-- LEFT HAND IK SETUP --")]
        [SerializeField, Range(0f, 1f)] private float leftHandWeight;
        [SerializeField] private Transform leftHandHint;
        private Transform _leftHandObject = null;

        public void Init(PlayerAnimationController playerAnimationController)
        {
            _animationController = playerAnimationController;

            PlayerEvents.OnDropCoffin += StopIK;
        }

        private void OnDisable()
        {
            PlayerEvents.OnDropCoffin -= StopIK;
        }

        private void StopIK() => _rightHandObject = _leftHandObject = null;
        private void OnAnimatorIK()
        {
            if (_animationController.Animator && _rightHandObject && _leftHandObject)
            {
                #region RIGHT HAND IK
                _animationController.Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandWeight);
                _animationController.Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandWeight);
                _animationController.Animator.SetIKPosition(AvatarIKGoal.RightHand, _rightHandObject.position);
                _animationController.Animator.SetIKRotation(AvatarIKGoal.RightHand, _rightHandObject.rotation);
                
                if (rightHandHint)
                {
                    _animationController.Animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, rightHandWeight);
                    _animationController.Animator.SetIKHintPosition(AvatarIKHint.RightElbow, rightHandHint.position);
                }
                #endregion

                #region LEFT HAND IK
                _animationController.Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandWeight);
                _animationController.Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandWeight);
                _animationController.Animator.SetIKPosition(AvatarIKGoal.LeftHand, _leftHandObject.position);
                _animationController.Animator.SetIKRotation(AvatarIKGoal.LeftHand, _leftHandObject.rotation);

                if (leftHandHint)
                {
                    _animationController.Animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, leftHandWeight);
                    _animationController.Animator.SetIKHintPosition(AvatarIKHint.LeftElbow, leftHandHint.position);
                }
                #endregion
            }
        }

        #region PUBLICS
        public void SetIKHandObjects(Transform leftHandObject, Transform rightHandObject)
        {
            _rightHandObject = rightHandObject;
            _leftHandObject = leftHandObject;
        }
        #endregion
    }
}
