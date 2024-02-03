# ProductMicroservice
## EF core - Code first
- Using viual studio 2022, Create an ASP.NET Core Web API project
- Create **Product** class in Model folder

```
public class Product
{
    public int id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
}
```
- Add connection string of your databse server (or file) into  **appsettings.json** of the project
- Create *DBContext" in Model folder
```
// DBContext syncronizes entities between database and code
public class ProductDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }
}
```
- In Program.cs Add DbContext service to the programe:
```
...
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ProductDbContext>(
  options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("ProductDB")
));
...
```

