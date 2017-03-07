public static class Constants
{
    public const int MAP_INVISIBLE_LAYER = 8;
    public const int MAP_VISIBLE_LAYER = 9;
    public const int MAP_TERRAIN_LAYER = 10;
    public const int MAP_TERRAIN_MASK = 1 << MAP_TERRAIN_LAYER;
}

/// <summary>
/// Enumerates known input behaviours across different platforms.
/// Behaviour must be added here to convert from corresponding settings
/// to actual behaviour.
/// </summary>
public enum RegisteredInputs
{
    Drag,
    Zoom,
    Billboard,
    Hitbox
}