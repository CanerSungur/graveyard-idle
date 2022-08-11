using System.Collections.Generic;
using UnityEngine;

namespace GraveyardIdle
{
    public class CoffinMachineStackHandler : MonoBehaviour
    {
        private CoffinMachine _coffinMachine;

        [Header("-- SETUP --")]
        [SerializeField] private Transform stackContainer;

        #region OFFSETS
        private readonly float _rowOffset = -1.15f;
        private readonly float _columnOffset = -2.4f;
        private readonly float _heightOffset = 0.52f;
        #endregion

        #region PROPERTIES
        public Vector3 TargetStackPosition => new Vector3(_coffinMachine.CurrentRowCount * _rowOffset, _coffinMachine.CurrentHeightCount * _heightOffset, _coffinMachine.CurrentColumnCount * _columnOffset);
        public Transform StackContainer => stackContainer;
        #endregion

        #region STACKED COFFINS LIST
        private static List<EmptyCoffin> _emptyCoffinsInArea;
        public static List<EmptyCoffin> EmptyCoffinsInArea => _emptyCoffinsInArea == null ? _emptyCoffinsInArea = new List<EmptyCoffin>() : _emptyCoffinsInArea;
        public static void AddEmptyCoffin(EmptyCoffin coffin)
        {
            if (!EmptyCoffinsInArea.Contains(coffin))
                EmptyCoffinsInArea.Add(coffin);

            Debug.Log(EmptyCoffinsInArea.Count);
        }
        public static void RemoveEmptyCoffin(EmptyCoffin coffin)
        {
            if (EmptyCoffinsInArea.Contains(coffin))
                EmptyCoffinsInArea.Remove(coffin);

            Debug.Log(EmptyCoffinsInArea.Count);
        }
        #endregion

        public void Init(CoffinMachine coffinMachine)
        {
            _coffinMachine = coffinMachine;
        }
    }
}
