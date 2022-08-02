using UnityEngine;
using ZestGames;
using TMPro;
using ZestCore.Utility;

namespace GraveyardIdle
{
    public class PopupManager : MonoBehaviour
    {
        public void Init(GameManager gameManager)
        {
            UiEvents.OnPopupText += Popup;
        }

        private void OnDisable()
        {
            UiEvents.OnPopupText -= Popup;
        }

        private void Popup(Vector3 position, string message)
        {
            GameObject popup = ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.PopupText, position, Quaternion.identity);
            popup.GetComponentInChildren<TextMeshProUGUI>().text = message;
            Delayer.DoActionAfterDelay(this, 2f, () => popup.SetActive(false));
        }
    }
}
