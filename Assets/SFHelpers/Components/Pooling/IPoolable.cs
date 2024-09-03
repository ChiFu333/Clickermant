
namespace SFH {
    public interface IPoolable {
        public void OnReuse();
        public void OnRelease();
    }
}