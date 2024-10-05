
using System;

public class Flock
{
    Drone[] agents;
    int num;

    public Flock(int maxnum)
    {
        agents = new Drone[maxnum];
    }

    public void Init(int num)
    {
        this.num = num;
        for (int i = 0; i < num; i++)
        {
            agents[i] = new Drone(i);
        }
    }

    public void Update()
    {
        for (int i = 0; i < num; i++)
        {
            agents[i].Update();
        }
    }

    public float average()
    {
        float totalTemperature = 0;
        for (int i = 0; i < num; i++)
        {
            totalTemperature += agents[i].Temperature;
        }
        return totalTemperature / num;
    }

    public int max()
    {
        float maxTemp = agents[0].Temperature;
        for (int i = 1; i < num; i++)
        {
            if (agents[i].Temperature > maxTemp)
                maxTemp = agents[i].Temperature;
        }
        return (int)maxTemp;
    }

    public int min()
    {
        float minTemp = agents[0].Temperature;
        for (int i = 1; i < num; i++)
        {
            if (agents[i].Temperature < minTemp)
                minTemp = agents[i].Temperature;
        }
        return (int)minTemp;
    }

    public void print()
    {

    }

    public void append(Drone val)
    {

    }

    public void appendFront(Drone val)
    {

    }

    public void insert(Drone val, int index)
    {

    }

    public void deleteFront()
    {

    }

    public void deleteBack()
    {

    }

    public void delete(int index)
    {
        
    }

    public void bubblesort()
    {
        for (int i = 0; i < num - 1; i++)
        {
            for (int j = 0; j < num - i - 1; j++)
            {
                if (agents[j].Temperature > agents[j + 1].Temperature)
                {
                    var temp = agents[j];
                    agents[j] = agents[j + 1];
                    agents[j + 1] = temp;
                }
            }
        }
    }

    public void insertionsort()
    {
       for (int i = 1; i < num; i++)
    {
        var key = agents[i];
        int j = i - 1;
        
        
        while (j >= 0)
        {
            
            if (agents[j].Temperature > key.Temperature)
            {
                agents[j + 1] = agents[j];
            }
            j--;
        }
        
        agents[j + 1] = key;  
    }
    }
}