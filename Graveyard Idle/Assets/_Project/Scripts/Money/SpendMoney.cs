using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class SpendMoney : MonoBehaviour
    {
        private MoneyCanvas _moneyCanvas;
        private RectTransform _canvasRect;
        private RectTransform _rectTransform;
        private Camera _camera;
        private Vector2 _currentPosition;
        //private PhaseUnlocker _phaseUnlocker = null;
        private GraveUpgradeHandler _graveUpgradeHandler = null;
        private float _disableTime;

        //public void Init(MoneyCanvas moneyCanvas, PhaseUnlocker phaseUnlocker)
        //{
        //    if (!_moneyCanvas)
        //    {
        //        _moneyCanvas = moneyCanvas;
        //        _canvasRect = _moneyCanvas.GetComponent<RectTransform>();
        //        _rectTransform = GetComponent<RectTransform>();
        //        _camera = Camera.main;
        //    }

        //    _phaseUnlocker = phaseUnlocker;
        //    _disableTime = Time.time + 0.9f;
        //    _currentPosition = Hud.MoneyAnchoredPosition;
        //    _rectTransform.anchoredPosition = _currentPosition;
        //}

        public void Init(MoneyCanvas moneyCanvas, GraveUpgradeHandler graveUpgradeHandler)
        {
            if (!_moneyCanvas)
            {
                _moneyCanvas = moneyCanvas;
                _canvasRect = _moneyCanvas.GetComponent<RectTransform>();
                _rectTransform = GetComponent<RectTransform>();
                _camera = Camera.main;
            }

            _graveUpgradeHandler = graveUpgradeHandler;
            _disableTime = Time.time + 0.9f;
            _currentPosition = Hud.MoneyAnchoredPosition;
            _rectTransform.anchoredPosition = _currentPosition;
        }

        private void OnDisable()
        {
            //_phaseUnlocker = null;
            _graveUpgradeHandler = null;
        }

        private void Update()
        {
            //if (_phaseUnlocker)
            //{
            //    Vector2 travel = GetWorldPointToScreenPoint(_phaseUnlocker.MoneyTransform) - _rectTransform.anchoredPosition;
            //    _rectTransform.Translate(travel * 10f * Time.deltaTime, _camera.transform);

            //    if (Vector2.Distance(_rectTransform.anchoredPosition, GetWorldPointToScreenPoint(_phaseUnlocker.MoneyTransform)) < 25f)
            //    {
            //        gameObject.SetActive(false);
            //    }

            //    if (Time.time >= _disableTime)
            //        gameObject.SetActive(false);
            //}
            if (_graveUpgradeHandler)
            {
                Vector2 travel = GetWorldPointToScreenPoint(_graveUpgradeHandler.MoneyImageTransform) - _rectTransform.anchoredPosition;
                _rectTransform.Translate(travel * 10f * Time.deltaTime, _camera.transform);


                if (Vector2.Distance(_rectTransform.anchoredPosition, GetWorldPointToScreenPoint(_graveUpgradeHandler.MoneyImageTransform)) < 25f)
                {
                    gameObject.SetActive(false);
                }

                if (Time.time >= _disableTime)
                    gameObject.SetActive(false);
            }
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
