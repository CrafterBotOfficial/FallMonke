
namespace FallMonke;

public static class GameConfig
{
    public const int REQUIRED_PLAYER_COUNT = 2;
    public const int MIN_PLAYERS_TO_CONTINUE = 1;

    public const float SLOW_JUMP_LIMIT = 6.5f;
    public const float FAST_JUMP_LIMIT = 8.5f;
    public const float SLOW_JUMP_MULTIPLIER = 1.1f;
    public const float FAST_JUMP_MULTIPLIER = 1.3f;

    public const int START_GAME_COUNTDOWN_SECONDS = 10;
    public const float START_GAME_MOVEMENT_COOLDOWN_SECONDS = 3f;
    public const int FINISHED_DELAY_SECONDS = 5;
}
