using UnityEngine;

namespace GraveyardIdle
{
    public class CoffinMaker : MonoBehaviour
    {
        #region SCRIPT REFERENCES
        private CoffinMakerAnimationController _animationController;
        public CoffinMakerAnimationController AnimationController => _animationController == null ? _animationController = GetComponent<CoffinMakerAnimationController>() : _animationController;
        private CoffinMakerMovement _coffinMakerMovement;
        public CoffinMakerMovement CoffinMakerMovement => _coffinMakerMovement == null ? _coffinMakerMovement = GetComponent<CoffinMakerMovement>() : _coffinMakerMovement;
        #endregion

        private void Awake()
        {
            CoffinMakerMovement.Init(this);
            AnimationController.Init(this);
        }
    }
}
