namespace GoodVibesCitadelBackend.Models;

public record UpdatePasswordRequest(string Username, string OldPassword, string NewPassword);