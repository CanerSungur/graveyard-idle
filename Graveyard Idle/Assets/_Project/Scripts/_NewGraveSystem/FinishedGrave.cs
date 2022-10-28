using System.Collections.Generic;
using UnityEngine;

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

        #region PUBLICS
        public Grave Grave => _grave;
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
        }

        private void OnDisable()
        {
            if (_grave == null) return;
            Save();
        }

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
                //gravePiece.Init(this);
            }
            else
            {
                GravePiece previousGravePiece = _gravePiecesDict[_currentLevel - 1];
                previousGravePiece.gameObject.SetActive(false);

                gravePiece.gameObject.SetActive(true);
                //gravePiece.Init(this);
            }
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
            _currentLevel = 5;
            EnableRelevantGravePiece();
        }
        #endregion
    }
}
