using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class PlayerDigHandler : MonoBehaviour
    {
        private Player _player;

        private readonly float _digTime = 3f;
        private float _timer;
        private bool _startCounter;

        public void Init(Player player)
        {
            _player = player;

            _startCounter = false;
            _timer = _digTime;

            PlayerEvents.OnStartDigging += StartCounter;
        }

        private void OnDisable()
        {
            PlayerEvents.OnStartDigging -= StartCounter;
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (!_player.IsDigging)
                    PlayerEvents.OnStartDigging?.Invoke();
                else
                    PlayerEvents.OnStopDigging?.Invoke();
            }
#endif

            //if (_startCounter)
            //{
            //    _timer -= Time.deltaTime;
            //    if (_timer < 0f && _player.IsDigging)
            //    {
            //        _timer = _digTime;
            //        _startCounter = false;
            //        PlayerEvents.OnStopDigging?.Invoke();
            //    }
            //}
        }

        private void StartCounter()
        {
            _startCounter = true;
        }
    }
}
