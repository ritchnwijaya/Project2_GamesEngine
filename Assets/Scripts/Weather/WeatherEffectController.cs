using System.Collections;
using UnityEngine;

public class WeatherEffectController : MonoBehaviour
{
    [SerializeField]
    GameObject rain,snow;
    Transform player;
    private void Start()
    {
        player = FindFirstObjectByType<PlayerMovement>().transform;
        LoadParticle();
    }


    private void FixedUpdate()
    {
        //Move the particle to the player
        if (player != null)
        {
            Vector3 pos = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.position = Vector3.Lerp(transform.position, pos, Time.fixedDeltaTime);
        }
    }

    public void LoadParticle()
    {
        //Disable everything
        rain.SetActive(false);
        snow.SetActive(false);

        //Move the particle to the player
        if (player != null)
        {
            Vector3 pos = new(player.position.x, transform.position.y, player.position.z);
            transform.position = pos; 
        }


        //Check if indoor
        if (SceneTransitionManager.Instance.CurrentlyIndoor())
        {
            return; 
        }

        if(WeatherManager.Instance.WeatherToday == WeatherData.WeatherType.Rain)
        {
            //Rain
            rain.SetActive(true); 
        }

        if (WeatherManager.Instance.WeatherToday == WeatherData.WeatherType.Snow)
        {
            snow.SetActive(true);
        }
    }

}
