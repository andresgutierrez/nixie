﻿
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Nixie;

/// <summary>
/// This class maintains an inventory of created actors of a specific type, as well as references to the runners that contain the mailboxes.
/// </summary>
/// <typeparam name="TActor"></typeparam>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public sealed class ActorRepositoryStruct<TActor, TRequest, TResponse> : IActorRepositoryRunnable
    where TActor : IActorStruct<TRequest, TResponse> where TRequest : struct where TResponse : struct
{
    private readonly ActorSystem actorSystem;

    private readonly IServiceProvider? serviceProvider;

    private readonly ILogger? logger;

    private readonly ConcurrentDictionary<string, Lazy<(ActorRunnerStruct<TActor, TRequest, TResponse>, ActorRefStruct<TActor, TRequest, TResponse>)>> actors = new();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="actorSystem"></param>
    /// <param name="serviceProvider"></param>
    /// <param name="logger"></param>
    public ActorRepositoryStruct(ActorSystem actorSystem, IServiceProvider? serviceProvider, ILogger? logger)
    {
        this.actorSystem = actorSystem;
        this.serviceProvider = serviceProvider;
        this.logger = logger;
    }

    /// <summary>
    /// Check if any of the actors have pending messages
    /// </summary>
    /// <returns></returns>
    public bool HasPendingMessages(out string? actorName)
    {
        foreach (KeyValuePair<string, Lazy<(ActorRunnerStruct<TActor, TRequest, TResponse> runner, ActorRefStruct<TActor, TRequest, TResponse> actorRef)>> actor in actors)
        {
            Lazy<(ActorRunnerStruct<TActor, TRequest, TResponse> runner, ActorRefStruct<TActor, TRequest, TResponse> actorRef)> lazyValue = actor.Value;

            if (lazyValue.IsValueCreated)
            {
                ActorRunnerStruct<TActor, TRequest, TResponse> runner = lazyValue.Value.runner;

                if (!runner.IsShutdown && !lazyValue.Value.runner.IsEmpty)
                {
                    actorName = runner.Name;
                    return true;
                }
            }
        }

        actorName = null;
        return false;
    }

    /// <summary>
    /// Check if any of the actors are processing messages
    /// </summary>
    /// <returns></returns>
    public bool IsProcessing(out string? actorName)
    {
        foreach (KeyValuePair<string, Lazy<(ActorRunnerStruct<TActor, TRequest, TResponse> runner, ActorRefStruct<TActor, TRequest, TResponse> actorRef)>> actor in actors)
        {
            Lazy<(ActorRunnerStruct<TActor, TRequest, TResponse> runner, ActorRefStruct<TActor, TRequest, TResponse> actorRef)> lazyValue = actor.Value;

            if (lazyValue.IsValueCreated)
            {
                ActorRunnerStruct<TActor, TRequest, TResponse> runner = lazyValue.Value.runner;

                if (!runner.IsShutdown && runner.IsProcessing)
                {
                    actorName = runner.Name;
                    return true;
                }
            }
        }

        actorName = null;
        return false;
    }

    /// <summary>
    /// Creates a new actor and returns a reference to it
    /// </summary>
    /// <param name="name"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    /// <exception cref="NixieException"></exception>
    public IActorRefStruct<TActor, TRequest, TResponse> Spawn(string? name = null, params object[]? args)
    {
        if (!string.IsNullOrEmpty(name))
        {
            name = name.ToLowerInvariant();

            if (actors.ContainsKey(name))
                throw new NixieException("Actor already exists");
        }
        else
        {
            name = Guid.NewGuid().ToString();
        }

        Lazy<(ActorRunnerStruct<TActor, TRequest, TResponse> runner, ActorRefStruct<TActor, TRequest, TResponse> actorRef)> actor = actors.GetOrAdd(
            name,
            (string name) => new Lazy<(ActorRunnerStruct<TActor, TRequest, TResponse>, ActorRefStruct<TActor, TRequest, TResponse>)>(() => CreateInternal(name, args))
        );

        return actor.Value.actorRef;
    }

    private (ActorRunnerStruct<TActor, TRequest, TResponse>, ActorRefStruct<TActor, TRequest, TResponse>) CreateInternal(string name, params object[]? args)
    {
        ActorRunnerStruct<TActor, TRequest, TResponse> runner = new(actorSystem, logger, name);

        ActorRefStruct<TActor, TRequest, TResponse>? actorRef = (ActorRefStruct<TActor, TRequest, TResponse>?)Activator.CreateInstance(typeof(ActorRefStruct<TActor, TRequest, TResponse>), runner);

        if (actorRef is null)
            throw new NixieException("Invalid props");

        ActorContextStruct<TActor, TRequest, TResponse> actorContext = new(actorSystem, logger, actorRef);

        TActor? actor;

        if (args is not null && args.Length > 0)
        {
            object[] arguments = new object[args.Length + 1];

            arguments[0] = actorContext;

            for (int i = 0; i < args.Length; i++)
                arguments[i + 1] = args[i];

            if (serviceProvider is not null)
                actor = (TActor?)ActivatorUtilities.CreateInstance(serviceProvider, typeof(TActor), arguments);
            else
                actor = (TActor?)Activator.CreateInstance(typeof(TActor), arguments);
        }
        else
        {
            if (serviceProvider is not null)
                actor = (TActor?)ActivatorUtilities.CreateInstance(serviceProvider, typeof(TActor), actorContext);
            else
                actor = (TActor?)Activator.CreateInstance(typeof(TActor), actorContext);
        }

        if (actor is null)
            throw new NixieException("Invalid actor");

        runner.ActorContext = actorContext;
        runner.Actor = actor;

        return (runner, actorRef);
    }

    /// <summary>
    /// Returns a reference to an existing actor
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IActorRefStruct<TActor, TRequest, TResponse>? Get(string name)
    {
        name = name.ToLowerInvariant();

        if (actors.TryGetValue(name, out Lazy<(ActorRunnerStruct<TActor, TRequest, TResponse> runner, ActorRefStruct<TActor, TRequest, TResponse> actorRef)>? actor))
            return actor.Value.actorRef;

        return null;
    }

    /// <summary>
    /// Shutdowns an actor by its name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Shutdown(string name)
    {
        name = name.ToLowerInvariant();

        if (actors.TryGetValue(name, out Lazy<(ActorRunnerStruct<TActor, TRequest, TResponse> runner, ActorRefStruct<TActor, TRequest, TResponse> actorRef)>? actor))
        {
            if (actor.Value.runner.Shutdown())
            {
                actors.TryRemove(name, out _);
                return true;
            }
        }

        return true;
    }

    /// <summary>
    /// Shutdowns an actor by its reference
    /// </summary>
    /// <param name="actorRef"></param>
    /// <returns></returns>
    public bool Shutdown(IActorRefStruct<TActor, TRequest, TResponse> actorRef)
    {
        string name = actorRef.Runner.Name;

        if (actors.TryGetValue(name, out Lazy<(ActorRunnerStruct<TActor, TRequest, TResponse> runner, ActorRefStruct<TActor, TRequest, TResponse> actorRef)>? actor))
        {
            if (actor.Value.runner.Shutdown())
            {
                actors.TryRemove(name, out _);
                return true;
            }
        }

        return true;
    }

    /// <summary>
    /// Tries to shutdown an actor by its name and returns a task whose result confirms shutdown within the specified timespan
    /// </summary>
    /// <param name="actorRef"></param>
    /// <param name="maxWait"></param>
    /// <returns></returns>
    public async Task<bool> GracefulShutdown(IActorRefStruct<TActor, TRequest, TResponse> actorRef, TimeSpan maxWait)
    {
        string name = actorRef.Runner.Name;

        if (actors.TryGetValue(name, out Lazy<(ActorRunnerStruct<TActor, TRequest, TResponse> runner, ActorRefStruct<TActor, TRequest, TResponse> actorRef)>? actor))
        {
            bool success = await actor.Value.runner.GracefulShutdown(maxWait);
            actors.TryRemove(name, out _);
            actorSystem.StopAllTimers(actor.Value.actorRef);
            return success;
        }

        return true;
    }
}
