using System.Collections;
using System.Linq;
using RoboRyanTron.SceneReference;
using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
	public class Portal : MonoBehaviour
	{
		// Connects portals across scenes with the same destination enum
		private enum DestinationIdentifier
		{
			A,
			B,
			C,
			D,
			E
		}

		[SerializeField] private SceneReference sceneToLoad;
		[SerializeField] private Transform spawnPoint;
		[SerializeField] private DestinationIdentifier destination;
		[SerializeField] private float fadeInTime = 2f, fadeOutTime = 2f, fadeWaitTime = 2f;

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.CompareTag("Player"))
			{
				StartCoroutine(Transition());
			}
		}

		private IEnumerator Transition()
		{
			if (sceneToLoad == null)
			{
				Debug.LogError("Scene to load was empty.");
				yield return null;
			}

			DontDestroyOnLoad(gameObject);
			var fader = FindObjectOfType<Fader>();
			var wrapper = FindObjectOfType<SavingWrapper>();
			
			wrapper.Save();
			
			var player = PlayerFinder.Player;
			var playerController = player.GetComponent<PlayerController>();
			playerController.enabled = false;

			yield return fader.FadeOutRoutine(fadeOutTime);
			yield return SceneManager.LoadSceneAsync(sceneToLoad.SceneName);

			PlayerFinder.ResetPlayer();
			var newPlayerController = PlayerFinder.Player.GetComponent<PlayerController>();
			newPlayerController.enabled = false;

			wrapper.Load();

			var otherPortal = GetOtherPortal();
			UpdatePlayer(otherPortal, newPlayerController.transform);

			wrapper.Save();

			yield return new WaitForSeconds(fadeWaitTime);
			fader.FadeInRoutine(fadeInTime);

			newPlayerController.enabled = true;
			Destroy(gameObject);
		}

		private Portal GetOtherPortal() => FindObjectsOfType<Portal>().Where(portal => portal != this).FirstOrDefault(portal => portal.destination == destination);

		private void UpdatePlayer(Portal otherPortal, Transform player)
		{
			player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
			player.rotation = otherPortal.spawnPoint.rotation;
		}
	}
}