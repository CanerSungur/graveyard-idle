using UnityEngine;

namespace ZestGames
{
    public class SettingsMinimalUi : MonoBehaviour
    {
        private Animator _animator;
        private readonly int _openID = Animator.StringToHash("Open");
        private bool _isOpen;

        [Header("-- SETUP --")]
        [SerializeField] private GameObject vibrationOn;
        [SerializeField] private GameObject vibrationOff;
        [SerializeField] private GameObject soundOn;
        [SerializeField] private GameObject soundOff;

        public void Init(UiManager uiManager)
        {
            if (_animator == null)
                _animator = GetComponent<Animator>();

            _isOpen = false;

            UpdateSoundSprite();
            UpdateVibrationSprite();
        }

        #region MENU
        public void ToggleMenu()
        {
            if (_isOpen)
                CloseMenu();
            else
                OpenMenu();
        }
        private void CloseMenu()
        {
            _animator.SetBool(_openID, false);
            _isOpen = false;
        }
        private void OpenMenu()
        {
            _animator.SetBool(_openID, true);
            _isOpen = true;
        }
        #endregion

        #region VIBRATION
        public void ToggleVibration()
        {
            if (SettingsManager.VibrationOn)
                CloseVibration();
            else
                OpenVibration();
        }
        private void OpenVibration()
        {
            SettingsManager.VibrationOn = true;
            UpdateVibrationSprite();
        }
        private void CloseVibration()
        {
            SettingsManager.VibrationOn = false;
            UpdateVibrationSprite();
        }
        #endregion

        #region SOUND
        public void ToggleSound()
        {
            if (SettingsManager.SoundOn)
                CloseSound();
            else
                OpenSound();
        }
        private void OpenSound()
        {
            SettingsManager.SoundOn = true;
            UpdateSoundSprite();
        }
        private void CloseSound()
        {
            SettingsManager.SoundOn = false;
            UpdateSoundSprite();
        }
        #endregion

        private void UpdateSoundSprite()
        {
            soundOn.SetActive(SettingsManager.SoundOn);
            soundOff.SetActive(!SettingsManager.SoundOn);
        }
        private void UpdateVibrationSprite()
        {
            vibrationOn.SetActive(SettingsManager.VibrationOn);
            vibrationOff.SetActive(!SettingsManager.VibrationOn);
        }
    }
}
