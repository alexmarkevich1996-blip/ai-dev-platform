namespace Platform.Host.Endpoints;

/// <summary>What a Company Administrator or Employee sends to start a session.</summary>
public sealed record SignInRequest(string Email, string Password);
