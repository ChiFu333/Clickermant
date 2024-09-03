using System.Collections.Generic;
using UnityEngine;

namespace SFH {
    public class ParticleFX : MonoBehaviour {
        public float removalTime { get; private set; }
        public GameObject handle { get; private set; }
        public List<TemporalSpritePayload> temporalSprites { get; private set; }
        public void Init(float _removalTime, GameObject _handle, List<TemporalSpritePayload> _temporalSprites) {
            removalTime = _removalTime;
            handle = _handle;
            temporalSprites = _temporalSprites;
        }

        private void UpdateTemporalSprites() {
            for (int i = temporalSprites.Count - 1; i >= 0; i--) {
                TemporalSpritePayload tsp = temporalSprites[i];
                tsp.time -= Time.deltaTime;
                if (tsp.time <= 0) {
                    //Destroy temporal sprite
                    Destroy(tsp.spriteObject);
                    temporalSprites.RemoveAt(i);
                }
                if (tsp.animateSize) {
                    //Scale temporal sprite
                    float t = tsp.scaleCurve.Evaluate(1 - (tsp.time / tsp.initialTime));
                    Vector2 scale = tsp.spriteScale * t;
                    tsp.spriteObject.transform.localScale = scale;
                }
            }
        }

        private void Update() {
            UpdateTemporalSprites();
        }
    }
}