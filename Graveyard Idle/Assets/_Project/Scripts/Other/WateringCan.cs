using UnityEngine;
using ZestGames;

namespace GraveyardIdle
{
    public class WateringCan : MonoBehaviour
    {
        private Player _player;
        private MeshRenderer _meshRenderer;
        private ParticleSystem _waterParticle;

        public void Init(Player player)
        {
            _player = player;

            _meshRenderer = GetComponent<MeshRenderer>();
            _waterParticle = transform.GetChild(0).GetComponent<ParticleSystem>();
            DisableMesh();

            WateringCanEvents.OnStartedWatering += EnableMesh;
            WateringCanEvents.OnStoppedWatering += DisableMesh;
        }

        private void OnDisable()
        {
            if (!_player) return;

            WateringCanEvents.OnStartedWatering -= EnableMesh;
            WateringCanEvents.OnStoppedWatering -= DisableMesh;
        }

        private void EnableMesh()
        {
            _meshRenderer.enabled = true;
            _waterParticle.Play();
        }
        private void DisableMesh()
        {
            _meshRenderer.enabled = false;
            _waterParticle.Stop();
        }
    }
}
