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
			var configName = (Application.platform == RuntimePlatform.IPhonePlayer) ? "Config-iOS" : "Config-Android";
			var config = Resources.Load<TextAsset>(configName);
			var videoUrls = JsonConvert.DeserializeObject<VideoUrls>(config.text);

			// Register commands
			Facade.RegisterCommand(TitleScreenMediator.Notifications.TitleScreenShown, () => new ChangeMediatorCommand<TitleScreenMediator, PreferencesMediator>());
			Facade.RegisterCommand(PreferencesMediator.Notifications.PreferencesSelected, () => new ChangeMediatorCommand<PreferencesMediator, StoryMediator>());
			Facade.RegisterCommand(StoryMediator.Notifications.ShowPreferences, () => new ReturnToMenuCommand());

			// Register mediators that we always have
			Facade.RegisterMediator(new SoundMediator());
			Facade.RegisterMediator(new VideoMediator(videoUrls));
			Facade.RegisterMediator(new FaderMediator());

			// Start up the main menu
			Facade.RegisterMediator(new TitleScreenMediator());
		}
	}
}
