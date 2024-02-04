# Products EntityFramework Tutorial
## EF core - Code first
### 1. Creating the project 
Using viual studio 2022, Create an ASP.NET Core Web API project

### 2. Add needd Nuget packages

(intto the project:)

Microsoft.EntityFrameworkCore

Microsoft.EntityFrameworkCore.Tools

Microsoft.EntityFrameworkCore.Design

Microsoft.EntityFrameworkCore.Relational

Microsoft.EntityFrameworkCore.Abstractions 

and the selected database.one of these:

| Database  | Nuget package |
| ------------- | ------------- |
|SQL Server and SQL Azure |	Microsoft.EntityFrameworkCore.SqlServer |
|SQLite |	Microsoft.EntityFrameworkCore.Sqlite|
|Azure Cosmos DB|	Microsoft.EntityFrameworkCore.Cosmos|
|PostgreSQL|	Npgsql.EntityFrameworkCore.PostgreSQL|
|MySQL|	Pomelo.EntityFrameworkCore.MySql|
|EF Core in-memory database|	Microsoft.EntityFrameworkCore.InMemory|

### 3- Create Product **model**

( In the Model folder)
```
public class Product
{
    public int id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
}
```
### 4. Add **connection string** 

(of your databse server (or file) into  **appsettings.json** of the project)

(In visual studio>Server explorer> right click on Data Connections and add data connection.

 you can copy connectionstring from the properties of this connection)

...
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "ProductDB":"Data Source=(localdb)\\MSSQLLocalDB;AttachDbFilename=D:\\dbs\\products.mdf;Integrated Security=true;"
  }
...

### 5. Create **DBContext*"
( Into Model folder )

 ( DBContext syncronizes entities between database and code)
```
public class ProductDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }
}
```

### 6. Add DbContext **service** 

(In Program.cs , Add it to the program, to make it available in the project:)

(ASP.NET Core uses dependency injection to manage dependencies  in form of “services”.)
```
..
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ProductDbContext>(
  options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("ProductDB")
));
 ..
```
### 7. Populate the model into the database using  [**migration**](https://www.learnentityframeworkcore.com/migrations#:~:text=To%20use%20migrations%20in%20EF,made%20to%20your%20database%20schema.)
  
(Migration is a tool to kepp the model and the database sync.)

In visualstudio, open **Package Manager console** and use following commands.

If migration is not enable:

``` Enable-Migrations```

```Add-Migration [MigrationName]```

To execute last migration :

```Update-Database```
(Remove-migration, Removes the last migration snapshot)

Note 1: if the connectionString is not correct, the table would not be created 

Note 2: If you have hard time to update the database, you can create the database table and create another   **Ado.Net entity data model" project and foloow the  wizard  to generate the neede elements to reverse engineer them to your main project and then delete the table and run migration again
Note 3 : [Reference](https://github.com/rstropek/htl-csharp/blob/master/entity-framework/ef-aspnet-cheat-sheet.md)
