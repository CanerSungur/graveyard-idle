using UnityEngine;

namespace GraveyardIdle
{
    public class GravePiece : MonoBehaviour
    {
        private Animator _animator;

        #region ANIMATION VARIABLES
        private readonly int _openID = Animator.StringToHash("Open");
        #endregion

        public void Init(Grave grave)
        {
            _animator = GetComponent<Animator>();
            
            if (_animator)
                Open();
        }

        private void Open() => _animator.SetBool(_openID, true);
        public void Close() => _animator.SetBool(_openID, false);
    }
}
