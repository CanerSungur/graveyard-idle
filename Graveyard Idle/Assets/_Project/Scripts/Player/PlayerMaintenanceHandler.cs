using System.Collections;
using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class PlayerMaintenanceHandler : MonoBehaviour
    {
        private Player _player;

        private readonly WaitForSeconds _waitForNormalRewardDelay = new WaitForSeconds(1f);
        private readonly WaitForSeconds _waitForSuccessRewardDelay = new WaitForSeconds(0.05f);
        private IEnumerator _wateringRewardCoroutine;

        private const int MAX_MAINTENANCE_REWARD_MONEY= 10;
        private int _currentMaintenanceRewardMoney = 0;

        public void Init(Player player)
        {
            _player = player;

            WateringCanEvents.OnStartedWatering += StartRewarding;
            WateringCanEvents.OnStoppedWatering += StopRewarding;
            WateringCanEvents.OnMaintenanceSuccessfull += MaintenanceSuccessfull;
            WateringCanEvents.OnMaintenanceIsSuccessfull += MaintenanceIsSuccessfull;
        }

        private void OnDisable()
        {
            if (!_player) return;

            WateringCanEvents.OnStartedWatering -= StartRewarding;
            WateringCanEvents.OnStoppedWatering -= StopRewarding;
            WateringCanEvents.OnMaintenanceSuccessfull -= MaintenanceSuccessfull;
            WateringCanEvents.OnMaintenanceIsSuccessfull -= MaintenanceIsSuccessfull;
        }

        private void StartRewarding()
        {
            _currentMaintenanceRewardMoney = 0;
            _wateringRewardCoroutine = WateringRewardCoroutine();
            StartCoroutine(_wateringRewardCoroutine);
        }
        private void StopRewarding()
        {
            StopCoroutine(_wateringRewardCoroutine);
        }
        private void MaintenanceSuccessfull(Grave grave)
        {
            StopRewarding();
            // reward total money;
            StartCoroutine(SpawnWateringFinishedRewardMoney());
        }
        private void MaintenanceIsSuccessfull(GraveyardIdle.GraveSystem.FinishedGrave finishedGrave)
        {
            StopRewarding();
            StartCoroutine(SpawnWateringFinishedRewardMoney());
        }

        #region COROUTINE
        private IEnumerator WateringRewardCoroutine()
        {
            while (true)
            {
                yield return _waitForNormalRewardDelay;
                MoneyCanvas.Instance.SpawnCollectMoney(transform);
                _currentMaintenanceRewardMoney++;
            }
        }
        private IEnumerator SpawnWateringFinishedRewardMoney()
        {
            if (_currentMaintenanceRewardMoney >= MAX_MAINTENANCE_REWARD_MONEY)
                _currentMaintenanceRewardMoney = MAX_MAINTENANCE_REWARD_MONEY;

            while (_currentMaintenanceRewardMoney > 0)
            {
                MoneyCanvas.Instance.SpawnCollectMoney(transform);
                _currentMaintenanceRewardMoney--;

                yield return _waitForSuccessRewardDelay;
            }
        }
        #endregion

        #region PUBLICS
        //public void StartMaintenance(Grave grave)
        //{
        //    grave.OnStopSpoiling?.Invoke();
        //}
        public void StartMaintenance(GraveyardIdle.GraveSystem.FinishedGrave finishedGrave)
        {
            finishedGrave.OnStopSpoiling?.Invoke();
        }
        #endregion
    }
}
