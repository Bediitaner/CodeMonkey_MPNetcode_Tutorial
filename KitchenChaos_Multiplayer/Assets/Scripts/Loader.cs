using UnityEngine.SceneManagement;

public enum Scene
{
    MainMenuScene,
    GameScene,
    LoadingScene
}

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

    #region LoaderCallback

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }

    #endregion
}