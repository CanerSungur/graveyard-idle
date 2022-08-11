using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class CoffinMakerAnimationController : MonoBehaviour
    {
        private CoffinMaker _coffinMaker;
        private Animator _animator;

        private readonly int _moveID = Animator.StringToHash("Move");
        private readonly int _pushButtonID = Animator.StringToHash("PushButton");

        public void Init(CoffinMaker coffinMaker)
        {
            _coffinMaker = coffinMaker;
            _animator = GetComponent<Animator>();

            CoffinMakerEvents.OnGoToCounter += Move;
            CoffinMakerEvents.OnGoToMachine += Move;
            CoffinMakerEvents.OnPushTheButton += PushButton;
            CoffinMakerEvents.OnStartWaiting += Idle;
        }

        private void OnDisable()
        {
            if (!_coffinMaker) return;

            CoffinMakerEvents.OnGoToCounter -= Move;
            CoffinMakerEvents.OnGoToMachine -= Move;
            CoffinMakerEvents.OnPushTheButton -= PushButton;
            CoffinMakerEvents.OnStartWaiting -= Idle;
        }

        private void Move() => _animator.SetBool(_moveID, true);
        private void Idle() => _animator.SetBool(_moveID, false);
        private void PushButton() => _animator.SetTrigger(_pushButtonID);
    }
}
