using UnityEngine;

namespace GraveyardIdle.GraveSystem
{
    public class FinishedGraveSpoilHandler : MonoBehaviour
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
