namespace Bake_Link.Constants;

public static class DeliveryStatuses
{
    public const string Waiting = "waiting";
    public const string Picked = "picked";
    public const string InTransit = "in_transit";
    public const string Delivered = "delivered";

    public static readonly string[] ActiveFlow =
    [
        Picked,
        InTransit,
        Delivered
    ];
}
