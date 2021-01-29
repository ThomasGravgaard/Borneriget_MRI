using Newtonsoft.Json;
using PureMVC.Patterns.Facade;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace Borneriget.MRI
{
	public class Bootstrap
	{
		public static Facade Facade;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void OnBeforeSceneLoad()
		{
			Application.targetFrameRate = 60;
			// Initialize the PureMVC framework
			Facade = (Facade)Facade.GetInstance("MRI", () => { return new Facade("Borneriget.MRI"); });
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		static void OnAfterSceneLoad()
		{
#if UNITY_EDITOR
			if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Main")
			{
				return;
			}
#endif
			InitializeProgram();
		}

		private static void InitializeProgram()
		{
			// Load in the video Urls
			var config = Resources.Load<TextAsset>("Config");
			var videoUrls = JsonConvert.DeserializeObject<VideoUrls>(config.text);
			Facade.RegisterProxy(new VideoProxy(videoUrls));

			// Register commands
			Facade.RegisterCommand(PreferencesMediator.Notifications.PreferencesSelected, () => new ChangeMediatorCommand<PreferencesMediator, LobbyMediator>());
			Facade.RegisterCommand(LobbyMediator.Notifications.StartNormalVideo, () => new ShowNormalVideo());
			Facade.RegisterCommand(LobbyMediator.Notifications.StartVrVideo, () => new ShowVrVideo());

			// Start up the main menu
			Facade.RegisterMediator(new PreferencesMediator());
		}
	}
}
