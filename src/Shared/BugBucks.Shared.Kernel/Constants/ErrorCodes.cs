namespace BugBucks.Shared.Kernel.Constants;

/// <summary>
///     Centralized application error codes, grouped by category.
///     Format: {category}:{specific_error}
/// </summary>
public static class ErrorCodes
{
    // General errors
    public const string UnknownError = "kernel:unknown_error";
    public const string ValidationError = "kernel:validation_error";
    public const string NotFound = "kernel:not_found";
    public const string Conflict = "kernel:conflict";
    public const string Unauthorized = "kernel:unauthorized";
    public const string Forbidden = "kernel:forbidden";
    public const string Timeout = "kernel:timeout";

    // Authentication / Identity
    public const string InvalidCredentials = "auth:invalid_credentials";
    public const string TokenExpired = "auth:token_expired";
    public const string TokenInvalid = "auth:token_invalid";
    public const string UserAlreadyExists = "auth:user_already_exists";
    public const string UserNotFound = "auth:user_not_found";

    // Order-specific
    public const string OrderNotCreated = "order:not_created";
    public const string OrderAlreadyExists = "order:already_exists";
    public const string OrderOutOfStock = "order:out_of_stock";
    public const string OrderAlreadyPaid = "order:already_paid";
    public const string OrderCancellationFailed = "order:cancellation_failed";

    // Payment-specific
    public const string PaymentDeclined = "payment:declined";
    public const string PaymentGatewayTimeout = "payment:gateway_timeout";
    public const string InsufficientFunds = "payment:insufficient_funds";
    public const string CurrencyMismatch = "payment:currency_mismatch";
    public const string PaymentProcessingError = "payment:processing_error";

    // Checkout / Cart
    public const string CheckoutFailed = "checkout:failed";
    public const string CartEmpty = "checkout:cart_empty";
    public const string CartItemInvalid = "checkout:item_invalid";

    // Inventory
    public const string StockUnavailable = "inventory:stock_unavailable";
    public const string StockReservationFailed = "inventory:reservation_failed";

    // Shipping
    public const string ShippingAddressInvalid = "shipping:address_invalid";
    public const string ShippingCalculationFailed = "shipping:calculation_failed";
    public const string ShippingLabelFailed = "shipping:label_failed";

    // Discount / Promotion
    public const string DiscountNotFound = "discount:not_found";
    public const string DiscountExpired = "discount:expired";
    public const string DiscountApplicationError = "discount:application_error";

    // Invoice / Billing
    public const string InvoiceGenerationFailed = "invoice:generation_failed";
    public const string TaxCalculationFailed = "invoice:tax_calculation_failed";

    // Messaging / Eventing
    public const string MessagingPublishFailed = "messaging:publish_failed";
    public const string MessagingConsumeFailed = "messaging:consume_failed";
    public const string MessagingConnectionError = "messaging:connection_error";
}