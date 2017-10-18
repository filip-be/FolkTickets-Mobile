using System.Runtime.Serialization;

namespace SharpCommerce.Data.Orders
{
    public enum OrderStatus
    {
        [EnumMember(Value = "pending")]
        Pending,

        [EnumMember(Value = "processing")]
        Processing,

        [EnumMember(Value = "on-hold")]
        OnHold,

        [EnumMember(Value = "completed")]
        Completed,

        [EnumMember(Value = "cancelled")]
        Cancelled,

        [EnumMember(Value = "refunded")]
        Refunded,

        [EnumMember(Value = "failed")]
        Failed
    }
}
