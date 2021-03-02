using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class LiveCountdown : MonoBehaviour
{
    [Tooltip("The following TMP_Text components will display the time left until the live event.")]
    public TMP_Text[] Timer;
    [Header("Live Event Time (UTC)")]
    [Tooltip("Invoke OnEventTime() on this date and time. (Uses Universal Coordinated Time.)")]
    public int Year = 2021;
    [Range(1, 12)][Tooltip("Invoke OnEventTime() on this date and time. (Uses Universal Coordinated Time.)")]
    public int Month;
    [Range(1, 31)][Tooltip("Invoke OnEventTime() on this date and time. (Uses Universal Coordinated Time.)")]
    public int Day;
    [Range(0, 23)][Tooltip("Invoke OnEventTime() on this date and time. (Uses Universal Coordinated Time.)")]
    public int Hour;
    [Range(0, 59)][Tooltip("Invoke OnEventTime() on this date and time. (Uses Universal Coordinated Time.)")]
    public int Minute;
    [Range(0, 59)][Tooltip("Invoke OnEventTime() on this date and time. (Uses Universal Coordinated Time.)")]
    public int Second;
    public DateTime EventDate;
    public DateTime CurrentTime;
    public DateTime StartTime;
    public float ElapsedTime;
    [Header("Invokable Events")]
    [Tooltip("Invoked when the scene is started before the specified date and time.")]
    public UnityEvent PreEventTime;
    [Tooltip("Invoked when the specified date and time is reached.")]
    public UnityEvent OnEventTime;
    [Tooltip("Invoked when the scene is started after the specified date and time.")]
    public UnityEvent PostEventTime;
    [Tooltip("Invoked when the date and time couldn't be retrieved from the internet.")]
    public UnityEvent OnInternetError;
    [Header("Settings")]
    [Tooltip("Whether this GameObject will be destroyed when OnEventTime or PostEventTime are invoked.")]
    public bool DestroyOnEventTime = true;
    [Tooltip("How the TextMeshPro display will be formatted.")]
    public string TimerFormatting = (@"dd\:hh\:mm\:ss");
    [Tooltip("When enabled, the time will be retrieved from http://www.microsoft.com at the start. When disabled, the time will be retrieved from the system every update.")]
    public bool GetTimeFromInternet = true;
    [Tooltip("ADVANCED: When disabled, this component will stop updating CurrentTime, so you can change it using another script.")]
    public bool UpdateTime = true;
    [Tooltip("This boolean is true when you cannot retrieve the date/time to the website.")]
    public bool InternetError;
    public TimeSpan TimeLeft
    {
        get
        {
            return CurrentTime.Subtract(new DateTime(Year, Month, Day, Hour, Minute, Second));
        }
    }
    public int MillisecondsLeft
    {
        get
        {
            return Convert.ToInt32((CurrentTime.Subtract(new DateTime(Year, Month, Day, Hour, Minute, Second))).TotalMilliseconds) * -1;
        }
    }
    public int SecondsLeft
    {
        get
        {
            return Convert.ToInt32((CurrentTime.Subtract(new DateTime(Year, Month, Day, Hour, Minute, Second))).TotalSeconds) * -1;
        }
    }
    public int MinutesLeft
    {
        get
        {
            return Convert.ToInt32((CurrentTime.Subtract(new DateTime(Year, Month, Day, Hour, Minute, Second))).TotalMinutes) * -1;
        }
    }
    public int HoursLeft
    {
        get
        {
            return Convert.ToInt32((CurrentTime.Subtract(new DateTime(Year, Month, Day, Hour, Minute, Second))).TotalHours) * -1;
        }
    }
    public int DaysLeft
    {
        get
        {
            return Convert.ToInt32((CurrentTime.Subtract(new DateTime(Year, Month, Day, Hour, Minute, Second))).TotalDays) * -1;
        }
    }
    private DateTime GetNetTime()
    {
        var myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.google.com");
        var response = myHttpWebRequest.GetResponse();
        string todaysDates = response.Headers["date"];
        return DateTime.ParseExact(todaysDates,
                                   "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                                   CultureInfo.InvariantCulture.DateTimeFormat,
                                   DateTimeStyles.AssumeUniversal);
    }

    void Awake()
    {
        Application.runInBackground = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (UpdateTime && !GetTimeFromInternet)
        {
            CurrentTime = DateTime.UtcNow;
        }
        else if (UpdateTime && GetTimeFromInternet)
        {
            try
            {
                StartTime = GetNetTime();
                print(StartTime.ToString(TimerFormatting, CultureInfo.InvariantCulture));
                CurrentTime = StartTime;
            }
            catch
            {
                InternetError = true;
                ActivateInternetError();
            }
        }
        if (CurrentTime > new DateTime(Year, Month, Day, Hour, Minute, Second) && !InternetError)
        {
            ActivatePostEvent();
        }
        else if (CurrentTime < new DateTime(Year, Month, Day, Hour, Minute, Second) && !InternetError)
        {
            ActivatePreEvent();
        }
    }

    // Update is called once per frame
    void Update()
    {
        ElapsedTime += Time.unscaledDeltaTime;
        if (UpdateTime && !GetTimeFromInternet)
        {
            CurrentTime = DateTime.UtcNow;
        }
        else if (UpdateTime && GetTimeFromInternet && !InternetError)
        {
            CurrentTime = StartTime.AddSeconds(ElapsedTime);
        }
        if (((CurrentTime.Subtract(new DateTime(Year, Month, Day, Hour, Minute, Second))).ToString(@"dd\:hh\:mm\:ss", CultureInfo.InvariantCulture) == "00:00:00:00") && !InternetError)
        {
            ActivateLiveEvent();
        }
        else
        {
            if (Timer.Length != 0)
            {
                foreach (TMP_Text element in Timer)
                {
                    if (!InternetError)
                    {
                        element.text = (CurrentTime.Subtract(new DateTime(Year, Month, Day, Hour, Minute, Second))).ToString(TimerFormatting, CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        element.text = ("ERROR");
                    }
                }
            }
        }
    }

    [ContextMenu("Test Pre-Event Changes")]
    public void ActivatePreEvent()
    {
        if (Application.isPlaying)
        {
            print("A live event hasn't happened yet.");
            PreEventTime.Invoke();
        }
        else
        {
            Debug.LogWarning("You cannot test pre-event conditions in Edit mode!");
        }
    }

    [ContextMenu("Test Live Event")]
    public void ActivateLiveEvent()
    {
        if (Application.isPlaying)
        {
            print("A live event is starting!");
            OnEventTime.Invoke();
            if (DestroyOnEventTime)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            Debug.LogWarning("You cannot test live events in Edit mode!");
        }
    }

    [ContextMenu("Test Post-Event Changes")]
    public void ActivatePostEvent()
    {
        if (Application.isPlaying)
        {
            print("A live event has happened.");
            PostEventTime.Invoke();
            if (DestroyOnEventTime)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            Debug.LogWarning("You cannot test post-event changes in Edit mode!");
        }
    }

    [ContextMenu("Test Internet Error")]
    public void ActivateInternetError()
    {
        if (Application.isPlaying)
        {
            print("The date and time couldn't be retrieved from the internet.");
            OnInternetError.Invoke();
        }
        else
        {
            Debug.LogWarning("You cannot test an internet connection error in Edit mode!");
        }
    }

    [ContextMenu("Print Time Left")]
    public void PrintTimeLeft()
    {
        print(MillisecondsLeft + " milliseconds.");
        print(SecondsLeft + " seconds.");
        print(MinutesLeft + " minutes.");
        print(HoursLeft + " hours.");
        print(DaysLeft + " days.");
    }
}