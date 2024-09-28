# Nixie

A Lightweight Actor Model Implementation for C#/.NET 

[![Run Tests](https://github.com/andresgutierrez/nixie/actions/workflows/run-tests.yml/badge.svg)](https://github.com/andresgutierrez/nixie/actions/workflows/run-tests.yml)
[![NuGet](https://img.shields.io/nuget/v/Nixie.svg?style=flat-square)](https://www.nuget.org/packages/Nixie)
[![Nuget](https://img.shields.io/nuget/dt/Nixie)](https://www.nuget.org/packages/Nixie)

## Overview

Nixie is a lightweight, high-performance implementation of the [actor model](https://en.wikipedia.org/wiki/Actor_model) tailored for the latest versions of C#/.NET. Developed with a focus on type safety, Nixie provides strongly-typed actors and takes full advantage of nullable support, thereby promoting a reduced error-prone codebase and bolstering performance. Built atop the [Task Parallel Library (TPL)](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/task-parallel-library-tpl) provided by .NET, it manages the lifecycle of actors diligently, ensuring a seamless and efficient concurrent programming experience.

### Advantages of using Nixie Actor/Model vs. Traditional Threads Programming

1. **Simplified Concurrency Model:** Nixie abstracts away low-level thread management, providing a higher-level concurrency model that is easier to reason about and manage.
2. **Improved Scalability:** Actor systems can efficiently scale across multiple CPU cores and even multiple machines, whereas managing threads for scalability can be complex and error-prone.
3. **Enhanced Fault Tolerance:** Nixie include built-in fault tolerance mechanisms, such as supervision strategies, which help to recover from failures gracefully.
4. **Isolation and Encapsulation:** Each actor encapsulates its state and behavior, reducing the risk of shared state conflicts and making the system more modular and easier to maintain.
5. **Simplified Error Handling:** Actors can handle errors locally and propagate them in a controlled manner, improving the robustness and reliability of the application.
6. **Natural Asynchronous Programming:** Actor models promote asynchronous message passing, which can lead to more responsive applications compared to blocking thread-based models.
7. **Reduced Complexity:** Traditional thread programming can be complex due to the need for synchronization primitives (locks, semaphores, etc.), whereas actor models eliminate the need for these by design.
8. **Concurrency Safety:** The message-passing nature of actor models inherently avoids many concurrency issues, such as deadlocks and race conditions, which are common in traditional threading.
9. **Event-Driven Architecture:** Actor models align well with event-driven architectures, enabling more reactive and event-driven design patterns that are suited for modern applications.
10. **Easier Maintenance:** With clear boundaries and less shared state, actor-based systems are generally easier to maintain and evolve over time compared to traditional thread-based systems.

## Features of Nixie

- **Strongly-Typed Actors:** Ensuring that your actor interactions are type-safe and as per expectations. High use of [generics](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/generics) to ensure that errors related to type mismatches are caught at compile time rather than at runtime. This reduces the risk of runtime exceptions and makes the code more robust.
- **Nullable Support:** Full support for [nullability](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/nullable-reference-types) in line with the latest C# features, ensuring your code is robust and safeguarded against null reference issues.
- **Lifecycle Management:** Nixie handles the meticulous management of actor lifecycle, allowing developers to focus on implementing logic.
- **High Performance:** Thanks to being lightweight and leveraging the powerful TPL, Nixie ensures that your actor systems are both scalable and performant. Additionally the use of Generics eliminate the need for boxing and unboxing when working with value types, which can improve performance (Boxing is the process of converting a value type to an object type, and unboxing is the reverse).
- **Less Error Prone:** The strongly-typed nature and nullability checks inherently make your actor system more reliable and resilient.
- **Built on TPL:** Make the most out of the robust, scalable, and performant asynchronous programming features offered by TPL.
- **Multi-Threading:** To increase throughput, Nixie makes use of thread-safe structures that avoid locks wherever possible and use fine-grained locking where locks are necessary. Abstracting the complexities of multithreaded programming into an API that is easy to use and understand.
- **Production-Ready:** Nixie has been tested in production environments with many concurrent users, ensuring reliability and stability under real-world conditions.

## Getting Started

### Prerequisites

- .NET SDK 8.0 or later
- A suitable IDE (e.g., Visual Studio, Visual Studio Code, or Rider)

### Installation

To install Nixie into your C#/.NET project, you can use the .NET CLI or the NuGet Package Manager.

#### Using .NET CLI

```shell
dotnet add package Nixie --version 1.0.9
```

### Using NuGet Package Manager

Search for Nixie and install it from the NuGet package manager UI, or use the Package Manager Console:

```shell
Install-Package Nixie -Version 1.0.9
```

## Usage

Here's a basic example to get you started with Nixie. More comprehensive documentation and usage examples can be found in the /docs folder:


```csharp
using Nixie;

public class GreetMessage
{
    public string Greeting { get; }

    public GreetMessage(string greeting)
    {
        Greeting = greeting;
    }
}

public class GreeterActor : IActor<GreetMessage>
{    
    public GreeterActor(IActorContext<GreeterActor, GreetMessage> _)
    {

    }

    public async Task Receive(GreetMessage message)
    {
        Console.WriteLine("Message: {0}", message.Greeting);
    }
}

var system = new ActorSystem();

var greeter = system.Spawn<GreeterActor, GreetMessage>();

greeter.Send(new GreetMessage("Hello, Nixie!"));
```

## Contribution

Nixie is an open-source project, and contributions are heartily welcomed! Whether you are looking to fix bugs, add new features, or improve documentation, your efforts and contributions will be appreciated. Check out the [CONTRIBUTING](CONTRIBUTING.md) file for guidelines on how to get started with contributing to Nixie.

## License

Nixie is released under the MIT License.

## Name origin

[Nixies](https://en.wikipedia.org/wiki/Nixie_(folklore)) are mysterious shapeshifting water spirits in Germanic mythology and folklore. 

## Acknowledgements

Sincere thanks to all contributors and the C#/.NET community for the continual support and inspiration.

---

Let's build robust and efficient actor systems with Nixie! 🚀
