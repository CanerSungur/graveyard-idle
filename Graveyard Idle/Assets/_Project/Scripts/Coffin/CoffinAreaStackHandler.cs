using System.Collections.Generic;
using UnityEngine;

namespace GraveyardIdle
{
    public class CoffinAreaStackHandler : MonoBehaviour
    {
        private CoffinArea _coffinArea;

        [Header("-- SETUP --")]
        [SerializeField] private Transform stackContainer;

        #region OFFSETS
        private readonly float _rowOffset = -1.15f;
        private readonly float _columnOffset = -2.4f;
        private readonly float _heightOffset = 0.52f;
        #endregion

        #region PROPERTIES
        public Vector3 TargetStackPosition => new Vector3(_coffinArea.CurrentRowCount * _rowOffset, _coffinArea.CurrentHeightCount * _heightOffset, _coffinArea.CurrentColumnCount * _columnOffset);
        public Transform StackContainer => stackContainer;
        #endregion

        #region STACKED COFFINS LIST
        private static List<Coffin> _coffinsInArea;
        public static List<Coffin> CoffinsInArea => _coffinsInArea == null ? _coffinsInArea = new List<Coffin>() : _coffinsInArea;
        public static void AddCoffin(Coffin coffin)
        {
            if (!CoffinsInArea.Contains(coffin))
                CoffinsInArea.Add(coffin);
        }
        public static void RemoveCoffin(Coffin coffin)
        {
            if (CoffinsInArea.Contains(coffin))
                CoffinsInArea.Remove(coffin);
        }
        #endregion

        public void Init(CoffinArea coffinArea)
        {
            _coffinArea = coffinArea;
        }
    }
}
