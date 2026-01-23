using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimeManager : MonoBehaviour
{   
    public static TimeManager Instance { get; private set; }

    [Header ("Internal Clock")]
    [SerializeField] 
    GameTimeStamp timestamp;
    public float timeScale = 1.0f;

    [Header ("Day and Night cycle")]
    public Transform sunTransform;  

    // list of objects to inform of changes to the time
    List< ITimeTracker> listeners = new List<ITimeTracker>();
    private void Awake()
    {
        //if there is more than one instance, destroy the extra
        if(Instance!= null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            //setting the static instance to this instance
            Instance = this;
        }
    }

    void Start()
    {
        timestamp = new GameTimeStamp(0, GameTimeStamp.Season.Spring, 1, 6, 0);
        StartCoroutine(TimeUpdate());

    }
    IEnumerator TimeUpdate()
    {
        while(true){
            Tick();
            yield return new WaitForSeconds(1/timeScale);
        }
    }
    // a tick of the game  
    public void Tick() {

        timestamp.UpdateClock();

         // inform the listeners of the new time state
        foreach(ITimeTracker listener in listeners)
        { 
            listener.ClockUpdate(timestamp);
        }

        UpdateSunMovement();
    }

    public void SkipTime(GameTimeStamp timeToSkipTo)
    {
        //Convert to minutes
        int timeToSkipInMinutes = GameTimeStamp.TimestampInMinutes(timeToSkipTo);
        Debug.Log("Time to skip to:" + timeToSkipInMinutes);
        int timeNowInMinutes = GameTimeStamp.TimestampInMinutes(timestamp);
        Debug.Log("Time now: " + timeNowInMinutes);

        int differenceInMinutes = timeToSkipInMinutes - timeNowInMinutes;
        Debug.Log(differenceInMinutes + " minutes will be advanced");

        //Check if the timestamp to skip to has already been reached
        if (differenceInMinutes <= 0) return; 

        for(int i = 0; i<differenceInMinutes; i++)
        {
            Tick(); 
        }
    }
    
// day and night cycle
    void UpdateSunMovement()
    {
        int timeInMinutes = GameTimeStamp.HoursToMinutes(timestamp.hour) + timestamp.minute;

        // sun moves 15 degrees in an hour
        // .25 degrees in a minute 
        // at midnight, the angle of the sun should be 90  
        float sunAngle = .25f * timeInMinutes - 90;

        // Apply the angle to the directional light
        sunTransform.eulerAngles = new Vector3(sunAngle, 0, 0);  
    }

    public GameTimeStamp GetGameTimeStamp()
    {
        return new GameTimeStamp(timestamp);
    }

    //Handling listeners
    // add the objects to the list of listeners
    public void RegisterTracker(ITimeTracker listener)
    {
        listeners.Add(listener);
    }

    // remove the object from list of listeners
    public void UnregisterTracker(ITimeTracker listener)
    {
        listeners.Remove(listener);
    }

}
