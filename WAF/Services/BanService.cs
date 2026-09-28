using System.Collections.Concurrent;

namespace WAF.Services;

public sealed class BanService
{
    private static readonly TimeSpan AttackWindow = TimeSpan.FromMinutes(3);
    private static readonly TimeSpan BanDuration = TimeSpan.FromMinutes(1);
    private const int AttackLimit = 5;

    private readonly ConcurrentDictionary<string, ClientState> _clients = new();

    public BanStatus GetStatus(string ipAddress)
    {
        var now = DateTimeOffset.UtcNow;
        var client = _clients.GetOrAdd(ipAddress, _ => new ClientState());

        lock (client.SyncRoot)
        {
            RemoveExpiredAttempts(client, now);

            if (client.BannedUntil is not null && client.BannedUntil > now)
            {
                return CreateBannedStatus(client.BannedUntil.Value, now);
            }

            client.BannedUntil = null;
            return new BanStatus(false, 0, client.AttackTimestamps.Count);
        }
    }

    public BanStatus RegisterAttack(string ipAddress)
    {
        var now = DateTimeOffset.UtcNow;
        var client = _clients.GetOrAdd(ipAddress, _ => new ClientState());

        lock (client.SyncRoot)
        {
            RemoveExpiredAttempts(client, now);

            if (client.BannedUntil is not null && client.BannedUntil > now)
            {
                return CreateBannedStatus(client.BannedUntil.Value, now);
            }

            client.BannedUntil = null;
            client.AttackTimestamps.Enqueue(now);

            if (client.AttackTimestamps.Count >= AttackLimit)
            {
                client.BannedUntil = now.Add(BanDuration);
                return CreateBannedStatus(client.BannedUntil.Value, now);
            }

            return new BanStatus(false, 0, client.AttackTimestamps.Count);
        }
    }

    private static void RemoveExpiredAttempts(ClientState client, DateTimeOffset now)
    {
        while (client.AttackTimestamps.TryPeek(out var timestamp) &&
               now - timestamp >= AttackWindow)
        {
            client.AttackTimestamps.Dequeue();
        }
    }

    private static BanStatus CreateBannedStatus(DateTimeOffset bannedUntil, DateTimeOffset now)
    {
        var remainingSeconds = Math.Max(
            1,
            (int)Math.Ceiling((bannedUntil - now).TotalSeconds));

        return new BanStatus(true, remainingSeconds, 0);
    }

    private sealed class ClientState
    {
        public object SyncRoot { get; } = new();
        public Queue<DateTimeOffset> AttackTimestamps { get; } = new();
        public DateTimeOffset? BannedUntil { get; set; }
    }
}

public sealed record BanStatus(
    bool IsBanned,
    int RemainingBanSeconds,
    int AttackCount);