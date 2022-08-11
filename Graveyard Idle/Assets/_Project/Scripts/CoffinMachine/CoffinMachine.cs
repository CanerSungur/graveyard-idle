using System;
using UnityEngine;
using ZestCore.Utility;
using ZestGames;

namespace GraveyardIdle
{
    public class CoffinMachine : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private Transform middlePoint;
        [SerializeField] private Transform endPoint;

        #region SCRIPT REFERENCES
        private BuyCoffinArea _buyCoffinArea;
        public BuyCoffinArea BuyCoffinArea => _buyCoffinArea == null ? _buyCoffinArea = GetComponentInChildren<BuyCoffinArea>() : _buyCoffinArea;
        private CoffinMachineTimerHandler _timerHandler;
        public CoffinMachineTimerHandler TimerHandler => _timerHandler == null ? _timerHandler = GetComponent<CoffinMachineTimerHandler>() : _timerHandler;
        private CoffinMachineStackHandler _stackHandler;
        public CoffinMachineStackHandler StackHandler => _stackHandler == null ? _stackHandler = GetComponent<CoffinMachineStackHandler>() : _stackHandler;
        private Log _log;
        public Log Log => _log == null ? _log = GetComponentInChildren<Log>() : _log;
        #endregion

        #region PROPERTIES
        public Transform MiddlePoint => middlePoint;
        public Transform EndPoint => endPoint;
        public bool CanMakeCoffin => CurrentCount < Capacity && DataManager.TotalMoney > 0 && !TimerHandler.IsMakingCoffin;
        public float CoffinMakeTime { get; private set; }
        public int Capacity { get; private set; }
        public int CurrentCount { get; private set; }
        public int CurrentRowCount { get; private set; }
        public int CurrentColumnCount { get; private set; }
        public int CurrentHeightCount { get; private set; }
        public int RowLength { get; private set; }
        public int ColumnLength { get; private set; }
        #endregion

        #region EVENTS
        public Action OnStartMachine;
        #endregion

        private void OnEnable()
        {
            Capacity = 5;
            CoffinMakeTime = 10f;
            CurrentCount = CurrentRowCount = CurrentColumnCount = CurrentHeightCount = 0;
            RowLength = 3;
            ColumnLength = 1;

            BuyCoffinArea.Init(this);
            TimerHandler.Init(this);
            StackHandler.Init(this);
            Log.gameObject.SetActive(false);

            Delayer.DoActionAfterDelay(this, 2f, () => TruckEvents.OnEnableSellTruck?.Invoke(this));

            //CoffinMachineEvents.OnStartMakingACoffin += HandleStartMakingACoffin;
            //CoffinMachineEvents.OnMadeACoffin += SpawnACoffin;
            //CoffinMachineEvents.OnACoffinTaken += HandleCoffinUnStack;
            CoffinMachineEvents.OnStackedEmptyCoffin += HandleCoffinStack;
            CoffinMachineEvents.OnUnStackedEmptyCoffin += HandleCoffinUnStack;
        }

        private void OnDisable()
        {
            //CoffinMachineEvents.OnStartMakingACoffin -= HandleStartMakingACoffin;
            //CoffinMachineEvents.OnMadeACoffin -= SpawnACoffin;
            //CoffinMachineEvents.OnACoffinTaken -= HandleCoffinUnStack;

            CoffinMachineEvents.OnStackedEmptyCoffin -= HandleCoffinStack;
            CoffinMachineEvents.OnUnStackedEmptyCoffin -= HandleCoffinUnStack;
        }

        private void HandleStartMakingACoffin()
        {
            //TimerHandler.StartFilling(() => SpawnACoffin());
        }
        private void HandleCoffinStack()
        {
            CurrentCount++;
            CurrentRowCount++;

            if (CurrentRowCount == RowLength)
            {
                CurrentRowCount = 0;
                CurrentColumnCount++;
            }

            if (CurrentColumnCount == ColumnLength)
            {
                CurrentColumnCount = 0;
                CurrentHeightCount++;
            }
        }
        private void HandleCoffinUnStack()
        {
            if (CurrentRowCount == 0)
            {
                if (CurrentColumnCount == 0 && CurrentHeightCount > 0)
                {
                    CurrentHeightCount--;
                    CurrentColumnCount = ColumnLength - 1;
                    CurrentRowCount = RowLength;
                }
                else
                {
                    CurrentColumnCount--;
                    CurrentRowCount = RowLength;
                }
            }

            CurrentCount--;
            CurrentRowCount--;
        }
        #region PUBLICS
        public void ActivateLog()
        {
            Log.gameObject.SetActive(true);
            Log.Init(this);

            CoffinMakerEvents.OnGoToMachine?.Invoke();
        }
        public void SpawnEmptyCoffin()
        {
            EmptyCoffin emptyCoffin = ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.EmptyCoffin, Vector3.zero, Quaternion.Euler(0f, 90f, 0f)).GetComponent<EmptyCoffin>();
            emptyCoffin.Init(this);
        }
        public void GiveEmptyCoffin(Transform transform)
        {
            EmptyCoffin emptyCoffin = CoffinMachineStackHandler.EmptyCoffinsInArea[CoffinMachineStackHandler.EmptyCoffinsInArea.Count - 1];
            CoffinMachineStackHandler.RemoveEmptyCoffin(emptyCoffin);
            emptyCoffin.GoToTruck(transform);
        }
        #endregion
    }
}
