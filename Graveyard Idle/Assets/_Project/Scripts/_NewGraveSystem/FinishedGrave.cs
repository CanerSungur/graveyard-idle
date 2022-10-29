using System.Collections.Generic;
using UnityEngine;
using ZestCore.Utility;
using System;

namespace GraveyardIdle.GraveSystem
{
    public class FinishedGrave : MonoBehaviour
    {
        [Header("-- GRAVE PIECES SETUP --")]
        [SerializeField] private GravePiece[] gravePieces;
        private Dictionary<int, GravePiece> _gravePiecesDict = new Dictionary<int, GravePiece>();

        #region SCRIPT REFERENCES
        private Grave _grave;
        private FinishedGraveUpgradeHandler _upgradeHandler;
        private FinishedGraveMoneyHandler _moneyHandler;
        private FinishedGraveSpoilHandler _spoilHandler;
        #endregion

        #region LEVEL SECTION
        private const int MAX_LEVEL = 5;
        private int _currentLevel;
        #endregion

        #region UPGRADE SECTION
        private bool _playerIsInUpgradeArea, _playerIsInMaintenanceArea;
        #endregion

        #region PUBLICS
        public Grave Grave => _grave;
        public int CurrentLevel => _currentLevel;
        public bool PlayerIsInUpgradeArea => _playerIsInUpgradeArea;
        public bool PlayerIsInMaintenanceArea => _playerIsInMaintenanceArea;
        public FinishedGraveUpgradeHandler UpgradeHandler => _upgradeHandler;
        public FinishedGraveSpoilHandler SpoilHandler => _spoilHandler;
        public FinishedGraveMoneyHandler MoneyHandler => _moneyHandler;
        #endregion

        #region EVENTS
        public Action OnStartSpoiling, OnStopSpoiling, OnContinueSpoiling;
        #endregion

        public void Init(Grave grave)
        {
            if (_grave == null)
            {
                _grave = grave;
                _upgradeHandler = GetComponent<FinishedGraveUpgradeHandler>();
                _moneyHandler = GetComponent<FinishedGraveMoneyHandler>();
                _spoilHandler = GetComponent<FinishedGraveSpoilHandler>();

                InitializeGravePiecesDictionary();
            }

            Load();

            _upgradeHandler.Init(this);
            _moneyHandler.Init(this);
            _spoilHandler.Init(this);

            _playerIsInUpgradeArea = _playerIsInMaintenanceArea = false;
            CheckForUpgradeAreaActivation();
        }
        private void OnDisable()
        {
            if (_grave == null) return;
            Save();
        }

        #region PUBLICS
        public void EnteredUpgradeArea() => _playerIsInUpgradeArea = true;
        public void ExitedUpgradeArea() => _playerIsInUpgradeArea = false;
        public void EnteredMaintenanceArea() => _playerIsInMaintenanceArea = true;
        public void ExitedMaintenanceArea() => _playerIsInMaintenanceArea = false;
        public void UpgradeLevel()
        {
            if (_currentLevel < MAX_LEVEL)
            {
                _currentLevel++;

                // start closing current grave
                _spoilHandler.enabled = false;
                DisableCurrentGravePiece();
                // enable other grave with a delay(3 seconds)
                Delayer.DoActionAfterDelay(this, 3f, () => {
                    _spoilHandler.enabled = true;
                    _spoilHandler.Init(this);
                    EnableUpgradedGravePiece();
                    });
            }
        }
        #endregion

        #region HELPERS
        private void InitializeGravePiecesDictionary()
        {
            for (int i = 0; i < gravePieces.Length; i++)
            {
                if (!_gravePiecesDict.ContainsValue(gravePieces[i]))
                    _gravePiecesDict.Add(i + 1, gravePieces[i]);

                gravePieces[i].gameObject.SetActive(false);
            }
        }
        private void EnableRelevantGravePiece()
        {
            GravePiece gravePiece = _gravePiecesDict[_currentLevel];
            if (_currentLevel == 1)
            {
                gravePiece.gameObject.SetActive(true);
                gravePiece.Init(this);
            }
            else
            {
                GravePiece previousGravePiece = _gravePiecesDict[_currentLevel - 1];
                previousGravePiece.gameObject.SetActive(false);

                gravePiece.gameObject.SetActive(true);
                gravePiece.Init(this);
            }
        }
        private void CheckForUpgradeAreaActivation()
        {
            if (_currentLevel < MAX_LEVEL)
                _upgradeHandler.EnableUpgradeArea();
        }
        private void DisableCurrentGravePiece()
        {
            _upgradeHandler.DisableUpgradeArea();
            _gravePiecesDict[_currentLevel - 1].Disable();
        }
        private void EnableUpgradedGravePiece()
        {
            if (_currentLevel == MAX_LEVEL)
                _upgradeHandler.DisableUpgradeArea();
            else
                _upgradeHandler.EnableUpgradeArea();

            _gravePiecesDict[_currentLevel].gameObject.SetActive(true);
            _gravePiecesDict[_currentLevel].Init(this);
        }
        #endregion

        #region SAVE-LOAD FUNCTIONS
        private void Save()
        {
            PlayerPrefs.SetInt($"Grave-{_grave.ID}-CurrentLevel", _currentLevel);
            PlayerPrefs.Save();
        }
        private void Load()
        {
            _currentLevel = PlayerPrefs.GetInt($"Grave-{_grave.ID}-CurrentLevel", 1);
            EnableRelevantGravePiece();
        }
        #endregion
    }
}
