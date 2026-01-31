using UnityEngine;

public class HandPointBinder : MonoBehaviour
{
    [SerializeField] private Transform handPoint;   // dein Hand Point
    [SerializeField] private Animator animator;
    [SerializeField] private bool useRightHand = true;

    private void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();

        if (!handPoint)
        {
            Debug.LogError("HandPointBinder: handPoint ist nicht gesetzt!");
            return;
        }

        if (!animator)
        {
            Debug.LogError("HandPointBinder: Animator nicht gefunden!");
            return;
        }

        Transform bone = animator.GetBoneTransform(useRightHand ? HumanBodyBones.RightHand : HumanBodyBones.LeftHand);

        if (bone == null)
        {
            Debug.LogError("HandPointBinder: Hand bone nicht gefunden. Ist das Rig Humanoid?");
            return;
        }

        handPoint.SetParent(bone, false);
        handPoint.localPosition = Vector3.zero;
        handPoint.localRotation = Quaternion.identity;
        handPoint.localScale = Vector3.one;
    }
}
