using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
  public void NotifyAncestors(string message)
   {
        SendMessageUpwards(message);
   }
}
