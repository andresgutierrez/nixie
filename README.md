# Nixie

A Lightweight Actor Model Implementation for C#/.NET [![Run Tests](https://github.com/andresgutierrez/nixie/actions/workflows/run-tests.yml/badge.svg)](https://github.com/andresgutierrez/nixie/actions/workflows/run-tests.yml)

## Overview

Nixie is a lightweight, high-performance implementation of the [actor model](https://en.wikipedia.org/wiki/Actor_model) tailored for the latest versions of C#/.NET. Developed with a focus on type safety, Nixie provides strongly-typed actors and takes full advantage of nullable support, thereby promoting a reduced error-prone codebase and bolstering performance. Built atop the [Task Parallel Library (TPL)](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/task-parallel-library-tpl) provided by .NET, it manages the lifecycle of actors diligently, ensuring a seamless and efficient concurrent programming experience.

## Features

- **Strongly-Typed Actors:** Ensuring that your actor interactions are type-safe and as per expectations. High use of [generics](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/generics) to ensure that errors related to type mismatches are caught at compile time rather than at runtime. This reduces the risk of runtime exceptions and makes the code more robust.
- **Nullable Support:** Full support for [nullability](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/nullable-reference-types) in line with the latest C# features, ensuring your code is robust and safeguarded against null reference issues.
- **Lifecycle Management:** Nixie handles the meticulous management of actor lifecycle, allowing developers to focus on implementing logic.
- **High Performance:** Thanks to being lightweight and leveraging the powerful TPL, Nixie ensures that your actor systems are both scalable and performant. Additionally the use of Generics eliminate the need for boxing and unboxing when working with value types, which can improve performance. Boxing is the process of converting a value type to an object type, and unboxing is the reverse.
- **Less Error Prone:** The strongly-typed nature and nullability checks inherently make your actor system more reliable and resilient.
- **Built on TPL:** Make the most out of the robust, scalable, and performant asynchronous programming features offered by TPL.
- **Multi-Threading:** To increase throughput, Nixie makes use of thread-safe structures that avoid locks wherever possible and use fine-grained locking where locks are necessary. Abstracting the complexities of multithreaded programming into an API that is easy to use and understand.
- **Production-Ready:** Nixie has been tested in production environments with many concurrent users, ensuring reliability and stability under real-world conditions.

## Getting Started

### Prerequisites

- .NET SDK 6.0 or later
- A suitable IDE (e.g., Visual Studio, Visual Studio Code, or Rider)

### Installation

To install Nixie into your C#/.NET project, you can use the .NET CLI or the NuGet Package Manager.

#### Using .NET CLI

```shell
dotnet add package Nixie --version 1.0.0
```

### Using NuGet Package Manager

Search for Nixie and install it from the NuGet package manager UI, or use the Package Manager Console:

```shell
Install-Package Nixie -Version 1.0.0
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

Nixie is an open-source project, and contributions are heartily welcomed! Whether you are looking to fix bugs, add new features, or improve documentation, your efforts and contributions will be appreciated. Check out the CONTRIBUTING.md file for guidelines on how to get started with contributing to Nixie.

## License

Nixie is released under the MIT License.

## Name origin

[Nixies](https://en.wikipedia.org/wiki/Nixie_(folklore)) are mysterious shapeshifting water spirits in Germanic mythology and folklore. 

## Acknowledgements

Sincere thanks to all contributors and the C#/.NET community for the continual support and inspiration.

---

Let's build robust and efficient actor systems with Nixie! 🚀
