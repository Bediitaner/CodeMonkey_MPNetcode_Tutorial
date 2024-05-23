using System;
using System.Collections.Generic;
using Counters;
using KitchenChaos_Multiplayer.Managers;
using Unity.Netcode;
using UnityEngine;

namespace KitchenChaos_Multiplayer.Game
{
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    public class Player : NetworkBehaviour, IKitchenObjectParent
    {
        #region Events

        public static event EventHandler OnAnyPlayerSpawnedEvent;
        public static event EventHandler OnAnyPickedSomethingEvent;
        public event EventHandler OnPickedSomethingEvent;
        public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChangedEvent;

        #endregion

        #region Singleton

        public static Player LocalInstance { get; private set; }

        #endregion

        #region Reset: StaticData

        public static void ResetStaticData()
        {
            OnAnyPlayerSpawnedEvent = null;
        }

        #endregion


        #region Contents

        [SerializeField] private float moveSpeed = 7f;
        [SerializeField] private LayerMask countersLayerMask;
        [SerializeField] private LayerMask collisionsLayerMask;
        [SerializeField] private Transform kitchenObjectHoldPoint;
        [SerializeField] private List<Vector3> spawnPositionList;

        #endregion

        #region Fields

        private bool isWalking;
        private Vector3 lastInteractDir;
        private BaseCounter selectedCounter;
        private KitchenObject kitchenObject;

        #endregion


        #region Unity: Awake | Start | Update

        private void Awake()
        {
        }

        private void Start()
        {
            AddEvents();
        }

        private void Update()
        {
            if (!IsLocalPlayer) return;

            HandleMovement();
            HandleInteractions();
        }

        #endregion

        #region Override: OnNetworkSpawn

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                LocalInstance = this;
            }

            transform.position = spawnPositionList[(int)OwnerClientId];

            OnAnyPlayerSpawnedEvent?.Invoke(this, EventArgs.Empty);

            if (IsServer)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
            }
        
            Debug.Log("ClientId: " + OwnerClientId + " ServerId: " + NetworkManager.ServerClientId);
        }

        #endregion


        #region Is: Walking

        public bool IsWalking()
        {
            return isWalking;
        }

        #endregion

        #region Has: KitchenObject

        public bool HasKitchenObject()
        {
            return kitchenObject != null;
        }

        #endregion


        #region Handle: Interactions

        private void HandleInteractions()
        {
            Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();

            Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

            if (moveDir != Vector3.zero)
            {
                lastInteractDir = moveDir;
            }

            float interactDistance = 2f;
            if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask))
            {
                if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
                {
                    // Has ClearCounter
                    if (baseCounter != selectedCounter)
                    {
                        SetSelectedCounter(baseCounter);
                    }
                }
                else
                {
                    SetSelectedCounter(null);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }

        #endregion

        #region Handle: Movement

        private void HandleMovement()
        {
            Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();

            Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

            float moveDistance = moveSpeed * Time.deltaTime;
            float playerRadius = .7f;
            float playerHeight = 2f;
            bool canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDir, Quaternion.identity, moveDistance, collisionsLayerMask);

            if (!canMove)
            {
                // Cannot move towards moveDir

                // Attempt only X movement
                Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
                canMove = (moveDir.x < -.5f || moveDir.x > +.5f) &&
                          !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirX, Quaternion.identity, moveDistance, collisionsLayerMask);

                if (canMove)
                {
                    // Can move only on the X
                    moveDir = moveDirX;
                }
                else
                {
                    // Cannot move only on the X

                    // Attempt only Z movement
                    Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                    canMove = (moveDir.z < -.5f || moveDir.z > +.5f) && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirZ, Quaternion.identity,
                        moveDistance, collisionsLayerMask);

                    if (canMove)
                    {
                        // Can move only on the Z
                        moveDir = moveDirZ;
                    }
                    else
                    {
                        // Cannot move in any direction
                    }
                }
            }

            if (canMove)
            {
                transform.position += moveDir * moveDistance;
            }

            isWalking = moveDir != Vector3.zero;

            float rotateSpeed = 10f;
            transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
        }

        #endregion


        #region Set: SelectedCounter

        private void SetSelectedCounter(BaseCounter selectedCounter)
        {
            this.selectedCounter = selectedCounter;

            OnSelectedCounterChangedEvent?.Invoke(this, new OnSelectedCounterChangedEventArgs
            {
                selectedCounter = selectedCounter
            });
        }

        #endregion

        #region Set: KitchenObject

        public void SetKitchenObject(KitchenObject kitchenObject)
        {
            this.kitchenObject = kitchenObject;

            if (kitchenObject != null)
            {
                OnPickedSomethingEvent?.Invoke(this, EventArgs.Empty);
                OnAnyPickedSomethingEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion


        #region Get: KitchenObjectFollowTransform

        public Transform GetKitchenObjectFollowTransform()
        {
            return kitchenObjectHoldPoint;
        }

        #endregion

        #region Get: KitchenObject

        public KitchenObject GetKitchenObject()
        {
            return kitchenObject;
        }

        #endregion

        #region Get: NetworkObject

        public NetworkObject GetNetworkObject()
        {
            return NetworkObject;
        }

        #endregion


        #region Clear: KitchenObject

        public void ClearKitchenObject()
        {
            kitchenObject = null;
        }

        #endregion


        #region Event: OnClientDisconnectCallback

        private void OnClientDisconnectCallback(ulong clientId)
        {
            if (clientId == OwnerClientId && HasKitchenObject())
            {
                KitchenObject.DestroyKitchenObject(GetKitchenObject());
            }
        }

        #endregion


        #region Event: OnInteractAlternateAction

        private void OnInteractAlternateAction(object sender, EventArgs e)
        {
            if (!KitchenGameManager.Instance.IsGamePlaying()) return;

            if (selectedCounter != null)
            {
                selectedCounter.InteractAlternate(this);
            }
        }

        #endregion

        #region Event: OnInteractAction

        private void OnInteractAction(object sender, EventArgs e)
        {
            if (!KitchenGameManager.Instance.IsGamePlaying()) return;

            if (selectedCounter != null)
            {
                selectedCounter.Interact(this);
            }
        }

        #endregion

        #region Events: Add | Remove

        private void AddEvents()
        {
            GameInput.Instance.OnInteractActionEvent += OnInteractAction;
            GameInput.Instance.OnInteractAlternateActionEvent += OnInteractAlternateAction;
        }

        private void RemoveEvents()
        {
            GameInput.Instance.OnInteractActionEvent -= OnInteractAction;
            GameInput.Instance.OnInteractAlternateActionEvent -= OnInteractAlternateAction;
        }

        #endregion
    }
}