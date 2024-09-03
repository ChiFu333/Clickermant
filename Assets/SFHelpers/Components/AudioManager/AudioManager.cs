using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SFH {
    public class AudioManager : MonoSingleton<AudioManager> {
        public const string musicPrefsKey = "musicVolume";
        public const string soundPrefsKey = "soundVolume";
        public AudioClip musicClip;

        private AudioSource musicSource;
        private Dictionary<TargetSource,AudioSource> audioSourceLookup;
        private GameObjectPool<AudioShot> musicPool;

        public void SetVolume(TargetSource target, float volume) {
            audioSourceLookup[target].volume = volume;
        }

        public void Release(AudioShot shot) {
            musicPool.Release(shot);
        }

        public void PlayQuery(AudioQuerySO query) {
            musicPool.Get().Play(query);
        }

        public void PlayRandomFromSet(AudioQuerySetSO set) {
            AudioQuerySO query = set.audioQueries.RandomItem();
            PlayQuery(query);
        }

        public void PlayMusic(AudioClip musicClip) {
            musicSource.PlayOneShot(musicClip);
        }

        #region Internal

        protected override void SingletonAwake() {
            //Create music player
            GameObject musicSourceObject = new GameObject();
            musicSourceObject.transform.SetParent(transform);
            musicSource = musicSourceObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            //Create sound player
            /*GameObject soundSourceObject = new GameObject();
            soundSourceObject.transform.SetParent(transform);
            soundSource = soundSourceObject.AddComponent<AudioSource>();*/
            //Put into dictionary
            audioSourceLookup = new Dictionary<TargetSource, AudioSource>(){
            { TargetSource.Music, musicSource },
            /*{ TargetSource.Sound, soundSource }*/};
            //Load and apply volumes
            SetVolume(TargetSource.Music, PlayerPrefs.GetFloat(musicPrefsKey, 0.4f));
            //SetVolume(TargetSource.Sound, PlayerPrefs.GetFloat(soundPrefsKey, 0.7f));
            //Setup audioPool
            musicPool = new GameObjectPool<AudioShot>(Resources.Load<AudioShot>("AudioShotPrefab"));
            //Play music
            PlayMusic(musicClip);
            DontDestroyOnLoad(gameObject);
        }

        public enum TargetSource {
            Music,
            Sound
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/SFH/Audio Manager")]
        private static void CreateAudioManager() {
            GameObject audio = new GameObject("Audio Manager");
            audio.AddComponent<AudioManager>();
        }
#endif

        #endregion
    }
}