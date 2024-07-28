using UnityEngine;
using System.Collections;

namespace DomeMasterSystem {
	public class Rotator : MonoBehaviour {
		public const float SEED_SCALE = 1000f;

		public float speed = 1f;
		public float freq = 0.1f;

		Vector3 _seed;

		void Start() {
			_seed = SEED_SCALE * new Vector3 (Random.value, Random.value, Random.value);
		}
		void Update () {
			var t = Time.timeSinceLevelLoad;
			var dt = Time.deltaTime;
			transform.localRotation *= Quaternion.Euler (
				speed * dt * Noise (freq * t + _seed.x, _seed.y),
				speed * dt * Noise (freq * t + _seed.y, _seed.z),
				speed * dt * Noise (freq * t + _seed.z, _seed.x));
		}

		float Noise(float x, float y) {
			return 2f * Mathf.PerlinNoise (x, y) - 1f;
		}
	}
}