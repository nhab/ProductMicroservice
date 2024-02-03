# ProductMicroservice
## EF core - Code first
- Using viual studio 2022, Create an ASP.NET Core Web API project
- Create Product **model** in Model folder

```
public class Product
{
    public int id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
}
```
- Add c**onnection string** of your databse server (or file) into  **appsettings.json** of the project

(In visual studio>Server explorer> right click on Data Connections and add data connection.

 you can copy connectionstring from the properties of this connection)
```
...
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "ProductDB":"Data Source=(localdb)\\MSSQLLocalDB;AttachDbFilename=D:\\dbs\\products.mdf;Integrated Security=true;"
  }
...
```
- Create **DBContext*" in Model folder

 ( DBContext syncronizes entities between database and code)
```

public class ProductDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }
}
```

- In Program.cs Add DbContext **service** to the program, to make it available in the project:

(ASP.NET Core uses dependency injection to manage dependencies  in form of “services”.)
...
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ProductDbContext>(
  options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("ProductDB")
));
...
- Now you can pupulate your model(s) into the database table using **migration**
  
(Migration is a tool to kepp the model and the database sync.)

-



