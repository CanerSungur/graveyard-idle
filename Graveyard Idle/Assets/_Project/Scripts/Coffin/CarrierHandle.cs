using UnityEngine;

namespace GraveyardIdle
{
    public class CarrierHandle : MonoBehaviour
    {
        public bool IsTaken { get; private set; }

        public void Init(Coffin coffin)
        {
            IsTaken = false;
        }
    }
}
