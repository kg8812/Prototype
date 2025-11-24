public partial class GameManager
{
    public static IController DefaultController;
    public static IController PlayerController;
    private static IController uiController;

    public static IController UiController
    {
        get => uiController;
        set
        {
            uiController = value;
            // Debug.LogError($"컨트롤러 바뀜 {value}");
        }
    }
}
