using UnityEngine;
using ZestCore.Utility;
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
        public bool CanSpawn => GraveSystem.GraveManager.Instance.CoffinCanBeSpawnedCount > 0 && GameManager.GameState == Enums.GameState.Started && TruckManager.ThereIsAvailableTruck;
        public int CurrentCount { get; private set; }
        public int CurrentRowCount { get; private set; }
        public int CurrentColumnCount { get; private set; }
        public int CurrentHeightCount { get; private set; }
        public int RowLength { get; private set; }
        public int ColumnLength { get; private set; }
        #endregion

        #region STATICS
        public static Transform[] CarrierTakeTransforms;
        public static Transform CoffinThrowPointForCarriers;
        #endregion

        private int _coffinCount;

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

            Delayer.DoActionAfterDelay(this, 0.5f, Load);
        }

        private void OnDisable()
        {
            CoffinAreaEvents.OnStackedCoffin -= HandleCoffinStack;
            CoffinAreaEvents.OnUnStackedCoffin -= HandleCoffinUnStack;

            Save();
        }

        #region EVENT HANDLER FUNCTIONS
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
            _coffinCount--;
        }
        #endregion

        #region SAVE-LOAD FUNCTIONS
        private void Save()
        {
            _coffinCount = CurrentCount;
            PlayerPrefs.SetInt("CoffinArea_CoffinCount", _coffinCount);
            PlayerPrefs.Save();
        }
        private void Load()
        {
            _coffinCount = PlayerPrefs.GetInt("CoffinArea_CoffinCount", 0);

            if (_coffinCount <= 0) return;

            for (int i = 0; i < _coffinCount; i++)
            {
                SpawnHandler.SpawnCoffinForInit();
                GraveManagerEvents.OnCheckForCarrierActivation?.Invoke();
            }
        }
        #endregion
    }
}
