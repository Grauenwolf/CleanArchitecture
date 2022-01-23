# Cleaning Clean Architecture

The purpose of this repository is to investigate "Clean Architecture" and see if it can be improved upon. The goal is to remove complexity without reducing functionality.

To order to see the transformation incrementally, a branch has been created for each step. Simply compare the branch with the one before it to see the progression.

## Round 0 - Base State Validation

To being with we need to see if the application can be compiled and run. Likewise, we need to verify that all of the automated tests run.

The first hurdle is Docker. The application won't compile without it. So Docker Desktop needs to be installed, as well as the Linux Subsystem for Windows.

Next is Node. The application doesn't run under the current version of node (17.4.0). But according to the error messages, we can roll back to the 14.x series. At the time of this writing, 14.18.3 is the latest version that works.

The .NET Core template is broken, so you'll need to manually start the Angular frontend using `npm start` from the ClientApp folder.

All tests are passing.



## Round 1 - Removing Docker

Simply building a .NET Core/Angular application shouldn't require a commercial product such as Docker. And since we're looking at the architecture, not the deployment strategy, we can remove it.

To clear a warning message, we'll also update the TypeScript SDK to use Microsoft.TypeScript.MSBuild.

## Round 2 - Authorization Behavior

Looking at the `AuthorizationBehaviour` class, we noticed that it was never being hit. All of the requests that required authorization, for example `TodoListsController.Get`, were being aborted before we got this far.

As it turns out, ASP.NET Core has a built-in system for handling authorization. And this project is using it. So the `AuthorizationBehaviour` class is completely redundant and can be removed.

Reviewing the commands and handlers, we do see one that uses the `Authorization` attribute directly. This is the `PurgeTodoListsCommand`. In theory we would move the attribute to the controller method. But since this command is never used, we can just delete it. Which means we can also delete the matching test cases.

## Round 3 - Unhandled Exception Behavior

To test the `UnhandledExceptionBehaviour`, we start by adding a division by zero to the `GetTodosQueryHandler`. Then we set a break point in both `UnhandledExceptionBehaviour` and `ApiExceptionFilterAttribute` to see which is actually being used. 
 
And the answer is... both of them. Which means we can combine them. When choosing which of the two to keep, there are a couple considerations.

1. Which has more information available? The `ApiExceptionFilterAttribute` has access to the entire `HttpContext`, while `UnhandledExceptionBehaviour` only has the `ExportTodosQuery` object and the exception itself.
2. Which is broader? The `ApiExceptionFilterAttribute` covers all API requests, `UnhandledExceptionBehaviour` only handles API requests that go through MediatR.
3. Which is eariler in the request pipeline? Not only does `UnhandledExceptionBehaviour` only handles requests that go through MediatR, it can't see errors that occur before or after MediatR does its thing. For example, serialization errors won't be caught.

So clearly we should keep `ApiExceptionFilterAttribute` and move the logging `UnhandledExceptionBehaviour` does into it. (Specifically, what to log is left as an exercise for the reader.)

## Round 4 - Performance Logging

Just like exception logging should be handled in the ASP.NET Core pipeline, so should performance logging.

For this we'll use ASP.NET Core middleware. For simple use cases, the `app.Use` method can be employed to create ad-hoc middleware from a function. But it is cleaner to create a separate class.

## Round 5 - Validation

The next behavior we'll be looking at is validation. This is a strange one. In the startup logic for ASP.NET Core, FluentValidation was added and then disabled. Then a MediatR behavior called `ValidationBehaviour` was created to do the same thing.

That's just plain silly. So `ValidationBehaviour` is going to be deleted and FluentValidation is going to be turned back on.

## Round 6 - Logging

Like performance logging, request logging should be pulled up into the ASP.NET Core pipeline so that all requests are logged.

There are many options for this, but to keep it simple we'll just enabled the built-in request logging via `app.UseHttpLogging();`. 

## Round 7 - Fixing the Tests

When working on a refactoring job, it's easy to forget to rerun all of the tests after each round. As a result, we now have some broken tests need to be addressed.

All of the failing integration tests deal with validation. Which reveals some concerns.

1. The validation logic that can be unit tested is mixed in with the validation that requires database access.
2. The validation logic for things like missing values can't be tested without attempting to write to the database.
3. The validation logic tests are intertwined with the MediatR pipeline.
4. Most of the tests don't check if the correct validation error is returned, only whether or not a validation error happened.

Fixing all of these issues is a bit much for one round. So we're just going to do the minimum needed for getting the tests running. Which means ripping out MediatR and using the validators directly.


## Round 8 - Reducing the Namespaces

Having only one or two classes per namespace is a sign of poor organization. It makes it harder to follow the list of `using` directives unnecessarily long. For example, consider this list from the `TodoItemsController` class.

```
using CleanArchitecture.Application.TodoItems.Commands.CreateTodoItem;
using CleanArchitecture.Application.TodoItems.Commands.DeleteTodoItem;
using CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItem;
using CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItemDetail;
using CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsWithPagination;

```

Those five namespaces could be rolled into one to reduce verbosity and make it easier to look at multiple, related classes at once.

At the same time, we'll break up the files so that each has only one class. There is no reason to put multiple classes in one file and it makes them harder to find in the solution explorer.

## Round 9 - Cleaning up the Controllers

Now that we are no longer relying on MediatR for cross-cutting concerns, we can strip it out from the controllers. This will make the dependency between the controllers and handlers explicit rather than hidden.

Some will object and say that they are now "tightly bound" and by using MediatR they are "loosely coupled". They are wrong. No amount of indirection will change the fact that the controllers and handlers tightly coupled.

This will be messy at first, as each handler will need to be registered in ASP.NET Core’s dependency injection and the controller’s constructor. 


## Round 10 - Cleaning up the Handlers

The only reason for having one method per handler class was to satisfy the MediatR design pattern. There is no real advantage for having them divided up like that and it makes it harder to see places where common functionality can be factored out.
 
The method names will have to be changed so that they are unique within a class. But when reading stack traces, it is better to have methods named `Create` or `Update` than having everything generically called “Handle’.


## Round 11 - Wiring up the Cancellation Tokens

Though the handlers (now renamed services) had support for cancellation tokens, they were not provided by the controller layer to the MediatR layer. This means that when a client cancels a request, the web server never finds out about it.

Fixing this is fairly easy. Simply add a `CancellationToken` parameter to each controller method that should be cancellable. 

Generally speaking, only read methods should allow cancellation. Write operations can run into some difficult timing issues if canceled, and should be fast enough that cancellation is never necessary. So for the write method, the cancellation token will be removed from the service class.

## Round 12 - Remove the Unnecessary Event Handlers

The TodoItemCompletedEventHandler and TodoItemCreatedEventHandler classes don’t actually do anything. The log message they create serves no purpose, and if it was useful, it could have been handled by the service class. 

While it is possible that an internal service bus is needed for a project, in this case it is not. And even if a need arises in the future, the information needed in the events may be very different than what’s available today. Which means the current messages will have to be rewritten anyways or the new feature that needs them will have to accept a compromise. This is a risk of trying to predict future needs without a clear roadmap. 

## Round 13 - Remove the Layer Violation

The persistence layer, specifically the EF Core DB context, has a dependency on the business logic layer and MediatR. This is used to signal status events such as the creation or completion of a `ToDoItem`. Aside from the fact that nothing listens for these events, that kind of logic shouldn’t be hidden inside the persistence layer. Like database triggers, there is no way to know that these exist in the `DbContext` unless you already know to go searching for them. 

If needed, the service classes could trigger the events. Though more often than not, the service class should just perform the work itself rather than deferring to something external.  

Then there is the timing concern. The `IDomainEventService`, which handles the publishing, may operate  within the context of a database transaction. Or it may not. By the time you get to the `IDomainEventService` implementation, that information is lost in the call stack. Currently the `DomainEventService` class handles it well because it doesn’t have database access, but there is no telling what future implementations of `IDomainEventService` will need.

The counter-point to this argument is that there should never be a future implementation of `IDomainEventService`. All logic should be in MediatR event listeners, meaning that the `DomainEventService` won’t need to be changed in the future. Though of course that brings into question why `IDomainEventService` exists in the first place.

Further compounding the issue is `DomainEvents` collection. This is placed on entities to act as a scratch space to store events before they are published. But from the user of the entity, they look like just another child table. Though if you look at the database, no such table exists. This can lead to both unnecessary confusion and the opportunity to misuse the collection.

Only some of the entities offer the `DomainEvents` collection. This inconsistency will lead to further confusion, as developers wonder what the rule is for whether or not a given entity can trigger events. 

There is also the fact that the `DomainEvents` collection is exposed as a property. This means it needs to be explicitly excluded on each entity so EF Core doesn’t go searching for a matching table. Had it been exposed as a property, then this wouldn’t be an issue.

Now you may be thinking, “well at least I get notifications for inserts and deletes for free”. And that’s a reasonable assumption, as the `DbContext` has all of the information necessary to make that happen. But no, that’s not how it works. The service classes (formally the handlers) need to explicitly add events to the `DomainEvents` collection in order to indicate that a create, update, or delete has occurred.

All in all, this was a poorly conceived feature with an equally bad implementation and it needs to be removed before anything important takes a dependency on it.


## Round 14 - Realign Controller and Service Parameters

There are a couple small classes that no longer a purpose. Originally they were used as messages for MediatR, but since that is no longer being used we can pass the parameters directly from the controller classes to the service classes.

Likewise, we can remove the parameters on the controllers that are not needed. For example, the `Update` method on `TodoListsController` receives two copies of the `id` parameter, one in the route and one in the command object. At best this duplication is an annoyance, as both the client and server need to ensure both copies are populated. 

Removing the `id` parameter will cause a breaking change and the typescript code will have to be updated to match. This is only acceptable because both are in the same code base and a simultaneous release is possible.



 <img align="left" width="116" height="116" src="https://raw.githubusercontent.com/jasontaylordev/CleanArchitecture/main/.github/icon.png" />
 
 # Clean Architecture Solution Template
![.NET Core](https://github.com/jasontaylordev/CleanArchitecture/workflows/.NET%20Core/badge.svg) 
[![Clean.Architecture.Solution.Template NuGet Package](https://img.shields.io/badge/nuget-6.0.1-blue)](https://www.nuget.org/packages/Clean.Architecture.Solution.Template) 
[![NuGet](https://img.shields.io/nuget/dt/Clean.Architecture.Solution.Template.svg)](https://www.nuget.org/packages/Clean.Architecture.Solution.Template)
[![Discord](https://img.shields.io/discord/893301913662148658?label=Discord&logo=discord&logoColor=white)](https://discord.gg/p9YtBjfgGe)
[![Twitter Follow](https://img.shields.io/twitter/follow/jasontaylordev.svg?style=social&label=Follow)](https://twitter.com/jasontaylordev)


<br/>

This is a solution template for creating a Single Page App (SPA) with Angular and ASP.NET Core following the principles of Clean Architecture. Create a new project based on this template by clicking the above **Use this template** button or by installing and running the associated NuGet package (see Getting Started for full details). 

## Learn about Clean Architecture

[![Clean Architecture with ASP.NET Core 3.0 • Jason Taylor • GOTO 2019](https://img.youtube.com/vi/dK4Yb6-LxAk/0.jpg)](https://www.youtube.com/watch?v=dK4Yb6-LxAk)

## Technologies

* [ASP.NET Core 6](https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-6.0)
* [Entity Framework Core 6](https://docs.microsoft.com/en-us/ef/core/)
* [Angular 12](https://angular.io/)
* [MediatR](https://github.com/jbogard/MediatR)
* [AutoMapper](https://automapper.org/)
* [FluentValidation](https://fluentvalidation.net/)
* [NUnit](https://nunit.org/), [FluentAssertions](https://fluentassertions.com/), [Moq](https://github.com/moq) & [Respawn](https://github.com/jbogard/Respawn)
* [Docker](https://www.docker.com/)

## Getting Started

The easiest way to get started is to install the [NuGet package](https://www.nuget.org/packages/Clean.Architecture.Solution.Template) and run `dotnet new ca-sln`:

1. Install the latest [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
2. Install the latest [Node.js LTS](https://nodejs.org/en/)
3. Run `dotnet new --install Clean.Architecture.Solution.Template` to install the project template
4. Create a folder for your solution and cd into it (the template will use it as project name)
5. Run `dotnet new ca-sln` to create a new project
6. Navigate to `src/WebUI/ClientApp` and run `npm install`
7. Navigate to `src/WebUI/ClientApp` and run `npm start` to launch the front end (Angular)
8. Navigate to `src/WebUI` and run `dotnet run` to launch the back end (ASP.NET Core Web API)

Check out my [blog post](https://jasontaylor.dev/clean-architecture-getting-started/) for more information.

### Docker Configuration

In order to get Docker working, you will need to add a temporary SSL cert and mount a volume to hold that cert.
You can find [Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-6.0) that describe the steps required for Windows, macOS, and Linux.

For Windows:
The following will need to be executed from your terminal to create a cert
`dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p Your_password123`
`dotnet dev-certs https --trust`

NOTE: When using PowerShell, replace %USERPROFILE% with $env:USERPROFILE.

FOR macOS:
`dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123`
`dotnet dev-certs https --trust`

FOR Linux:
`dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123`

In order to build and run the docker containers, execute `docker-compose -f 'docker-compose.yml' up --build` from the root of the solution where you find the docker-compose.yml file.  You can also use "Docker Compose" from Visual Studio for Debugging purposes.
Then open http://localhost:5000 on your browser.

To disable Docker in Visual Studio, right-click on the **docker-compose** file in the **Solution Explorer** and select **Unload Project**.

### Database Configuration

The template is configured to use an in-memory database by default. This ensures that all users will be able to run the solution without needing to set up additional infrastructure (e.g. SQL Server).

If you would like to use SQL Server, you will need to update **WebUI/appsettings.json** as follows:

```json
  "UseInMemoryDatabase": false,
```

Verify that the **DefaultConnection** connection string within **appsettings.json** points to a valid SQL Server instance. 

When you run the application the database will be automatically created (if necessary) and the latest migrations will be applied.

### Database Migrations

To use `dotnet-ef` for your migrations please add the following flags to your command (values assume you are executing from repository root)

* `--project src/Infrastructure` (optional if in this folder)
* `--startup-project src/WebUI`
* `--output-dir Persistence/Migrations`

For example, to add a new migration from the root folder:

 `dotnet ef migrations add "SampleMigration" --project src\Infrastructure --startup-project src\WebUI --output-dir Persistence\Migrations`

## Overview

### Domain

This will contain all entities, enums, exceptions, interfaces, types and logic specific to the domain layer.

### Application

This layer contains all application logic. It is dependent on the domain layer, but has no dependencies on any other layer or project. This layer defines interfaces that are implemented by outside layers. For example, if the application need to access a notification service, a new interface would be added to application and an implementation would be created within infrastructure.

### Infrastructure

This layer contains classes for accessing external resources such as file systems, web services, smtp, and so on. These classes should be based on interfaces defined within the application layer.

### WebUI

This layer is a single page application based on Angular 10 and ASP.NET Core 5. This layer depends on both the Application and Infrastructure layers, however, the dependency on Infrastructure is only to support dependency injection. Therefore only *Startup.cs* should reference Infrastructure.

## Support

If you are having problems, please let us know by [raising a new issue](https://github.com/jasontaylordev/CleanArchitecture/issues/new/choose).

## License

This project is licensed with the [MIT license](LICENSE).
