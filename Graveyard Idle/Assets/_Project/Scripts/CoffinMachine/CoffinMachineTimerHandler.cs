using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

namespace GraveyardIdle
{
    public class CoffinMachineTimerHandler : MonoBehaviour
    {
        private CoffinMachine _coffinMachine;

        [Header("-- SETUP --")]
        [SerializeField] private Transform timerCanvas;
        [SerializeField] private Image fillImage;

        #region PROPERTIES
        public bool IsMakingCoffin { get; private set; }
        #endregion

        #region SEQUENCE
        private Sequence _fillSequence;
        private Guid _fillSequenceID;
        #endregion

        public void Init(CoffinMachine coffinMachine)
        {
            _coffinMachine = coffinMachine;

            fillImage.fillAmount = 0;
            IsMakingCoffin = false;
            timerCanvas.gameObject.SetActive(false);
        }

        #region PUBLICS
        public void StartFilling()
        {
            Debug.Log("start making coffin");
            IsMakingCoffin = true;
            fillImage.fillAmount = 0;
            timerCanvas.gameObject.SetActive(true);

            CreateFillSequence();
            _fillSequence.Play();
        }
        #endregion

        #region DOTWEEN FUNCTIONS
        private void CreateFillSequence()
        {
            if (_fillSequence == null)
            {
                _fillSequence = DOTween.Sequence();
                _fillSequenceID = Guid.NewGuid();
                _fillSequence.id = _fillSequenceID;

                _fillSequence.Append(timerCanvas.DOScale(Vector3.one * 0.01f, 0.2f))
                    .Append(DOVirtual.Float(fillImage.fillAmount, 1f, _coffinMachine.CoffinMakeTime, r => {
                        fillImage.fillAmount = r;
                    }))
                    .Append(timerCanvas.DOScale(Vector3.zero, 0.2f)).OnComplete(() => {
                        DeleteFillSequence();
                        _coffinMachine.SpawnACoffin();
                        _coffinMachine.BuyCoffinArea.ActivateCanvas();
                    });
            }
        }
        private void DeleteFillSequence()
        {
            DOTween.Kill(_fillSequenceID);
            _fillSequence = null;
            IsMakingCoffin = false;
        }
        #endregion
    }
}
