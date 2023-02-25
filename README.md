# gRPC Microservices example using .NET6 Gateway gRPC stub and .NET 6 gRPC Server

gRPC is a RPC protocol using HTTP/2 as transport layer and sending/recieving messages using Protobuf.

In this example we have a .NET 6 API Gateway which acts a gRPC stub and .NET 6 gRPC server connected to Azure Postgres SQL for persistence.

In this example we implement 5 restful API(s) which make a gRPC call to the gRPC server.
- `/api/Products?v=1&searchAfter=2023-02-16T09:52:57.6046760Z&limit=100` (HTTP GET)
- `/api/Products/2e2c4a74-a6d2-4c79-ad1f-a5e9bdc7a005?v=1` (HTTP GET)
- `/api/Products` (HTTP POST)
- `/api/Products/2e2c4a74-a6d2-4c79-ad1f-a5e9bdc7a005?v=1` (HTTP PUT)
- `/api/Products/2e2c4a74-a6d2-4c79-ad1f-a5e9bdc7a005?v=1` (HTTP DELETE)

Following is a high-level flow diagram between the client, gRPC client stub api and the gRPC server.

![High level flow](./gRPC%20Gateway%20API.png)

<hr/>

<strong>To Run</strong>

**Pre-requisites** -
- Provision a Azure Postgres SQL with the following settings mentioned [here](./Azure%20Postgres%20SQL%20settings.png).
- Install Postman for testing gRPC
- Add server reflection to exposing services metadata to be detected to Postman.
- Install Azure Data Studio
- Run [seed script](./seed.sql).

**Server side configurations** -

```code
dotnet new sln
mkdir src
cd src

dotnet new grpc -o Api --Framework net6.0
dotnet new classlib -o Domain --framework=net6.0
dotnet new classlib -o Infrastructure --framework=net6.0

dotnet sln add .\src\Api
dotnet sln add .\src\Domain
dotnet sln add .\src\Infrastructure
```

Install the following nuget packages to work with the gRPC server and connect to Azure Postgres SQL.

- Grpc.AspNetCore
- Grpc.Tools
- AutoMapper
- AutoMapper.Extensions.Microsoft.DependencyInjection
- Dapper
- Npgsql

We can share the `.proto` file with the client so that the client can generate the code.

<hr/>

**Client side configurations** -

```code
dotnet new sln
mkdir src
cd src

dotnet new webapi -o Api --framework=net6.0
dotnet new classlib -o Domain --framework=net6.0
dotnet new classlib -o Application --framework=net6.0
dotnet new classlib -o Infrastructure --framework=net6.0

dotnet sln add .\src\Api
dotnet sln add .\src\Domain
dotnet sln add .\src\Application
dotnet sln add .\src\Infrastructure
```

Install the following nuget packages to work with the gRPC stub client.

- Google.Protobuf
- Grpc.Net.Client
- Grpc.Tools
- Microsoft.AspNetCore.Mvc.NewtonsoftJson
- Microsoft.AspNetCore.Mvc.Versioning
- MediatR
- AutoMapper
- AutoMapper.Extensions.Microsoft.DependencyInjection
- FluentValidation
- FluentValidation.AspNetCore
- FluentValidation.DependencyInjectionExtensions

<hr/>
<br/>

**To deploy gRPC server in Azure App Service** -

- Provision a Linux Azure App Service targetting runtime .NET 6 LTS or .NET 7 STS
- Enable the options in the configuration section of the Azure App Service
![Configuration1](./Configurations%20to%20be%20changed%20in%20Azure%20App%20Service%20-%201.png)
- Add the same port number of https which is present in the launchSettings.json file in the HTTP20ONLY_PORT configuration.
![Configuration2](./Configurations%20to%20be%20changed%20in%20Azure%20App%20Service%20-%202.png)
- For more details check out the official documentation [here](https://github.com/Azure/app-service-linux-docs/blob/master/HowTo/gRPC/use_gRPC_with_dotnet.md).