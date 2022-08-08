using System.Collections.Generic;
using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class GravePiece : MonoBehaviour
    {
        private Grave _grave;
        private Animator _animator;

        [Header("-- SPOIL SETUP --")]
        [SerializeField] private MeshRenderer[] flowerRenderers;
        private List<Material> _flowerSpoilMaterials = new List<Material>();
        private readonly Color _disableColor = new Color(0.3f, 0.3f, 0.3f, 0f);

        #region ANIMATION VARIABLES
        private readonly int _openID = Animator.StringToHash("Open");
        #endregion

        public void Init(Grave grave)
        {
            _grave = grave;
            _animator = GetComponent<Animator>();
            InitFlowerSpoilMaterials();

            if (_animator)
                Open();
        }

        private void Update()
        {
            if (!_grave || !_grave.SpoilHandler.CanSpoil) return;

            if (_grave.SpoilHandler.IsSpoiling)
                ChangeSpoilMaterialAlpha(1, _grave.SpoilHandler.SpoilSpeed);
            else if (Player.IsMaintenancing && _grave.PlayerIsInMaintenanceArea)
            {
                ChangeSpoilMaterialAlpha(0, DataManager.MaintenanceSpeed);

                if (Player.IsMaintenancing && _grave.PlayerIsInMaintenanceArea && _flowerSpoilMaterials[0].color.a <= 0.02f)
                {
                    PlayerEvents.OnStoppedMaintenance?.Invoke();
                    WateringCanEvents.OnStoppedWatering?.Invoke();
                    WateringCanEvents.OnMaintenanceSuccessfull?.Invoke(_grave);
                }
            }
        }

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
                _grave.SpoilHandler.CurrentSpoilRate = Mathf.Lerp(_flowerSpoilMaterials[i].color.a, alpha, speed * Time.deltaTime);
                _flowerSpoilMaterials[i].color = new Color(0.3f, 0.3f, 0.3f, _grave.SpoilHandler.CurrentSpoilRate);
            }
        }
        private void Open() => _animator.SetBool(_openID, true);
        public void Close() => _animator.SetBool(_openID, false);
    }
}
