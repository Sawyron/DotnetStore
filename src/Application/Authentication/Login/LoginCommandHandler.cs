using MediatR;

namespace Application.Authentication.Login;

internal class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IJwtProvider _jwtProvider;

    public LoginCommandHandler(IJwtProvider jwtProvider)
    {
        _jwtProvider = jwtProvider;
    }

    public Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(
            new LoginResponse(_jwtProvider.GenerateToken(request.Email)));
    }
}
