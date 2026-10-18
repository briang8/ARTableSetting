using UnityEngine;

public enum CutleryType { Fork, Knife, Spoon }

public class Cutlery : TablewareItem
{
    [SerializeField] private CutleryType cutleryType;

    public override bool IsOrientationValid()
    {
        if (targetSlot == null) return false;
        float diff = Mathf.DeltaAngle(transform.eulerAngles.y, targetSlot.ExpectedYaw);
        return Mathf.Abs(diff) < 1f;
    }

    public override string GetHint()
    {
        switch (cutleryType)
        {
            case CutleryType.Fork:
                return "The fork goes to the left of the plate.";
            case CutleryType.Knife:
                return "The knife goes to the right of the plate, blade facing the plate.";
            default:
                return "The spoon goes to the right of the knife.";
        }
    }
}