using UnityEngine;

namespace SFH {
    public class TemporalSpritePayload {
        public GameObject spriteObject { get; private set; }
        public SpriteRenderer spriteRenderer { get; private set; }
        public Vector2 spriteScale { get; private set; }
        public AnimationCurve scaleCurve { get; private set; }
        public float initialTime { get; private set; }
        public bool animateSize { get; private set; }
        public float time;
        public TemporalSpritePayload(GameObject _spriteObject, SpriteRenderer _spriteRenderer, Vector2 _spriteScale, AnimationCurve _scaleCurve, float _time, bool _animateSize) {
            spriteObject = _spriteObject;
            spriteRenderer = _spriteRenderer;
            spriteScale = _spriteScale;
            scaleCurve = _scaleCurve;
            initialTime = _time;
            time = _time;
            animateSize = _animateSize;
        }
    }
}