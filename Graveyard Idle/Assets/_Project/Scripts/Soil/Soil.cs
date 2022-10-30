using UnityEngine;
using System;
using DG.Tweening;
using ZestGames;
using ZestCore.Utility;

namespace GraveyardIdle
{
    public class Soil : MonoBehaviour
    {
        //private bool _diggingStarted = false;
        //private bool _initialized = false;
        //private InteractableGround _interactableGround;

        //private SkinnedMeshRenderer _skinnedMeshRenderer;
        //private Mesh _changedMesh;
        //private MeshCollider _meshCollider;

        //private bool _playerIsInArea = false;
        //public bool PlayerIsInArea => _playerIsInArea;
        //private int _currentBlendWeightIndex = -1;

        //[Header("-- WALL SETUP --")]
        //[SerializeField] private GameObject[] walls;

        //#region DIGGING
        //private int _digCount = 5;
        //private int _currentDigCount = 0;
        //private readonly float _getDiggedDuration = 1f;
        //#endregion

        //#region FILLING
        //private int _fillCount = 5;
        //private int _currentFillCount = 0;
        //private readonly float _getFilledDuration = 1f;
        //#endregion
        
        //#region POSITION
        //private readonly float _defaultHeight = 0f;
        //private readonly float _diggedHeight = -0.3f;
        //#endregion

        //#region SEQUENCE
        //private Sequence _getDiggedSequence, _getFilledSequence;
        //private Guid _getDiggedSequenceID, _getFilledSequenceID;
        //#endregion

        //public void Init(InteractableGround interactableGround)
        //{
        //    _initialized = true;
        //    _diggingStarted = false;
        //    _interactableGround = interactableGround;

        //    _skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        //    _skinnedMeshRenderer.SetBlendShapeWeight(0, 0f);
        //    _meshCollider = GetComponent<MeshCollider>();

        //    _playerIsInArea = false;
        //    transform.localPosition = Vector3.zero;

        //    DisableWalls();

        //    ShovelEvents.OnDigHappened += GetDigged;
        //    ShovelEvents.OnThrowSoilToGrave += GetFilled;
        //}

        //private void OnDisable()
        //{
        //    if (!_initialized) return;
        //    ShovelEvents.OnDigHappened -= GetDigged;
        //    ShovelEvents.OnThrowSoilToGrave -= GetFilled;
        //}

        //private void OnTriggerEnter(Collider other)
        //{
        //    if (other.TryGetComponent(out Player player) && !_playerIsInArea && _currentDigCount < _digCount && _interactableGround.CanBeDigged)
        //    {
        //        _playerIsInArea = true;
        //        PlayerEvents.OnEnteredDigZone?.Invoke();
        //        PlayerEvents.OnPullOutShovel?.Invoke();
        //    }
        //}

        //private void OnTriggerExit(Collider other)
        //{
        //    if (other.TryGetComponent(out Player player) && _playerIsInArea)
        //    {
        //        _playerIsInArea = false;
        //        PlayerEvents.OnExitedDigZone?.Invoke();
        //        PlayerEvents.OnPutDownShovel?.Invoke();
        //    }
        //}

        //public void GetFilled()
        //{
        //    if (!_interactableGround.SoilPile.PlayerIsInArea) return;

        //    _currentFillCount++;
        //    _currentBlendWeightIndex = _fillCount - _currentFillCount;

        //    StartGetFilledSequence(_currentBlendWeightIndex);

        //    //if (_currentFillCount == _fillCount)
        //    //{
        //    //    PlayerEvents.OnExitedFillZone?.Invoke();
        //    //    PlayerEvents.OnStopFilling?.Invoke();

                
        //    //}
        //}
        //private void GetDigged()
        //{
        //    if (!_playerIsInArea || !_interactableGround.CanBeDigged) return;

        //    EnableWalls();
        //    _currentBlendWeightIndex = _currentDigCount;
        //    _currentDigCount++;

        //    StartGetDiggedSequence(_currentBlendWeightIndex);

        //    if (_currentDigCount == _digCount)
        //    {
        //        DisableWalls();
        //        PlayerEvents.OnExitedDigZone?.Invoke();
        //        PlayerEvents.OnStopDigging?.Invoke();

        //        _interactableGround.CanBeDigged = false;
        //        _interactableGround.CanBeThrownCoffin = true;

        //        GraveManagerEvents.OnGraveDigged?.Invoke();
        //        GraveManager.AddEmptyGrave(_interactableGround.Grave);
        //        GraveManagerEvents.OnCheckForCarrierActivation?.Invoke();
        //    }
        //}
        //private void UpdateMeshCollider()
        //{
        //    _changedMesh = new Mesh();
        //    _skinnedMeshRenderer.BakeMesh(_changedMesh);
        //    _meshCollider.sharedMesh = null;
        //    _meshCollider.sharedMesh = _changedMesh;
        //}

        //private void EnableWalls()
        //{
        //    for (int i = 0; i < walls.Length; i++)
        //        walls[i].SetActive(true);
        //}
        //private void DisableWalls()
        //{
        //    Delayer.DoActionAfterDelay(this, 2f, () => {
        //        for (int i = 0; i < walls.Length; i++)
        //            walls[i].SetActive(false);
        //    });
        //}

        //#region DOTWEEN FUNCTIONS
        //private void StartGetDiggedSequence(int index)
        //{
        //    _getDiggedSequence.Pause();
        //    DeleteGetDiggedSequence();
        //    CreateGetDiggedSequence(index);
        //    _getDiggedSequence.Play();

        //    //_interactableGround.SoilPile.GetPiled();
        //}
        //private void CreateGetDiggedSequence(int index)
        //{
        //    if (_getDiggedSequence == null)
        //    {
        //        _getDiggedSequence = DOTween.Sequence();
        //        _getDiggedSequenceID = Guid.NewGuid();
        //        _getDiggedSequence.id = _getDiggedSequenceID;

        //        _getDiggedSequence.Append(DOVirtual.Float(0f, 100f, _getDiggedDuration,r => {
        //            _skinnedMeshRenderer.SetBlendShapeWeight(index, r);
        //            UpdateMeshCollider();
        //        }))
        //            .Join(DOVirtual.Float(_defaultHeight, _diggedHeight, _getDiggedDuration, r => {
        //                if (_currentBlendWeightIndex == _digCount - 1)
        //                    transform.localPosition = new Vector3(0f, r, 0f);
        //            }))
        //            .OnComplete(() => {
        //            DeleteGetDiggedSequence();
        //        });
        //    }
        //}
        //private void DeleteGetDiggedSequence()
        //{
        //    DOTween.Kill(_getDiggedSequenceID);
        //    _getDiggedSequence = null;
        //}

        //private void StartGetFilledSequence(int index)
        //{
        //    _getFilledSequence.Pause();
        //    DeleteGetFilledSequence();
        //    CreateGetFilledSequence(index);
        //    _getFilledSequence.Play();
        //}
        //private void CreateGetFilledSequence(int index)
        //{
        //    if (_getFilledSequence == null)
        //    {
        //        _getFilledSequence = DOTween.Sequence();
        //        _getFilledSequenceID = Guid.NewGuid();
        //        _getFilledSequence.id = _getFilledSequenceID;

        //        _getFilledSequence.Append(DOVirtual.Float(100f, 0f, _getFilledDuration, r => {
        //            _skinnedMeshRenderer.SetBlendShapeWeight(index, r);
        //            UpdateMeshCollider();
        //        }))
        //            .Join(DOVirtual.Float(_diggedHeight, _defaultHeight, _getFilledDuration, r =>
        //            {
        //                if (_currentBlendWeightIndex == _fillCount - 1)
        //                    transform.localPosition = new Vector3(0f, r, 0f);
        //            }))
        //            .OnComplete(() => {
        //                DeleteGetFilledSequence();

        //                if (_currentFillCount == _fillCount)
        //                {
        //                    Debug.Log("GRAVE FINISHED");
        //                    _interactableGround.OnGraveBuilt?.Invoke();
        //                    _interactableGround.Grave.MoneyHandler.StartSpawningMoney();
        //                    AudioHandler.PlayAudio(Enums.AudioType.MoneySpawn);
        //                }
        //            });
        //    }
        //}
        //private void DeleteGetFilledSequence()
        //{
        //    DOTween.Kill(_getFilledSequenceID);
        //    _getFilledSequence = null;
        //}
        //#endregion
    }
}
