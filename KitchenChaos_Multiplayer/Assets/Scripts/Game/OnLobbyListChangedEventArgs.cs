using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

namespace Game
{
    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> LobbyList { get; set; }
    }
}