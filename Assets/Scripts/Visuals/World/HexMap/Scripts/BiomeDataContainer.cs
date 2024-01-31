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

        public int GetTextureIndex(BiomeTerrainType terrainType)
        {
            for(int i = 0; i < biomeData.Count; i++)
            {
                if(biomeData[i].terrainType == terrainType)
                {
                    return i + biomeData[i].GetTextureIndex();
                }
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
            length += biomeData.Sum(x => x.textures.Count);

            Texture2D t = biomeData[0].textures[0].texture;
            Texture2DArray textureArray = new Texture2DArray(t.width, t.height, length, t.format, t.mipmapCount > 1);
            textureArray.anisoLevel = t.anisoLevel;
            textureArray.filterMode = t.filterMode;
            textureArray.wrapMode = t.wrapMode;

            int mismatchCount = 0;
            for(int i = 0; i < biomeData.Count; i++)
            {
                var bd = biomeData[i];
                for(int j = 0; j < bd.textures.Count; j++)
                {
                    //m < t.mipMapCount seems wrong, fix later
                    for(int m = 0; m < t.mipmapCount; m++)
                    {
                        var tex = bd.textures[j].texture;
                        if (tex.width != t.width)
                        {
                            mismatchCount++;
                            OutputLogger.LogError($"Texture {tex.name} doesn't match size of other elements.");
                        }
                        Graphics.CopyTexture(tex, 0, m, textureArray, i, m);
                    }
                }
            }

            //AssetDatabase.CreateAsset(textureArray, path);
             return mismatchCount > 0 ? null : textureArray;
        }
    }
}