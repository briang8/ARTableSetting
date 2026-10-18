public class Napkin : TablewareItem
{
    public override bool IsOrientationValid() => true;

    public override string GetHint() => "The napkin goes to the left of the fork.";
}