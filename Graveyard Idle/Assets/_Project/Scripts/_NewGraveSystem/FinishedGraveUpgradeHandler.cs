using TMPro;
using UnityEngine;

namespace GraveyardIdle.GraveSystem
{
    public class FinishedGraveUpgradeHandler : MonoBehaviour
    {
        //[Header("-- GENERAL SETUP --")]
        //[SerializeField] private GameObject upgradeAreaObj;

        //[Header("-- MONEY CONSUME SETUP --")]
        //[SerializeField] private TextMeshProUGUI remainingMoneyText;
        //[SerializeField] private Transform moneyImageTransform;

        private FinishedGrave _finishedGrave;

        //#region MONEY SECTION
        //private int _consumedMoney;
        //#endregion

        public void Init(FinishedGrave finishedGrave)
        {
            if (_finishedGrave == null)
            {
                _finishedGrave = finishedGrave;
            }
        }

        //#region SAVE-LOAD
        //private void Save()
        //{
        //    PlayerPrefs.SetInt($"Grave-{_finishedGrave.Grave.ID}-FinishedGrave", _consumedMoney);
        //    PlayerPrefs.Save();
        //}
        //private void Load()
        //{

        //}
        //#endregion
    }
}
