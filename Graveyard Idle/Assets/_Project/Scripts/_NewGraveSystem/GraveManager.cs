using System.Collections.Generic;
using UnityEngine;
using System;
using ZestGames;

namespace GraveyardIdle.GraveSystem
{
    public class GraveManager : MonoBehaviour
    {
        public static GraveManager Instance { get; private set; }

        [Header("-- GRAVE SETUP --")]
        [SerializeField] private List<Grave> graves;

        #region TOTAL COUNT SECTION
        private int _activatedGraveCount, _dugGraveCount, _beingCarriedCoffinCount, _coffinCanBeSpawnedCount;
        #endregion

        #region GETTERS
        public List<Grave> Graves => graves;
        public int DugGraveCount => _dugGraveCount;
        public bool CanCoffinBeCarried => _dugGraveCount - _beingCarriedCoffinCount > 0;
        public int CoffinCanBeSpawnedCount => _coffinCanBeSpawnedCount;
        #endregion

        #region EMPTY GRAVE SECTION
        private List<Grave> _emptyGraves;
        public List<Grave> EmptyGraves => _emptyGraves == null ? _emptyGraves = new List<Grave>() : _emptyGraves;
        public void AddEmptyGrave(Grave grave)
        {
            if (!EmptyGraves.Contains(grave))
                EmptyGraves.Add(grave);
        }
        public void RemoveEmptyGrave(Grave grave)
        {
            if (EmptyGraves.Contains(grave))
                EmptyGraves.Remove(grave);
        }
        #endregion

        //private void Start()
        //{
        //    _activatedGraveCount = _dugGraveCount = 0;

        //    for (int i = 0; i < graves.Count; i++)
        //    {
        //        graves[i].Init(this, i);
        //    }
        //}

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public void Init(GameManager gameManager)
        {
            _activatedGraveCount = _dugGraveCount = _coffinCanBeSpawnedCount = _beingCarriedCoffinCount = 0;

            for (int i = 0; i < graves.Count; i++)
            {
                graves[i].Init(this, i);
            }

            GraveManagerEvents.OnGraveActivated += GraveActivated;
            GraveManagerEvents.OnGraveDigged += GraveDigged;
            GraveManagerEvents.OnCoffinThrownToGrave += CoffinThrownToGrave;
            GraveManagerEvents.OnCoffinPickedUp += StartedCarryingCoffin;
            GraveManagerEvents.OnEmptyCoffinSold += EmptyCoffinSold;
            GraveManagerEvents.OnCoffinSpawned += CoffinSpawned;
        }

        private void OnDisable()
        {
            GraveManagerEvents.OnGraveActivated -= GraveActivated;
            GraveManagerEvents.OnGraveDigged -= GraveDigged;
            GraveManagerEvents.OnCoffinThrownToGrave -= CoffinThrownToGrave;
            GraveManagerEvents.OnCoffinPickedUp -= StartedCarryingCoffin;
            GraveManagerEvents.OnEmptyCoffinSold -= EmptyCoffinSold;
            GraveManagerEvents.OnCoffinSpawned -= CoffinSpawned;
        }

        #region EVENT HANDLER FUNCTIONS
        private void GraveActivated() => _activatedGraveCount++;
        private void GraveDigged() => _dugGraveCount++;
        private void CoffinThrownToGrave()
        {
            _dugGraveCount--;
            _beingCarriedCoffinCount--;
        }
        private void StartedCarryingCoffin() => _beingCarriedCoffinCount++;
        private void EmptyCoffinSold() => _coffinCanBeSpawnedCount++;
        private void CoffinSpawned() => _coffinCanBeSpawnedCount--;
        #endregion

        #region PUBLICS
        public void GraveIsActivated()
        {
            _activatedGraveCount++;
        }
        public void GraveIsDug()
        {
            _dugGraveCount++;
        }
        #endregion
    }
}
