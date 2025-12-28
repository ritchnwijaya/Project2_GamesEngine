using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameTimeStamp 
{
    public int year;
    public enum Season
    {
        Spring, 
        Summer, 
        Fall, 
        Winter
    }

    public Season season;

    public enum DayOfTheWeek
    { // day one will be on Monday
        Sunday,
        Monday, 
        Tuesday, 
        Wednesday,
        Thursday, 
        Friday, 
        Saturday
    }
    public int day;
    public int hour;
    public int minute;

    public GameTimeStamp(int year, Season season, int day, int hour, int minute){
        this.year = year;
        this.season = season;
        this.day = day;
        this.hour = hour;
        this.minute = minute;
    }
    // creating a new instance of a gametimestamp from another pre-existing one
    public GameTimeStamp(GameTimeStamp timestamp)
    {
        this.year = timestamp.year;
        this.season = timestamp.season;
        this.day = timestamp.day;
        this.hour = timestamp.hour;
        this.minute = timestamp.minute;
    }

    // increments time by one minute
    public void UpdateClock()
    {
        minute++;

        if(minute >= 60) 
        {
            minute = 0;
            hour++;
        }
        if(hour >= 24) 
        {
            hour = 0;
            day++;
        }
        if(day >= 30) 
        {
            // reset days
            day = 1;

            if (season == Season.Winter)
            {
                season= Season.Spring;
                year++; // start of new year
        } else {
                season++;
            }

        }
    }

    public DayOfTheWeek GetDayOfTheWeek()
    {
        // convert the total time passed into days
        int daysPassed = YearsToDays(year) + SeasonsToDays(season) + day;

        // remainder after dividing daysPassed by 7 
        int dayIndex = daysPassed % 7;

        // cast  into day of the week enum
        return (DayOfTheWeek)dayIndex;
    }

    public static int HoursToMinutes(int hour)
    {
        return hour * 60;
    }

    public static int DaysToHours(int days)
    {
        return days * 24;
    }

    public static int SeasonsToDays(Season season)
    {
        int seasonIndex = (int)season;
        return seasonIndex * 30;
    }

    public static int YearsToDays(int years){
        return years * 4 * 30;
    }

    //calculate difference between 2 timestamps
    public static int CompareTimeStamps(GameTimeStamp timestamp1, GameTimeStamp timestamp2)
    {
        int timestamp1Hours = DaysToHours(YearsToDays(timestamp1.year)) + DaysToHours(SeasonsToDays(timestamp1.season)) + DaysToHours(timestamp1.day) + timestamp1.hour;
        int timestamp2Hours = DaysToHours(YearsToDays(timestamp2.year)) + DaysToHours(SeasonsToDays(timestamp2.season)) + DaysToHours(timestamp2.day) + timestamp2.hour;
        int difference = timestamp2Hours - timestamp1Hours;
 
        return Mathf.Abs(difference);
    }
}

