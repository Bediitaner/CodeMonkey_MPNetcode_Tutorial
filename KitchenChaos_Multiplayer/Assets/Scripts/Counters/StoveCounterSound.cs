using UnityEngine;

namespace Counters
{
    public class StoveCounterSound : MonoBehaviour
    {
        #region Content

        [SerializeField] private StoveCounter stoveCounter;

        #endregion
        
        #region Fields

        private AudioSource audioSource;
        private float warningSoundTimer;
        private bool playWarningSound;

        #endregion

        #region Unity: Awake | Start | Update

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            AddEvents();
        }
        
        private void Update()
        {
            if (playWarningSound)
            {
                warningSoundTimer -= Time.deltaTime;
                if (warningSoundTimer <= 0f)
                {
                    float warningSoundTimerMax = .2f;
                    warningSoundTimer = warningSoundTimerMax;

                    SoundManager.Instance.PlayWarningSound(stoveCounter.transform.position);
                }
            }
        }

        #endregion


        #region Event: OnProgressChanged

        private void OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
        {
            float burnShowProgressAmount = .5f;
            playWarningSound = stoveCounter.IsFried() && e.progressNormalized >= burnShowProgressAmount;
        }
        

        #endregion

        #region Event: OnStateChanged

        private void OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
        {
            bool playSound = e.state == State.Frying || e.state == State.Fried;
            if (playSound)
            {
                audioSource.Play();
            }
            else
            {
                audioSource.Pause();
            }
        }
        

        #endregion
        
        #region Events: Add | Remove

        private void AddEvents()
        {
            stoveCounter.OnProgressChanged += OnProgressChanged;
            stoveCounter.OnStateChanged += OnStateChanged;
        }

        private void RemoveEvents()
        {
            stoveCounter.OnProgressChanged -= OnProgressChanged;
            stoveCounter.OnStateChanged -= OnStateChanged;
        }
        
        #endregion
    }
}