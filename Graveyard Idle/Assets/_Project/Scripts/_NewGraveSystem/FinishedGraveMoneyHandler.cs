using UnityEngine;

namespace GraveyardIdle.GraveSystem
{
    public class FinishedGraveMoneyHandler : MonoBehaviour
    {
        private FinishedGrave _finishedGrave;

        public void Init(FinishedGrave finishedGrave)
        {
            if (_finishedGrave == null)
            {
                _finishedGrave = finishedGrave;
            }
        }
    }
}
