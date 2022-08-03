using System;
using System.Collections.Generic;
using UnityEngine;
using ZestCore.Utility;

namespace GraveyardIdle
{
    public class Grave : MonoBehaviour
    {
        private InteractableGround _interactableGround;
        private GraveUpgradeHandler _upgradeHandler;

        [Header("-- LEVEL SETUP --")]
        [SerializeField] private GravePiece[] gravePieces;

        private Dictionary<int, GravePiece> gravePiecesDict = new Dictionary<int, GravePiece>();

        private readonly int _maxLevel = 5;
        private int _level = 0;
        private bool _isBuilt = false;

        #region PROPERTIES
        public bool PlayerIsInUpgradeArea { get; set; }
        public int Level => _level;
        public int MaxLevel => _maxLevel;
        public InteractableGround InteractableGround => _interactableGround;
        #endregion

        #region EVENTS
        public Action OnFullLevel;
        #endregion

        public void Init(InteractableGround interactableGround)
        {
            _interactableGround = interactableGround;
            PlayerIsInUpgradeArea = false;

            _upgradeHandler = GetComponentInChildren<GraveUpgradeHandler>();
            _upgradeHandler.Init(this);

            _level = 0;
            _isBuilt = false;

            InitializeGravePiecesDictionary();

            _interactableGround.OnGraveBuilt += GetBuilt;
            _interactableGround.OnGraveUpgraded += Upgrade;
        }

        private void OnDisable()
        {
            if (!_interactableGround) return;
            _interactableGround.OnGraveBuilt -= GetBuilt;
            _interactableGround.OnGraveUpgraded -= Upgrade;
        }

        private void InitializeGravePiecesDictionary()
        {
            for (int i = 0; i < gravePieces.Length; i++)
            {
                if (!gravePiecesDict.ContainsValue(gravePieces[i]))
                    gravePiecesDict.Add(i + 1, gravePieces[i]);

                //gravePieces[i].Init(this);
                gravePieces[i].gameObject.SetActive(false);
            }

            //for (int i = 0; i < gravePiecesDict.Count; i++)
            //{
            //    Debug.Log($"{i+1}) {gravePiecesDict[i+1]} is Level {i + 1}");
            //}
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
        private void LoadGravePiece(int level)
        {
            for (int i = 1; i <= gravePiecesDict.Count; i++)
                gravePiecesDict[i].gameObject.SetActive(false);

            if (level == 0) return;
            Delayer.DoActionAfterDelay(this, 2f, () => gravePiecesDict[level].gameObject.SetActive(true));
        }
        private void GetBuilt()
        {
            if (_isBuilt) return;

            _isBuilt = true;
            _level = 1;

            EnableGravePiece(_level);
            _upgradeHandler.ActivateUpgradeArea();
        }
        private void Upgrade()
        {
            if (_level > 0)
                gravePiecesDict[_level].Close();

            _level++;
            EnableGravePiece(_level);

            if (_level == _maxLevel)
            {
                // Disable upgrade
                OnFullLevel?.Invoke();
                _upgradeHandler.DeActivateUpgradeArea();
            }
        }
        private void LoadGraveData(int level)
        {
            _level = level;

            LoadGravePiece(_level);
        }
    }
}
