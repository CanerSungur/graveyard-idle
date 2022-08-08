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
        private IEnumerator _rewardCoroutine;

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
            _rewardCoroutine = RewardCoroutine();
            StartCoroutine(_rewardCoroutine);
        }
        private void StopRewarding()
        {
            StopCoroutine(_rewardCoroutine);
        }
        private void MaintenanceSuccessfull(Grave grave)
        {
            StopRewarding();
            // reward total money;
            StartCoroutine(SpawnRewardMoney());
        }

        #region COROUTINE
        private IEnumerator RewardCoroutine()
        {
            while (true)
            {
                MoneyCanvas.Instance.SpawnCollectMoney(transform);
                yield return _waitForNormalRewardDelay;
            }
        }
        private IEnumerator SpawnRewardMoney()
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
