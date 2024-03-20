using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    public enum AssetType
    {
        DEFAULT,
        SPAWNED_ASSET,
        TRANSFORMED_ASSET
    }

    [CreateAssetMenu(fileName = "AssetData", menuName = "Scriptables/AssetData", order = 0)]
    public class AssetData : ScriptableObject
    {
        public string Name;
        public AssetType Type;
        public Vector3 Rotation;
    }
}