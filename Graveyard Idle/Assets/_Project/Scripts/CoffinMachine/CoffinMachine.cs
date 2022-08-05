using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class CoffinMachine : MonoBehaviour
    {
        #region SCRIPT REFERENCES
        private BuyCoffinArea _buyCoffinArea;
        public BuyCoffinArea BuyCoffinArea => _buyCoffinArea == null ? _buyCoffinArea = GetComponentInChildren<BuyCoffinArea>() : _buyCoffinArea;
        private CoffinMachineTimerHandler _timerHandler;
        public CoffinMachineTimerHandler TimerHandler => _timerHandler == null ? _timerHandler = GetComponent<CoffinMachineTimerHandler>() : _timerHandler;
        private CoffinMachineStackHandler _stackHandler;
        public CoffinMachineStackHandler StackHandler => _stackHandler == null ? _stackHandler = GetComponent<CoffinMachineStackHandler>() : _stackHandler;
        #endregion

        #region PROPERTIES
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

        private void OnEnable()
        {
            Capacity = 5;
            CoffinMakeTime = 5f;
            CurrentCount = CurrentRowCount = CurrentColumnCount = CurrentHeightCount = 0;
            RowLength = 3;
            ColumnLength = 1;

            BuyCoffinArea.Init(this);
            TimerHandler.Init(this);
            StackHandler.Init(this);

            //CoffinMachineEvents.OnStartMakingACoffin += HandleStartMakingACoffin;
            //CoffinMachineEvents.OnMadeACoffin += SpawnACoffin;
            //CoffinMachineEvents.OnACoffinTaken += HandleCoffinUnStack;
        }

        private void OnDisable()
        {
            //CoffinMachineEvents.OnStartMakingACoffin -= HandleStartMakingACoffin;
            //CoffinMachineEvents.OnMadeACoffin -= SpawnACoffin;
            //CoffinMachineEvents.OnACoffinTaken -= HandleCoffinUnStack;
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
        public void SpawnACoffin()
        {
            Debug.Log("spawned empty coffin");
            GameObject emptyCoffin = ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.EmptyCoffin, Vector3.zero, Quaternion.identity, StackHandler.StackContainer);
            emptyCoffin.transform.localPosition = StackHandler.TargetStackPosition;
            emptyCoffin.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

            HandleCoffinStack();
        }
        #endregion
    }
}
