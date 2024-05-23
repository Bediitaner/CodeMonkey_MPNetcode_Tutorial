using System;

namespace KitchenChaos_Multiplayer.Game
{
    public interface IHasProgress
    {
        public event EventHandler<OnProgressChangedEventArgs> OnProgressChangedEvent;

        public class OnProgressChangedEventArgs : EventArgs
        {
            public float progressNormalized;
        }
    }
}