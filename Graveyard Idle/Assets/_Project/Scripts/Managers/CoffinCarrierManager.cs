using UnityEngine;
using ZestGames;
namespace GraveyardIdle
{
    public class CoffinCarrierManager : MonoBehaviour
    {
        private bool _isActivated;
        private bool _carriersAreBusy;

        [Header("-- SETUP --")]
        [SerializeField] private CoffinCarrier[] coffinCarriers;
        [SerializeField] private Transform[] waitTransforms;

        public static Transform[] WaitTransforms { get; private set; }
        public static bool CoffinTakeTriggered, CoffinThrowTriggered;

        public void Init(GameManager gameManager)
        {
            _carriersAreBusy = false;
            WaitTransforms = waitTransforms;
            CoffinTakeTriggered = CoffinThrowTriggered = false;

            LoadData();

            GraveManagerEvents.OnCheckForCarrierActivation += HandleGraveDigged;
            CoffinCarrierEvents.OnReadyForDuty += HandleCarriersReadyForDuty;
            CoffinCarrierEvents.OnActivatedCarriers += EnableCarriers;
        }

        private void OnDisable()
        {
            GraveManagerEvents.OnCheckForCarrierActivation -= HandleGraveDigged;
            CoffinCarrierEvents.OnReadyForDuty -= HandleCarriersReadyForDuty;
            CoffinCarrierEvents.OnActivatedCarriers -= EnableCarriers;

            SaveData();
        }

        private void EnableCarriers()
        {
            for (int i = 0; i < coffinCarriers.Length; i++)
            {
                coffinCarriers[i].gameObject.SetActive(true);
            }
        }
        private void DisableCarriers()
        {
            for (int i = 0; i < coffinCarriers.Length; i++)
            {
                coffinCarriers[i].gameObject.SetActive(false);
            }
        }
        private void HandleGraveDigged()
        {
            if (!_carriersAreBusy && GraveManager.CanCoffinBeingCarried && CoffinAreaStackHandler.CoffinsInArea.Count > 0)
            {
                _carriersAreBusy = true;

                GraveManagerEvents.OnCoffinPickedUp?.Invoke();
                AssignCarriers();
            }
        }
        private void AssignCarriers()
        {
            for (int i = 0; i < coffinCarriers.Length; i++)
                coffinCarriers[i].OnMoveToCoffinArea?.Invoke();
        }
        private void HandleCarriersReadyForDuty()
        {
            _carriersAreBusy = CoffinTakeTriggered = CoffinThrowTriggered = false;
        }

        #region SAVE-LOAD
        private void LoadData()
        {
            _isActivated = PlayerPrefs.GetInt($"CoffinCarriersActivated", 0) == 1;
            if (_isActivated)
                EnableCarriers();
            else
                DisableCarriers();
        }
        private void SaveData()
        {
            PlayerPrefs.SetInt($"CoffinCarriersActivated", _isActivated == true ? 1 : 0);
            PlayerPrefs.Save();
        }
        private void OnApplicationQuit()
        {
            SaveData();
        }
        private void OnApplicationPause(bool pause)
        {
            SaveData();
        }
        #endregion
    }
}
