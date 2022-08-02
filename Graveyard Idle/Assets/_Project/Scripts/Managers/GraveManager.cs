using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class GraveManager : MonoBehaviour
    {
        public static int DiggedEmptyGraveCount { get; private set; }
        public static int AvailableGraveCount { get; private set; }
        public static int BeingCarriedCoffinCount { get; private set; }
        public static bool CanCoffinBeingCarried => DiggedEmptyGraveCount - BeingCarriedCoffinCount > 0;

        public void Init(GameManager gameManager)
        {
            AvailableGraveCount = DiggedEmptyGraveCount = BeingCarriedCoffinCount = 0;

            GraveManagerEvents.OnGraveActivated += GraveActivated;
            GraveManagerEvents.OnGraveDigged += GraveDigged;
            GraveManagerEvents.OnCoffinThrownToGrave += CoffinThrownToGrave;
            GraveManagerEvents.OnCoffinPickedUp += StartedCarryingCoffin;
        }

        private void OnDisable()
        {
            GraveManagerEvents.OnGraveActivated -= GraveActivated;
            GraveManagerEvents.OnGraveDigged -= GraveDigged;
            GraveManagerEvents.OnCoffinThrownToGrave -= CoffinThrownToGrave;
            GraveManagerEvents.OnCoffinPickedUp -= StartedCarryingCoffin;
        }

        private void GraveActivated()
        {
            AvailableGraveCount++;
            Debug.Log("activated grave: " + AvailableGraveCount);
        }
        private void GraveDigged()
        {
            DiggedEmptyGraveCount++;
            Debug.Log("digged grave: " +  DiggedEmptyGraveCount);
        }
        private void CoffinThrownToGrave()
        {
            DiggedEmptyGraveCount--;
            BeingCarriedCoffinCount--;
            Debug.Log("digged grave: " + DiggedEmptyGraveCount);
            Debug.Log("coffins that are being carried: " + BeingCarriedCoffinCount);
        }
        private void StartedCarryingCoffin()
        {
            BeingCarriedCoffinCount++;
            Debug.Log("coffins that are being carried: " + BeingCarriedCoffinCount);
        }
    }
}
