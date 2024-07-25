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
        
		public const string PROP_DIRECTION = "_Dir";
        public const string PROP_ROTATION_QUATERNION = "_RotationQuat";

        public TextureEvent OnUpdateCubemap;
		public bool generateMips = false;
		public int lod = 10;
		public FilterMode filterMode = FilterMode.Trilinear;
		public int anisoLevel = 16;
		[InspectorFlags]
		public FaceMaskFlags faceMask;

        [SerializeField]
        protected Transform rotationTr;
        [SerializeField]
        protected Material domemasterFrontMat;
        [SerializeField]
        protected Material domemasterBackMat;
        [SerializeField]
        protected Material equirectangularMat;

		Camera _attachedCam;
		RenderTexture _cubert;

        #region Unity
        void OnEnable() {
			_attachedCam = GetComponent<Camera> ();
		}
		void Update() {
			var res = (1 << Mathf.Clamp(lod, 1, 13));

			CheckInitRenderTexture(res, ref _cubert);
			_attachedCam.RenderToCubemap (_cubert, (int)faceMask);
			_attachedCam.enabled = false;
			OnUpdateCubemap.Invoke (_cubert);
		}
		void OnDisable() {
			Release ();
		}
        private void OnGUI() {
            if (Event.current.type != EventType.Repaint)
                return;

            var r = (rotationTr != null ? rotationTr.rotation : Quaternion.identity);
            var rotationVec4 = new Vector4(r.x, r.y, r.z, r.w);

            if (equirectangularMat != null)
                equirectangularMat.SetVector(PROP_ROTATION_QUATERNION, rotationVec4);
            if (domemasterFrontMat != null)
                domemasterFrontMat.SetVector(PROP_ROTATION_QUATERNION, rotationVec4);
            if (domemasterBackMat != null)
                domemasterBackMat.SetVector(PROP_ROTATION_QUATERNION, rotationVec4);

            Graphics.DrawTexture(new Rect(10f, 10f, 400f, 200f), _cubert, equirectangularMat);
            Graphics.DrawTexture(new Rect(10f, 220f, 200f, 200f), _cubert, domemasterFrontMat);
            Graphics.DrawTexture(new Rect(210f, 220f, 200f, 200f), _cubert, domemasterBackMat);
        }
        #endregion

        void Release() {
			DestroyImmediate (_cubert);
		}

		void CheckInitRenderTexture (int res, ref RenderTexture rt) {
			if (rt == null || rt.width != res || rt.autoGenerateMips != generateMips) {
				Release ();
				rt = new RenderTexture (res, res, 24);
                rt.dimension = UnityEngine.Rendering.TextureDimension.Cube;
                rt.filterMode = filterMode;
                rt.anisoLevel = anisoLevel;
                rt.autoGenerateMips = generateMips;
				rt.useMipMap = generateMips;
				Debug.LogFormat ("Create Cubemap RenderTexture {0}x{1}", res, res);
			}
		}

		[System.Serializable]
		public class TextureEvent : UnityEngine.Events.UnityEvent<Texture> {}
	}
}
