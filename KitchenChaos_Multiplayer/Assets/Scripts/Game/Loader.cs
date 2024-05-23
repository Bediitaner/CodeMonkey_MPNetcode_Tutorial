using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace KitchenChaos_Multiplayer.Game
{
    public static class Loader
    {
        #region Fields

        private static Scene targetScene;

        #endregion

        #region Load

        public static void Load(Scene targetScene)
        {
            Loader.targetScene = targetScene;

            SceneManager.LoadScene(Scene.LoadingScene.ToString());
        }

        #endregion

        #region Load: Network

        public static void LoadNetwork(Scene targetScene)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
        }

        #endregion

        #region LoaderCallback

        public static void LoaderCallback()
        {
            SceneManager.LoadScene(targetScene.ToString());
        }

        #endregion
    }
}