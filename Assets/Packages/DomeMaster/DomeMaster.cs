using UnityEngine;
using System.Collections;

namespace DomeMasterSystem {
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class DomeMaster : MonoBehaviour {
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
		public enum DirEnum { Front = 1, Back = -1 }
		public TextureEvent OnUpdateCubemap;
		public bool generateMips = false;
		public int lod = 10;
		[InspectorFlags]
		public FaceMaskFlags faceMask;

		Camera _attachedCam;
		RenderTexture _cube;

		void OnEnable() {
			_attachedCam = GetComponent<Camera> ();

			Debug.LogFormat ("CubemapFace {0}={1}", CubemapFace.PositiveX, (int)CubemapFace.PositiveX);
		}
		void Update() {
			var res = (1 << Mathf.Clamp(lod, 1, 13));
			if (_cube == null || _cube.width != res || _cube.generateMips != generateMips) {
				Debug.LogFormat ("Create Cubemap {0}x{1}", res, res);
				Release();
				_cube = new RenderTexture (res, res, 24);
				_cube.filterMode = FilterMode.Bilinear;
				_cube.isCubemap = true;
				_cube.generateMips = generateMips;
				_cube.useMipMap = generateMips;
				_cube.Create ();
			}
			_attachedCam.enabled = false;
			_attachedCam.RenderToCubemap (_cube, (int)faceMask);
			Shader.SetGlobalTexture(PROP_DOME_MASTER_CUBE, _cube);

			OnUpdateCubemap.Invoke (_cube);
		}
		void OnDisable() {
			Release ();
		}

		void Release() {
			DestroyImmediate (_cube);
		}

		[System.Serializable]
		public class TextureEvent : UnityEngine.Events.UnityEvent<Texture> {}
	}
}
