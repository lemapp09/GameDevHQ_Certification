using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MetroMayhem.Manager.Audio;
using UnityEngine;

namespace MetroMayhem.Manager
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        
        [SerializeField] List<MusicClipGroup> _musicClipGroups;
        [SerializeField] private List<AudioClip> _hurtSounds;
        [SerializeField] private  List<AudioClip> _deathSounds;
        [SerializeField] List<MusicMix> _musicMixes;
        private int _currentMixIndex = 0;
        private MusicClipGroup _currentMusicGroup;

        [ContextMenu("Peaceful Music")]
        public void PlayPeacefulMusic() {
            PlayMusic("Peaceful");
        }

        [ContextMenu("Combat Music 1")]
        public void PlayCombat1Music() {
            PlayMusic("Combat1");
        }

        [ContextMenu("Combat Music 2")]
        public void PlayCombat2Music() {
            PlayMusic("Combat2");
        }

        [ContextMenu("Combat Music 3")]
        public void PlayCombat3Music() {
            PlayMusic("Combat3");
        }

        private void Start() {
            InitializeMusicClipGroups();
            InitializeMusicMixes();
            PlayMusic("Peaceful");
        }

        public AudioClip PlayHurtSound() {
                return _hurtSounds[Random.Range(0,_hurtSounds.Count)];
        }

        public AudioClip PlayDeathSound() {
            return _deathSounds[Random.Range(0,_deathSounds.Count)];
        }

        private void PlayMusic(string musicGroupName) {
            StopAllCoroutines();
            var temp = GetMusicGroup(musicGroupName);
            if (temp != null) {
                _currentMusicGroup = temp;
            }

            PlayNextTrack();
        }

        private void PlayNextTrack() {
            var clip = _currentMusicGroup.GetNextMusicClip();
            ToggleCurrentMix();
            _musicMixes[_currentMixIndex].PlayClip(clip);
            StartCoroutine(QueueNextTrack(clip.length));
        }

        private void ToggleCurrentMix() {
            _currentMixIndex = _currentMixIndex == 0 ? 1 : 0;
        }

        private MusicClipGroup GetMusicGroup(string musicGroupName)
        {
            if (_musicClipGroups != null) {
                return _musicClipGroups.FirstOrDefault(group => group.name == musicGroupName);
            }
            return null;
        }

        IEnumerator QueueNextTrack(float delay) {
            yield return new WaitForSeconds(delay);
            PlayNextTrack();
        }

        private void InitializeMusicClipGroups() {
            foreach (var musicClipGroup in _musicClipGroups) {
                musicClipGroup.Initialize(this);
            }
        }

        private void InitializeMusicMixes() {
            foreach (var musicMix in _musicMixes) {
                musicMix.Initialize(this);
            }
        }
    }
}