using Domain.Entities.Dtos;

namespace Infra.Security.Services;

public interface IJwtService
{
    string CreateToken(UserDto user);
}