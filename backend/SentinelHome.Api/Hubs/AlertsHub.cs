using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SentinelHome.Api.Hubs;

[Authorize]
public sealed class AlertsHub : Hub;
