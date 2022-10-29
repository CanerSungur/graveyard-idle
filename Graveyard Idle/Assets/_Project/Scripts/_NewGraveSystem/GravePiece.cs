using System.Collections.Generic;
using UnityEngine;
using ZestGames;

namespace GraveyardIdle.GraveSystem
{
    public class GravePiece : MonoBehaviour
    {
        [Header("-- SPOIL SETUP --")]
        [SerializeField] private SkinnedMeshRenderer[] flowerRenderers;
        private List<Material> _flowerSpoilMaterials = new List<Material>();
        private readonly Color _disableColor = new Color(0.3f, 0.3f, 0.3f, 0f);

        private FinishedGrave _finishedGrave;

        #region ANIMATION SECTION
        private Animator _animator;
        private readonly int _openID = Animator.StringToHash("Open");
        #endregion

        public void Init(FinishedGrave finishedGrave)
        {
            if (_finishedGrave == null)
            {
                _finishedGrave = finishedGrave;
                _animator = GetComponent<Animator>();
            }

            InitFlowerSpoilMaterials();
            Open();
        }

        private void Update()
        {
            if (_finishedGrave == null) return;

            if (_finishedGrave.SpoilHandler.IsSpoiling)
            {
                ChangeSpoilMaterialAlpha(1, _finishedGrave.SpoilHandler.SpoilSpeed);
                ChangeBlendShapeWeight(100, _finishedGrave.SpoilHandler.SpoilSpeed);
            }
            else if (Player.IsMaintenancing && _finishedGrave.PlayerIsInMaintenanceArea)
            {
                ChangeSpoilMaterialAlpha(0, DataManager.MaintenanceSpeed);
                ChangeBlendShapeWeight(0, DataManager.MaintenanceSpeed);

                if (Player.IsMaintenancing && _finishedGrave.PlayerIsInMaintenanceArea && _flowerSpoilMaterials[0].color.a <= 0.02f)
                {
                    PlayerEvents.OnStoppedMaintenance?.Invoke();
                    WateringCanEvents.OnStoppedWatering?.Invoke();
                    WateringCanEvents.OnMaintenanceIsSuccessfull?.Invoke(_finishedGrave);
                }
            }
        }

        #region PRIVATES
        private void InitFlowerSpoilMaterials()
        {
            for (int i = 0; i < flowerRenderers.Length; i++)
            {
                if (!_flowerSpoilMaterials.Contains(flowerRenderers[i].materials[1]))
                {
                    _flowerSpoilMaterials.Add(flowerRenderers[i].materials[1]);
                    _flowerSpoilMaterials[i].color = _disableColor;
                }
            }
        }
        private void ChangeSpoilMaterialAlpha(float alpha, float speed)
        {
            for (int i = 0; i < _flowerSpoilMaterials.Count; i++)
            {
                _finishedGrave.SpoilHandler.CurrentSpoilRate = Mathf.Lerp(_flowerSpoilMaterials[i].color.a, alpha, speed * Time.deltaTime);
                _flowerSpoilMaterials[i].color = new Color(0.3f, 0.3f, 0.3f, _finishedGrave.SpoilHandler.CurrentSpoilRate);
            }
        }
        private void ChangeBlendShapeWeight(float alpha, float speed)
        {
            for (int i = 0; i < flowerRenderers.Length; i++)
            {
                flowerRenderers[i].SetBlendShapeWeight(0, Mathf.Lerp(flowerRenderers[i].GetBlendShapeWeight(0), alpha, speed * Time.deltaTime));
            }
        }
        #endregion

        #region ANIMATION FUNCTIONS
        private void Open() => _animator.SetBool(_openID, true);
        public void Disable() => _animator.SetBool(_openID, false);
        #endregion
    }
}
