using UnityEngine;
using ZestGames;
using TMPro;
using System;
using DG.Tweening;

namespace GraveyardIdle
{
    public class BuyCoffinArea : MonoBehaviour
    {
        private CoffinMachine _coffinMachine;

        [Header("-- SETUP --")]
        [SerializeField] private TextMeshProUGUI remainingMoneyText;
        private CanvasGroup _canvasGroup;

        #region PROPERTIES
        public CoffinMachine CoffinMachine => _coffinMachine;
        public bool PlayerIsInArea { get; set; }
        public bool MoneyCanBeSpent => DataManager.TotalMoney > 0 && _consumedMoney < RequiredMoney;
        public int RequiredMoney => _coreRequiredMoney;
        #endregion

        private readonly int _coreRequiredMoney = 10;
        private int _consumedMoney;

        #region SEQUENCE
        private Sequence _bounceSequence;
        private Guid _bounceSequenceID;
        #endregion

        public void Init(CoffinMachine coffinMachine)
        {
            _coffinMachine = coffinMachine;
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 1f;

            PlayerIsInArea = false;
            UpdateRemainingMoneyText();
        }

        private void UpdateRemainingMoneyText() => remainingMoneyText.text = (RequiredMoney - _consumedMoney).ToString();
        
        #region PUBLICS
        public void ActivateCanvas()
        {
            _canvasGroup.alpha = 1f;
            StartBounceSequence();
        }
        public void ConsumeMoney(int amount)
        {
            if (amount > (RequiredMoney - _consumedMoney))
            {
                if (amount > DataManager.TotalMoney)
                {
                    _consumedMoney += DataManager.TotalMoney;
                    CollectableEvents.OnConsume?.Invoke(DataManager.TotalMoney);
                }
                else
                {
                    CollectableEvents.OnConsume?.Invoke(RequiredMoney - _consumedMoney);
                    _consumedMoney = RequiredMoney;
                }
            }
            else
            {
                if (amount > DataManager.TotalMoney)
                {
                    _consumedMoney += DataManager.TotalMoney;
                    CollectableEvents.OnConsume?.Invoke(DataManager.TotalMoney);
                }
                else
                {
                    CollectableEvents.OnConsume?.Invoke(amount);
                    _consumedMoney += amount;
                }
            }

            UpdateRemainingMoneyText();

            if (_consumedMoney == RequiredMoney)
            {
                // Trigger making coffin timer.
                _coffinMachine.TimerHandler.StartFilling();
                _canvasGroup.alpha = 0.5f;
                
                _consumedMoney = 0;
                UpdateRemainingMoneyText();

                PlayerEvents.OnStopSpendingMoney?.Invoke();
                MoneyCanvas.Instance.StopSpendingMoney();
                //AudioHandler.PlayAudio(Enums.AudioType.GraveBuilt);
                //_interactableGround.ActivateGrave();
            }
        }
        #endregion

        #region DOTWEEN FUNCTIONS
        private void StartBounceSequence()
        {
            CreateBounceSequence();
            _bounceSequence.Play();
        }
        private void CreateBounceSequence()
        {
            if (_bounceSequence == null)
            {
                _bounceSequence = DOTween.Sequence();
                _bounceSequenceID = Guid.NewGuid();
                _bounceSequence.id = _bounceSequenceID;

                _bounceSequence.Append(transform.DOShakeScale(1f, 0.01f).OnComplete(() => {
                    DeleteBounceSequence();
                }));
            }
        }
        private void DeleteBounceSequence()
        {
            DOTween.Kill(_bounceSequenceID);
            _bounceSequence = null;
        }
        #endregion
    }
}
