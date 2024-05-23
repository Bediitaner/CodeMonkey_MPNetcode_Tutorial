using System;
using Counters;
using KitchenChaos_Multiplayer.Game;
using KitchenChaos_Multiplayer.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KitchenChaos_Multiplayer.Managers
{
    public class SoundManager : MonoBehaviour
    {
        #region Singleton

        public static SoundManager Instance { get; private set; }

        #endregion

        #region Contents

        [SerializeField] private AudioClipRefsSO audioClipRefsSO;

        #endregion

        #region Fields

        private float volume = 1f;

        private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";

        #endregion


        #region Unity: Awake | Start

        private void Awake()
        {
            Instance = this;

            volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f);
        }

        private void Start()
        {
            AddEvents();
        }

        #endregion


        #region Play: Sound

        private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f)
        {
            PlaySound(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volume);
        }

        private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f)
        {
            AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier * volume);
        }

        #endregion

        #region Play: FootstepsSound

        public void PlayFootstepsSound(Vector3 position, float volume)
        {
            PlaySound(audioClipRefsSO.footstep, position, volume);
        }

        #endregion

        #region Play: CountdownSound

        public void PlayCountdownSound()
        {
            PlaySound(audioClipRefsSO.warning, Vector3.zero);
        }

        #endregion

        #region Play: WarningSound

        public void PlayWarningSound(Vector3 position)
        {
            PlaySound(audioClipRefsSO.warning, position);
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

            PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, volume);
            PlayerPrefs.Save();
        }

        #endregion

        #region Get: Volume

        public float GetVolume()
        {
            return volume;
        }

        #endregion


        #region Event: OnAnyObjectTrashed

        private void OnAnyObjectTrashed(object sender, EventArgs e)
        {
            TrashCounter trashCounter = sender as TrashCounter;
            PlaySound(audioClipRefsSO.trash, trashCounter.transform.position);
        }

        #endregion

        #region Event: OnAnyObjectPlacedHere

        private void OnAnyObjectPlacedHere(object sender, EventArgs e)
        {
            BaseCounter baseCounter = sender as BaseCounter;
            PlaySound(audioClipRefsSO.objectDrop, baseCounter.transform.position);
        }

        #endregion

        #region Event: OnAnyPickedSomething

        private void OnAnyPickedSomething(object sender, EventArgs e)
        {
            Player player = sender as Player;
            PlaySound(audioClipRefsSO.objectPickup, player.transform.position);
        }

        #endregion

        #region Event: OnAnyCut

        private void OnAnyCut(object sender, EventArgs e)
        {
            CuttingCounter cuttingCounter = sender as CuttingCounter;
            PlaySound(audioClipRefsSO.chop, cuttingCounter.transform.position);
        }

        #endregion

        #region Event: OnRecipeFailed

        private void OnRecipeFailed(object sender, EventArgs e)
        {
            DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
            PlaySound(audioClipRefsSO.deliveryFail, deliveryCounter.transform.position);
        }

        #endregion

        #region Event: OnRecipeSuccess

        private void OnRecipeSuccess(object sender, EventArgs e)
        {
            DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
            PlaySound(audioClipRefsSO.deliverySuccess, deliveryCounter.transform.position);
        }

        #endregion

        #region Events: Add | Remove

        private void AddEvents()
        {
            DeliveryManager.Instance.OnRecipeSuccessEvent += OnRecipeSuccess;
            DeliveryManager.Instance.OnRecipeFailedEvent += OnRecipeFailed;
            CuttingCounter.OnAnyCutEvent += OnAnyCut;
            Player.OnAnyPickedSomethingEvent += OnAnyPickedSomething;
            BaseCounter.OnAnyObjectPlacedHereEvent += OnAnyObjectPlacedHere;
            TrashCounter.OnAnyObjectTrashedEvent += OnAnyObjectTrashed;
        }

        private void RemoveEvents()
        {
            DeliveryManager.Instance.OnRecipeSuccessEvent -= OnRecipeSuccess;
            DeliveryManager.Instance.OnRecipeFailedEvent -= OnRecipeFailed;
            CuttingCounter.OnAnyCutEvent -= OnAnyCut;
            Player.OnAnyPickedSomethingEvent -= OnAnyPickedSomething;
            BaseCounter.OnAnyObjectPlacedHereEvent -= OnAnyObjectPlacedHere;
            TrashCounter.OnAnyObjectTrashedEvent -= OnAnyObjectTrashed;
        }

        #endregion
    }
}