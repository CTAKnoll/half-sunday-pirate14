using System.Collections;
using Services;
using Stonks;
using TMPro;
using UnityEngine;

public class EndScreen : MonoBehaviour
{
    public float WaitPeriod;
    public string DukeText;
    public string NotDukeText;
    
    public TextMeshProUGUI GameOverText;
    public TextMeshProUGUI DukeOrGardenText;
    public TextMeshProUGUI ThanksForPlayingText;

    public GameOverMoney MoneyElement;
    
    public GameOverTulip Tulip1;
    public GameOverTulip Tulip2;
    public GameOverTulip Tulip3;
    public GameOverTulip Tulip4;
    public GameOverTulip Tulip5;
    public GameOverTulip Tulip6;

    [Header("SFX")]
    public AudioEvent sfx_smallImpact;
    public AudioEvent sfx_bigImpact;
    public AudioEvent sfx_funnyImpact;

    private Fadeout Fadeout;
    private AudioService Audio;
    private WaitForSeconds WaitYield;

    void Start()
    {
        ServiceLocator.TryGetService(out Audio);
        ServiceLocator.TryGetService(out Fadeout);
        WaitYield = new WaitForSeconds(WaitPeriod);
        Fadeout.EndTheWorld += StartEndScreen;
    }

    private void StartEndScreen()
    {
        MoneyElement.Initialize();
        
        if(Tulip1 != null)
            Tulip1.Initialize(0);
        
        if(Tulip2 != null)
            Tulip2.Initialize(1);
        
        if(Tulip3 != null)
            Tulip3.Initialize(2);
        
        if(Tulip4 != null)
            Tulip4.Initialize(3);
        
        if(Tulip5 != null)
            Tulip5.Initialize(4);
        
        if(Tulip6 != null)
            Tulip6.Initialize(5);
        
        Debug.Log("HOI");
        
        StartCoroutine(EndScreenCoroutine());
    }

    private IEnumerator EndScreenCoroutine()
    {
        GameOverText.gameObject.SetActive(true);
        yield return WaitYield;
        
        MoneyElement.gameObject.SetActive(true);
        yield return WaitYield;

        if (Tulip1 != null && Tulip1.HasElement)
        {
            Tulip1.gameObject.SetActive(true);
            Audio.PlayOneShot(sfx_smallImpact);
            yield return WaitYield;
        }
        
        if (Tulip2 != null && Tulip2.HasElement)
        {
            Tulip2.gameObject.SetActive(true);
            Audio.PlayOneShot(sfx_smallImpact);
            yield return WaitYield;
        }
        
        if (Tulip3 != null && Tulip3.HasElement)
        {
            Tulip3.gameObject.SetActive(true);
            Audio.PlayOneShot(sfx_smallImpact);
            yield return WaitYield;
        }
        
        if (Tulip4 != null && Tulip4.HasElement)
        {
            Tulip4.gameObject.SetActive(true);
            Audio.PlayOneShot(sfx_smallImpact);
            yield return WaitYield;
        }
        
        if (Tulip5 != null && Tulip5.HasElement)
        {
            Tulip5.gameObject.SetActive(true);
            Audio.PlayOneShot(sfx_smallImpact);
            yield return WaitYield;
        }
        
        if (Tulip6 != null && Tulip6.HasElement)
        {
            Tulip6.gameObject.SetActive(true);
            Audio.PlayOneShot(sfx_smallImpact);
            yield return WaitYield;
        }

        DukeOrGardenText.text = ServiceLocator.LazyLoad<Economy>().IsDuke ? DukeText : NotDukeText;
        DukeOrGardenText.gameObject.SetActive(true);
        Audio.PlayOneShot(sfx_bigImpact);
        yield return WaitYield;
        
        ThanksForPlayingText.gameObject.SetActive(true);
        Audio.PlayOneShot(sfx_funnyImpact);
    }

 
}
