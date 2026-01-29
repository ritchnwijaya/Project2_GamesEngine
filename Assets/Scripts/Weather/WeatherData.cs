using UnityEngine;

[CreateAssetMenu(fileName = "WeatherData", menuName = "Weather/WeatherData")]

public class WeatherData : ScriptableObject
{

    public enum WeatherType {
    Sunny, Rain, Snow
    }

    public WeatherProbability[] springWeather;

    public WeatherProbability[] summerWeather;

    public WeatherProbability[] fallWeather;

    public WeatherProbability[] winterWeather; 

    
}