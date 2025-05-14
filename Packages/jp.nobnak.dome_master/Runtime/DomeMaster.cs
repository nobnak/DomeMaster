using UnityEngine;
using UnityEngine.Rendering;

namespace DomeMasterSystem {
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class DomeMaster : MonoBehaviour {

        [SerializeField] protected Events events = new();
		[SerializeField] protected Config config = new();

        bool valid;
        Camera _attachedCam;
		RenderTexture _cubert;

		void OnEnable() {
            valid = false;
			_attachedCam = GetComponent<Camera> ();
            _attachedCam.enabled = false;
        }
		void Update() {
			var res = (1 << Mathf.Clamp(config.lod, 1, 13));

            if (!valid || _cubert == null || _cubert.width != res) {
                valid = true;
                ReleaseCubemap();
                _cubert = InitCubemap(res);
            }
			_attachedCam.RenderToCubemap(_cubert, (int)config.faceMask);
            if (!string.IsNullOrEmpty(config.globalCubeTexName))
                Shader.SetGlobalTexture(config.globalCubeTexName, _cubert);
			events.OnUpdateCubemap?.Invoke(_cubert);
		}
		void OnDisable() {
			ReleaseCubemap ();
		}
        private void OnValidate() {
            valid = false;
        }

        void ReleaseCubemap() {
            CoreUtils.Destroy(_cubert);
            _cubert = null;
        }

        private RenderTexture InitCubemap(int res) {
            var cubemap = new RenderTexture(res, res, 24);
            cubemap.dimension = TextureDimension.Cube;
            cubemap.filterMode = config.filterMode;
            cubemap.anisoLevel = config.anisoLevel;
            cubemap.autoGenerateMips = true;
            cubemap.useMipMap = config.generateMips;
            Debug.LogFormat("Create Cubemap RenderTexture {0}x{1}", res, res);
            return cubemap;
        }

        #region declarations
        public const string PROP_DIRECTION = "_Dir";

        public enum TextureTypeEnum { RenderTexture = 0, CubeMap }
        [System.Flags]
        public enum FaceMaskFlags {
            Right = 1 << 0,
            Left = 1 << 1,
            Top = 1 << 2,
            Bottom = 1 << 3,
            Front = 1 << 4,
            Back = 1 << 5
        }

        public const string P_DEFAULT_CUBE_TEX = "_DomeMasterCube";

		[System.Serializable]
		public class Events {
			public TextureEvent OnUpdateCubemap = new();

            [System.Serializable]
            public class TextureEvent : UnityEngine.Events.UnityEvent<Texture> { }
        }

        [System.Serializable]
        public class  Config {
            public string globalCubeTexName = P_DEFAULT_CUBE_TEX;
            public bool generateMips = false;
            public int lod = 10;
            public FilterMode filterMode = FilterMode.Trilinear;
            public int anisoLevel = 16;
            [InspectorFlags]
            public FaceMaskFlags faceMask = (FaceMaskFlags)(-1);

        }
        #endregion
    }
}
