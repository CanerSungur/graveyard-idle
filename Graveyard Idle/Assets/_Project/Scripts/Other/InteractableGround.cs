using UnityEngine;

namespace GraveyardIdle
{
    public class InteractableGround : MonoBehaviour
    {
        private MeshRenderer _meshRenderer;
        private Soil _diggableSoil;
        private GameObject _interactableGroundCanvasPrefab;
        private InteractableGroundCanvas _canvas;

        public void Init(ChangeableGraveGround changeableGraveGround)
        {
            _diggableSoil = transform.GetChild(0).GetComponent<Soil>();
            _diggableSoil.gameObject.SetActive(false);
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.enabled = true;

            _interactableGroundCanvasPrefab = changeableGraveGround.InteractableGroundCanvasPrefab;
            _canvas = Instantiate(_interactableGroundCanvasPrefab, transform.position, Quaternion.identity, transform).GetComponent<InteractableGroundCanvas>();
            _canvas.transform.localPosition = new Vector3(0f, 0.32f, 0f);
            _canvas.transform.localRotation = Quaternion.Euler(91f, 180f, 0f);
            _canvas.Init(this);
        }

        #region PUBLICS
        public void ActivateGrave()
        {
            _meshRenderer.enabled = false;
            _canvas.gameObject.SetActive(false);
            _diggableSoil.gameObject.SetActive(true);
            Debug.Log("activate this grave");
        }
        #endregion
    }
}
