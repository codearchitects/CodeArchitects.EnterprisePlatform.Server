using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Messaging;

internal delegate Task HandlerDelegate(HttpContext context, JObject messageJson);
