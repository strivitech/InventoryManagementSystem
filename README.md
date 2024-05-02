## Introduction
This Inventory Management System (IMS) is designed to manage product inventories and order processing efficiently.
The system is built with FaaS in mind and utilizes various Azure services including Azure Functions, Azure Key Vault, Azure Database for PostgreSQL, 
Azure Cache for Redis, Cosmos DB, and Azure Logic Apps to ensure scalability, security and high performance.
## Architecture
The system is split into 3 main components:
- Products.Functions: Manages product-related operations.
- Ordering.Functions: Handles order verification and processing.
- Inventory.PostgresMigrationsApp: Manages database for Postgres migrations.

### Products.Functions
- CreateProduct
- GetProduct
- GetProductOverviews
- GetAllProducts
- UpdateProduct
- DeleteProduct
- ReserveProducts
- ReleaseProducts
### Ordering.Functions
- VerifyOrder
- ProcessOrder

These functions support CRUD operations for product management and implement business logic for order processing.

The Products.Functions, Ordering.Functions and their connections with Azure services are showed in the image below.
Worth noting that Products.Functions operates with the use of Postgre + Redis cache, meanwhile Ordering.Functions utilizes CosmosDb to store information about orders. 

![IMS-Azure](https://github.com/strivitech/InventoryManagementSystem/assets/111649007/d94c2f47-fe6c-4f76-be80-8b974f5b224a)

## Technologies Used
- Azure Functions: Serverless computing service that lets you run event-triggered code without having to explicitly provision or manage infrastructure.
- Azure Key Vault: Helps safeguard cryptographic keys and secrets used by cloud applications and services.
- Azure Database for PostgreSQL: Relational database service based on the open-source Postgres database engine.
- Azure Cache for Redis: A fully managed in-memory data store, backed by Redis.
- Cosmos DB: A globally distributed, multi-model database service.
- Azure Logic Apps: Used to design and build scalable solutions for app integration, data integration, system integration, enterprise application integration (EAI), and business process automation (BPA).

## Logic App Ordering Happy Workflow
The Logic App orchestrates the workflow of processing orders. Below is the high-level workflow:
1. Receive HTTP Request: Triggers the Logic App with order details.
2. Verify Order: Checks if the order can be fulfilled (validates order data and checks if warehouse quantity is enough).
3. Reserve Products: Reserve products with specified amount for the order.
4. Process Order: Finalizes the order if the reservation is successful.

## Logic App Saga Pattern Implementation
This implementation allows handling long-running and distributed transactions, where each step might succeed or fail independently.
The Logic App ensures that either all operations (Verify, Reserve, Process) succeed, or compensatory actions (Release Products) are triggered to revert the system to a consistent state.

The final implementation presented on the image below.

![image](https://github.com/strivitech/InventoryManagementSystem/assets/111649007/cc9f7da1-4d75-47b4-a66a-7f7edf86510e)
![image](https://github.com/strivitech/InventoryManagementSystem/assets/111649007/68e04819-3ec7-4064-90ce-df933a8aa9d2)

## API Usage
Here are few examples of how to interact with the system via HTTP requests

### CreateProduct
URL: `http://localhost:7071/api/CreateProduct`

Body:
```
{
  "Name": "Sample Product 3",
  "Description": "This is a detailed description of the Sample Product.",
  "Price": 2255.99,
  "Quantity": 7
}
```

### GetProduct
URL: `http://localhost:7071/api/GetProduct/08c871ab-5910-457c-9edf-eea899d21685`

### ProcessOrder
URL: `http://localhost:7072/api/ProcessOrder`

Body: 
```
{
    "OrderLines": [
        {
            "ProductId": "e63c2f40-d959-4167-9603-4c2797f1062e",
            "Quantity": 2,
            "Price": 2255.99
        },
        {
            "ProductId": "230ba8e1-bc5b-4599-93e8-421a707fa30c",
            "Quantity": 3,
            "Price": 255.99
        }
    ],
    "ShippingAddress": {
        "Street": "Street1",
        "City": "City1",
        "State": "State1",
        "ZipCode": "ZipCode1"
    },
    "CustomerId": "230ba8e1-bc5b-4599-93e8-421a707fa311"
}
```

For other endpoints, check request type definition that it receives to pass appropriate parameters/body.

## Logic App flow execution examples
- Success ordering flow

![image](https://github.com/strivitech/InventoryManagementSystem/assets/111649007/25673edf-990f-4f56-bc63-27db751926ee)
![image](https://github.com/strivitech/InventoryManagementSystem/assets/111649007/5cbedcd2-a482-4a6c-9ecc-e40553c1ea32)
- Failed flow

![image](https://github.com/strivitech/InventoryManagementSystem/assets/111649007/c23570e0-b44c-4848-896c-0e8718f9d2aa)


## Local setup
This section guides you through the process of running the Inventory Management System on the local machine.

### Prerequisites
- Git installed on your local machine (for cloning the repository)
- .Net 8 SDK
- Docker

### Step 1: Clone the Repository
Clone the project repository to your local machine using Git:
```
git clone https://github.com/strivitech/InventoryManagementSystem.git
cd InventoryManagementSystem
```
### Step 2: Run docker-compose file
Run docker compose file that is located in the root of solution with `docker compose up -d`.

### Step 3: Set up Azure Cosmos Db Emulator
You can find steps how to install and run it here: `https://learn.microsoft.com/en-us/azure/cosmos-db/how-to-develop-emulator?tabs=windows%2Ccsharp&pivots=api-nosql`.
Just make sure you have established TLS connection with the database.

### Step 4: Functions mapping ports
You need to setup `Products.Functions` and `Ordering.Functions` to use different ports if they are on the same 7071 by default. You can do it inside run Profiles.

### Step 5: Add `local.settings.json`
You will need to create `local.settings.json` in both `Ordering.Functions`, `Products.Functions`.

Fill them in with next content:
- `Products.Functions`:
```
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
        "ProductsPostgres": "Host=localhost;Port=5701;Database=products;Username=admin;Password=admin",
        "RedisCacheConfiguration": "localhost:5702",
        "RedisCacheInstanceName": "ProductsInstance_"
    }
}
```
- `Ordering.Functions`:
```
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
        "ProductsUrl": "http://localhost:7071/api/",
        "AuthorizationCodesProductsGetProductOverviews": "dummysomecodenotneededunderdevelopment",
        "CosmosConnectionString": "AccountEndpoint=https://localhost:5801/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="
    }
}
```
Replace necessary variables with your infrastructure values.

### Step 6: Run both `Products.Functions` and `Ordering.Functions` projects

## Manual Deployment through Azure Portal
This section guides you through the process of deploying the Inventory Management System on Azure.
Ensure that you have administrative access to your Azure subscription and that you have installed the Azure CLI tool.

### Prerequisites
- An active Azure subscription
- Azure CLI installed on your local machine
- Git installed on your local machine (for cloning the repository)
- Make sure you have logged in Azure CLI

### Step 1: Clone the Repository
Clone the project repository to your local machine using Git:
```
git clone https://github.com/strivitech/InventoryManagementSystem.git
cd InventoryManagementSystem
```

### Step 2: Set Up Azure Resources
You will need to create and configure several Azure resources including:
- Create Azure group
- Azure Functions Apps for `Products.Functions` and `Ordering.Functions` in .Net 8 Isolation Worker mode
- Cosmos DB with `OrdersDatabase` and `Orders` container
- Azure Database for PostgreSQL with `products` database
- Azure Cache for Redis
- Azure Logic Apps
- Azure Azure Key Vault

### Step 3: Add Environment variables for Function Apps
You need to `AzureKeyVaultUrl` with your Key Vault url as Environment variable for both `Products.Functions` and `Ordering.Functions`.

### Step 4: `Products.Functions` and `Ordering.Functions` Identities
To work properly, Azure functions require access to a set of resources such as Postgres, Redis, CosmosDb, Key Vault.
You have to turn on `Identity` on each Fucntion Apps and add system-assigned managed identities roles for both actions and dataActions.
You would go with granularity for production but for development simplicity setting `Contributor` to resource group and `Key Vault Secrets Officer` to Key Vault data actions is the way to go.

### Step 5: Set secrets in Azure Key Vault
Then you need to add secrets to Azure Key Vault:
- CosmosConnectionString -> Connection string for cosmos db
- ProductsPostgres -> Connection string for Postgres `products` database
- RedisCacheConfiguration -> Redis connection string
- RedisCachelnstanceName -> Redis prefix, can be any prefix that is allowed by Azure Cache for Redis

### Step 6: Use migrations app to apply migrations on database or add tables manually.
If you created public database with your IP Firewall, you can apply migrations locally by changing connection string and run `dotnet ef database update -c ProductsDbContext` from `Inventory.PostgresMigrationsApp` project.

You also can deploy `Inventory.PostgresMigrationsApp` into your private network with database access and it will automatically apply up to latest migrations.

You also have ability to manually run script inside database with the use of public acccess or setting up bastion for private database.

### Step 7: Deploy Products.Functions
You can deploy your function by running `func azure functionapp publish <Products.Functions AppName>` in your command line at the root of `Products.Functions` project.

### Step 8: Set Products.Functions secrets in Azure Key Vault
- AuthorizationCodesProductsGetProductOverviews -> Authorization code for deployed Products.Functions -> GetProductOverviews
- ProductsUrl -> url for Products.Functions Azure Functions app

### Step 9: Deploy Orders.Functions
You can deploy your function by running `func azure functionapp publish <Orders.Functions AppName>` in your command line at the root of `Orders.Functions` project.

### Step 10: Create Logic App
You can build an ordering workflow provided in the Logic App Saga Pattern Implementation or altering some changes as you wish.
