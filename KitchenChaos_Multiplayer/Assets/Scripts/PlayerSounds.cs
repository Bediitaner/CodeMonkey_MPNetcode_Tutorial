using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    #region Fields

    private Player player;
    private float footstepTimer;
    private float footstepTimerMax = .1f;

    #endregion
    
    #region Unity: Awake | Update

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        footstepTimer -= Time.deltaTime;
        if (footstepTimer < 0f)
        {
            footstepTimer = footstepTimerMax;

            if (player.IsWalking())
            {
                float volume = 1f;
                SoundManager.Instance.PlayFootstepsSound(player.transform.position, volume);
            }
        }
    }

    #endregion
}