
namespace Nixie.Routers;

public class BalancingActor<TActor, TRequest> : IActor<TRequest>
    where TActor : IActor<TRequest> where TRequest : class
{
    /// <summary>
    /// Router actor context
    /// </summary>
    private readonly IActorContext<BalancingActor<TActor, TRequest>, TRequest> context;

    /// <summary>
    /// Instances to send messages to
    /// </summary>
    private readonly List<IActorRef<TActor, TRequest>> instances = [];

    /// <summary>
    /// Returns the current list of instances
    /// </summary>
    public List<IActorRef<TActor, TRequest>> Instances => instances;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context"></param>
    /// <param name="numberInstances"></param>
    public BalancingActor(
        IActorContext<BalancingActor<TActor, TRequest>, TRequest> context, 
        int numberInstances
    )
    {
        this.context = context;

        for (int i = 0; i < numberInstances; i++)
            instances.Add(context.ActorSystem.Spawn<TActor, TRequest>());
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context"></param>
    /// <param name="instances"></param>
    public BalancingActor(
        IActorContext<BalancingActor<TActor, TRequest>, TRequest> context, 
        List<IActorRef<TActor, TRequest>> instances
    )
    {
        this.context = context;
        this.instances = instances;
    }

    /// <summary>
    /// Receives a message that must be routed to the least leaded routee
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public Task Receive(TRequest message)
    {
        // Step 1. Find a router that is not processing messages
        foreach (IActorRef<TActor, TRequest> instance in instances)
        {
            if (instance.Runner.IsProcessing)
                continue;
            
            instance.Send(message);
            return Task.CompletedTask;
        }
        
        // Step 2. Find a router where is queue is empty (next to be free)
        foreach (IActorRef<TActor, TRequest> instance in instances)
        {
            if (!instance.Runner.IsEmpty)
                continue;
            
            instance.Send(message);
            return Task.CompletedTask;
        }
        
        // Step 3. Find a router with the least number of queued messages
        IActorRef<TActor, TRequest> leastLoaded = instances
            .OrderBy(q => q.Runner.MessageCount)
            .First();
        
        leastLoaded.Send(message);

        return Task.CompletedTask;
    }
}