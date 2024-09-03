using UnityEngine;

namespace SFH {
    public class AnimationForwardOrder : IAnimationOrder {
        private int frame = 0;
        public int Clamp(int framesAmount) {
            frame %= framesAmount;
            return frame;
        }

        public void Step() {
            frame++;
        }

        public void Reset() {
            frame = 0;
        }
    }

    public class AnimationPingPongOrder : IAnimationOrder {
        private int frame = 0;
        private bool isForward = true;
        public int Clamp(int framesAmount) {
            if (frame >= framesAmount - 1) {
                isForward = false;
                frame = framesAmount - 1;
            }
            if (frame <= 0) {
                isForward = true;
                frame = 0;
            }
            return frame;
        }

        public void Step() {
            frame = isForward ? frame + 1 : frame - 1;
        }

        public void Reset() {
            frame = 0;
        }
    }
}