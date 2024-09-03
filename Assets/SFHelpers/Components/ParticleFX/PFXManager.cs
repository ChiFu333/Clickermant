using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SFH {
    public class PFXManager : MonoSingleton<PFXManager> {
        private readonly PriorityQueue<ParticleFX> particleQueue = new SFH.PriorityQueue<ParticleFX>();

        public void Emit(ParticleFXSO fx, Vector2 position) {
            ParticleFX particleFX = fx.CreateFX(position);
            particleQueue.Enqueue(particleFX, particleFX.removalTime);
        }

        #region Internal

        private void HandleParticleQueue() {
            while (particleQueue.Count > 0 && particleQueue.Peek().removalTime <= Time.time) {
                ParticleFX particleFX = particleQueue.Dequeue();
                Destroy(particleFX.handle);
            }
        }

        private void Update() {
            HandleParticleQueue();
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/SFH/ParticleFX Manager")]
        private static void CreateParticleFXManager() {
            GameObject pfx = new GameObject("ParticleFX Manager");
            pfx.AddComponent<PFXManager>();
        }
#endif

        #endregion
    }
}