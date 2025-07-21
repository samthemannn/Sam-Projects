// Program 4: Polymorphism with Exercise Tracking

using System;
using System.Collections.Generic;

public abstract class Activity
{
    private DateTime date;
    protected int minutes;

    public Activity(DateTime date, int minutes)
    {
        this.date = date;
        this.minutes = minutes;
    }

    public abstract double GetDistance();
    public abstract double GetSpeed();
    public abstract double GetPace();

    public virtual string GetSummary()
    {
        return $"{date.ToString("dd MMM yyyy")} {GetType().Name} ({minutes} min) - Distance: {GetDistance():F1} miles, Speed: {GetSpeed():F1} mph, Pace: {GetPace():F1} min per mile";
    }
}

public class Running : Activity
{
    private double distance;

    public Running(DateTime date, int minutes, double distance)
        : base(date, minutes)
    {
        this.distance = distance;
    }

    public override double GetDistance()
    {
        return distance;
    }

    public override double GetSpeed()
    {
        return (distance / minutes) * 60;
    }

    public override double GetPace()
    {
        return minutes / distance;
    }
}

public class StationaryBicycle : Activity
{
    private double speed;

    public StationaryBicycle(DateTime date, int minutes, double speed)
        : base(date, minutes)
    {
        this.speed = speed;
    }

    public override double GetDistance()
    {
        return (speed * minutes) / 60;
    }

    public override double GetSpeed()
    {
        return speed;
    }

    public override double GetPace()
    {
        return 60 / speed;
    }
}

public class Swimming : Activity
{
    private int laps;

    public Swimming(DateTime date, int minutes, int laps)
        : base(date, minutes)
    {
        this.laps = laps;
    }

    public override double GetDistance()
    {
        return laps * 50 / 1000 * 0.62;
    }

    public override double GetSpeed()
    {
        return (GetDistance() / minutes) * 60;
    }

    public override double GetPace()
    {
        return minutes / GetDistance();
    }
}

public class Polymorphism
{
    public static void Run()
    {
        List<Activity> activities = new List<Activity>();
        activities.Add(new Running(new DateTime(2022, 11, 3), 30, 3.0));
        activities.Add(new StationaryBicycle(new DateTime(2022, 11, 3), 30, 6.0));
        activities.Add(new Swimming(new DateTime(2022, 11, 3), 30, 20));

        foreach (var activity in activities)
        {
            Console.WriteLine(activity.GetSummary());
        }
    }
}
