using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class GraveManager : MonoBehaviour
    {
        public static int DiggedEmptyGraveCount { get; private set; }
        public static int AvailableGraveCount { get; private set; }
        public static int BeingCarriedCoffinCount { get; private set; }
        public static int CoffinCanBeSpawnedCount { get; private set; }
        public static bool CanCoffinBeingCarried => DiggedEmptyGraveCount - BeingCarriedCoffinCount > 0;

        public void Init(GameManager gameManager)
        {
            CoffinCanBeSpawnedCount = AvailableGraveCount = DiggedEmptyGraveCount = BeingCarriedCoffinCount = 0;

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
        private void EmptyCoffinSold()
        {
            CoffinCanBeSpawnedCount++;
            Debug.Log("coffin can be spawned count: " + CoffinCanBeSpawnedCount);
        }
        private void CoffinSpawned()
        {
            CoffinCanBeSpawnedCount--;
            Debug.Log("coffin can be spawned count: " + CoffinCanBeSpawnedCount);
        }
    }
}
