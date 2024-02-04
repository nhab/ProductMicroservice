# .Net Core Tutorial by example
## EntityFramework core - Code first
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

References : 

[EF summary](https://github.com/rstropek/htl-csharp/blob/master/entity-framework/ef-aspnet-cheat-sheet.md)

[microsoft](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-8.0&tabs=visual-studio)

[WebAPI with MongoDB](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-8.0&tabs=visual-studio)

## USing EF CRRUD Operations within Web API
### Dependency injection

1-Add a new API controller into controllers folder of the project 

2- Add the ProductDbContext into its constructor to be usable within the controller:
```
 [Route("api/[controller]")]
 [ApiController]
 public class ProductsController : ControllerBase
 {
     ProductDbContext dbContext;

    public ProductsController(ProductDbContext dbContext)
     {
         this.dbContext = dbContext;
     }
..
```
### CRUD
3.Get all Product:
```
 [HttpGet]
 public IEnumerable<Product> Get()
 {
   return dbContext.Products;
 }
```
4.Get by ID
```
 // GET api/<ProductsController>/5
 [HttpGet("{id}")]
 public async Task<ActionResult<Product>> Get(int id)
 {
     var product = await dbContext.Products.FindAsync(id);

     if (product == null)
     {
         return NotFound();
     }

     return product;
 }
```
5.Add a product (POST) :
```
 [HttpPost]
 public async Task<ActionResult<Product>> Post([FromBody] Product product)
 {
     dbContext.Products.Add(product);
     await dbContext.SaveChangesAsync();

     return CreatedAtAction(nameof(Get), new { id = product.id }, product);
 }
```
6- Edit a product (PUT)
```
  bool productExists(long id)
{
    return dbContext.Products.Any(e => e.id == id);
}
public class ProductEditInput {
    public long id { get; set; }
    public Product product { get; set; }
}


// PUT api/<ProductsController>/5
[HttpPut()]
public async Task<IActionResult> Put([FromBody]  Product product)
{
   dbContext.Entry(product).State = EntityState.Modified;

 try
 {
     await dbContext.SaveChangesAsync();
 }
 
 catch (DbUpdateConcurrencyException)
 {
     if (!productExists(product.id))
     {
         return NotFound();
     }
     else
     {
         throw;
     }
 }

   return NoContent();
}
        
```
7. Delete Product
```
 [HttpDelete("{id}")]
 public async Task<IActionResult> DeleteProduct(long id)
 {
    var product = dbContext.Products.Where(c => c.id == id).First();
    if (product == null)
    {
        return NotFound();
    }

    dbContext.Products.Remove(product);
    await dbContext.SaveChangesAsync();

    return NoContent();
 }
```
Up until now, we can run the project and Try each operation using swagger UI, but a better UI can be applid Using javascript

## Calling an ASP.NET Core web API with JavaScript ( Optional )
1. Configure the app to serve static files and enable default file mapping in Program.cs:
```
..
var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseDefaultFiles();
app.UseStaticFiles();
..
```
2. Create a wwwroot folder in the project root.

Create a js folder and a css folder inside of the wwwroot folder.

Add an HTML file named index.html t
```
<!DOCTYPE html>
<html>
<head>
    <meta charset="UTF-8">
    <title>To-do CRUD</title>
    <link rel="stylesheet" href="css/site.css" />
</head>
<body>
    <h1>To-do CRUD</h1>
    <h3>Add</h3>
    <form action="javascript:void(0);" method="POST" onsubmit="addItem()">
        <input type="text" id="add-name" placeholder="New to-do">
        <input type="submit" value="Add">
    </form>

    <div id="editForm">
        <h3>Edit</h3>
        <form action="javascript:void(0);" onsubmit="updateItem()">
            <input type="hidden" id="edit-id">
            <input type="checkbox" id="edit-isComplete">
            <input type="text" id="edit-name">
            <input type="submit" value="Save">
            <a onclick="closeInput()" aria-label="Close">&#10006;</a>
        </form>
    </div>

    <p id="counter"></p>

    <table>
        <tr>
            <th>Is Complete?</th>
            <th>Name</th>
            <th></th>
            <th></th>
        </tr>
        <tbody id="todos"></tbody>
    </table>

    <script src="js/site.js" asp-append-version="true"></script>
    <script type="text/javascript">
        getItems();
    </script>
</body>
</html>
```
site.css :
```  
input[type='submit'], button, [aria-label] {
    cursor: pointer;
}

#editForm {
    display: none;
}

table {
    font-family: Arial, sans-serif;
    border: 1px solid;
    border-collapse: collapse;
}

th {
    background-color: #f8f8f8;
    padding: 5px;
}

td {
    border: 1px solid;
    padding: 5px;
}
```
site.js :
```
const uri = 'api/todoitems';
let todos = [];

function getItems() {
  fetch(uri)
    .then(response => response.json())
    .then(data => _displayItems(data))
    .catch(error => console.error('Unable to get items.', error));
}

function addItem() {
  const addNameTextbox = document.getElementById('add-name');

  const item = {
    isComplete: false,
    name: addNameTextbox.value.trim()
  };

  fetch(uri, {
    method: 'POST',
    headers: {
      'Accept': 'application/json',
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(item)
  })
    .then(response => response.json())
    .then(() => {
      getItems();
      addNameTextbox.value = '';
    })
    .catch(error => console.error('Unable to add item.', error));
}

function deleteItem(id) {
  fetch(`${uri}/${id}`, {
    method: 'DELETE'
  })
  .then(() => getItems())
  .catch(error => console.error('Unable to delete item.', error));
}

function displayEditForm(id) {
  const item = todos.find(item => item.id === id);
  
  document.getElementById('edit-name').value = item.name;
  document.getElementById('edit-id').value = item.id;
  document.getElementById('edit-isComplete').checked = item.isComplete;
  document.getElementById('editForm').style.display = 'block';
}

function updateItem() {
  const itemId = document.getElementById('edit-id').value;
  const item = {
    id: parseInt(itemId, 10),
    isComplete: document.getElementById('edit-isComplete').checked,
    name: document.getElementById('edit-name').value.trim()
  };

  fetch(`${uri}/${itemId}`, {
    method: 'PUT',
    headers: {
      'Accept': 'application/json',
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(item)
  })
  .then(() => getItems())
  .catch(error => console.error('Unable to update item.', error));

  closeInput();

  return false;
}

function closeInput() {
  document.getElementById('editForm').style.display = 'none';
}

function _displayCount(itemCount) {
  const name = (itemCount === 1) ? 'to-do' : 'to-dos';

  document.getElementById('counter').innerText = `${itemCount} ${name}`;
}

function _displayItems(data) {
  const tBody = document.getElementById('todos');
  tBody.innerHTML = '';

  _displayCount(data.length);

  const button = document.createElement('button');

  data.forEach(item => {
    let isCompleteCheckbox = document.createElement('input');
    isCompleteCheckbox.type = 'checkbox';
    isCompleteCheckbox.disabled = true;
    isCompleteCheckbox.checked = item.isComplete;

    let editButton = button.cloneNode(false);
    editButton.innerText = 'Edit';
    editButton.setAttribute('onclick', `displayEditForm(${item.id})`);

    let deleteButton = button.cloneNode(false);
    deleteButton.innerText = 'Delete';
    deleteButton.setAttribute('onclick', `deleteItem(${item.id})`);

    let tr = tBody.insertRow();
    
    let td1 = tr.insertCell(0);
    td1.appendChild(isCompleteCheckbox);

    let td2 = tr.insertCell(1);
    let textNode = document.createTextNode(item.name);
    td2.appendChild(textNode);

    let td3 = tr.insertCell(2);
    td3.appendChild(editButton);

    let td4 = tr.insertCell(3);
    td4.appendChild(deleteButton);
  });

  todos = data;
}
```
4. Open Properties\launchSettings.json.
Remove the launchUrl property to force the app to open at index.html—the project's default file.

[referece](https://learn.microsoft.com/en-us/aspnet/core/tutorials/web-api-javascript?view=aspnetcore-8.0)
