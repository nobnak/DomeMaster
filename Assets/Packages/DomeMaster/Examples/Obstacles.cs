using UnityEngine;
using System.Collections;

namespace DomeMasterSystem {
	[ExecuteInEditMode]
	public class Obstacles : MonoBehaviour {
	    public const string PROP_COLOR = "_Color";

	    public GameObject fab;
	    public float radius = 100f;
	    public int count = 1000;
		public int variation = 10;

	    GameObject[] _instances;
	    Renderer[] _renderers;
	    MaterialPropertyBlock[] _blocks;

		void Awake() {
	        _instances = new GameObject[count];
	        _renderers = new Renderer[count];
			_blocks = new MaterialPropertyBlock[variation];

			for (var i = 0; i < variation; i++) {
				var block = _blocks [i] = new MaterialPropertyBlock ();
				block.SetColor (PROP_COLOR, Random.ColorHSV ());
			}

	        for (var i = 0; i < count; i++) {
	            var inst = _instances[i] = Instantiate (fab);
	            inst.hideFlags = HideFlags.DontSave;

	            inst.transform.SetParent (transform, false);
	            inst.transform.localPosition = radius * Random.onUnitSphere;
	            inst.transform.localRotation = Random.rotationUniform;

	            var rend = _renderers[i] = inst.GetComponent<Renderer> ();
				rend.SetPropertyBlock (_blocks[Random.Range(0, _blocks.Length)]);
	        }
		}
	}
}