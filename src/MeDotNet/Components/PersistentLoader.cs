using Microsoft.AspNetCore.Components;

namespace MeDotNet.Components;

/// <summary>
/// Loads data once across the prerender and interactive passes of a component.
/// On prerender: runs the loader and registers the result for persistence.
/// On the interactive pass: takes the persisted value instead of re-querying.
/// A null loaded value is not persisted usefully, so not-found lookups query twice;
/// that only affects 404 paths.
/// </summary>
public sealed class PersistentLoader(PersistentComponentState state) : IDisposable
{
    private readonly List<PersistingComponentStateSubscription> _subscriptions = [];

    public async Task<T> LoadAsync<T>(string key, Func<Task<T>> loader)
    {
        if (state.TryTakeFromJson<T>(key, out var persisted) && persisted is not null)
            return persisted;

        var data = await loader();
        _subscriptions.Add(state.RegisterOnPersisting(() =>
        {
            state.PersistAsJson(key, data);
            return Task.CompletedTask;
        }));
        return data;
    }

    public void Dispose()
    {
        foreach (var subscription in _subscriptions)
            subscription.Dispose();
    }
}
