using UnityEngine;
using ZestGames;

namespace GraveyardIdle.GraveSystem
{
    public class Grave : MonoBehaviour
    {
        [Header("-- GENERAL SETUP --")]
        [SerializeField, Tooltip("Delay for updating the pile that we are not digging or filling.")] private float pileUpdateDelay = 2f;
        [SerializeField] private GameObject coffinObj;

        [Header("-- CARRIER DROP SETUP --")]
        [SerializeField] private Transform carrierDropTransform;

        private GraveManager _graveManager;
        private Enums.GraveState _currentState;
        private int _id;
        private bool _activated;

        #region COMPONENTS
        private Collider _collider;
        private GraveGround _graveGround;
        private SoilDiggable _soilDiggable;
        private SoilFillable _soilFillable;
        private FinishedGrave _finishedGrave;
        #endregion

        #region GETTERS
        public int ID => _id;
        public Transform CarrierDropTransform => carrierDropTransform;
        public bool CarrierAssigned => _carrierAssigned;
        public bool HasCoffin => _hasCoffin;
        public bool CanTakeCoffin => !_hasCoffin && !_carrierAssigned && _currentState == Enums.GraveState.Dug;
        public float PileUpdateDelay => pileUpdateDelay;
        #endregion

        private bool _playerIsInArea, _hasCoffin, _carrierAssigned;

        public void Init(GraveManager graveManager, int id)
        {
            if (_graveManager == null)
            {
                _currentState = Enums.GraveState.NotActivated;
                _graveManager = graveManager;
                _id = id;

                _collider = GetComponent<Collider>();
                _graveGround = GetComponentInChildren<GraveGround>();
                _soilDiggable = GetComponentInChildren<SoilDiggable>();
                _soilFillable = GetComponentInChildren<SoilFillable>();
                _finishedGrave = GetComponentInChildren<FinishedGrave>();
            }

            _collider.enabled = _playerIsInArea = _carrierAssigned = false;

            Load();
        }

        #region MONO FUNCTIONS
        private void OnDisable()
        {
            //if (_graveManager == null) return;

            Save();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player) && !_playerIsInArea && CanTakeCoffin && player.IsCarryingCoffin )
            {
                _playerIsInArea = _hasCoffin = true;
                _collider.enabled = false;
                CoffinIsThrown();
                // Throw Coffin here
                player.TimerHandler.StartFilling(() => {
                    PlayerEvents.OnThrowCoffin?.Invoke(player.CoffinCarryingNow, this);
                    GraveManagerEvents.OnCoffinThrownToGrave?.Invoke();
                });
            }            
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Player player) && _playerIsInArea)
                _playerIsInArea = false;
        }
        #endregion

        #region PRIVATES
        private void SetState()
        {
            if (_currentState == Enums.GraveState.NotActivated)
                IsNotActivated();
            else if (_currentState == Enums.GraveState.Activated)
                ActivateGraveGround(_graveGround);
            else if (_currentState == Enums.GraveState.Dug)
                DiggingIsComplete();
            else if (_currentState == Enums.GraveState.WaitingToBeFilled)
                CoffinIsThrown();
            else if (_currentState == Enums.GraveState.Completed)
                FillingIsCompleted();

            _hasCoffin = _currentState == Enums.GraveState.WaitingToBeFilled || _currentState == Enums.GraveState.Completed;
            coffinObj.SetActive(_hasCoffin);
        }
        private void IsNotActivated()
        {
            Debug.Log("Grave no-" + ID + "is not activated");
            _graveGround.gameObject.SetActive(true);
            _soilDiggable.gameObject.SetActive(false);
            _soilFillable.gameObject.SetActive(false);
            _finishedGrave.gameObject.SetActive(false);
            _graveGround.Init(this);
        }
        private void CoffinIsThrown()
        {
            _graveManager.RemoveEmptyGrave(this);

            Debug.Log("Grave No-" + ID + " is waiting to be filled");
            _currentState = Enums.GraveState.WaitingToBeFilled;

            #region INITIALIZE IF GAME IS RESTARTED
            if (GameManager.GameState == Enums.GameState.WaitingToStart)
            {
                // For game initialization
                _graveGround.gameObject.SetActive(false);
                _soilDiggable.gameObject.SetActive(false);
                _soilFillable.gameObject.SetActive(true);
                _finishedGrave.gameObject.SetActive(false);
                _soilFillable.Init(this);
            }
            #endregion
        }
        #endregion

        #region PUBLICS
        public void ActivateGraveGround(GraveGround graveGround)
        {
            GraveManagerEvents.OnGraveActivated?.Invoke();

            Debug.Log("Activated grave no-" + ID);
            _currentState = Enums.GraveState.Activated;
            _graveGround.gameObject.SetActive(false);
            _soilDiggable.gameObject.SetActive(true);
            _soilFillable.gameObject.SetActive(false);
            _finishedGrave.gameObject.SetActive(false);
            _soilDiggable.Init(this);
        }
        public void DiggingIsComplete()
        {
            _collider.enabled = true;

            GraveManagerEvents.OnGraveDigged?.Invoke();
            _graveManager.AddEmptyGrave(this);
            GraveManagerEvents.OnCheckForCarrierActivation?.Invoke();

            Debug.Log("Grave no-" + ID + " is waiting for coffin");
            _currentState = Enums.GraveState.Dug;
            _graveGround.gameObject.SetActive(false);
            _soilDiggable.gameObject.SetActive(false);
            _soilFillable.gameObject.SetActive(true);
            _finishedGrave.gameObject.SetActive(false);
            _soilFillable.Init(this);
        }
        public void FillingIsCompleted()
        {
            Debug.Log("Grave No-" + ID + " is finished. Calculate its level");
            _currentState = Enums.GraveState.Completed;
            _graveGround.gameObject.SetActive(false);
            _soilDiggable.gameObject.SetActive(false);
            _soilFillable.gameObject.SetActive(false);
            _finishedGrave.gameObject.SetActive(true);
            _finishedGrave.Init(this);
        }
        public void AssignCarriers() => _carrierAssigned = true;
        public void UnAssignCarriers() => _carrierAssigned = false;
        #endregion

        #region SAVE-LOAD FUNCTIONS
        private void Save()
        {
            PlayerPrefs.SetInt($"Grave-{_id}-State", (int)_currentState);
            PlayerPrefs.Save();
        }
        private void Load()
        {
            _currentState = (Enums.GraveState)PlayerPrefs.GetInt($"Grave-{_id}-State", 0);

            SetState();
        }
        #endregion
    }
}
