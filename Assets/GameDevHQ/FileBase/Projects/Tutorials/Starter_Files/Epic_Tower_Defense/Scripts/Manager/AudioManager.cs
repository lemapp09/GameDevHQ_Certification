using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MetroMayhem.Manager.Audio;
using UnityEngine;

namespace MetroMayhem.Manager
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        /// <summary>
        /// Represents a list of music clip groups.
        /// </summary>
        [SerializeField] List<MusicClipGroup> _musicClipGroups;

        /// <summary>
        /// The list of audio clips for hurt sounds.
        /// </summary>
        [SerializeField] private List<AudioClip> _hurtSounds;

        /// <summary>
        /// Serialized field for storing a list of death sound AudioClips.
        /// </summary>
        [SerializeField] private  List<AudioClip> _deathSounds;

        /// <summary>
        /// List of MusicMix objects.
        /// </summary>
        [SerializeField] List<MusicMix> _musicMixes;

        /// <summary>
        /// The current index of mixing.
        /// </summary>
        private int _currentMixIndex = 0;

        /// <summary>
        /// The currently selected music clip group.
        /// </summary>
        private MusicClipGroup _currentMusicGroup;

        /// <summary>
        /// Plays peaceful music.
        /// </summary>
        /// <remarks>
        /// This method is called when the "Peaceful Music" context menu option is selected. It plays the peaceful music by calling the <see cref="PlayMusic"/> method with the "Peaceful" parameter
        /// .
        /// </remarks>
        [ContextMenu("Peaceful Music")]
        public void PlayPeacefulMusic() {
            PlayMusic("Peaceful");
        }

        /// <summary>
        /// Plays Combat Music 1.
        /// </summary>
        [ContextMenu("Combat Music 1")]
        public void PlayCombat1Music() {
            PlayMusic("Combat1");
        }

        /// <summary>
        /// Initiates the playing of Combat Music 2.
        /// </summary>
        /// <remarks>
        /// This method is used to play Combat Music 2. It can be accessed by right-clicking on the script in the Unity editor and selecting "Combat Music 2".
        /// When called, it invokes the PlayMusic method with the specified music clip ("Combat2").
        /// </remarks>
        [ContextMenu("Combat Music 2")]
        public void PlayCombat2Music() {
            PlayMusic("Combat2");
        }

        /// <summary>
        /// Play the Combat 3 music.
        /// </summary>
        /// <remarks>
        /// This method is used to play the Combat 3 music track.
        /// </remarks>
        [ContextMenu("Combat Music 3")]
        public void PlayCombat3Music() {
            PlayMusic("Combat3");
        }

        /// <summary>
        /// Initializes the music clip groups, initializes the music mixes, and plays the specified music.
        /// </summary>
        private void Start() {
            InitializeMusicClipGroups();
            InitializeMusicMixes();
            PlayMusic("Peaceful");
        }

        /// <summary>
        /// Plays a random hurt sound.
        /// </summary>
        /// <returns>The AudioClip of the hurt sound.</returns>
        public AudioClip PlayHurtSound() {
                return _hurtSounds[Random.Range(0,_hurtSounds.Count)];
        }

        /// <summary>
        /// Plays a death sound clip.
        /// </summary>
        /// <returns>The selected audio clip from the list of death sounds.</returns>
        public AudioClip PlayDeathSound() {
            return _deathSounds[Random.Range(0,_deathSounds.Count)];
        }

        /// <summary>
        /// Plays music for a specified music group.
        /// </summary>
        /// <param name="musicGroupName">The name of the music group to play.</param>
        private void PlayMusic(string musicGroupName) {
            StopAllCoroutines();
            var temp = GetMusicGroup(musicGroupName);
            if (temp != null) {
                _currentMusicGroup = temp;
            }

            PlayNextTrack();
        }

        /// <summary>
        /// Plays the next track in the current music group.
        /// </summary>
        private void PlayNextTrack() {
            var clip = _currentMusicGroup.GetNextMusicClip();
            ToggleCurrentMix();
            _musicMixes[_currentMixIndex].PlayClip(clip);
            StartCoroutine(QueueNextTrack(clip.length));
        }

        /// <summary>
        /// Toggles the current mix index between 0 and 1.
        /// </summary>
        private void ToggleCurrentMix() {
            _currentMixIndex = _currentMixIndex == 0 ? 1 : 0;
        }

        /// <summary>
        /// Retrieves a music group by its name.
        /// </summary>
        /// <param name="musicGroupName">The name of the music group to retrieve.</param>
        /// <returns>The MusicClipGroup object matching the given name, or null if not found.</returns>
        private MusicClipGroup GetMusicGroup(string musicGroupName)
        {
            if (_musicClipGroups != null) {
                return _musicClipGroups.FirstOrDefault(group => group.name == musicGroupName);
            }
            return null;
        }

        /// <summary>
        /// Queue the next track to be played after a specified delay.
        /// </summary>
        /// <param name="delay">The delay in seconds before playing the next track.</param>
        /// <returns>An enumerator that can be used to iterate through the queue.</returns>
        IEnumerator QueueNextTrack(float delay) {
            yield return new WaitForSeconds(delay);
            PlayNextTrack();
        }

        /// <summary>
        /// Initializes the music clip groups.
        /// </summary>
        private void InitializeMusicClipGroups() {
            foreach (var musicClipGroup in _musicClipGroups) {
                musicClipGroup.Initialize(this);
            }
        }

        /// Initializes the music mixes.
        /// This method calls the Initialize method of each music mix in the _musicMixes list,
        /// passing the current object as an argument.
        /// @return void
        /// /
        private void InitializeMusicMixes() {
            foreach (var musicMix in _musicMixes) {
                musicMix.Initialize(this);
            }
        }
    }
}