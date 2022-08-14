using System;
using System.Collections.Generic;
using UnityEngine;
using ZestCore.Utility;

namespace GraveyardIdle
{
    public class Grave : MonoBehaviour
    {
        private InteractableGround _interactableGround;

        #region SCRIPT REFERENCES
        private GraveUpgradeHandler _upgradeHandler;
        public GraveUpgradeHandler UpgradeHandler => _upgradeHandler == null ? _upgradeHandler = GetComponentInChildren<GraveUpgradeHandler>() : _upgradeHandler;
        private GraveMoneyHandler _moneyHandler;
        public GraveMoneyHandler MoneyHandler => _moneyHandler == null ? _moneyHandler = GetComponent<GraveMoneyHandler>() : _moneyHandler;
        private GraveSpoilHandler _spoilHandler;
        public GraveSpoilHandler SpoilHandler => _spoilHandler == null ? _spoilHandler = GetComponent<GraveSpoilHandler>() : _spoilHandler;
        #endregion

        [Header("-- LEVEL SETUP --")]
        [SerializeField] private GravePiece[] gravePieces;
        private Dictionary<int, GravePiece> gravePiecesDict = new Dictionary<int, GravePiece>();

        [Header("-- CARRIER DROP SETUP --")]
        [SerializeField] private Transform carrierDropPoint;

        private readonly int _maxLevel = 5;
        private int _level = 0;
        private bool _isBuilt = false;

        #region PROPERTIES
        public bool CarriersAssigned { get; set; }
        public bool IsBuilt => _isBuilt;
        public bool PlayerIsInUpgradeArea { get; set; }
        public bool PlayerIsInMaintenanceArea { get; set; }
        public int Level => _level;
        public int MaxLevel => _maxLevel;
        public InteractableGround InteractableGround => _interactableGround;
        public Transform CarrierDropPoint => carrierDropPoint;
        #endregion

        #region EVENTS
        public Action OnStartSpoiling, OnStopSpoiling;
        #endregion

        public void Init(InteractableGround interactableGround)
        {
            _interactableGround = interactableGround;
            PlayerIsInUpgradeArea = PlayerIsInMaintenanceArea = CarriersAssigned = false;
            InitializeGravePiecesDictionary();

            LoadData();

            UpgradeHandler.Init(this);
            MoneyHandler.Init(this);
            SpoilHandler.Init(this);

            _interactableGround.OnGraveBuilt += GetBuilt;
            _interactableGround.OnGraveUpgraded += Upgrade;
        }

        private void OnDisable()
        {
            if (!_interactableGround) return;
            _interactableGround.OnGraveBuilt -= GetBuilt;
            _interactableGround.OnGraveUpgraded -= Upgrade;

            SaveData();
        }

        private void GetBuilt()
        {
            if (_isBuilt) return;

            _isBuilt = true;
            _level = 1;

            EnableGravePiece(_level);
            UpgradeHandler.ActivateUpgradeArea();

            //if (!SpoilHandler.IsSpoiling)
            //    OnStartSpoiling?.Invoke();
        }
        private void Upgrade()
        {
            if (_level > 0)
                gravePiecesDict[_level].Close();

            _level++;
            EnableGravePiece(_level);
            UpgradeHandler.UpdateRemainingMoneyText();

            if (_level == _maxLevel)
            {
                UpgradeHandler.DeActivateUpgradeArea();
            }
        }

        #region HELPERS
        private void InitializeGravePiecesDictionary()
        {
            for (int i = 0; i < gravePieces.Length; i++)
            {
                if (!gravePiecesDict.ContainsValue(gravePieces[i]))
                    gravePiecesDict.Add(i + 1, gravePieces[i]);

                gravePieces[i].gameObject.SetActive(false);
            }
        }
        private void EnableGravePiece(int level)
        {
            if (level == 0) return;
            if (level == 1)
            {
                gravePiecesDict[level].gameObject.SetActive(true);
                gravePiecesDict[level].Init(this);
            }
            else
            {

                Delayer.DoActionAfterDelay(this, 3f, () => {
                    gravePiecesDict[level].gameObject.SetActive(true);
                    gravePiecesDict[level].Init(this);
                });
            }
        }
        private void LoadGravePiece(int currentLevel)
        {
            for (int i = 1; i <= gravePiecesDict.Count; i++)
                gravePiecesDict[i].gameObject.SetActive(false);

            gravePiecesDict[currentLevel].gameObject.SetActive(true);
            gravePiecesDict[currentLevel].Init(this);
        }
        #endregion

        #region SAVE-LOAD
        private void LoadData()
        {
            _level = PlayerPrefs.GetInt($"Grave-{_interactableGround.ID}-Level", 0);
            
            if (_level != 0)
                LoadGravePiece(_level);

            _isBuilt = _level > 0;
        }
        private void SaveData()
        {
            PlayerPrefs.SetInt($"Grave-{_interactableGround.ID}-Level", _level);
            PlayerPrefs.Save();
        }
        private void OnApplicationQuit()
        {
            if (!_interactableGround) return;
            SaveData();
        }
        private void OnApplicationPause(bool pause)
        {
            if (!_interactableGround) return;
            SaveData();
        }
        #endregion
    }
}
