using UnityEngine;
using DG.Tweening;
using ZestGames;

namespace GraveyardIdle
{
    public class CollectMoney : MonoBehaviour
    {
        private Camera _camera;
        private RectTransform _canvasRect;

        private MoneyCanvas _moneyCanvas;
        private RectTransform _rectTransform;

        public void Init(MoneyCanvas moneyCanvas, Transform spawnTransform)
        {
            if (!_moneyCanvas)
            {
                _moneyCanvas = moneyCanvas;
                _camera = Camera.main;
                _canvasRect = _moneyCanvas.GetComponent<RectTransform>();
            }

            _rectTransform = GetComponent<RectTransform>();
            _rectTransform.anchoredPosition = GetWorldPointToScreenPoint(spawnTransform);
            //_rectTransform.anchoredPosition = _moneyCanvas.MiddlePointRectTransform.anchoredPosition;
            _rectTransform.DOAnchorPos(Hud.MoneyAnchoredPosition, 1f).OnComplete(() => {
                AudioEvents.OnPlayCollectMoney?.Invoke();
                CollectableEvents.OnCollect?.Invoke(DataManager.MoneyValue);
                gameObject.SetActive(false);
            });
        }

        private Vector2 GetWorldPointToScreenPoint(Transform transform)
        {
            Vector2 viewportPosition = _camera.WorldToViewportPoint(transform.position);
            Vector2 phaseUnlockerScreenPosition = new Vector2(
               (viewportPosition.x * _canvasRect.sizeDelta.x) - (_canvasRect.sizeDelta.x * 1f),
               (viewportPosition.y * _canvasRect.sizeDelta.y) - (_canvasRect.sizeDelta.y * 1f));

            return phaseUnlockerScreenPosition;
        }
    }
}
