using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ZestCore.Utility;
using ZestGames;

namespace GraveyardIdle
{
    public class GraveSpoilHandler : MonoBehaviour
    {
        //private Grave _grave;

        //[Header("-- SETUP --")]
        //[SerializeField] private ParticleSystem fliesParticle;

        //[Header("-- SPOIL CANVAS SETUP --")]
        //[SerializeField] private GameObject spoilCanvas;
        //[SerializeField] private Image spoilFill;

        //private readonly float _coreSpoilSpeed = 0.05f;
        //private readonly float _spoilSpeedDecreaseRate = 0.005f;
        //private readonly int _spoilChance = 50;
        ////private readonly float _coreSpoilStartDelay = 20f;
        ////private readonly float _spoilStartDelayIncrease = 1f;
        ////private float _spoilDelayTimer = 0f;
        ////private bool _startSpoilTimer = false;

        //#region PROPERTIES
        //public float CurrentSpoilRate { get; set; }
        //public bool CanSpoil => _grave.IsBuilt;
        //public bool CanBeWatered => CurrentSpoilRate < 0.9f;
        //public float SpoilSpeed => _coreSpoilSpeed - (_grave.Level * _spoilSpeedDecreaseRate);
        //public bool IsSpoiling { get; private set; }
        ////public float SpoilStartDelay => _coreSpoilStartDelay + (_spoilStartDelayIncrease * _grave.Level);
        //#endregion

        //public void Init(Grave grave)
        //{
        //    _grave = grave;
        //    IsSpoiling = /*_startSpoilTimer = */false;
        //    fliesParticle.Stop();
        //    CurrentSpoilRate = /*_spoilDelayTimer = */0f;

        //    spoilFill.fillAmount = 0;
        //    spoilCanvas.SetActive(false);

        //    _grave.OnStartSpoiling += StartSpoiling;
        //    _grave.OnStopSpoiling += StopSpoiling;
        //    WateringCanEvents.OnMaintenanceSuccessfull += DisableCanvas;

        //    //if (_grave.IsBuilt && !IsSpoiling)
        //    //    _grave.OnStartSpoiling?.Invoke();

        //    StartCoroutine(CheckForSpoil());
        //}

        //private void OnDisable()
        //{
        //    if (!_grave) return;

        //    _grave.OnStartSpoiling -= StartSpoiling;
        //    _grave.OnStopSpoiling -= StopSpoiling;
        //    WateringCanEvents.OnMaintenanceSuccessfull -= DisableCanvas;
        //}

        //private void Update()
        //{
        //    //if (_startSpoilTimer)
        //    //{
        //    //    _spoilDelayTimer += Time.deltaTime;
        //    //    if (_spoilDelayTimer >= SpoilStartDelay && !IsSpoiling)
        //    //    {
        //    //        IsSpoiling = true;
        //    //        _startSpoilTimer = false;
        //    //        _spoilDelayTimer = 0;

        //    //        if (!spoilCanvas.activeSelf)
        //    //            spoilCanvas.SetActive(true);
        //    //    }
        //    //}

        //    //if (CurrentSpoilRate <= 0. && spoilCanvas.activeSelf)
        //    //    spoilCanvas.SetActive(false);

        //    spoilFill.fillAmount = 1 - CurrentSpoilRate;
        //}

        //private void StartSpoiling()
        //{
        //    //_startSpoilTimer = true;
        //    //_spoilDelayTimer = 0;
        //    IsSpoiling = true;
        //    if (!spoilCanvas.activeSelf)
        //        spoilCanvas.SetActive(true);
        //}
        //private void StopSpoiling()
        //{
        //    IsSpoiling = /*_startSpoilTimer = */false;
        //}
        //private void DisableCanvas(Grave grave)
        //{
        //    if (_grave != grave) return;
        //    spoilCanvas.SetActive(false);
        //}
        //private IEnumerator CheckForSpoil()
        //{
        //    while (true)
        //    {
        //        if (_grave.IsBuilt && !IsSpoiling && !Player.IsMaintenancing && RNG.RollDice(_spoilChance))
        //        {
        //            _grave.OnStartSpoiling?.Invoke();
        //        }

        //        yield return new WaitForSeconds(10f);
        //    }
        //}
    }
}
