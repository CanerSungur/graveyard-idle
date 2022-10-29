using UnityEngine;
using DG.Tweening;
using ZestGames;
using System;
using Random = UnityEngine.Random;

namespace GraveyardIdle
{
    public class CollectMoney2D : MonoBehaviour
    {
        private Camera _camera;
        private RectTransform _canvasRect;

        private MoneyCanvas _moneyCanvas;
        private RectTransform _rectTransform;

        #region SEQUENCE
        private Sequence _collectSequence;
        private Guid _collectSequenceID;
        private readonly float _collectDuration = 1f;
        #endregion

        public void Init(MoneyCanvas moneyCanvas, Transform spawnTransform)
        {
            if (!_moneyCanvas)
            {
                _moneyCanvas = moneyCanvas;
                _camera = Camera.main;
                _canvasRect = _moneyCanvas.GetComponent<RectTransform>();
            }

            _rectTransform = GetComponent<RectTransform>();
            _rectTransform.localScale = Vector3.one * 0.5f;
            _rectTransform.anchoredPosition = GetWorldPointToScreenPoint(spawnTransform);

            _rectTransform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0, 360));
            StartCollectSequence();
        }

        private Vector2 GetWorldPointToScreenPoint(Transform transform)
        {
            Vector2 viewportPosition = _camera.WorldToViewportPoint(transform.position);
            Vector2 phaseUnlockerScreenPosition = new Vector2(
               (viewportPosition.x * _canvasRect.sizeDelta.x) - (_canvasRect.sizeDelta.x * 1f),
               (viewportPosition.y * _canvasRect.sizeDelta.y) - (_canvasRect.sizeDelta.y * 1f));

            return phaseUnlockerScreenPosition;
        }

        #region DOTWEEN FUNCTIONS
        private void StartCollectSequence()
        {
            CreateCollectSequence();
            _collectSequence = null;
        }
        private void CreateCollectSequence()
        {
            if (_collectSequence == null)
            {
                _collectSequence = DOTween.Sequence();
                _collectSequenceID = Guid.NewGuid();
                _collectSequence.id = _collectSequenceID;

                _collectSequence.Append(_rectTransform.DOJumpAnchorPos(Hud.MoneyAnchoredPosition, Random.Range(-200, 200), 1, _collectDuration))
                    .Join(_rectTransform.DOScale(Vector3.one * 0.8f, _collectDuration))
                    .Join(_rectTransform.DORotate(Vector3.zero, _collectDuration))
                    .OnComplete(() => {
                        AudioEvents.OnPlayCollectMoney?.Invoke();
                        CollectableEvents.OnCollect?.Invoke(DataManager.MoneyValue);
                        DeleteCollectSequence();
                        gameObject.SetActive(false);
                    });
            }
        }
        private void DeleteCollectSequence()
        {
            DOTween.Kill(_collectSequenceID);
            _collectSequence = null;
        }
        #endregion
    }
}
