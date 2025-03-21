using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenVision.Client.Core.Helpers;

namespace OpenVision.Client.Core.Controllers;

/// <summary>
/// Base controller for other controllers in the Vision.Portal application. Provides methods to handle notifications and logging.
/// </summary>
public class BaseController : Controller
{
    #region Fields/Consts

    private readonly ILogger<BaseController> _logger;

    #endregion

    /// <summary>
    /// Base controller for all controllers in the application. Provides methods for creating notifications and logging.
    /// </summary>
    /// <param name="logger">The logger instance used for logging in the controller.</param>
    public BaseController(ILogger<BaseController> logger)
    {
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Adds a success notification with the given message and title.
    /// </summary>
    /// <param name="message">The message of the notification.</param>
    /// <param name="title">The title of the notification. (optional)</param>
    protected void SuccessNotification(string message, string title = "")
    {
        CreateNotification(NotificationHelpers.AlertType.Success, message, title);
    }

    /// <summary>
    /// Adds an error notification with the given message and title.
    /// </summary>
    /// <param name="message">The message of the notification.</param>
    /// <param name="title">The title of the notification. (optional)</param>
    protected void ErrorNotification(string message, string title = "")
    {
        CreateNotification(NotificationHelpers.AlertType.Error, message, title);
    }

    /// <summary>
    /// Creates a new notification with the given type, message and title, and adds it to the list of notifications.
    /// </summary>
    /// <param name="type">The type of the notification (success or error).</param>
    /// <param name="message">The message of the notification.</param>
    /// <param name="title">The title of the notification. (optional)</param>
    protected void CreateNotification(NotificationHelpers.AlertType type, string message, string title = "")
    {
        var toast = new NotificationHelpers.Alert
        {
            Type = type,
            Message = message,
            Title = title
        };

        var alerts = new List<NotificationHelpers.Alert>();

        if (TempData.TryGetValue(NotificationHelpers.NotificationKey, out object? value))
        {
            alerts = JsonConvert.DeserializeObject<List<NotificationHelpers.Alert>>(value?.ToString());
            TempData.Remove(NotificationHelpers.NotificationKey);
        }

        alerts.Add(toast);

        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        var alertJson = JsonConvert.SerializeObject(alerts, settings);

        TempData.Add(NotificationHelpers.NotificationKey, alertJson);
    }

    /// <summary>
    /// Reads and generates the notifications stored in TempData and sets the result to ViewBag.Notifications.
    /// </summary>
    protected void GenerateNotifications()
    {
        if (!TempData.ContainsKey(NotificationHelpers.NotificationKey)) return;
        ViewBag.Notifications = TempData[NotificationHelpers.NotificationKey];
        TempData.Remove(NotificationHelpers.NotificationKey);
    }

    /// <summary>
    /// Reads and generates the notifications stored in TempData before returning the view.
    /// </summary>
    public override ViewResult View(object? model)
    {
        GenerateNotifications();

        return base.View(model);
    }

    /// <summary>
    /// Reads and generates the notifications stored in TempData before executing an action.
    /// </summary>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        GenerateNotifications();

        base.OnActionExecuting(context);
    }

    #endregion
}