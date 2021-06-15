# Linq To SQL Queries

The aim of the project is to create SQL queries via LINQ. Specifically for Dapper lovers.

## Prerequisite
``` csharp
using System.ComponentModel.DataAnnotaitions;
using Microsoft.Data.SQLClient;
using Dapper; //(required when you need to fetch results)

``` 

## Installation

Just clone / download / fork and start using it in your project.

## Usage
**Entities/Models** <br/>
*Supplier.cs*
``` csharp
[Table("Supplier")]
public class Supplier
{
  [Identity(AutoGenerated = true)]
  [Column("Id")]
  public int Id { get; set; }
  [Required]
  public string CompanyName { get; set; }
  public string ContactName { get; set; }
  public string ContactTitle { get; set; }
  public string City { get; set; }
  public string Country { get; set; }
  public string Phone { get; set; }
  public string Fax { get; set; }
  [Ignore]
  public IList<Product> Products { get; set; }
  [Ignore]
  public OrderItem Order { get; set; }
}
```
*Product.cs*
``` csharp
public class Product
{
  [Identity(AutoGenerated = true)]
  public int Id { get; set; }
  public string ProductName { get; set; }
  [ForeignKey("SupplierId")]
  public int SupplierId { get; set; }
  public string UnitPrice { get; set; }
  public string Package { get; set; }
  public string IsDiscontinued { get; set; }
  public Supplier SupplierInfo { get; set; }
}
```
*OrderItem.cs*
``` csharp
[Table("OrderItem")]
public class OrderItem
{
  public int Id { get; set; }
  public int OrderId { get; set; }
  public int ProductId { get; set; }
  public float UnitPrice { get; set; }
  public int Quantity { get; set; }
}
```

*Create Connection to database*
``` csharp
var conn = new SqlConnection("Server=[YourServer];Database=[YouDatabase];Trusted_Connection=True;");

```
*Simple Select*
``` csharp
//To Generate Query for SQL Server 2012 syntax
var query = conn.Select<Supplier>()
            .Columns(c => c.All())
            .ToSqlServer2012Query();

//To Generate Query for MySQL
var query = conn.Select<Supplier>()
            .Columns(c => c.All())
            .ToMySqlQuery();

//You can also fetch the result
var suppliers = conn.Select<Supplier>()
            .Columns(c => c.All())
            .ToResult();
```

*Select with Where*
``` csharp
var suppliersInStockholm = conn.Select<Supplier>()
  .Columns(c => c.All())
  .Where(x=>x.Column(c=>c.City).Contains("Stockholm"))
  .ToResult();

```
*Select with Join*
``` csharp
//Fetch all products for all suppliers
var productsBySupplier = conn.Select<Product>()
  .Columns(c => c.All())
  .Join(j => j.Column<Supplier>(c => c.Id, JoinTypes.InnerJoin).With(s => s.SupplierId))
  .ToResult();
        
var productOfSuppliersInStockholm = conn.Select<Product>()
  .Columns(c => c.All())
  .Join(j => j.Column<Supplier>(c => c.Id, JoinTypes.InnerJoin).With(s => s.SupplierId))
  .Where(x => x.Column<Supplier>(c => c.City).Contains("Stockholm"))
  .ToResult();

```
*Select with Aggregated function*
``` csharp
var suppliersInStockholm = conn.Select<Supplier>()
  .Columns(c => c.Count(c=>c.Id).All())
  .Where(x=>x.Column(c=>c.City).Contains("Stockholm"))
  .ToResult();

```

**There are more options with complex and easy joins** *Please use and find it out*

*To insert data*
``` csharp

conn.Insert().Entity(new Supplier
  {
    Id=1,
    CompanyName = "Supplier Company",
    City = "Stockholm"
  }).Execute(IsolationLevel.ReadCommitted);

```

*Update Query*
``` csharp
conn.Update<Supplier>()
  .Columns(c => c
    .Set(x => x.CompanyName, "A new Company again")
  )
  .Join(j=>j
    .Column<Product>(p=>p.SupplierId,JoinTypes.InnerJoin).With(c=>c.Id)
  )
  .Where(w => w
    .Column<Product>(x => x.ProductName)
    .EqualsTo("Test")
  )
  .Execute(IsolationLevel.ReadCommitted);
```
*To delete data*
``` csharp

conn.Delete<Supplier>()
  .Where(c => c.Column(x => x.CompanyName).EqualsTo("Some Company"))
  .ExecuteSqlServerQuery(IsolationLevel.ReadCommitted);

```
## Contributing

1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request :D

## Credits

Riyasat Ali - https://www.linkedin.com/in/riyasat-ali/
### Limitations
This project doesn't cover more complex scenarios but still you can use it for many simple to mid level complex queries. If you have more ideas please feel free to fork and please update in this repository. Thanks
## License
Copyright 2017 Riyasat Ali and contributors

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.

## Conditions
*Add Credits to me in your project*
