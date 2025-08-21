namespace FallMonke.Networking;

public enum EventCodesEnum : byte
{
    FALL_TILE = 25, // todo: find safe codes to use 
    SPAWN_PLAYER_ON_RANDOM_TILE,
    SHOW_NOTIFICATION,
    REQUEST_TO_START_GAME,
}
