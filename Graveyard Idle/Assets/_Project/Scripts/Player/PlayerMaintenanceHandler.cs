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

        private readonly int _maintenanceRewardMoney = 10;

        public void Init(Player player)
        {
            _player = player;

            WateringCanEvents.OnStartedWatering += StartRewarding;
            WateringCanEvents.OnStoppedWatering += StopRewarding;
            WateringCanEvents.OnMaintenanceSuccessfull += MaintenanceSuccessfull;
        }

        private void OnDisable()
        {
            if (!_player) return;

            WateringCanEvents.OnStartedWatering -= StartRewarding;
            WateringCanEvents.OnStoppedWatering -= StopRewarding;
            WateringCanEvents.OnMaintenanceSuccessfull -= MaintenanceSuccessfull;
        }

        private void StartRewarding()
        {
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

        #region COROUTINE
        private IEnumerator WateringRewardCoroutine()
        {
            while (true)
            {
                yield return _waitForNormalRewardDelay;
                MoneyCanvas.Instance.SpawnCollectMoney(transform);
            }
        }
        private IEnumerator SpawnWateringFinishedRewardMoney()
        {
            int currentCount = 0;
            while (currentCount < _maintenanceRewardMoney)
            {
                MoneyCanvas.Instance.SpawnCollectMoney(transform);
                currentCount++;

                yield return _waitForSuccessRewardDelay;
            }
        }
        #endregion

        #region PUBLICS
        public void StartMaintenance(Grave grave)
        {
            grave.OnStopSpoiling?.Invoke();
        }
        #endregion
    }
}
