namespace Application.Authentication;

public interface IJwtProvider
{
    string GenerateToken(string email);
}
