using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[ExecuteAlways]
public class TextureTiling : MonoBehaviour {

    [SerializeField] protected Presets presets = new();
    [SerializeField] protected Config config = new();

    protected bool valid;

    #region unity
    private void OnEnable() {
        valid = false;
    }
    private void Update() {
        if (!valid) {
            var tiles = math.max(config.tiles, 0);
            foreach (var mat in presets.mats) {
                if (mat == null) continue;
                mat.SetTextureScale(presets.texName, new float2(tiles, tiles));
            }
        }
    }
    private void OnValidate() {
        valid = false;
    }
    #endregion

    #region declarations
    [System.Serializable]
    public class Presets {
        public string texName = "_MainTex";
        public List<Material> mats = new();
    }
    [System.Serializable]
    public class  Config {
        [Range(1, 100)]
        public int tiles = 1;        
    }
    #endregion
}