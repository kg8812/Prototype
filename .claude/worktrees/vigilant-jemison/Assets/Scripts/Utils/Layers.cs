using UnityEngine;

namespace Apis
{
    public class LayerMasks
    {
        public static readonly LayerMask Default = LayerMask.GetMask("Default");
        public static readonly LayerMask TransparentFX = LayerMask.GetMask("TransparentFX");
        public static readonly LayerMask IgnoreRaycast = LayerMask.GetMask("Ignore Raycast");
        public static readonly LayerMask Map = LayerMask.GetMask("Map");
        public static readonly LayerMask Ground = LayerMask.GetMask("Ground");
        public static readonly LayerMask Water = LayerMask.GetMask("Water");
        public static readonly LayerMask UI = LayerMask.GetMask("UI");
        public static readonly LayerMask Player = LayerMask.GetMask("Player");
        public static readonly LayerMask DashPlayer = LayerMask.GetMask("DashPlayer");
        public static readonly LayerMask Enemy = LayerMask.GetMask("Enemy");
        public static readonly LayerMask DropItem = LayerMask.GetMask("DropItem");
        public static readonly LayerMask Effect = LayerMask.GetMask("Effect");
        public static readonly LayerMask Detect = LayerMask.GetMask("Detect");
        public static readonly LayerMask Wall = LayerMask.GetMask("Wall");
        public static readonly LayerMask Summon = LayerMask.GetMask("Summon");
        public static readonly LayerMask Platform = LayerMask.GetMask("Platform");
        public static readonly LayerMask Destroyable = LayerMask.GetMask("Destroyable");
        public static readonly LayerMask Fragment = LayerMask.GetMask("Fragment");
        public static readonly LayerMask BackGround = LayerMask.GetMask("BackGround");
        public static readonly LayerMask Trigger = LayerMask.GetMask("Trigger");

        public static readonly LayerMask MapAndPlatform = Map | Platform;
        public static readonly LayerMask GroundAndPlatform = Ground | Platform | Map; // 일단 Map도 포함시켜둠
        public static readonly LayerMask GroundWall = Ground | Wall;
        public static readonly LayerMask GroundWallPlatform = Ground | Wall | Platform;
        public static readonly LayerMask GroundWallPlatformMap = Ground | Wall | Platform | Map;
    }

    public class Layers
    {
        public static readonly int Default = LayerMask.NameToLayer("Default");
        public static readonly int TransparentFX = LayerMask.NameToLayer("TransparentFX");
        public static readonly int IgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
        public static readonly int Map = LayerMask.NameToLayer("Map");
        public static readonly int Water = LayerMask.NameToLayer("Water");
        public static readonly int UI = LayerMask.NameToLayer("UI");
        public static readonly int Player = LayerMask.NameToLayer("Player");
        public static readonly int DashPlayer = LayerMask.NameToLayer("DashPlayer");
        public static readonly int Enemy = LayerMask.NameToLayer("Enemy");
        public static readonly int DropItem = LayerMask.NameToLayer("DropItem");
        public static readonly int Effect = LayerMask.NameToLayer("Effect");
        public static readonly int Detect = LayerMask.NameToLayer("Detect");
        public static readonly int Wall = LayerMask.NameToLayer("Wall");
        public static readonly int Summon = LayerMask.NameToLayer("Summon");
        public static readonly int Platform = LayerMask.NameToLayer("Platform");
        public static readonly int Destroyable = LayerMask.NameToLayer("Destroyable");
        public static readonly int Fragment = LayerMask.NameToLayer("Fragment");
        public static readonly int BackGround = LayerMask.NameToLayer("BackGround");
        public static readonly int Ground = LayerMask.NameToLayer("Ground");
    }

    public static class SortingLayers
    {
        public static readonly int Bg = SortingLayer.NameToID("Bg");
        public static readonly int Default = SortingLayer.NameToID("Default");
        public static readonly int Environment = SortingLayer.NameToID("Environment");
        public static readonly int InGameUI = SortingLayer.NameToID("IngameUI");
        public static readonly int AttackObject = SortingLayer.NameToID("AttackObject");
        public static readonly int Monster = SortingLayer.NameToID("Monster");
        public static readonly int Summon = SortingLayer.NameToID("Summon");
        public static readonly int Player = SortingLayer.NameToID("Player");
        public static readonly int Drop = SortingLayer.NameToID("Drop");
        public static readonly int Effect = SortingLayer.NameToID("Effect");
        public static readonly int Fg = SortingLayer.NameToID("Fg");
        public static readonly int UI = SortingLayer.NameToID("UI");
        public static readonly int Fade = SortingLayer.NameToID("Fade");
    }
}