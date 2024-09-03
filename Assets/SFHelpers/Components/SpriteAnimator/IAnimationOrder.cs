
namespace SFH {
    public interface IAnimationOrder {
        public void Step();
        public int Clamp(int framesAmount);
        public void Reset();
    }
}