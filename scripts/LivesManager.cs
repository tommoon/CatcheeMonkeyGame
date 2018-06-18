using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if ANDROID_NOTIFICATIONS
using Assets.SimpleAndroidNotifications;
#endif

namespace ExaGames.Common {
	/// <summary>
	/// Lives system manager by ExaGames.
	/// </summary>
	/// <coauthor>Eduardo Casillas</coauthor>
	/// <coauthor>Nicolás Michelini</coauthor>
	public class LivesManager : MonoBehaviour {
		#region Constants
		
		/// <summary>
		/// Key to save the maximum number of lives for the player.
		/// </summary>
		private const string MAX_LIVES_SAVEKEY = "ExaGames.Common.MaxLives";
		/// <summary>
		/// Key to save the number of currently available lives in the player preferences file.
		/// </summary>
		private const string LIVES_SAVEKEY = "ExaGames.Common.Lives";
		/// <summary>
		/// Key to save the recovery start time in the player preferences file.
		/// </summary>
		private const string RECOVERY_TIME_SAVEKEY = "ExaGames.Common.LivesRecoveryTime";
		/// <summary>
		/// Key to save the starting time of infinite lives.
		/// </summary>
		private const string INFINITE_LIVES_TIME_SAVEKEY = "ExaGames.Common.InfiniteLivesStartTime";
		/// <summary>
		/// Key to save the total minutes of infinite lives given to the player.
		/// </summary>
		private const string INFINITE_LIVES_MINUTES_SAVEKEY = "ExaGames.Common.InfiniteLivesMinutes";
		
		#endregion
		
		[Serializable]
		public class _CustomTexts {
			public string FullLives = "Full";
			public string Infinite = "∞";
		}
		
		#region Fields
		
		/// <summary>
		/// For debug purposes. Set this value to true to reset the LivesManager preferences when playing in the Editor.
		/// </summary>
		public bool ResetPlayerPrefsOnPlay = false;
		/// <summary>
		/// Maximum number of lives by default for all players.
		/// Additional life slots can be added for the player with <see cref="AddLifeSlots"/>
		/// </summary>
		public int DefaultMaxLives = 5;
		/// <summary>
		/// Time to recover one life in minutes.
		/// </summary>
		public double MinutesToRecover = 30D;
		/// <summary>
		/// Texts to be used in the time and life observers in special cases.
		/// </summary>
		public _CustomTexts CustomTexts;
		/// <summary>
		/// When this value is true and the remaining time is greater than one hour, shows the remaining time as >Xhrs.
		/// </summary>
		public bool SimpleHourFormat = false;
		
		/// <summary>
		/// Event to be called when the number of lives has changed.
		/// </summary>
		public UnityEvent OnLivesChanged;
		/// <summary>
		/// Evento to be called when the time to recover one life has changed.
		/// </summary>
		public UnityEvent OnRecoveryTimeChanged;
		
		#region Normal lives count
		
		/// <summary>
		/// Current number of available lives.
		/// </summary>
		private int lives;
		/// <summary>
		/// Timestamp from the last life recovery.
		/// </summary>
		private DateTime recoveryStartTime;
		/// <summary>
		/// Time remaining until the next life in seconds.
		/// </summary>
		private double secondsToNextLife;
		
		#endregion
		
		#region Infinite lives count
		
		/// <summary>
		/// Time remaining with infinite lives.
		/// </summary>
		private double remainingSecondsWithInfiniteLives;
		/// <summary>
		/// Timestamp when the infinite lives started.
		/// </summary>
		private DateTime infiniteLivesStartTime;
		/// <summary>
		/// Total minutes of infinite lives given to the player.
		/// </summary>
		private int infiniteLivesMinutes;
		
		#endregion
		
		/// <summary>
		/// Specifies whether the timer to next life should be calculated.
		/// </summary>
		private bool calculateSteps;
		/// <summary>
		/// Specifies whether the application was paused and should reinit the timer at OnApplicationPause.
		/// </summary>
		/// <remarks>
		/// The use of this field prevents a bug in Unity Editor where the OnApplicationPause is sometimes called
		/// before Start or Awake methods, just after pressing the play button in the editor.
		/// </remarks>
		private bool applicationWasPaused;
		
		#endregion
		
		#region Properties
		
		/// <summary>
		/// Gets the current number of available lives.
		/// </summary>
		/// <value>Current number of available lives.</value>
		public int Lives { get { return lives; } }
		
		/// <summary>
		/// Gets the maximum number of lives for the current player.
		/// </summary>
		/// <value>Maximum number of lives for the current player.</value>
		public int MaxLives { get; private set; }
		
		/// <summary>
		/// Gets the text that should be shown as the number of lives remaining: either a number or an infinite symbol.
		/// </summary>
		/// <value>Text that should be shown as the number of lives remaining: either a number or an infinite symbol.</value>
		public string LivesText { get { return HasInfiniteLives ? CustomTexts.Infinite : lives.ToString(); } }
		
		/// <summary>
		/// Gets the time remaining until the next life is restored, in seconds.
		/// </summary>
		/// <value>Time remaining until the next life in seconds..</value>
		public double SecondsToNextLife { get { return secondsToNextLife; } }
		
		/// <summary>
		/// Convinience property that returns <c>true</c> when there's lives available.
		/// </summary>
		/// <value><c>true</c> if this instance can play; otherwise, <c>false</c>.</value>
		public bool CanPlay{ get { return lives > 0; } }
		
		/// <summary>
		/// Gets a value indicating whether lives are at their maximum number.
		/// </summary>
		/// <value><c>true</c> if lives are at their max; otherwise, <c>false</c>.</value>
		public bool HasMaxLives { get { return (lives >= MaxLives); } }
		
		/// <summary>
		/// Gets a value indicating whether the player has infinite lives.
		/// </summary>
		/// <value><c>true</c> if this player has infinite lives; otherwise, <c>false</c>.</value>
		public bool HasInfiniteLives{ get { return remainingSecondsWithInfiniteLives > 0D; } }
		
		/// <summary>
		/// Gets the remaining time for next life formatted as mm:ss
		/// </summary>
		/// <value>Remaining time for next life formatted as mm:ss</value>
		/// <remarks>
		/// When lives are full and <c>CustomFullLivesText</c> is set, 
		/// the value of <c>CustomFullLivesText</c> is returned.
		/// This value is also affected by the <c>SimpleHourFormat</c> when the remaining time is greater than one hour;
		/// When <c>SimpleHourFormat</c> = <c>true</c>, the string is formatted as "> X hrs",
		/// when <c>SimpleHourFormat</c> = <c>false</c>, the string is formatted as hh:mm:ss
		/// </remarks>
		public string RemainingTimeString {
			get { 
				if(!HasInfiniteLives && HasMaxLives && !string.IsNullOrEmpty(CustomTexts.FullLives)) {
					return CustomTexts.FullLives;
				}
				TimeSpan timerToShow = TimeSpan.FromSeconds(HasInfiniteLives ? remainingSecondsWithInfiniteLives : secondsToNextLife);
				if(timerToShow.TotalHours > 1D) {
					if(SimpleHourFormat) {
						int hoursLeft = Mathf.RoundToInt((float)timerToShow.TotalHours);
						return string.Format(">{0} hr{1}", hoursLeft, hoursLeft > 1 ? string.Empty : "");
					}
					return timerToShow.ToString().Substring(0, 8);
				}
				return timerToShow.ToString().Substring(3, 5);
			}
		}
		
		/// <summary>
		/// Gets the total number of seconds remaining to replenish all lives.
		/// </summary>
		/// <value>The seconds to full lives.</value>
		public double SecondsToFullLives { get { return secondsToNextLife + ((MaxLives - lives - 1) * (MinutesToRecover * 60)); } }
		
		#endregion
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ExaGames.Common.LivesManager"/> class.
		/// </summary>
		public LivesManager() {
			calculateSteps = false;
		}
		
		#region Unity Behaviour Methods
		
		/// <summary>
		/// Initializes lives, max lives and recoveryStartTime from local persistance if player is comming back, or
		/// with full lives and default max lives if this is the first time she plays.
		/// </summary>
		private void Awake() {
			if(FindObjectsOfType<LivesManager>().Length > 1) {
				Debug.LogError("More than one LivesManager found in scene.");
			}
			#if !UNITY_EDITOR
			// This line ensures that preferences won't be reset by error with the game published.
			ResetPlayerPrefsOnPlay = false;
			#endif
			#if UNITY_IOS
			// Register for local notifications if set in the inspector.
			if(LocalNotificationSettings.AllowLocalNotifications) {
				UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert | UnityEngine.iOS.NotificationType.Badge | UnityEngine.iOS.NotificationType.Sound);
			}
			#endif
			if(ResetPlayerPrefsOnPlay) {
				ResetPlayerPrefs();
			} else {
				retrievePlayerPrefs();
			}
		}
		
		/// <summary>
		/// On start, calculates the number of lives that must be recovered and the remaining seconds for the next life.
		/// </summary>
		private void Start() {
			initTimer();
		}
		
		/// <summary>
		/// Every frame calculates the next step of the timer for the next life.
		/// </summary>
		private void Update() {
			if(calculateSteps) {
				stepRecoveryTime();
			}
		}
		
		/// <summary>
		/// When paused, saves the recovery start time to use it when unpaused.
		/// </summary>
		/// <param name="pauseStatus">If set to <c>true</c> pause status.</param>
		private void OnApplicationPause(bool pauseStatus) {
			if(pauseStatus) {
				applicationWasPaused = true;
				calculateSteps = false;
			} else if(applicationWasPaused) {
				applicationWasPaused = false;
				initTimer();
			}
		}
		
		/// <summary>
		/// On destroy, saves the recovery start time to use it next time the Lives Manager is available 
		/// (on application restart, for example).
		/// </summary>
		private void OnDestroy() {
			savePlayerPrefs();
		}
		
		#endregion
		
		#region Public Methods
		
		/// <summary>
		/// Consumes one life if available, and starts counting time for recovery.
		/// </summary>
		/// <returns><c>true</c>, if life was consumed, <c>false</c> otherwise.</returns>
		public bool ConsumeLife() {
			if(HasInfiniteLives) {
				return true;
			}
			bool result;
			if(lives > 0) {
				result = true;
				
				// If lifes where full, starts counting time for recovery.
				if(HasMaxLives) {
					recoveryStartTime = DateTime.Now;
					resetSecondsToNextLife();
				}
				lives--;
				notifyLivesChanged();
				savePlayerPrefs();
				if(LocalNotificationSettings.AllowLocalNotifications) {
					scheduleNotification();
				}
			} else {
				result = false;
			}
			return result;
		}
		
		/// <summary>
		/// Gives one life.
		/// </summary>
		public void GiveOneLife() {
			if(!HasMaxLives && !HasInfiniteLives) {
				lives++;
				recoveryStartTime = DateTime.Now;
				savePlayerPrefs();
				notifyAll();
			}
		}
		
		/// <summary>
		/// Sets the number of available lives to its maximum.
		/// </summary>
		public void FillLives() {
			if(!HasInfiniteLives) {
				lives = MaxLives;
				setSecondsToNextLifeToZero();
				notifyAll();
			}
		}
		
		/// <summary>
		/// Adds life slots for the player.
		/// </summary>
		/// <param name="quantity">The quantity of slots to add.</param>
		/// <param name="fillLives">If set to <c>true</c> fills lives to the new maximum.</param>
		public void AddLifeSlots(int quantity, bool fillLives = false) {
			if(HasMaxLives && !HasInfiniteLives) {
				recoveryStartTime = DateTime.Now;
				resetSecondsToNextLife();
			}
			MaxLives += quantity;
			if(fillLives) {
				FillLives();
			} else {
				savePlayerPrefs();
			}
			initTimer();
		}
		
		/// <summary>
		/// Gives infinite lives for the specified amount of minutes.
		/// </summary>
		/// <param name="minutes">The amount of minutes to grant infinite lives.</param>
		public void GiveInifinite(int minutes) {
			if(minutes <= 0) {
				return;
			}
			if(!HasInfiniteLives) {
				FillLives();
				infiniteLivesStartTime = DateTime.Now;
			}
			infiniteLivesMinutes += minutes;
			remainingSecondsWithInfiniteLives += minutes * 60;
			savePlayerPrefs();
			notifyAll();
		}
		
		#endregion
		
		/// <summary>
		/// Initializes the timer for next life.
		/// </summary>
		private void initTimer() {
			remainingSecondsWithInfiniteLives = calculateRemainingInfiniteLivesTime().TotalSeconds;
			if(!HasInfiniteLives) {
				secondsToNextLife = calculateLifeRecovery().TotalSeconds;
			}
			calculateSteps = true;
			notifyAll();
		}
		
		#region Data persistance
		
		/// <summary>
		/// Retrieves lives information from previous sessions.
		/// </summary>
		private void retrievePlayerPrefs() {
			remainingSecondsWithInfiniteLives = 0D;
			MaxLives = PlayerPrefs.HasKey(MAX_LIVES_SAVEKEY) ? PlayerPrefs.GetInt(MAX_LIVES_SAVEKEY) : DefaultMaxLives;
			if(PlayerPrefs.HasKey(INFINITE_LIVES_TIME_SAVEKEY) && PlayerPrefs.HasKey(INFINITE_LIVES_MINUTES_SAVEKEY)) {
				infiniteLivesStartTime = new DateTime(long.Parse(PlayerPrefs.GetString(INFINITE_LIVES_TIME_SAVEKEY)));
				infiniteLivesMinutes = PlayerPrefs.GetInt(INFINITE_LIVES_MINUTES_SAVEKEY);
			} else {
				infiniteLivesStartTime = DateTime.MinValue;
				infiniteLivesMinutes = 0;
			}
			if(PlayerPrefs.HasKey(LIVES_SAVEKEY) && PlayerPrefs.HasKey(RECOVERY_TIME_SAVEKEY)) {
				lives = PlayerPrefs.GetInt(LIVES_SAVEKEY);
				recoveryStartTime = new DateTime(long.Parse(PlayerPrefs.GetString(RECOVERY_TIME_SAVEKEY)));
			} else {
				lives = MaxLives;
				recoveryStartTime = DateTime.Now;
			}
			if(lives > MaxLives) {
				FillLives();
			}
		}
		
		/// <summary>
		/// Saves the recovery start time to use it next time the Lives Manager is available 
		/// (on application restart, for example).
		/// </summary>
		private void savePlayerPrefs() {
			PlayerPrefs.SetInt(MAX_LIVES_SAVEKEY, MaxLives);
			PlayerPrefs.SetInt(LIVES_SAVEKEY, lives);
			PlayerPrefs.SetString(RECOVERY_TIME_SAVEKEY, recoveryStartTime.Ticks.ToString());
			PlayerPrefs.SetString(INFINITE_LIVES_TIME_SAVEKEY, infiniteLivesStartTime.Ticks.ToString());
			PlayerPrefs.SetInt(INFINITE_LIVES_MINUTES_SAVEKEY, infiniteLivesMinutes);
			try {
				PlayerPrefs.Save();
			} catch(Exception e) {
				Debug.LogError("Could not save LivesManager preferences: " + e.Message);
			}
		}
		
		/// <summary>
		/// Resets the all the preferences of the LivesManager. Use with care.
		/// </summary>
		public void ResetPlayerPrefs() {
			PlayerPrefs.DeleteKey(MAX_LIVES_SAVEKEY);
			PlayerPrefs.DeleteKey(RECOVERY_TIME_SAVEKEY);
			PlayerPrefs.DeleteKey(LIVES_SAVEKEY);
			PlayerPrefs.DeleteKey(INFINITE_LIVES_TIME_SAVEKEY);
			PlayerPrefs.DeleteKey(INFINITE_LIVES_MINUTES_SAVEKEY);
			clearNotification();
			retrievePlayerPrefs();
		}
		
		#endregion
		
		#region TimeToNextLife control
		
		/// <summary>
		/// Calculates the time remaining for the next life, and recovers all possible lives.
		/// </summary>
		/// <returns>Time remaining for the next life.</returns>
		private TimeSpan calculateLifeRecovery() {
			DateTime now = DateTime.Now;
			TimeSpan elapsed = now - recoveryStartTime;
			double minutesElapsed = elapsed.TotalMinutes;
			
			while((!HasMaxLives) && (minutesElapsed >= MinutesToRecover)) {
				lives++;
				recoveryStartTime = DateTime.Now;
				minutesElapsed -= MinutesToRecover;
			}
			
			savePlayerPrefs();
			
			if(HasMaxLives) {
				return TimeSpan.Zero;
			} else {
				return TimeSpan.FromMinutes(MinutesToRecover - minutesElapsed);
			}
		}
		
		/// <summary>
		/// Calculates the time remaining with infinite lives.
		/// </summary>
		/// <returns>The remaining infinite lives time.</returns>
		private TimeSpan calculateRemainingInfiniteLivesTime() {
			DateTime now = DateTime.Now;
			TimeSpan elapsed = now - infiniteLivesStartTime;
			double minutesElapsed = elapsed.TotalMinutes;
			
			if(minutesElapsed < (double)infiniteLivesMinutes) {
				return TimeSpan.FromMinutes(infiniteLivesMinutes - minutesElapsed);
			} else {
				return TimeSpan.Zero;
			}
		}
		
		/// <summary>
		/// Calculates one step in the timer for next life.
		/// </summary>
		private void stepRecoveryTime() {
			if(HasInfiniteLives) {
				remainingSecondsWithInfiniteLives -= Time.unscaledDeltaTime;
				if(remainingSecondsWithInfiniteLives < 0D) {
					remainingSecondsWithInfiniteLives = 0D;
					infiniteLivesMinutes = 0;
					infiniteLivesStartTime = new DateTime(0);
					notifyLivesChanged();
				}
				notifyRecoveryTimeChanged();
			} else if(!HasMaxLives) {
				if(secondsToNextLife > 0D) {
					secondsToNextLife -= Time.unscaledDeltaTime;
					notifyRecoveryTimeChanged();
				} else {
					GiveOneLife();
					notifyLivesChanged();
					if(HasMaxLives) {
						setSecondsToNextLifeToZero();
					} else {
						resetSecondsToNextLife();
					}
				}
			}
		}
		
		/// <summary>
		/// Sets the seconds to next life to zero.
		/// </summary>
		private void setSecondsToNextLifeToZero() {
			secondsToNextLife = 0;
			notifyRecoveryTimeChanged();
		}
		
		/// <summary>
		/// Resets the seconds to next life.
		/// </summary>
		private void resetSecondsToNextLife() {
			secondsToNextLife = MinutesToRecover * 60;
			notifyRecoveryTimeChanged();
		}
		
		#endregion
		
		#region Notifications for observers
		
		/// <summary>
		/// Notifies all changes (time and lives) to the observer.
		/// </summary>
		private void notifyAll() {
			notifyRecoveryTimeChanged();
			notifyLivesChanged();
		}
		
		/// <summary>
		/// Notifies obvservers on recovery time changed.
		/// </summary>
		private void notifyRecoveryTimeChanged() {
			OnRecoveryTimeChanged.Invoke();
		}
		
		/// <summary>
		/// Notifies observers on lives changed.
		/// </summary>
		private void notifyLivesChanged() {
			OnLivesChanged.Invoke();
		}
		
		#endregion
		
		#region Local notifications
		
		/// <summary>
		/// Key to identify the LivesManager notification among others.
		/// </summary>
		private const string LOCAL_NOTIF_KEY = "ExaGames.TimeBasedLifeSystem";
		
		/// <summary>
		/// Notification settings container.
		/// </summary>
		[Serializable]
		public class NotificationSettingsStruct {
			/// <summary>
			/// Indicates wether the lives manager should support Unity Nofitication Services.
			/// </summary>
			public bool AllowLocalNotifications = true;
			/// <summary>
			/// Custom alert action for the shceduled notifications.
			/// </summary>
			public string AlertAction;
			/// <summary>
			/// Custom text for scheduled notifications.
			/// </summary>
			public string AlertBody = "All lives have been recovered!";
		}
		
		/// <summary>
		/// Local notification settings set in the Inspector.
		/// </summary>
		public NotificationSettingsStruct LocalNotificationSettings;
		
		/// <summary>
		/// Schedules a notification to inform the player on lives replenished.
		/// </summary>
		private void scheduleNotification() {
			double secondsDelay = 0D;
			
			#if UNITY_IOS || UNITY_ANDROID
			clearNotification();
			if(!HasMaxLives) {
				if(string.IsNullOrEmpty(LocalNotificationSettings.AlertBody)) {
					Debug.LogError("Could not schedule local notification because the AlertBody property has not been set.");
					return;
				}
				Debug.Log("Scheduling local notification...");
				secondsDelay = secondsToNextLife + ((MaxLives - lives - 1) * (MinutesToRecover * 60));
			}
			#endif
			
			#if UNITY_IOS
			if(!HasMaxLives) {
				var notification = new UnityEngine.iOS.LocalNotification();
				notification.fireDate = DateTime.Now.AddSeconds(secondsDelay);
				if(!string.IsNullOrEmpty(LocalNotificationSettings.AlertAction.Trim()))
					notification.alertAction = LocalNotificationSettings.AlertAction.Trim();
				notification.alertBody = LocalNotificationSettings.AlertBody;
				notification.soundName = UnityEngine.iOS.LocalNotification.defaultSoundName;
				
				var options = new Dictionary<string,string>();
				options.Add(LOCAL_NOTIF_KEY, "");
				notification.userInfo = options;
				UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(notification);
			}
			#endif
			#if UNITY_ANDROID && ANDROID_NOTIFICATIONS
			PlayerPrefs.SetInt(LOCAL_NOTIF_KEY,
			                   NotificationManager.SendWithAppIcon(
				TimeSpan.FromSeconds(secondsDelay), 
				Application.productName, 
				LocalNotificationSettings.AlertBody, 
				new Color(0, 0.6f, 1), 
				NotificationIcon.Star));
			#endif
		}
		
		/// <summary>
		/// Clears the LivesManager local notification if previously set.
		/// </summary>
		private void clearNotification() {
			#if UNITY_IOS
			UnityEngine.iOS.LocalNotification notifToCancel = null;
			var localNotifications = UnityEngine.iOS.NotificationServices.scheduledLocalNotifications;
			
			try {
				for(int i=0; i<localNotifications.Length; i++) {
					if(localNotifications[i].userInfo != null && localNotifications[i].userInfo.Count > 0 && localNotifications[i].userInfo.Contains(LOCAL_NOTIF_KEY)) {
						notifToCancel = localNotifications[i];
						break;
					}
				}
			} finally {
				if(notifToCancel != null) {
					UnityEngine.iOS.NotificationServices.CancelLocalNotification(notifToCancel);
				}
			}
			#endif
			#if UNITY_ANDROID && ANDROID_NOTIFICATIONS
			if(PlayerPrefs.HasKey(LOCAL_NOTIF_KEY)) {
				NotificationManager.Cancel(PlayerPrefs.GetInt(LOCAL_NOTIF_KEY));
			}
			#endif
		}
		
		#endregion
	}
}