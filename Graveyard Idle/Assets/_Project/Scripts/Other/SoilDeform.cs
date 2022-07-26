using UnityEngine;

namespace GraveyardIdle
{
    public class SoilDeform : MonoBehaviour
    {
        private bool _deformed = false;

        private MeshFilter _meshFilter;
        private Mesh _mesh;
        private Vector3[] _vertices;
        private MeshCollider _meshCollider;
        private float _defaultVerticeHeight;

        private int _verticeCount, _currentDiggedVertice;

        [Header("-- SETUP --")]
        [SerializeField] private float radius;
        [SerializeField] private float power;

        private void Start()
        {
            _meshCollider = GetComponent<MeshCollider>();
            _meshFilter = GetComponent<MeshFilter>();
            _mesh = _meshFilter.mesh;
            _vertices = _mesh.vertices;
            _defaultVerticeHeight = _vertices[0].y;
            _verticeCount = _mesh.vertices.Length;
            _currentDiggedVertice = 0;
        }

        private void CheckDeformationRate()
        {
            Debug.Log((100 *_currentDiggedVertice) / _verticeCount);
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
                    _currentDiggedVertice++;
                }
            }

            _mesh.vertices = _vertices;
            _meshCollider.sharedMesh = null;
            _meshCollider.sharedMesh = _meshFilter.mesh;
            CheckDeformationRate();
        }
        #endregion
    }
}
