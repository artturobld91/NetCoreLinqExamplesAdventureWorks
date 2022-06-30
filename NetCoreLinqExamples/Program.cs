using NetCoreLinqExamples.Models;
// See https://aka.ms/new-console-template for more information

// Important notes on LINQ
// Steps that linq performs: Obtain the datasource, Create query, Execute query.
// Linq by default returns IEnumerable collection.
// If wanted to return IQueryable implement AsIQueryable() method.
// If wanted to return List implement ToList() method.
// If wanted to return Array implement ToArray() method.

/*
string[] colors = new string[] { "green", "brown", "yellow", "black" };
string s = "e";
var query = colors.Where(c => c.Contains(s)).ToList();
var query2 = colors.Where(c => c.Contains(s));
s = "b";
query = query.Where(x => x.Contains(s)).ToList();
query2 = query2.Where(c => c.Contains(s));

Console.WriteLine(query.Count());
*/

// Using AdventureWorks2019 database do the follwing
using (AdventureWorks2019Context context = new AdventureWorks2019Context())
{
    // Write a query using the Sales.SalesOrderDetail table that displays a count of the detail lines for each SalesOrderID.
    // SQL - SELECT SalesOrderID, COUNT(*) as CountOfOrders FROM Sales.SalesOrderDetail GROUP BY SalesOrderID
    context.SalesOrderDetails.GroupBy(x => x.SalesOrderId)
                             .Select(x => new { salesOrder = x.Key, CountOfLines = x.Count() });
                             //.ToList().ForEach( x => Console.WriteLine($"Key: {x.salesOrder} - Count: {x.CountOfLines}"));

    // Write a query to determine the average freight amount in the Sales.SalesOrderHeader table.
    // SQL - SELECT AVG(Freight) AS AverageFreight FROM Sales.SalesOrderHeader;
    decimal averageRes = context.SalesOrderHeaders.Select(x => new { AverageFreightAmt = x.Freight })
                                                  .Average(x => x.AverageFreightAmt);
    decimal averageRes2 = context.SalesOrderHeaders.Select(x => x.Freight)
                                                   .Average();
    Console.WriteLine($"Average: {averageRes}");
    Console.WriteLine($"Average2: {averageRes2}");

    // Write a query to determine the price of the most expensive product ordered. Use the UnitPrice column of the Sales.SalesOrderDetail table.
    // SQL - SELECT MAX(UnitPrice) as MostExpensiveMaxPrice FROM Sales.SalesOrderDetail
    decimal maxRes = context.SalesOrderDetails.Select(x => x.UnitPrice)
                                              .Max();
    Console.WriteLine($"Max: {maxRes}");

    // Write a query that shows the total number of items ordered for each product. Use the Sales.SalesOrderDetail table to write the query
    // SQL - SELECT ProductID, SUM(OrderQty) as ProductsOrdered FROM Sales.SalesOrderDetail GROUP BY ProductID
    context.SalesOrderDetails.GroupBy(x => x.ProductId)
                             .Select(x => new { x.Key, CountOfOrders = x.Sum(x => x.OrderQty) });
    //.ToList().ForEach(x => Console.WriteLine($"ProductID: {x.Key} - Sum: {x.CountOfOrders}"));

    // Write a query that displays the count of orders placed by year for each customer using the Sales.SalesOrderHeader table.
    // SQL - SELECT TOP(10) CustomerID, YEAR(OrderDate) as OrderYear, COUNT(*) as CountOfOrders FROM Sales.SalesOrderHeader GROUP BY CustomerID, YEAR(OrderDate) ORDER BY CustomerID
    context.SalesOrderHeaders.GroupBy(x => new { x.CustomerId, x.OrderDate.Year })
                             .Select(x => new { x.Key.CustomerId, x.Key.Year, CountOfOrders = x.Count() })
                             .OrderBy(x => x.CustomerId)
                             .Take(10);
    //.ToList().ForEach(x => Console.WriteLine($"CustomerID: {x.CustomerId} - Year: {x.Year} - Count: {x.CountOfOrders}"));

    // Write a query using the Sales.SalesOrderHeader, Sales.SalesOrderDetail, and Production.Product tables to display the total sum of products by ProductID and OrderDate.
    // SQL -
    // SELECT detail.ProductID, header.OrderDate, SUM(detail.OrderQty) as SumOfProducts
    // FROM Sales.SalesOrderHeader as header
    // INNER JOIN Sales.SalesOrderDetail as detail ON header.SalesOrderID = detail.SalesOrderID
    // GROUP BY detail.ProductID, header.OrderDate
    // ORDER BY header.OrderDate

    context.SalesOrderHeaders.Join(context.SalesOrderDetails, orderh => orderh.SalesOrderId, orderd => orderd.SalesOrderId, (orderh, orderd) => new { OrderH = orderh, OrderD = orderd })
                             .Join(context.Products, ordershd => ordershd.OrderD.ProductId, products => products.ProductId, (ordershd, products) => new { OrdersHD = ordershd, ProductsO = products })
                             .GroupBy(x => new { x.ProductsO.ProductId, x.OrdersHD.OrderH.OrderDate })
                             .Select(x => new { x.Key.ProductId, x.Key.OrderDate, SumOfProducts = x.Sum(x => x.OrdersHD.OrderD.OrderQty) })
                             .OrderBy(x => x.OrderDate)
                             .ToList().ForEach(x => Console.WriteLine($"ProductID: {x.ProductId} - OrderDate: {x.OrderDate} - Sum: {x.SumOfProducts}"));
}

Console.ReadLine();
