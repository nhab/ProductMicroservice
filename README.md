# ProductMicroservice
## EF core - Code first
1. Using viual studio 2022, Create an ASP.NET Core Web API project
1. Create **Product** class in Model folder

```
public class Product
{
    public int id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
}
```
1. Add connection string of your databse server (or file) into  **appsettings.json** of the project
