using UnityEngine;
using ZestGames;
using TMPro;
using ZestCore.Utility;

namespace GraveyardIdle
{
    public class PopupManager : MonoBehaviour
    {
        private bool _isPlaying;

        public void Init(GameManager gameManager)
        {
            _isPlaying = false;
            UiEvents.OnPopupText += Popup;
        }

        private void OnDisable()
        {
            UiEvents.OnPopupText -= Popup;
        }

        private void Popup(Vector3 position, string message)
        {
            if (!_isPlaying)
            {
                _isPlaying = true;

                GameObject popup = ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.PopupText, position, Quaternion.identity);
                popup.GetComponentInChildren<TextMeshProUGUI>().text = message;
                Delayer.DoActionAfterDelay(this, 2f, () => {
                    _isPlaying = false;
                    popup.SetActive(false);
                });
            }
        }
    }
}
