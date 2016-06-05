using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.Assertions;

namespace Assets.VREF.Application.HUD
{

    public class AudioInstructions : MonoBehaviour
    {

        /// <summary>
        /// get the clips for faster access
        /// </summary>
        private Dictionary<string, AudioClip> clips;

        /// <summary>
        /// containing all clips in a serializable form
        /// </summary>
        public List<NameToClipMapping> clipMap;

        private AudioSource audioSource;

        void Awake()
        {
            if (clipMap == null)
                clipMap = new List<NameToClipMapping>();
            
            audioSource = GetComponent<AudioSource>();

            Assert.IsNotNull(audioSource);
            
            clips = new Dictionary<string, AudioClip>();
        }
        

        void Start()
        {
            foreach (var item in clipMap)
            {
                clips.Add(item.Name, item.clip);
            }
        }

        public void play(string nameOfAudioClip)
        {
            if (clips.ContainsKey(nameOfAudioClip))
            {
                var requestedClip = clips[nameOfAudioClip];
                audioSource.clip = requestedClip;
                audioSource.Play();

                return;
            }

            throw new ArgumentException(string.Format("The expected clip '{0}' was not found!", nameOfAudioClip));

        }
        
    }

    [Serializable]
    public class NameToClipMapping
    {
        public string Name;

        public AudioClip clip;
    }
}