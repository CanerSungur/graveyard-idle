using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class CoffinAreaTakeHandler : MonoBehaviour
    {
        private CoffinArea _coffinArea;

        public void Init(CoffinArea coffinArea)
        {
            _coffinArea = coffinArea;

            PlayerEvents.OnTakeACoffin += GiveACoffin;
        }

        private void OnDisable()
        {
            PlayerEvents.OnTakeACoffin -= GiveACoffin;
        }

        private void GiveACoffin()
        {
            Coffin coffin = CoffinAreaStackHandler.CoffinsInArea[CoffinAreaStackHandler.CoffinsInArea.Count - 1];
            PlayerEvents.OnSetCarryingCoffin?.Invoke(coffin);
            coffin.GoToPlayer(_coffinArea.Player.CarryTransform);
            CoffinAreaStackHandler.RemoveCoffin(coffin);
        }
    }
}
