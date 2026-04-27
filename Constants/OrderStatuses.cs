namespace Bake_Link.Constants;

public static class OrderStatuses
{
    public const string Pending = "pending";
    public const string Confirmed = "confirmed";
    public const string Preparing = "preparing";
    public const string Shipped = "shipped";
    public const string Delivered = "delivered";
    public const string Cancelled = "cancelled";

    public static readonly string[] All =
    [
        Pending,
        Confirmed,
        Preparing,
        Shipped,
        Delivered,
        Cancelled
    ];
}
