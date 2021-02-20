using UnityEngine;

namespace RPG.Saving
{
	[System.Serializable]
	public class SerializableVector3
	{
		[SerializeField] private float x, y, z;

		public SerializableVector3(Vector3 vector)
		{
			x = vector.x;
			y = vector.y;
			z = vector.z;
		}

		public Vector3 ToVector() => new Vector3(x, y, z);
	}
}