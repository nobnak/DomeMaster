using UnityEngine;
using System.Collections;

namespace DomeMasterSystem {
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class DomeMaster : MonoBehaviour {
		public enum TextureTypeEnum { RenderTexture = 0, CubeMap }
		[System.Flags]
		public enum FaceMaskFlags { 
			Right = 1 << 0, 
			Left = 1 << 1, 
			Top = 1 << 2, 
			Bottom = 1 << 3, 
			Front = 1 << 4,
			Back = 1 << 5 }

		public const string PROP_DOME_MASTER_CUBE = "_DomeMasterCube";
		public const string PROP_DIRECTION = "_Dir";

		public TextureTypeEnum textureType;
		public TextureEvent OnUpdateCubemap;
		public bool generateMips = false;
		public int lod = 10;
		public FilterMode filterMode = FilterMode.Trilinear;
		public int anisoLevel = 16;
		[InspectorFlags]
		public FaceMaskFlags faceMask;

		Camera _attachedCam;
		RenderTexture _cubert;

		void OnEnable() {
			_attachedCam = GetComponent<Camera> ();
		}
		void Update() {
			var res = (1 << Mathf.Clamp(lod, 1, 13));

			CheckInitRenderTexture(res, ref _cubert);
			_attachedCam.RenderToCubemap (_cubert, (int)faceMask);
			_attachedCam.enabled = false;
			Shader.SetGlobalTexture(PROP_DOME_MASTER_CUBE, _cubert);
			OnUpdateCubemap.Invoke (_cubert);
		}
		void OnDisable() {
			Release ();
		}

		void Release() {
			DestroyImmediate (_cubert);
		}

		void CheckInitRenderTexture (int res, ref RenderTexture rt) {
			if (rt == null || rt.width != res || rt.generateMips != generateMips) {
				Release ();
				rt = new RenderTexture (res, res, 24);
				rt.filterMode = filterMode;
				rt.anisoLevel = anisoLevel;
				rt.isCubemap = true;
				rt.generateMips = generateMips;
				rt.useMipMap = generateMips;
				rt.Create ();
				Debug.LogFormat ("Create Cubemap RenderTexture {0}x{1}", res, res);
			}
		}

		[System.Serializable]
		public class TextureEvent : UnityEngine.Events.UnityEvent<Texture> {}
	}
}
