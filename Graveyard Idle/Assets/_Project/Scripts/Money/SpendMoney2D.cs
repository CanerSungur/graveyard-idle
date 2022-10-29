using GraveyardIdle.GraveSystem;
using UnityEngine;
using ZestGames;
using System;
using Random = UnityEngine.Random;
using DG.Tweening;

namespace GraveyardIdle
{
    public class SpendMoney2D : MonoBehaviour
    {
        private MoneyCanvas _moneyCanvas;
        private RectTransform _canvasRect;
        private RectTransform _rectTransform;
        private Camera _camera;
        private Vector2 _currentPosition;
        private Transform _targetTransform = null;
        private float _disableTime;
        private RectTransform _mesh;

        #region SEQUENCE
        private Sequence _archSequence;
        private Guid _archSequenceID;
        private const float ARCH_SEQUENCE_DURATION = 0.5f;
        #endregion

        public void Init(MoneyCanvas moneyCanvas, Transform targetTransform)
        {
            if (!_moneyCanvas)
            {
                _moneyCanvas = moneyCanvas;
                _canvasRect = _moneyCanvas.GetComponent<RectTransform>();
                _rectTransform = GetComponent<RectTransform>();
                _camera = Camera.main;
                _mesh = transform.GetChild(0).GetComponent<RectTransform>();
            }

            _targetTransform = targetTransform;
            _disableTime = Time.time + 0.9f;
            _currentPosition = Hud.MoneyAnchoredPosition;
            _rectTransform.anchoredPosition = _currentPosition;

            //_mesh.localScale = Vector3.zero * 0.8f;
            _mesh.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0, 360));
            CreateArchSequence();
        }

        private void OnDisable()
        {
            _targetTransform = null;
        }

        private void Update()
        {
            if (_targetTransform)
            {
                Vector2 travel = GetWorldPointToScreenPoint(_targetTransform) - _rectTransform.anchoredPosition;
                _rectTransform.Translate(travel * 10f * Time.deltaTime, _camera.transform);

                if (Vector2.Distance(_rectTransform.anchoredPosition, GetWorldPointToScreenPoint(_targetTransform)) < 25f)
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
        private void StartArchSequence()
        {
            CreateArchSequence();
            _archSequence.Play();
        }

        #region DOTWEEN FUNCTIONS
        private void CreateArchSequence()
        {
            if (_archSequence == null)
            {
                _archSequence = DOTween.Sequence();
                _archSequenceID = Guid.NewGuid();
                _archSequence.id = _archSequenceID;

                _archSequence.Append(_mesh.DOJumpAnchorPos(Vector2.one * -40f, Random.Range(-200, 400), 1, ARCH_SEQUENCE_DURATION))
                    .Join(_mesh.DORotate(new Vector3(30f, 0f, 0f), ARCH_SEQUENCE_DURATION))
                    .Join(_mesh.DOScale(Vector3.one * 0.9f, ARCH_SEQUENCE_DURATION))
                    .OnComplete(() => {
                        DeleteArchSequence();
                    });
            }
        }
        private void DeleteArchSequence()
        {
            DOTween.Kill(_archSequenceID);
            _archSequence = null;
        }
        #endregion
    }
}
