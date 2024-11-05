using Domain.Users;

namespace Api.Dtos;

public record TokenGenerationRequest(
    string Login,
    string Password);