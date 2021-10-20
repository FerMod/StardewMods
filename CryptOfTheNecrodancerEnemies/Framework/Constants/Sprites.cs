using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CryptOfTheNecrodancerEnemies.Framework.Constants {

  interface ISpriteAsset {
    string Name { get; }
    string File { get; }
    Rectangle SourceRectangle { get; }
    float Scale { get; }
  }

  struct SpriteAsset {

    public SpriteAsset(string name, Rectangle sourceRectangle, float targetSize = 16f) {
      Name = name;
      File = $"assets/{name}.png";
      SourceRectangle = sourceRectangle;
      Scale = targetSize / Math.Max(SourceRectangle.Width, SourceRectangle.Height);
    }

    public string Name { get; internal set; }
    public string File { get; internal set; }

    public Rectangle SourceRectangle { get; internal set; }
    public float Scale { get; internal set; }
  }

  internal static class Sprites {

    /// <summary>
    /// The <c>Dictionary</c> of sprite assets.
    /// </summary>
    public static Dictionary<string, SpriteAsset> Assets { get; internal set; } = LoadSpriteAssets();

    /// <summary>
    /// Loads again the sprite assets and invalidates the assets from the content cache.
    /// </summary>
    public static void ReloadSpriteAssets() {
      Assets = LoadSpriteAssets();
      ModEntry.ModHelper.Content.InvalidateCache(asset => {
        return asset.DataType == typeof(Texture2D) && Assets.ContainsKey(asset.AssetName);
      });
    }

    /// <summary>
    /// Load the sprite assets.
    /// </summary>
    public static Dictionary<string, SpriteAsset> LoadSpriteAssets() {
      return new() {
        { BatAsset, new(BatAsset, new(0, 0, 24, 24)) },
        { FrostBatAsset, new(FrostBatAsset, new(0, 0, 24, 24)) },
        { LavaBatAsset, new(LavaBatAsset, new(0, 0, 24, 24)) },
        { IridiumBatAsset, new(IridiumBatAsset, new(0, 0, 24, 24)) },
        { BatDangerousAsset, new(BatDangerousAsset, new(0, 0, 36, 36)) },
        { FrostBatDangerousAsset, new(FrostBatDangerousAsset, new(0, 0, 36, 36)) },
        //{ HauntedSkullAsset, new(HauntedSkullAsset, new(0, 0, 30, 30), 16f) }, // TODO: Hardcoded size.
        //{ HauntedSkullDangerousAsset, new(HauntedSkullDangerousAsset, new(0, 0, 30, 30)) }, // TODO: Hardcoded size.
        { SpiderAsset, new(SpiderAsset, new(0, 0, 32, 32)) },
        { BigSlimeAsset, new(BigSlimeAsset, new(0, 0, 32, 32), 32) },
        { DuggyAsset, new(DuggyAsset, new(0, 0, 24, 24)) },
        { DuggyDangerousAsset, new(DuggyDangerousAsset, new(0, 0, 24, 24)) },
        { MagmaDuggyAsset, new(MagmaDuggyAsset, new(0, 0, 24, 24)) },
        { SquidKidAsset, new(SquidKidAsset, new(0, 0, 30, 30)) },
        //{ SquidKidDangerousAsset, new(SquidKidDangerousAsset, new(0, 0, 30, 30)) }, // TODO: Hardcoded size.
      };
    }


    public const string BatAsset = "Characters\\Monsters\\Bat";
    public const string FrostBatAsset = "Characters\\Monsters\\Frost Bat";
    public const string LavaBatAsset = "Characters\\Monsters\\Lava Bat";
    public const string IridiumBatAsset = "Characters\\Monsters\\Iridium Bat";
    public const string BatDangerousAsset = BatAsset + "_dangerous";
    public const string FrostBatDangerousAsset = FrostBatAsset + "_dangerous";
    public const string HauntedSkullAsset = "Characters\\Monsters\\Haunted Skull";
    public const string HauntedSkullDangerousAsset = HauntedSkullAsset + "_dangerous";

    public const string SpiderAsset = "Characters\\Monsters\\Spider";

    public const string BigSlimeAsset = "Characters\\Monsters\\Big Slime";

    public const string DuggyAsset = "Characters\\Monsters\\Duggy";
    public const string DuggyDangerousAsset = DuggyAsset + "_dangerous";
    public const string MagmaDuggyAsset = "Characters\\Monsters\\Magma Duggy";

    public const string SquidKidAsset = "Characters\\Monsters\\Squid Kid";
    public const string SquidKidDangerousAsset = SquidKidAsset + "_dangerous";
  }

}
