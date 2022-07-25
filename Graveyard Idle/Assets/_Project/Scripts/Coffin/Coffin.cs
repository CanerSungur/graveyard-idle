using UnityEngine;
using DG.Tweening;
using ZestGames;
using System;

namespace GraveyardIdle
{
    public class Coffin : MonoBehaviour
    {
        [Header("-- HANDLES FOR IK SETUP --")]
        [SerializeField] private Transform rightHandObjectIK;
        [SerializeField] private Transform leftHandObjectIK;

        #region STACK DATA
        private bool _stacked = false;
        private readonly float _stackJumpHeight = 1.5f;
        private readonly float _animationTime = 0.5f;
        #endregion

        #region COMPONENTS
        private Rigidbody _rigidbody;
        public Rigidbody Rigidbody => _rigidbody == null ? _rigidbody = GetComponent<Rigidbody>() : _rigidbody;
        private Collider _collider;
        public Collider Collider => _collider == null ? _collider = GetComponent<Collider>() : _collider;
        #endregion

        #region SCRIPT REFERENCES
        private CoffinAnimationController _animationController;
        public CoffinAnimationController AnimationController => _animationController == null ? _animationController = GetComponent<CoffinAnimationController>() : _animationController;
        #endregion

        #region PROPERTIES
        public bool IsBeingCarried { get; private set; }
        public bool CanBeCarried => _stacked;
        public Transform RightHandObjectIK => rightHandObjectIK;
        public Transform LeftHandObjectIK => leftHandObjectIK;
        #endregion

        #region SEQUENCE
        private Sequence _jumpSequence;
        private Guid _jumpSequenceID;
        #endregion

        public void Init(CoffinAreaSpawnHandler spawnHandler)
        {
            IsBeingCarried = false;
            Rigidbody.isKinematic = true;

            AnimationController.Init(this);
        }

        #region PUBLICS
        public void GetStacked(Vector3 position, Transform parent)
        {
            transform.SetParent(parent);

            StartJumpSequence(position, Vector3.zero);

            _stacked = true;

            CoffinAreaEvents.OnStackedCoffin?.Invoke();
        }
        public void GoToPlayer(Transform parent)
        {
            IsBeingCarried = true;
            transform.SetParent(parent);

            StartJumpSequence(Vector3.zero, new Vector3(23f, 0f, 0f));

            CoffinAreaEvents.OnUnStackedCoffin?.Invoke();
        }
        public void GetDropped()
        {
            AnimationController.StopMovement();
            Rigidbody.isKinematic = false;
            transform.SetParent(null);

            Rigidbody.AddForce(new Vector3(UnityEngine.Random.Range(-3f, 3f), UnityEngine.Random.Range(5f, 9f), UnityEngine.Random.Range(-3f, 3f)), ForceMode.Impulse);
        }
        #endregion

        #region DOTWEEN FUNCTIONS
        private void StartJumpSequence(Vector3 jumpPosition, Vector3 rotation)
        {
            _jumpSequence.Pause();
            DeleteJumpSequence();
            CreateJumpSequence(jumpPosition, rotation);
            _jumpSequence.Play();
        }
        private void CreateJumpSequence(Vector3 jumpPosition, Vector3 rotation)
        {
            if (_jumpSequence == null)
            {
                _jumpSequence = DOTween.Sequence();
                _jumpSequenceID = Guid.NewGuid();
                _jumpSequence.id = _jumpSequenceID;

                _jumpSequence.Append(transform.DOLocalJump(jumpPosition, _stackJumpHeight, 1, _animationTime))
                    .Join(transform.DOLocalRotate(rotation, _animationTime))
                    .Join(transform.DOShakeScale(_animationTime, 0.5f))
                    .OnComplete(() => {
                        DeleteJumpSequence();
                    });
            }
        }
        private void DeleteJumpSequence()
        {
            DOTween.Kill(_jumpSequenceID);
            _jumpSequence = null;
        }
        #endregion
    }
}
