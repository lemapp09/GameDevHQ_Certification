using System.Collections.Generic;
using MetroMayhem.Manager;
using UnityEngine;


namespace MetroMayhem.Manager.Audio
{
    [CreateAssetMenu(fileName = "Assets/Scripts/Audio/MusicClipGroups/MusicClipGroup", menuName = "MusicManager/Music Clip Group", order = 3)]
    public class MusicClipGroup : ScriptableObject
    {
        [SerializeField] private List<AudioClip> _musicClips;
        private Queue<AudioClip> _trackQueue;
        private List<AudioClip> _tempList;

        public void Initialize(AudioManager musicManager)
        {
            InitializeTrackQueues();
        }

        public AudioClip GetNextMusicClip()
        {
            if (_trackQueue.Count == 0) ShuffleTrackQueue();
            var clip = _trackQueue.Dequeue();
            return clip;
        }

        private void ShuffleTrackQueue()
        {
            if (_trackQueue.Count != 0) return;
            _tempList.Clear();
            foreach (var track in _musicClips)
            {
                _tempList.Add(track);
            }
            while(_tempList.Count > 0)
            {
                var randomIndex = Random.Range(0, _tempList.Count);
                _trackQueue.Enqueue(_tempList[randomIndex]);
                _tempList.RemoveAt(randomIndex);
            }
        }

        private void InitializeTrackQueues()
        {
            _trackQueue = new Queue<AudioClip>();
            _tempList = new List<AudioClip>();
        }
    }
}