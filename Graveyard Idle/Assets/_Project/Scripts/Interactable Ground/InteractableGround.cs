using System;
using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class InteractableGround : MonoBehaviour
    {
        [SerializeField] private bool startActivated = false;
        private int _id;

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
        private InteractableGroundCanvas _interactableGroundCanvas;
        public InteractableGroundCanvas InteractableGroundCanvas => _interactableGroundCanvas == null ? _interactableGroundCanvas = transform.GetChild(3).GetComponent<InteractableGroundCanvas>() : _interactableGroundCanvas;
        #endregion

        #region PROPERTIES
        public bool GraveIsActivated { get; private set; }
        public bool GraveIsBuilt { get; private set; }
        public bool HasCoffin { get; set; }
        public bool CanBeFilled => HasCoffin;
        public bool CanBeDigged { get; set; }
        public bool CanBeThrownCoffin { get; set; }
        public int ID => _id;
        public int RequiredMoney => _coreActivationMoney + GraveManager.AvailableGraveCount;
        #endregion

        #region EVENTS
        public Action OnGraveBuilt, OnGraveUpgraded;
        #endregion

        private readonly int _coreActivationMoney = 20;
        private int _currentActivationMoney;

        public void Init(ChangeableGraveGround changeableGraveGround, int id)
        {
            _id = id;
            GraveIsActivated = GraveIsBuilt = HasCoffin = CanBeDigged = CanBeThrownCoffin = false;
            
            MeshRenderer.enabled = true;
            Collider.enabled = true;

            DiggableSoil.gameObject.SetActive(false);
            SoilPile.gameObject.SetActive(false);
            Grave.Init(this);
            InteractableGroundCanvas.Init(this);

            //_interactableGroundCanvasPrefab = changeableGraveGround.InteractableGroundCanvasPrefab;
            //_interactableGroundCanvas = Instantiate(_interactableGroundCanvasPrefab, transform.position, Quaternion.identity, transform).GetComponent<InteractableGroundCanvas>();
            //_interactableGroundCanvas.transform.localPosition = new Vector3(0f, 0.32f, 0f);
            //_interactableGroundCanvas.transform.localRotation = Quaternion.Euler(91f, 180f, 0f);
            //_interactableGroundCanvas.Init(this);

            if (startActivated)
                ActivateGrave();

            LoadData();

            OnGraveBuilt += HandleGraveBuild;
        }

        private void OnDisable()
        {
            OnGraveBuilt -= HandleGraveBuild;

            SaveData();
        }

        private void HandleGraveBuild()
        {
            GraveIsBuilt = true;
            MeshRenderer.enabled = true;
            Collider.enabled = true;
            SoilPile.gameObject.SetActive(false);
            DiggableSoil.gameObject.SetActive(false);
            InteractableGroundCanvas.gameObject.SetActive(false);

            GraveIsActivated = CanBeDigged = false;
        }

        #region PUBLICS
        public void ActivateGrave()
        {
            MeshRenderer.enabled = false;
            Collider.enabled = false;
            InteractableGroundCanvas.gameObject.SetActive(false);
            DiggableSoil.gameObject.SetActive(true);
            DiggableSoil.Init(this);
            SoilPile.gameObject.SetActive(true);
            SoilPile.Init(this);

            GraveIsActivated = CanBeDigged = true;

            GraveManagerEvents.OnGraveActivated?.Invoke();
            GraveEvents.OnUpdateGraveActivationRequiredMoney?.Invoke();
        }
        #endregion

        #region SAVE-LOAD
        private void SaveData()
        {
            PlayerPrefs.SetInt($"Grave_{_id}_Activated", GraveIsActivated == true ? 1 : 0);
            PlayerPrefs.SetInt($"Grave_{_id}_Built", GraveIsBuilt == true ? 1 : 0);
            PlayerPrefs.Save();
        }
        private void LoadData()
        {
            GraveIsActivated = PlayerPrefs.GetInt($"Grave_{_id}_Activated", 0) == 1;
            GraveIsBuilt = PlayerPrefs.GetInt($"Grave_{_id}_Built", 0) == 1;

            if (GraveIsBuilt)
                HandleGraveBuild();
            else if (!GraveIsBuilt && GraveIsActivated)
                ActivateGrave();
        }
        private void OnApplicationQuit() => SaveData();
        //private void OnApplicationPause(bool pause) => SaveData();
        #endregion
    }
}
