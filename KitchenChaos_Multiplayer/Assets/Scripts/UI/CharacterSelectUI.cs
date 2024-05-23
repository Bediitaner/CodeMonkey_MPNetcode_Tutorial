using System;
using System.Collections;
using System.Collections.Generic;
using KitchenChaos_Multiplayer.Game;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button _btnMainMenu;
    [SerializeField] private Button _btnReady;


    private void Awake()
    {
        _btnMainMenu.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Scene.MainMenuScene);
        });
        _btnReady.onClick.AddListener(() =>
        {
            CharacterSelectReady.Instance.SetPlayerReady();
        });
    }
}
