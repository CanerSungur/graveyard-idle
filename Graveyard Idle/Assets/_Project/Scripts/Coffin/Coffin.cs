using UnityEngine;
using DG.Tweening;
using ZestGames;
using System;
using Random = UnityEngine.Random;

namespace GraveyardIdle
{
    public class Coffin : MonoBehaviour
    {
        [Header("-- HANDLES FOR IK SETUP --")]
        [SerializeField] private Transform rightHandObjectIK;
        [SerializeField] private Transform leftHandObjectIK;

        [Header("-- HANDLES FOR CARRIERS --")]
        [SerializeField] private CarrierHandle[] carrierHandles;

        #region STACK DATA
        private bool _stacked = false;
        private readonly float _stackJumpHeight = 4f;
        private readonly float _graveJumpHeight = 2f;
        private readonly float _animationTime = 1.5f;
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
        private CoffinMovement _movementHandler;
        public CoffinMovement MovementHandler => _movementHandler == null ? _movementHandler = GetComponent<CoffinMovement>() : _movementHandler;
        #endregion

        #region PROPERTIES
        public bool IsBeingCarried { get; private set; }
        public bool CanBeCarried => _stacked;
        public Transform RightHandObjectIK => rightHandObjectIK;
        public Transform LeftHandObjectIK => leftHandObjectIK;
        public CarrierHandle[] CarrierHandles => carrierHandles;
        #endregion

        #region SEQUENCE
        private Sequence _jumpSequence;
        private Guid _jumpSequenceID;
        #endregion

        #region EVENTS
        public Action OnMoveToAssignedGrave;
        #endregion

        public void Init(Truck truck)
        {
            IsBeingCarried = false;
            Rigidbody.isKinematic = true;

            MovementHandler.Init(this);
            AnimationController.Init(this);

            InitCarrierHandles();
        }

        #region CARRIER HANDLE FUNCTIONS
        private void InitCarrierHandles()
        {
            for (int i = 0; i < carrierHandles.Length; i++)
                carrierHandles[i].Init(this);
        }
        #endregion

        #region PUBLICS
        public void GetStacked(Vector3 position, Transform parent)
        {
            transform.SetParent(parent);

            StartJumpSequence(position, new Vector3(0f, 180f, 0f), _stackJumpHeight, _animationTime);

            _stacked = true;

            CoffinAreaEvents.OnStackedCoffin?.Invoke();
        }
        public void GoToPlayer(Transform parent)
        {
            IsBeingCarried = true;
            transform.SetParent(parent);

            StartJumpSequence(Vector3.zero, new Vector3(14f, 0f, 0f), _stackJumpHeight, _animationTime * 0.5f);

            CoffinAreaEvents.OnUnStackedCoffin?.Invoke();
            GraveManagerEvents.OnCoffinPickedUp?.Invoke();
        }
        public void GetDropped()
        {
            IsBeingCarried = false;
            AnimationController.StopMovement();
            Rigidbody.isKinematic = false;
            transform.SetParent(null);

            Rigidbody.AddForce(new Vector3(Random.Range(-3f, 3f), Random.Range(5f, 9f), Random.Range(-3f, 3f)), ForceMode.Impulse);
        }
        public void GetThrownToGrave(Transform graveTransform)
        {
            IsBeingCarried = false;
            AnimationController.StopMovement();
            transform.SetParent(graveTransform);

            StartJumpSequence(Vector3.up, Vector3.zero, _graveJumpHeight * 0.5f, _animationTime, () => {
                Rigidbody.isKinematic = false;
                Rigidbody.AddTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)), ForceMode.VelocityChange);
                });
        }
        public void GetThrownToCarriers()
        {
            transform.SetParent(null);

            StartJumpSequence(CoffinArea.CoffinThrowPointForCarriers.position + (Vector3.up * 0.8f), Vector3.zero, _stackJumpHeight, _animationTime, () => {
                CoffinCarrierEvents.OnSendCarriersToHandles?.Invoke(this);
            });

            CoffinAreaEvents.OnUnStackedCoffin?.Invoke();
        }
        public void GetThrownToGraveByCarriers(Transform graveTransform)
        {
            IsBeingCarried = false;
            AnimationController.StopMovement();
            transform.SetParent(graveTransform);

            StartJumpSequence(Vector3.up, new Vector3(0f, 180f, 0f), _graveJumpHeight * 0.25f, _animationTime, () => {
                Rigidbody.isKinematic = false;
                Rigidbody.AddTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)), ForceMode.VelocityChange);
            });
        }
        #endregion

        #region DOTWEEN FUNCTIONS
        private void StartJumpSequence(Vector3 jumpPosition, Vector3 rotation, float jumpHeight, float animationTime, Action action = null)
        {
            _jumpSequence.Pause();
            DeleteJumpSequence();
            CreateJumpSequence(jumpPosition, rotation, jumpHeight, animationTime, action);
            _jumpSequence.Play();
        }
        private void CreateJumpSequence(Vector3 jumpPosition, Vector3 rotation, float jumpHeight, float animationTime, Action action = null)
        {
            if (_jumpSequence == null)
            {
                _jumpSequence = DOTween.Sequence();
                _jumpSequenceID = Guid.NewGuid();
                _jumpSequence.id = _jumpSequenceID;

                _jumpSequence.Append(transform.DOLocalJump(jumpPosition, jumpHeight, 1, animationTime))
                    .Join(transform.DOLocalRotate(rotation, _animationTime))
                    .Join(transform.DOShakeScale(_animationTime, 0.5f))
                    .OnComplete(() => {
                        DeleteJumpSequence();
                        action?.Invoke();
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
