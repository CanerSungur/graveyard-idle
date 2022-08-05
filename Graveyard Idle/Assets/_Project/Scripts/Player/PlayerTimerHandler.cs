using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

namespace GraveyardIdle
{
    public class PlayerTimerHandler : MonoBehaviour
    {
        private Action _currentAction;

        [Header("-- SETUP --")]
        [SerializeField] private Transform timerCanvas;
        [SerializeField] private Image fillImage;
        private readonly float _fillTime = 1f;

        #region PROPERTIES
        //public bool PlayerIsInArea { get; private set; }
        public bool IsFilling { get; private set; }
        #endregion

        #region SEQUENCE
        private Sequence _fillSequence, _emptySequence;
        private Guid _fillSequenceID, _emptySequenceID;
        #endregion

        public void Init(Player player)
        {
            fillImage.fillAmount = 0;
            _currentAction = null;
            IsFilling = false;
            timerCanvas.gameObject.SetActive(false);
        }

        private void DoAction()
        {
            _currentAction?.Invoke();
            _currentAction = null;
        }

        #region PUBLICS
        public void StartFilling(Action action)
        {
            IsFilling = true;
            fillImage.fillAmount = 0;
            timerCanvas.gameObject.SetActive(true);

            _currentAction += action;
            //PlayerIsInArea = true;

            DeleteEmptySequence();
            CreateFillSequence();
            _fillSequence.Play();
        }
        public void StopFilling(Action action)
        {
            DoAction();
            //PlayerIsInArea = false;

            DeleteFillSequence();
            CreateEmptySequence();
            _emptySequence.Play();
        }
        public void StopFilling()
        {
            //PlayerIsInArea = false;
            _currentAction = null;

            DeleteFillSequence();
            CreateEmptySequence();
            _emptySequence.Play();
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

                _fillSequence.Append(timerCanvas.DOScale(Vector3.one * 0.01f, _fillTime * 0.1f))
                    .Append(DOVirtual.Float(fillImage.fillAmount, 1f, _fillTime, r => {
                        fillImage.fillAmount = r;
                    }))
                    .Append(timerCanvas.DOScale(Vector3.zero, _fillTime * 0.1f)).OnComplete(() => {
                        DeleteFillSequence();
                        DoAction();
                    });
            }
        }
        private void DeleteFillSequence()
        {
            DOTween.Kill(_fillSequenceID);
            _fillSequence = null;
        }

        private void CreateEmptySequence()
        {
            if (_emptySequence == null)
            {
                _emptySequence = DOTween.Sequence();
                _emptySequenceID = Guid.NewGuid();
                _emptySequence.id = _emptySequenceID;

                _emptySequence.Append(DOVirtual.Float(fillImage.fillAmount, 0f, _fillTime, r => {
                        fillImage.fillAmount = r;
                    }))
                    .Append(timerCanvas.DOScale(Vector3.zero, _fillTime * 0.1f)).OnComplete(() => {
                        DeleteEmptySequence();
                        IsFilling = false;
                        timerCanvas.gameObject.SetActive(false);
                    });
            }
        }
        private void DeleteEmptySequence()
        {
            DOTween.Kill(_emptySequenceID);
            _emptySequence = null;
        }
        #endregion
    }
}
