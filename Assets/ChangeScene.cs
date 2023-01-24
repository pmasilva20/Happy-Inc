using UnityEngine;

public class ChangeScene : MonoBehaviour
{

    public static void changeSceneLevelSelect()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelSelect");
    }

    public static void changeSceneMainScreen()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScreen");
    }

    public static void changeSceneLevel(int level)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(string.Format("Level{0}",level));
    }

    public static void changeLevel0()
    {
        changeSceneLevel(0);
    }
    public static void changeLevel1()
    {
        changeSceneLevel(1);
    }
    public static void changeLevel2()
    {
        changeSceneLevel(2);
    }
    public static void changeLevel3()
    {
        changeSceneLevel(3);
    }
}
