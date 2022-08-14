using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class CoffinArea : MonoBehaviour
    {
        #region SCRIPT REFERENCES
        private CoffinAreaStackHandler _stackHandler;
        public CoffinAreaStackHandler StackHandler => _stackHandler == null ? _stackHandler = GetComponent<CoffinAreaStackHandler>() : _stackHandler;
        private CoffinAreaSpawnHandler _spawnHandler;
        public CoffinAreaSpawnHandler SpawnHandler => _spawnHandler == null ? _spawnHandler = GetComponent<CoffinAreaSpawnHandler>() : _spawnHandler;
        private CoffinAreaTakeHandler _takeHandler;
        public CoffinAreaTakeHandler TakeHandler => _takeHandler == null ? _takeHandler = GetComponent<CoffinAreaTakeHandler>() : _takeHandler;
        #endregion

        [Header("-- SETUP --")]
        [SerializeField] private int capacity = 10;
        [SerializeField] private Transform coffinThrowPointForCarriers;
        [SerializeField] private Transform[] carrierTakeTransforms;

        #region PROPERTIES
        public Player Player { get; private set; }
        public bool CanSpawn => GraveManager.CoffinCanBeSpawnedCount > 0 && GameManager.GameState == Enums.GameState.Started && TruckManager.ThereIsAvailableTruck;
        public int CurrentCount { get; private set; }
        public int CurrentRowCount { get; private set; }
        public int CurrentColumnCount { get; private set; }
        public int CurrentHeightCount { get; private set; }
        public int RowLength { get; private set; }
        public int ColumnLength { get; private set; }
        #endregion

        public static Transform[] CarrierTakeTransforms;
        public static Transform CoffinThrowPointForCarriers;

        private void Start()
        {
            Player = FindObjectOfType<Player>();
            CarrierTakeTransforms = carrierTakeTransforms;
            CoffinThrowPointForCarriers = coffinThrowPointForCarriers;

            CurrentCount = CurrentRowCount = CurrentColumnCount = CurrentHeightCount = 0;
            RowLength = 4;
            ColumnLength = 2;

            StackHandler.Init(this);
            SpawnHandler.Init(this);
            TakeHandler.Init(this);

            CoffinAreaEvents.OnStackedCoffin += HandleCoffinStack;
            CoffinAreaEvents.OnUnStackedCoffin += HandleCoffinUnStack;
        }

        private void OnDisable()
        {
            CoffinAreaEvents.OnStackedCoffin -= HandleCoffinStack;
            CoffinAreaEvents.OnUnStackedCoffin -= HandleCoffinUnStack;
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
    }
}
