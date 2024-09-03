using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SFH {
    [CreateAssetMenu(fileName = "Particle Effect", menuName = "SFH/Particle Effect", order = 1)]
    public class ParticleFXSO : ScriptableObject {
        [field: SerializeField] public float duration { get; private set; }
        [field: SerializeField] private List<ParticleSystemDisplay> particles { get; set; } = new List<ParticleSystemDisplay>();
        [field: SerializeField] private List<TemporalSprite> temporalSprites { get; set; } = new List<TemporalSprite>();
        [field: SerializeField] public AudioQuerySetSO audioSet { get; private set; }
        public ParticleFX CreateFX(Vector2 position) {
            GameObject handle = new GameObject("Particle effect");
            ParticleFX pfx = handle.AddComponent<ParticleFX>();
            handle.transform.position = position;
            //Setup modules
            CreateParticles(handle);
            List<TemporalSpritePayload> temporalSprites = CreateTemporalSprites(handle);
            if (audioSet != null) AudioManager.inst.PlayRandomFromSet(audioSet);

            pfx.Init(Time.time + duration, handle, temporalSprites);
            return pfx;
        }

        private void CreateParticles(GameObject handle) {
            for (int i = 0; i < particles.Count; i++) {
                ParticleSystemDisplay psd = particles[i];
                ParticleSystem particleSystem = Instantiate(psd.particleSystem.gameObject, handle.transform,false).GetComponent<ParticleSystem>();
                if (psd.emissionType == ParticleSystemDisplay.EmissionType.Run) {
                    particleSystem.Play();
                } else if (psd.emissionType == ParticleSystemDisplay.EmissionType.Burst) {
                    particleSystem.Emit(psd.burstAmount);
                }
            }
        }

        private List<TemporalSpritePayload> CreateTemporalSprites(GameObject handle) {
            List<TemporalSpritePayload> temporalSpritePayloads = new List<TemporalSpritePayload>();
            for (int i = 0; i < temporalSprites.Count; i++) {
                TemporalSprite tSpr = temporalSprites[i];
                //Create gameObject with SpriteRenderer
                GameObject tSprHandle = new GameObject(tSpr.sprite.name);
                tSprHandle.transform.SetParent(handle.transform, false);
                SpriteRenderer tSprRenderer = tSprHandle.AddComponent<SpriteRenderer>();
                tSprRenderer.sprite = tSpr.sprite;
                //Create payload
                TemporalSpritePayload temporalSpritePayload = new TemporalSpritePayload(tSprHandle, tSprRenderer,tSpr.baseSize, tSpr.scaleCurve, tSpr.time, tSpr.animateSize);
                temporalSpritePayloads.Add(temporalSpritePayload);
            }
            return temporalSpritePayloads;
        }

        #region Module classes

        [System.Serializable]
        private class ParticleSystemDisplay {
            [HorizontalGroup, HideLabel] public ParticleSystem particleSystem;
            [HorizontalGroup, HideLabel] public EmissionType emissionType;
            [ShowIf("emissionType", EmissionType.Burst), HorizontalGroup, HideLabel] public int burstAmount;

            public enum EmissionType {
                Run,
                Burst
            }
        }

        [System.Serializable]
        private class TemporalSprite {
            public Sprite sprite;
            [HorizontalGroup(Width = .5f)] public float time;
            [HorizontalGroup(Width = .75f)] public bool animateSize;
            [ShowIf("animateSize", true)] public Vector2 baseSize;
            [ShowIf("animateSize", true)] public AnimationCurve scaleCurve;
        }

        #endregion
    }
}