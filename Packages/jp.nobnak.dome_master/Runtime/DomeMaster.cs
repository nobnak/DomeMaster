using UnityEngine;
using UnityEngine.Rendering;

namespace DomeMasterSystem {
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class DomeMaster : MonoBehaviour {

        [SerializeField] protected Events events = new();
		[SerializeField] protected Config config = new();

        Camera _attachedCam;
		RenderTexture _cubert;

		void OnEnable() {
			_attachedCam = GetComponent<Camera> ();
            _attachedCam.enabled = false;
        }
		void Update() {
			var res = (1 << Mathf.Clamp(config.lod, 1, 13));

			CheckInitRenderTexture(res, ref _cubert);
			_attachedCam.RenderToCubemap(_cubert, (int)config.faceMask);
            if (!string.IsNullOrEmpty(config.globalCubeTexName))
                Shader.SetGlobalTexture(config.globalCubeTexName, _cubert);
			events.OnUpdateCubemap?.Invoke(_cubert);
		}
		void OnDisable() {
			ReleaseTexture ();
		}

		void ReleaseTexture() {
            CoreUtils.Destroy(_cubert);
            _cubert = null;
        }

		void CheckInitRenderTexture (int res, ref RenderTexture rt) {
			if (rt == null || rt.width != res || rt.autoGenerateMips != config.generateMips) {
				ReleaseTexture ();
				rt = new RenderTexture (res, res, 24);
                rt.dimension = TextureDimension.Cube;
                rt.filterMode = config.filterMode;
                rt.anisoLevel = config.anisoLevel;
                rt.autoGenerateMips = config.generateMips;
				rt.useMipMap = config.generateMips;
				Debug.LogFormat ("Create Cubemap RenderTexture {0}x{1}", res, res);
			}
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
