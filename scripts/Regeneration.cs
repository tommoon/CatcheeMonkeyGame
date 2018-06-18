using System;
using UnityEngine;
using UnityEngine.UI;

public class RegenerationEventArg : EventArgs{
	public readonly  int lifeAmount = 0;
	public RegenerationEventArg(int lifeAmount){
		this.lifeAmount = lifeAmount;
	}
}

public class Regeneration : MonoBehaviour 
{
	[SerializeField] private Text lifeNumberText = null;
	[SerializeField] private Text timerText = null;
	private int maxLifeAmount = 5;
	private int lifeAmount = 0;
	public int LifeAmount { get { return this.lifeAmount; } }
	private DateTime previous;
	private DateTime fullLife;
	private DateTime next;

	private int minutesForNewLife = 10;
	private int secondsForUI;

	public event EventHandler<RegenerationEventArg> RaiseNewLife;
	protected void OnNewLife(RegenerationEventArg arg){
		if (RaiseNewLife != null) {
			RaiseNewLife (this, arg);
		}
	}

	private void Awake()
	{
		string str = PlayerPrefs.GetString("SaveData", null);
		if (string.IsNullOrEmpty (str) == false) 
		{
			string[] strs = str.Split ('?');
			// 1 - DateTime 
			DateTime recordedDt = Convert.ToDateTime (strs[0]);
			// 2 - lifeAmount
			this.lifeAmount = int.Parse(strs[1]);
			// 3 - maxLifeAmount
			this.maxLifeAmount = int.Parse(strs[2]);
			// 4 - minutesForNewLife
			this.minutesForNewLife = int.Parse(strs[3]);

			int result = DateTime.Compare (recordedDt, DateTime.Now);
			if (result < 0) 
			{
				// Full time for life
				this.lifeAmount = this.maxLifeAmount;
				timerText.text = this.minutesForNewLife.ToString () + ":00";

			} else {
				// Timer is not over
				TimeSpan ts = recordedDt - DateTime.Now;
				int timeRemaining = (int)ts.TotalSeconds;
				int timeForLife = (this.minutesForNewLife * 60);

				int remainingTimeForTimer = timeRemaining % timeForLife;
				int removeLife = (timeRemaining / timeForLife);
				this.lifeAmount = this.maxLifeAmount - removeLife -1;
				this.next = DateTime.Now.AddSeconds (remainingTimeForTimer);
			}
		}
		else {
			// first time playing
			this.lifeAmount = this.maxLifeAmount = 5;
			int min = this.minutesForNewLife = 10;
			timerText.text = min.ToString () + ":00";

		}
		this.lifeNumberText.text = this.lifeAmount.ToString ();
	}
	private void Update()
	{
		if (this.lifeAmount == this.maxLifeAmount) 
		{
			return; 
		}
		TimeSpan ts =  this.next.Subtract (DateTime.Now);
		if (ts.Duration().Seconds == 0 && ts.Milliseconds < 0) 
		{
			OnNewLife(new RegenerationEventArg(this.lifeAmount));
			this.lifeAmount++;
			this.lifeNumberText.text = this.lifeAmount.ToString ();
			if (this.lifeAmount != this.maxLifeAmount) {
				this.next = DateTime.Now.AddMinutes (minutesForNewLife);
			} else {
				timerText.text = (this.minutesForNewLife).ToString() + "00";
				return;
			}

		}
		if (ts.Seconds != secondsForUI) 
		{
			secondsForUI = ts.Seconds;
			timerText.text = ts.Minutes.ToString () + ":00" + ts.Seconds.ToString ("00") ; 
		}
		timerText.text = ts.Minutes.ToString () + ":" + ts.Seconds.ToString ("00") ; 
	}

	public void UseLife()
	{
		if (this.lifeAmount > 0) 
		{
			if (this.lifeAmount == this.maxLifeAmount) {
				this.next = DateTime.Now.AddMinutes(this.minutesForNewLife);
			}
			this.lifeAmount--;
			this.lifeNumberText.text = this.lifeAmount.ToString ();
		}
	}

	// If quitting the app, pausing the app or changing scene, Save data.
	private void OnApplicationQuit()
	{
		Save ();
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus == true) {
			Save ();
			return;
		}
		Awake();
	}

	private void OnDestroy()
	{
		Save ();
	}

	private void Save()
	{
		string save = null;
		if (this.lifeAmount != this.maxLifeAmount) 
		{
			TimeSpan ts =  this.next.Subtract (DateTime.Now);
			DateTime dt = DateTime.Now.Add (ts);
			int lifeDiff = this.maxLifeAmount - this.lifeAmount - 1;
			dt = dt.AddMinutes (lifeDiff * this.minutesForNewLife);
			save = dt.ToString () + " " + this.lifeAmount + " " + this.maxLifeAmount +" "+this.minutesForNewLife;
			PlayerPrefs.SetString ("SaveData",save);
			return;
		}
		save = DateTime.Now.ToString () + " " + this.lifeAmount + " " + this.maxLifeAmount + " " +this.minutesForNewLife;
		PlayerPrefs.SetString ("SaveData",save);
	}
}
