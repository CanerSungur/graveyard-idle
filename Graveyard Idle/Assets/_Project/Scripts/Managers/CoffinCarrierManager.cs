using UnityEngine;
using ZestGames;
namespace GraveyardIdle
{
    public class CoffinCarrierManager : MonoBehaviour
    {
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

            GraveManagerEvents.OnCheckForCarrierActivation += HandleGraveDigged;
            CoffinCarrierEvents.OnReadyForDuty += HandleCarriersReadyForDuty;
        }

        private void OnDisable()
        {
            GraveManagerEvents.OnCheckForCarrierActivation -= HandleGraveDigged;
            CoffinCarrierEvents.OnReadyForDuty -= HandleCarriersReadyForDuty;
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
    }
}
