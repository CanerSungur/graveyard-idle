using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class PlayerMovement : MonoBehaviour
    {
        private Player _player;
        private CharacterController _characterController;

        [Header("-- MOVEMENT SETUP --")]
        [SerializeField] private float defaultSpeed = 5f;
        [SerializeField] private float carrySpeed = 2f;
        [SerializeField] private LayerMask walkableLayerMask;
        private float _currentSpeed;
        private Vector3 _playerVelocity;
        private const float GRAVITY_VALUE = -5f;
        private bool _startedMoving = false;

        public bool IsMoving => _player.InputHandler.InputValue != Vector3.zero;
        public bool IsGrounded => Physics.Raycast(_player.Collider.bounds.center, Vector3.down, _player.Collider.bounds.extents.y + 0.01f, walkableLayerMask);

        public void Init(Player player)
        {
            _player = player;
            _characterController = GetComponent<CharacterController>();
            SetDefaultSpeed();

            PlayerEvents.OnTakeACoffin += SetCarrySpeed;
            PlayerEvents.OnDropCoffin += SetDefaultSpeed;
        }

        private void OnDisable()
        {
            PlayerEvents.OnTakeACoffin -= SetCarrySpeed;
            PlayerEvents.OnDropCoffin -= SetDefaultSpeed;
        }

        private void Update()
        {
            #region NORMAL MOVEMENT
            if (IsGrounded && _playerVelocity.y < 0f)
                _playerVelocity.y = 0f;

            Motor();

            if (IsMoving)
            {
                transform.forward = _player.InputHandler.InputValue;

                if (!_startedMoving)
                {
                    PlayerEvents.OnMove?.Invoke();
                    _startedMoving = true;

                    if (_player.IsInDigZone && _player.IsDigging)
                        PlayerEvents.OnStopDigging?.Invoke();
                }
            }
            else
            {
                if (_startedMoving)
                {
                    PlayerEvents.OnIdle?.Invoke();
                    _startedMoving = false;

                    if (_player.IsInDigZone && !_player.IsDigging)
                        PlayerEvents.OnStartDigging?.Invoke();
                }
            }

            _playerVelocity.y += GRAVITY_VALUE * Time.deltaTime;
            _characterController.Move(_playerVelocity * Time.deltaTime);
            #endregion
        }

        public void Motor()
        {
            if (GameManager.GameState == Enums.GameState.Started)
                _characterController.Move(_currentSpeed * Time.deltaTime * _player.InputHandler.InputValue);
        }
        private void RotateForUpgradeCamera() => transform.rotation = Quaternion.identity;
        private void SetDefaultSpeed() => _currentSpeed = defaultSpeed;
        private void SetCarrySpeed() => _currentSpeed = carrySpeed;
    }
}
