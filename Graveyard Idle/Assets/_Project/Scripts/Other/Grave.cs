using UnityEngine;

namespace GraveyardIdle
{
    public class Grave : MonoBehaviour
    {
        private InteractableGround _interactableGround;

        [Header("-- LEVEL SETUP --")]
        [SerializeField] private GameObject level_1_Object;
        [SerializeField] private GameObject level_2_Object;
        [SerializeField] private GameObject level_3_Object;
        [SerializeField] private GameObject level_4_Object;
        [SerializeField] private GameObject level_5_Object;

        private readonly int _maxLevel = 5;
        private int _level = 0;
        private bool _isBuilt = false;

        public void Init(InteractableGround interactableGround)
        {
            _interactableGround = interactableGround;

            _level = 0;
            _isBuilt = false;

            _interactableGround.OnGraveBuilt += GetBuilt;
        }

        private void OnDisable()
        {
            if (!_interactableGround) return;
            _interactableGround.OnGraveBuilt -= GetBuilt;
        }

        private void EnableRelevantGrave(int level)
        {
            if (_level == 1)
                level_1_Object.SetActive(true);
        }

        #region PUBLICS
        public void GetBuilt()
        {
            if (_isBuilt) return;

            _isBuilt = true;
            _level = 1;

            EnableRelevantGrave(_level);
        }
        public void Upgrade()
        {
            _level++;

            if (_level == _maxLevel)
            {
                // Disable upgrade
            }
        }
        #endregion
    }
}
