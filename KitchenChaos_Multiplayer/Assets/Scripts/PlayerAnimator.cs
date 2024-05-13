using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
    #region Contents

    [SerializeField] private Player player;

    #endregion

    #region Fields

    private Animator animator;

    private const string IS_WALKING = "IsWalking";

    #endregion

    #region Unity: Awake | Update

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        animator.SetBool(IS_WALKING, player.IsWalking());
    }

    #endregion
}