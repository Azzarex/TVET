using UnityEngine;

namespace AxlPlay
{
    public class PlayAudioOnInteraction : MonoBehaviour
    {

        public AudioSource AudioSource;
        // 0 = always 
        public int MaxTimesToPlaySound = 0;

        private int played;
        // play sound if click on it
        public void Interaction()
        {
            if (AudioSource && !AudioSource.isPlaying)
            {
                if (played < MaxTimesToPlaySound || MaxTimesToPlaySound == 0)
                {
                    AudioSource.Play();
                    played++;
                }
            }
        }
    }
}
