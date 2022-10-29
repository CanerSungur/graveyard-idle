using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ZestCore.Utility;
using ZestGames;

namespace GraveyardIdle.GraveSystem
{
    public class FinishedGraveSpoilHandler : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private ParticleSystem fliesParticle;
        private ParticleSystem.EmissionModule _fliesParticleEmission;

        [Header("-- SPOIL CANVAS SETUP --")]
        [SerializeField] private GameObject spoilCanvas;
        [SerializeField] private Image spoilFill;

        private FinishedGrave _finishedGrave;

        #region SPOIL DATA
        private const float CORE_SPOIL_SPEED = 0.05f;
        private const float SPOIL_SPEED_DECREASERATE = 0.005f;
        private const int CORE_SPOIL_CHANCE = 50;
        private int _spoilChance => CORE_SPOIL_CHANCE - (_finishedGrave.CurrentLevel * 2);
        #endregion

        #region FLIES PARTICLE DATA
        private const float MIN_EMISSION_RATE = 0f;
        private const float MAX_EMISSION_RATE = 20f;
        private float _currentEmissionRate;
        #endregion

        #region PROPERTIES
        public float CurrentSpoilRate { get; set; }
        public bool IsSpoiling { get; private set; }
        public bool MaintenanceIsSuccessfull { get; private set; }
        //public bool CanSpoil => _grave.IsBuilt;
        public bool CanBeWatered => CurrentSpoilRate < 0.9f;
        public float SpoilSpeed => CORE_SPOIL_SPEED - (_finishedGrave.CurrentLevel * SPOIL_SPEED_DECREASERATE);
        #endregion

        public void Init(FinishedGrave finishedGrave)
        {
            if (_finishedGrave == null)
            {
                _finishedGrave = finishedGrave;
                _fliesParticleEmission = fliesParticle.emission;
            }

            MaintenanceIsSuccessfull = true;
            IsSpoiling = false;
            _currentEmissionRate = 0;
            SetFliesParticleEmissionRate(_currentEmissionRate);
            CurrentSpoilRate = 0f;

            spoilFill.fillAmount = 0;
            spoilCanvas.SetActive(false);

            _finishedGrave.OnStartSpoiling += StartSpoiling;
            _finishedGrave.OnContinueSpoiling += StartSpoiling;
            _finishedGrave.OnStopSpoiling += StopSpoiling;
            WateringCanEvents.OnMaintenanceIsSuccessfull += DisableCanvas;

            StartCoroutine(CheckForSpoil());
        }
        #region MONO FUNCTIONS
        private void OnDisable()
        {
            if (_finishedGrave == null) return;

            _finishedGrave.OnStartSpoiling -= StartSpoiling;
            _finishedGrave.OnContinueSpoiling -= StartSpoiling;
            _finishedGrave.OnStopSpoiling -= StopSpoiling;
            WateringCanEvents.OnMaintenanceIsSuccessfull -= DisableCanvas;
        }
        private void Update()
        {
            spoilFill.fillAmount = 1 - CurrentSpoilRate;

            if (IsSpoiling)
            {
                _currentEmissionRate = Mathf.Lerp(_currentEmissionRate, MAX_EMISSION_RATE, SpoilSpeed * Time.deltaTime);
                SetFliesParticleEmissionRate(_currentEmissionRate);
            }
            else if (Player.IsMaintenancing && _finishedGrave.PlayerIsInMaintenanceArea)
            {
                _currentEmissionRate = Mathf.Lerp(_currentEmissionRate, MIN_EMISSION_RATE, DataManager.MaintenanceSpeed * Time.deltaTime);
                SetFliesParticleEmissionRate(_currentEmissionRate);
            }
        }
        #endregion

        #region EVENT HANDLERS
        private void StartSpoiling()
        {
            MaintenanceIsSuccessfull = false;
            IsSpoiling = true;
            if (!spoilCanvas.activeSelf)
                spoilCanvas.SetActive(true);
        }
        private void StopSpoiling()
        {
            IsSpoiling = false;
        }
        private void DisableCanvas(FinishedGrave finishedGrave)
        {
            if (_finishedGrave != finishedGrave) return;
            spoilCanvas.SetActive(false);
            MaintenanceIsSuccessfull = true;
        }
        #endregion

        #region HELPERS
        private void SetFliesParticleEmissionRate(float rate) => _fliesParticleEmission.rateOverTime = rate;
        #endregion

        #region COROUTINES
        private IEnumerator CheckForSpoil()
        {
            while (true)
            {
                if (/*_finishedGrave.IsBuilt &&*/MaintenanceIsSuccessfull && !IsSpoiling && !Player.IsMaintenancing && RNG.RollDice(_spoilChance))
                    _finishedGrave.OnStartSpoiling?.Invoke();

                yield return new WaitForSeconds(10f);
            }
        }
        #endregion
    }
}
