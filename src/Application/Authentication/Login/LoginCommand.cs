using MediatR;

namespace Application.Authentication.Login;

public sealed record LoginCommand(string Email) : IRequest<LoginResponse>;
