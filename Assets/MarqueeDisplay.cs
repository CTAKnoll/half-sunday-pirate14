using System;
using System.Linq;
using Services;
using TMPro;
using UnityEngine;
using Utils;

public class MarqueeDisplay : MonoBehaviour
{
    public TextMeshProUGUI MaruqeeTextbox;
    public TextMeshProUGUI RumoursText;
    public int WindowSize = 60;
    public float Speed = 0.03f;
    

    private float start = 0;
    private int lastStart = 0;
    
    private TextServer TextServer;

    private char twoBeforeChar;
    private char lastChar;
    private char currentChar;
    private float STOP;

    void Start()
    {
        ServiceLocator.LazyLoad<Timeline>().AddRecurring(this, CreateNewRumour, TimeSpan.FromDays(90));
        if (TextServer == null)
            ServiceLocator.TryGetService(out TextServer);

        CreateNewRumour();
    }

    private void CreateNewRumour()
    {
        if (TextServer == null)
            ServiceLocator.TryGetService(out TextServer);

        RumoursText.text = $"{TextServer.DeliverRumour()}";
    }
    
    void FixedUpdate()
    {
        if (TextServer == null)
            ServiceLocator.TryGetService(out TextServer);
        
        if (STOP == 0)
        {
            lastStart = (int)start;
            start += Speed;

            if ((int)start != lastStart)
            {
                MaruqeeTextbox.text = TextServer.DeliverMarquee((int) start, WindowSize);
                twoBeforeChar = lastChar;
                lastChar = currentChar;
                currentChar = MaruqeeTextbox.text.Last();
                
                if (STOP == 0 && twoBeforeChar != ' ' && lastChar == ' ' && currentChar == ' ')
                {
                    STOP = 20;
                }
            }
        }
        else
        {
            STOP -= Speed;
            if (STOP < 0)
                STOP = 0;
        }
        
    }
}
