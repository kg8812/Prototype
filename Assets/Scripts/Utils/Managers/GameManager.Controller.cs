public partial class GameManager
{
    public static IController DefaultController;
    public static IController PlayerController;

    public static IController UiController
    {
        get;
        set;
        // Debug.LogError($"컨트롤러 바뀜 {value}");
    }
}