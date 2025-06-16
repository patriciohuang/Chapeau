using System.Text.Json.Serialization;

namespace Chapeau.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentMethod
    {
        Cash,
        CreditCard,
        DebitCard,
        GiftCard
    }
}