public class Plate : TablewareItem
{
    public override bool IsOrientationValid() => true;

    public override string GetHint() => "The plate goes in the center of the place setting.";
}