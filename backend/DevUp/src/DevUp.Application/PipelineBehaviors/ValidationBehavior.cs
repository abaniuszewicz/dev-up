﻿using FluentValidation;
using MediatR;
using domain = DevUp.Domain.Seedwork.Exceptions;

namespace DevUp.Application.PipelineBehaviors
{
    internal sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var context = new ValidationContext<TRequest>(request);
            var errors = _validators
                .Select(v => v.Validate(context))
                .SelectMany(v => v.Errors)
                .Select(vf => vf.ErrorMessage)
                .ToArray();

            if (errors.Any())
                throw new domain.ValidationException(errors);

            return next();
        }
    }
}
