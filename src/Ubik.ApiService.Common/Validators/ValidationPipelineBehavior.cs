using FluentValidation;
using MediatR;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.ApiService.Common.Validators
{
    public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        private readonly IValidator<TRequest> _validator;

        public ValidationPipelineBehavior(IValidator<TRequest> validator)
        {
            _validator = validator;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request);

            return !validationResult.IsValid
                ? throw new CustomValidationException(validationResult.Errors.Select(e=>new CustomError()
                {
                    ErrorCode="VALIDATION_ERROR",
                    ErrorFriendlyMessage=e.ErrorMessage,
                    ErrorValueDetails = $"Field:{e.PropertyName} / Value:{e.AttemptedValue ?? string.Empty}"
                }).ToList())
                : await next();
        }
    }
}
