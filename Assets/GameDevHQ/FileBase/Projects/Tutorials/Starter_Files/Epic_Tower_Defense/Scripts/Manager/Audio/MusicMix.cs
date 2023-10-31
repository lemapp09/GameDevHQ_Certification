using UnityEngine;
using UnityEngine.Audio;


namespace MetroMayhem.Manager.Audio
{

    [CreateAssetMenu(fileName = "/Assets/Scripts/Audio/MusicMixes/musicMix", menuName = "MusicManager/Music Mix", order = 4)]
    public class MusicMix : ScriptableObject
    {
        [SerializeField] private AudioMixerGroup _mixerGroup;
        [SerializeField] private AudioMixerSnapshot _audioMixerSnapshot;
        
        AudioSource _audioSource;

        public void Initialize(AudioManager audioManager)
        {
            var audioSourceGameObject = new GameObject($"(name) Audio Source");
            audioSourceGameObject.transform.SetParent(audioManager.transform);
            _audioSource = audioSourceGameObject.AddComponent<AudioSource>();
            _audioSource.loop = true;
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0;
            _audioSource.volume = 1;
            _audioSource.outputAudioMixerGroup = _mixerGroup;
        }
        
        public void PlayClip(AudioClip clip)
        {
            _audioSource.clip = clip;
            _audioSource.Play();
            _audioMixerSnapshot.TransitionTo(1f);
        }
    }
}