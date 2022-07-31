using System.Collections.Generic;
using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class SoilDeform : MonoBehaviour
    {
        private GraveDiggable _grave;
        
        #region COMPONENTS
        private MeshFilter _meshFilter;
        private Mesh _mesh;
        private MeshCollider _meshCollider;
        #endregion

        #region MESH MANIPULATION
        private Vector3[] _vertices;
        private List<Vector3> _diggedVertices = new List<Vector3>();

        private float _defaultHeight, _diggedHeight, _totalVerticeCount, _diggedVerticeCount;
        private readonly float _diggedLimit = 0.65f;
        #endregion

        [Header("-- SETUP --")]
        [SerializeField] private float radius;
        [SerializeField] private float power;

        public void Init(GraveDiggable grave)
        {
            _grave = grave;

            #region COMPONENTS
            _meshCollider = GetComponent<MeshCollider>();
            _meshFilter = GetComponent<MeshFilter>();
            _mesh = _meshFilter.mesh;
            #endregion

            #region INIT MESH
            _vertices = _mesh.vertices;
            UpdateMeshCollider();
            #endregion

            #region CALCULATE DIGGED HEIGHT
            _defaultHeight = _vertices[0].y;
            _diggedHeight = _defaultHeight - _diggedLimit;
            _totalVerticeCount = _mesh.vertices.Length;
            _diggedVerticeCount = 0;
            #endregion
        }

        private void AddDiggedVertice(Vector3 vertice)
        {
            if (!_diggedVertices.Contains(vertice))
                _diggedVertices.Add(vertice);
        }
        private void CheckDeformationRate()
        {
            //for (int i = 0; i < _vertices.Length; i++)
            //{
            //    if (_vertices[i].y <= _diggedHeight)
            //        AddDiggedVertice(_vertices[i]);
            //}

            float percentage = (100 * _diggedVerticeCount) / _totalVerticeCount;
            Debug.Log(percentage);
            if (percentage >= 80)
                GraveEvents.OnAGraveIsDug?.Invoke(_grave);
        }
        private void UpdateMeshCollider()
        {
            _meshCollider.sharedMesh = null;
            _meshCollider.sharedMesh = _meshFilter.mesh;
        }

        #region PUBLICS
        public void DeformThis(Vector3 positionToDeform)
        {
            positionToDeform = transform.InverseTransformPoint(positionToDeform);

            for (int i = 0; i < _vertices.Length; i++)
            {
                float distance = (_vertices[i] - positionToDeform).sqrMagnitude;

                if (distance < radius)
                {
                    _vertices[i] -= Vector3.up * power;
                    _diggedVerticeCount++;
                }
            }

            _mesh.vertices = _vertices;
            UpdateMeshCollider();
            CheckDeformationRate();
        }
        #endregion
    }
}
