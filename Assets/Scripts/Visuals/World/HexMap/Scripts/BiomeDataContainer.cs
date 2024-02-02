using Game.Debug;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Terrain
{
    [CreateAssetMenu(fileName = nameof(BiomeDataContainer), menuName = "ScriptableObjects/Biomes/" + nameof(BiomeDataContainer), order = 1)]
    public class BiomeDataContainer : SerializedScriptableObject
    {
        public List<BiomeData> biomeData;

        public BiomeData GetBiomeData(BiomeTerrainType terrainType)
        {
            return biomeData.First(x => x.terrainType == terrainType);
        }

        public int GetTextureIndex(BiomeTerrainType terrainType)
        {
            var previousExtraTextures = 0;
            for(int i = 0; i < biomeData.Count; i++)
            {
                if(biomeData[i].terrainType == terrainType)
                {
                    var index = i + previousExtraTextures + biomeData[i].GetTextureIndex();
                    var p = 0;
                    return index;
                }
                previousExtraTextures += (biomeData[i].textures.Count - 1);
            }
            return 0 + biomeData[0].GetTextureIndex();
        }

        //[Button("Create Texture Array")]
        public Texture2DArray CreateTextureArray()
        {
            if(biomeData.Count == 0 || biomeData.Any(x => x.textures.Count == 0))
            {
                return null;
            }

            //string path = "Materials/Terrain/Terrain Texture Array";
            string path = "TerrainTextureArray.asset";
            int length = biomeData.Count;
            length += (biomeData.Sum(x => x.textures.Count) - length);
            OutputLogger.Log($"Expected length of texture array: {length}.");

            Texture2D t = biomeData[0].textures[0].texture;
            Texture2DArray textureArray = new Texture2DArray(t.width, t.height, length, t.format, t.mipmapCount > 1);
            textureArray.anisoLevel = t.anisoLevel;
            textureArray.filterMode = t.filterMode;
            textureArray.wrapMode = t.wrapMode;

            OutputLogger.Log($"Mipmap Count: {t.mipmapCount}.");

            int mismatchCount = 0;
            int additionalTextures = 0;

            for(int i = 0; i < biomeData.Count; i++)
            {
                var currentBiomeData = biomeData[i];

                for(int j = 0; j < currentBiomeData.textures.Count; j++)
                {
                    var tex = currentBiomeData.textures[j].texture;
                    if(j > 0)
                    {
                        additionalTextures++;
                    }
                    OutputLogger.Log($"Adding Texture {tex.name} to array.");
                    OutputLogger.Log($"Copying to position: {i + additionalTextures}");
                    //m < t.mipMapCount seems wrong, fix later
                    for (int m = 0; m < t.mipmapCount; m++)
                    {
                        if (tex.width != t.width)
                        {
                            mismatchCount++;
                            OutputLogger.LogError($"Texture {tex.name} doesn't match size of other elements.");
                        }
                        Graphics.CopyTexture(tex, 0, m, textureArray, i+additionalTextures, m);
                    }
                }
            }

            //AssetDatabase.CreateAsset(textureArray, path);
             return mismatchCount > 0 ? null : textureArray;
        }
    }
}