This is the documentation for the live countdown component, inspired by Fortnite's live events.

---------
Important
---------

UTC is used, so the time will be synchronised across all countries.

PostEventTime will only be invoked if the game is started after the live event, it won't trigger if you witnessed the live event in your session.
This means that whatever you plan to change in PostEventTime needs to happen in OnEventTime as well.

---------
Variables
---------

(TextMeshPro is referenced in the script, but you can change it to Unity's Text component if you want.)
TMP_Text[] Timer
"Every TextMeshPro Text component referenced in this array will display the time left until the live event, in a dd:hh:mm:ss format."

int Year
int Month
int Day
int Hour
int Minute
int Second
"In the Inspector, you'll need to set these variables to the desired date and time for your live event."

DateTime EventDate;
"This is just the above integers compressed into a DateTime format. It won't appear in the editor."

DateTime CurrentTime;
"This is the current time in UTC used for the countdown calculations."

DateTime StartTime;
"In Start(), this will be given the date and time from the internet."

float ElapsedTime;
"This is the project time that has passed since StartTime was set."

UnityEvent PreEventTime;
"Every event you assign here in the Inspector will be invoked when the game is started before the live event."

UnityEvent OnEventTime;
"Every event you assign here in the Inspector will be invoked when the game is running when the specified date and time is reached."

UnityEvent PostEventTime;
"Every event you assign here in the Inspector will be invoked when the game is started after the live event."

UnityEvent OnInternetError;
"Every event you assign here in the Inspector will be invoked when the game cannot get the date and time from the internet."

bool DestroyOnEventTime; (Default: true)
"Whether this GameObject will be destroyed when OnEventTime or PostEventTime are invoked."

string TimerFormatting; (Default: "dd\:hh\:mm\:ss")
"The formatting for the TextMeshPro timer."

bool GetTimeFromInternet; (Default: true)
"When enabled, the time will be retrieved from http://www.microsoft.com at the start. When disabled, the time will be retrieved from the system memory."

bool UpdateTime; (Default: true)
"When disabled, the component will no longer update its CurrentTime variable, meaning you can set it yourself with a script. This should be used when getting the DateTime from an online source."

bool InternetError;
"Returns true when the date and time cannot be retrieved from the internet."

TimeSpan TimeLeft; (Read-Only)
"The non-formatted time span until the live event."

int MillisecondsLeft; (Read-Only)
int SecondsLeft; (Read-Only)
int MinutesLeft; (Read-Only)
int HoursLeft; (Read-Only)
int DaysLeft; (Read-Only)
"The time span left until the live event, in individual units. Values will be negative after the live event."
"For example, SecondsLeft would be 120 on the last two minutes before the event."
"Another example, DaysLeft would be -7 one week after the live event."

-------
Methods
-------

ActivatePreEvent() [ContextMenu("Test Pre-Event Changes")]
"When this method is called in Play mode, every event in PreEventTime will be invoked."

ActivateLiveEvent() [ContextMenu("Test Live Event")]
"When this method is called in Play mode, every event in OnEventTime will be invoked."

ActivatePostEvent() [ContextMenu("Test Post-Event Changes")]
"When this method is called in Play mode, every event in PostEventTime will be invoked."

ActivateInternetError() [ContextMenu("Test Internet Error")]
"When this method is called in Play mode, every event in OnInternetError will be invoked."

PrintTimeLeft() [ContextMenu("Print Time Left")]
"When this method is called, the time left until the live event will be printed in the console."

--------
Examples
--------


-= Live Events =-

"Create an Animator component, with an animation for a Fortnite-style live event."

"Set OnEventTime to set a trigger on the Animator to begin the animation."

"If your live event will end with permanent changes to the playable area, create a script that recreates the final frame of the animation, then set PostEventTime to trigger it."

"If you want, you can have a TextMeshPro Text component display the time left until the live event starts."

"If everything goes right, the live event will happen on the assigned date and time, and anyone playing afterwards will see the aftermath of the event."



-= Live Changes =-

"You can also create live changes over time, similar to the Cube from Fortnite, or The Moon from The Legend of Zelda: Majora's Mask."

"First set the end date/time for these changes."

"Other GameObjects can reference this component, and behave based on the time left."

"For example, you could make a meteor move closer to the playable area, based on the seconds left until the event."

"Or, you could make an energy orb that cracks a little bit more with each day."

"This will work even if there is no live event to trigger."