using System.Collections.Generic;
using UnityEngine;
using static Game.Terrain.AssetPlaceholder;

namespace Game.Terrain
{
    public class AssetPositionInformationContainer
    {
        public List<AssetPositionInformation> positionInformation;

        public AssetPositionInformationContainer()
        {
            this.positionInformation = new List<AssetPositionInformation>();
        }

        public void AddRange(IEnumerable<AssetPlaceholder> placeholders)
        {
            foreach(var placeholder in placeholders)
            {
                positionInformation.Add(new AssetPositionInformation { position = placeholder.transform.position, assetType = placeholder.assetType, scale = placeholder.baseScale });
            }
        }
    }

    public struct AssetPositionInformation
    {
        public Vector3 position;
        public Vector3 scale;
        public AssetType assetType;
    }
}