using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OpenVision.Shared;
using OpenVision.Shared.Responses;
using Error = OpenVision.Shared.Responses.Error;

namespace OpenVision.Server.Core.Filters;

/// <summary>
/// Action filter to validate the model state before executing an action.
/// </summary>
public class ValidateModelFilter : ActionFilterAttribute
{
    #region Methods

    /// <summary>
    /// Called before the action result executes. Checks if the model state is valid.
    /// If the model state is invalid, it sets the context result to a BadRequestObjectResult with validation errors.
    /// </summary>
    /// <param name="context">The ResultExecutingContext.</param>
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        base.OnResultExecuting(context);

        if (!context.ModelState.IsValid)
        {
            var errorCollection = new List<Error>();
            var errors = context.ModelState.Where(state => state.Value is not null && state.Value.ValidationState == ModelValidationState.Invalid)
                                           .Select(state => GetValidationError(state!));

            errorCollection.AddRange(errors);

            var response = new ResponseMessage(Guid.NewGuid(), StatusCode.Failed, errorCollection);
            context.Result = new BadRequestObjectResult(response);
        }
    }

    /// <summary>
    /// Generates a validation error from a given model state entry.
    /// </summary>
    /// <param name="state">The KeyValuePair containing the property name and model state entry.</param>
    /// <returns>An Error object containing the validation error details.</returns>
    private static Error GetValidationError(KeyValuePair<string, ModelStateEntry> state)
    {
        var propertyName = state.Key;
        var errorMessages = string.Join(Environment.NewLine, state.Value.Errors.Select(error => error.ErrorMessage));
        var message = string.IsNullOrEmpty(errorMessages) ? "invalid body." : $"{errorMessages}";
        return new Error(ResultCode.ValidationError, message);
    }

    #endregion
}