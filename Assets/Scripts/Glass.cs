public class Glass : TablewareItem
{
    public override bool IsOrientationValid() => true;

    public override string GetHint() => "The glass goes above the knife, towards the upper right of the plate.";
}