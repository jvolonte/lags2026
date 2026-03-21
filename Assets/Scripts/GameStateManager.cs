public class GameStateManager
{
   GameState gameState;
}

public enum GameState
{
    Setup,
    EnemyPlaysCard,
    PlayerPlaysCard,
    PlayerPlaceSticker,
    EnemyPlaceSticker,
    ConflictResolution,
    Draw
}