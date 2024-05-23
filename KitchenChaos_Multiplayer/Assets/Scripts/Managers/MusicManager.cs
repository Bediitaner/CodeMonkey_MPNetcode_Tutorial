using UnityEngine;

namespace KitchenChaos_Multiplayer.Managers
{
    public class MusicManager : MonoBehaviour
    {
        #region Singleton

        public static MusicManager Instance { get; private set; }
    

        #endregion
    
        #region Fields

        private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";

        private AudioSource audioSource;
        private float volume = .3f;

        #endregion

        #region Unity: Awake

        private void Awake()
        {
            Instance = this;

            audioSource = GetComponent<AudioSource>();

            volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, .3f);
            audioSource.volume = volume;
        }

        #endregion


        #region Get: Volume

        public float GetVolume()
        {
            return volume;
        }

        #endregion

        #region Change: Volume

        public void ChangeVolume()
        {
            volume += .1f;
            if (volume > 1f)
            {
                volume = 0f;
            }

            audioSource.volume = volume;

            PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, volume);
            PlayerPrefs.Save();
        }

        #endregion
    }
}