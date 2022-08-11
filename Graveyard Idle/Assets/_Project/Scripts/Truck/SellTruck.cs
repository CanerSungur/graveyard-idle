using UnityEngine;
using System;
using Random = UnityEngine.Random;
using ZestGames;
using System.Collections;

namespace GraveyardIdle
{
    public class SellTruck : MonoBehaviour
    {
        [Header("-- APPEREANCE SETUP --")]
        [SerializeField] private Material[] materials;
        [SerializeField] private MeshRenderer meshRenderer;

        [Header("-- POINT SETUP --")]
        [SerializeField] private Transform startPoint;
        [SerializeField] private Transform emptyCoffinStackTransform;

        public EmptyCoffin CarriedEmptyCoffin { get; set; }
        private CoffinMachine _activatorCoffinMachine = null;
        public CoffinMachine ActivatorCoffinMachine => _activatorCoffinMachine;

        #region SCRIPT REFERENCES
        private SellTruckAnimationController _animationController;
        public SellTruckAnimationController AnimationController => _animationController == null ? _animationController = GetComponent<SellTruckAnimationController>() : _animationController;
        private SellTruckMoneyHandler _moneyHandler;
        public SellTruckMoneyHandler MoneyHandler => _moneyHandler == null ? _moneyHandler = GetComponent<SellTruckMoneyHandler>() : _moneyHandler;
        #endregion

        #region FLAGS
        private bool _initialized, _hasCoffin, _isBusy = false;
        #endregion

        #region EVENTS
        public Action OnMove, OnIdle, OnDrop, OnTurnAround;
        #endregion

        private readonly WaitForSeconds _waitForAskCoffinDelay = new WaitForSeconds(5f);

        public void Init(CoffinMachine coffinMachine)
        {
            _initialized = true;
            _activatorCoffinMachine = coffinMachine;
            transform.position = startPoint.position;
            meshRenderer.material = materials[Random.Range(0, materials.Length)];

            if (CarriedEmptyCoffin)
                CarriedEmptyCoffin.gameObject.SetActive(false);

            _isBusy = _hasCoffin = false;

            AnimationController.Init(this);
            MoneyHandler.Init(this);
            StartCoroutine(AskForEmptyCoffin());
        }

        private void OnEnable()
        {
            _initialized = false;
        }

        private void OnDisable()
        {
            _isBusy = false;
        }

        private IEnumerator AskForEmptyCoffin()
        {
            while (true)
            {
                if (CoffinMachineStackHandler.EmptyCoffinsInArea.Count > 0 && !_isBusy)
                {
                    // take a coffin
                    _isBusy = true;
                    CarriedEmptyCoffin = CoffinMachineStackHandler.EmptyCoffinsInArea[CoffinMachineStackHandler.EmptyCoffinsInArea.Count - 1];
                    _activatorCoffinMachine.GiveEmptyCoffin(emptyCoffinStackTransform);
                    
                    GraveManagerEvents.OnEmptyCoffinSold?.Invoke();
                    MoneyHandler.StartSpawningMoney();

                    OnMove?.Invoke();
                }
                
                yield return _waitForAskCoffinDelay;
            }
        }
    }
}
