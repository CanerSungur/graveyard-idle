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

        [Header("-- SPOIL CANVAS SETUP --")]
        [SerializeField] private GameObject spoilCanvas;
        [SerializeField] private Image spoilFill;

        private FinishedGrave _finishedGrave;

        private const float CORE_SPOIL_SPEED = 0.05f;
        private const float SPOIL_SPEED_DECREASERATE = 0.005f;
        private const int SPOIL_CHANCE = 50;

        #region PROPERTIES
        public float CurrentSpoilRate { get; set; }
        public bool IsSpoiling { get; private set; }
        //public bool CanSpoil => _grave.IsBuilt;
        public bool CanBeWatered => CurrentSpoilRate < 0.9f;
        public float SpoilSpeed => CORE_SPOIL_SPEED - (_finishedGrave.CurrentLevel * SPOIL_SPEED_DECREASERATE);
        #endregion

        public void Init(FinishedGrave finishedGrave)
        {
            if (_finishedGrave == null)
            {
                _finishedGrave = finishedGrave;
            }

            IsSpoiling = false;
            fliesParticle.Stop();
            CurrentSpoilRate = 0f;

            spoilFill.fillAmount = 0;
            spoilCanvas.SetActive(false);

            _finishedGrave.OnStartSpoiling += StartSpoiling;
            _finishedGrave.OnStopSpoiling += StopSpoiling;
            WateringCanEvents.OnMaintenanceIsSuccessfull += DisableCanvas;

            StartCoroutine(CheckForSpoil());
        }
        #region MONO FUNCTIONS
        private void OnDisable()
        {
            if (_finishedGrave == null) return;

            _finishedGrave.OnStartSpoiling -= StartSpoiling;
            _finishedGrave.OnStopSpoiling -= StopSpoiling;
            WateringCanEvents.OnMaintenanceIsSuccessfull -= DisableCanvas;
        }
        private void Update()
        {
            spoilFill.fillAmount = 1 - CurrentSpoilRate;
        }
        #endregion

        #region EVENT HANDLERS
        private void StartSpoiling()
        {
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
        }
        #endregion

        #region COROUTINES
        private IEnumerator CheckForSpoil()
        {
            while (true)
            {
                if (/*_finishedGrave.IsBuilt &&*/ !IsSpoiling && !Player.IsMaintenancing && RNG.RollDice(SPOIL_CHANCE))
                    _finishedGrave.OnStartSpoiling?.Invoke();

                yield return new WaitForSeconds(10f);
            }
        }
        #endregion
    }
}
