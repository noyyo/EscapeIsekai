using UnityEngine;

public static class TagsAndLayers
{
    #region Tags
    public static readonly string PlayerTag = "Player";
    public static readonly string EnemyTag = "Enemy";
    public static readonly string EnvironmentTag = "Environment";
    public static readonly string EnemySpawnerTag = "EnemySpawner";
    public static readonly string ItemSpawnerTag = "ItemSpawner";
    #endregion

    #region Layers
    public static readonly string GroundLayer = "Ground";
    public static readonly int GroundLayerIndex = LayerMask.NameToLayer(GroundLayer);
    public static readonly string CharacterLayer = "Character";
    public static readonly int CharacterLayerIndex = LayerMask.NameToLayer(CharacterLayer);
    public static readonly string PlayerLayer = "Player";
    public static readonly int PlayerLayerIndex = LayerMask.NameToLayer(PlayerLayer);
    #endregion
}
