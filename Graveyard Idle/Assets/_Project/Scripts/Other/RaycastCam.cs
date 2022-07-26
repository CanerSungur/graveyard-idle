using UnityEngine;

namespace GraveyardIdle
{
    public class RaycastCam : MonoBehaviour
    {
        Ray _ray;
        RaycastHit _hit;
        private Camera _camera;

        private void Start()
        {
            _camera = GetComponent<Camera>();
        }

        private void FixedUpdate()
        {
            //if (Input.GetMouseButtonDown(0))
            //{
            //    DeformMesh();
            //}
        }

        private void DeformMesh()
        {
            _ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(_ray, out _hit))
            {
                // Deform
                SoilDeform soilDeform = _hit.transform.GetComponent<SoilDeform>();
                soilDeform.DeformThis(_hit.point);
            }
        }
    }
}
