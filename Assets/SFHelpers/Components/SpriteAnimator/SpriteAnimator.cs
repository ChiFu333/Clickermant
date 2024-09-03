using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SFH {
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimator : MonoBehaviour {
        [SerializeField] private SpriteAnimationSO animationData;
        private SpriteRenderer spriteRenderer => GetComponent<SpriteRenderer>();
        private float accumulatedDt;
        private float delta;
        private IAnimationOrder animationOrder;

        public void SetAnimation(SpriteAnimationSO newAnimationData, bool resetAnimation = false) {
            animationData = newAnimationData;
            delta = 1 / animationData.framerate;
            animationOrder = animationOrders[newAnimationData.animationOrder];
            if (resetAnimation) animationOrder.Reset();
        }

        #region Internal

        private void UpdateAnimation() {
            if (animationData == null) return;
            accumulatedDt += Time.deltaTime;
            while (accumulatedDt >= delta) {
                accumulatedDt -= delta;
                animationOrder.Step();
            }
            SetFrame();
        }

        private void SetFrame() {
            int frame = animationOrder.Clamp(animationData.GetFramesAmount());
            spriteRenderer.sprite = animationData.GetSprite(frame);
        }

        private void Start() {
            SetAnimation(animationData);
        }

        private void Update() {
            UpdateAnimation();
        }

        private Dictionary<SpriteAnimationSO.Order, IAnimationOrder> animationOrders = new Dictionary<SpriteAnimationSO.Order, IAnimationOrder>{
            { SpriteAnimationSO.Order.Forward,  new AnimationForwardOrder() },
            { SpriteAnimationSO.Order.PingPong, new AnimationPingPongOrder() }
        };

        #endregion
    }
}