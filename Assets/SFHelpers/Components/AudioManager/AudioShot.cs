using SFH;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioShot : MonoBehaviour, IPoolable {
    private AudioSource soundSource => GetComponent<AudioSource>();

    public void OnRelease() {}

    public void OnReuse() {}

    public void Play(AudioQuerySO query) {
        soundSource.pitch = 1 + Random.Range(query.pitchVariance.x, query.pitchVariance.y);
        soundSource.PlayOneShot(query.clip);
        Invoke(nameof(Release), query.clip.length);
    }

    private void Start() {
        transform.SetParent(AudioManager.inst.transform);
    }

    private void Release() {
        AudioManager.inst.Release(this);
    }
}