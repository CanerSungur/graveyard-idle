using UnityEngine;

namespace GraveyardIdle
{
    public class ChangeableGraveGround : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private GameObject interactableGroundCanvasPrefab;
        [SerializeField] private InteractableGround[] interactableGrounds;

        #region GETTERS
        public GameObject InteractableGroundCanvasPrefab => interactableGroundCanvasPrefab;
        #endregion

        private void Start()
        {
            InitializeGrounds();   
        }

        private void InitializeGrounds()
        {
            for (int i = 0; i < interactableGrounds.Length; i++)
                interactableGrounds[i].Init(this);
        }
    }
}
