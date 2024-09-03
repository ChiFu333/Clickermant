using System;
using System.Reflection;
using UnityEngine;


namespace SFH {
    public static class SpriteUtils {
        //GetSecondaryTexture 
        private delegate Texture2D GetSecondaryTextureDelegate(Sprite sprite, int index);

        private static readonly GetSecondaryTextureDelegate GetSecondaryTextureCached =
        (GetSecondaryTextureDelegate)Delegate.CreateDelegate(
            typeof(GetSecondaryTextureDelegate),
            typeof(Sprite).GetMethod("GetSecondaryTexture", BindingFlags.NonPublic | BindingFlags.Instance) ??
            throw new Exception("Unity has changed/removed the internal method Sprite.GetSecondaryTexture"));

        public static Texture GetSecondaryTexture(this Sprite sprite, int index) => GetSecondaryTextureCached(sprite, index);
    }
}