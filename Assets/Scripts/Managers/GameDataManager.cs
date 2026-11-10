
public class GameDataManager {
    private static GameDataManager SInstance = null;

    public static GameDataManager Instance {
        get { return SInstance ??= new GameDataManager(); }
    }

    public bool FirstTimeInMainMenu = true;
}
