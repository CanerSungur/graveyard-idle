using System;
using UnityEngine;

namespace GraveyardIdle
{
    public class InteractableGround : MonoBehaviour
    {
        #region COMPONENTS
        private MeshRenderer _meshRenderer;
        public MeshRenderer MeshRenderer => _meshRenderer == null ? _meshRenderer = GetComponent<MeshRenderer>() : _meshRenderer;
        private Collider _collider;
        public Collider Collider => _collider == null ? _collider = GetComponent<Collider>() : _collider;
        #endregion

        #region SCRIPT REFERENCES
        private Soil _diggableSoil;
        public Soil DiggableSoil => _diggableSoil == null ? _diggableSoil = transform.GetChild(0).GetComponent<Soil>() : _diggableSoil;
        private SoilPile _soilPile;
        public SoilPile SoilPile => _soilPile == null ? _soilPile = transform.GetChild(1).GetComponent<SoilPile>() : _soilPile;
        private Grave _grave;
        public Grave Grave => _grave == null ? _grave = transform.GetChild(2).GetComponent<Grave>() : _grave;
        #endregion

        private GameObject _interactableGroundCanvasPrefab;
        private InteractableGroundCanvas _canvas;

        #region PROPERTIES
        public bool CanBeFilled { get; set; }
        public bool CanBeDigged { get; set; }
        #endregion

        #region EVENTS
        public Action OnGraveBuilt;
        #endregion

        public void Init(ChangeableGraveGround changeableGraveGround)
        {
            CanBeFilled = CanBeDigged = false;

            DiggableSoil.gameObject.SetActive(false);
            SoilPile.gameObject.SetActive(false);
            Grave.Init(this);

            MeshRenderer.enabled = true;
            Collider.enabled = true;

            _interactableGroundCanvasPrefab = changeableGraveGround.InteractableGroundCanvasPrefab;
            _canvas = Instantiate(_interactableGroundCanvasPrefab, transform.position, Quaternion.identity, transform).GetComponent<InteractableGroundCanvas>();
            _canvas.transform.localPosition = new Vector3(0f, 0.32f, 0f);
            _canvas.transform.localRotation = Quaternion.Euler(91f, 180f, 0f);
            _canvas.Init(this);

            OnGraveBuilt += GraveIsBuilt;
        }

        private void OnDisable()
        {
            OnGraveBuilt -= GraveIsBuilt;
        }

        private void GraveIsBuilt()
        {
            MeshRenderer.enabled = true;
            Collider.enabled = true;
            SoilPile.gameObject.SetActive(false);
            DiggableSoil.gameObject.SetActive(false);
        }

        #region PUBLICS
        public void ActivateGrave()
        {
            MeshRenderer.enabled = false;
            Collider.enabled = false;
            _canvas.gameObject.SetActive(false);
            DiggableSoil.gameObject.SetActive(true);
            DiggableSoil.Init(this);
            SoilPile.gameObject.SetActive(true);
            SoilPile.Init(this);

            CanBeDigged = true;
        }
        #endregion
    }
}
