using UnityEngine;

public class WeatherManager : MonoBehaviour, ITimeTracker
{
    public static WeatherManager Instance { get; private set; }
    private void Awake()
    {
        //If there is more than one instance, destroy the extra
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            //Set the static instance to this instance
            Instance = this;
        }
    }

    public WeatherData.WeatherType WeatherToday { get; private set; }

    public WeatherData.WeatherType WeatherTomorrow { get; private set; }

    bool weatherSet = false; 

    [SerializeField] WeatherData weatherData;


    void Start()
    {
        TimeManager.Instance.RegisterTracker(this);
    }




        public WeatherData.WeatherType ComputeWeather(GameTimeStamp.Season season)
    {
        if(weatherData == null)
        {
            throw new System.Exception("No weather data loaded"); 
        }

        //What are the possible weathers to compute
        WeatherProbability[] weatherSet = null;

        switch (season)
        {
            case GameTimeStamp.Season.Spring:
                weatherSet = weatherData.springWeather;
                break; 
            case GameTimeStamp.Season.Summer:
                weatherSet = weatherData.summerWeather;
                break;

            case GameTimeStamp.Season.Fall:
                weatherSet = weatherData.fallWeather;
                break;

            case GameTimeStamp.Season.Winter:
                weatherSet = weatherData.winterWeather;
                break; 
        }

        //Roll a random value 
        float randomValue = Random.Range(0, 1f);

        //Initialise probability
        float culmProbability = 0; 
        foreach(WeatherProbability weatherProbability in weatherSet)
        {
            culmProbability += weatherProbability.probability;
            if (randomValue <= culmProbability) {
                return weatherProbability.weatherType;
            }

        }

        //Sunny by default
        return WeatherData.WeatherType.Sunny; 
    }

    public void LoadWeather(WeatherSaveState saveState)
    {
        weatherSet = true;
        WeatherToday = saveState.weather;

        //Set the forecast 
        WeatherTomorrow = ComputeWeather(TimeManager.Instance.GetGameTimeStamp().season);
    }


    public void ClockUpdate(GameTimeStamp timestamp)
    {
        //Check if it is 6am 
        if(timestamp.hour == 6 && timestamp.minute == 0)
        {
            //Set the current weather
            if (!weatherSet)
            {
                WeatherToday = ComputeWeather(timestamp.season);
               
            }
            else
            {
                WeatherToday = WeatherTomorrow;
                
            }
            FindFirstObjectByType<WeatherEffectController>()?.LoadParticle();
            UIManager.Instance.ChangeWeatherUI();

            //Set the forecast 
            WeatherTomorrow = ComputeWeather(timestamp.season);

            weatherSet = true;
            Debug.Log("The weather is " + WeatherToday.ToString());


        }
    }
}