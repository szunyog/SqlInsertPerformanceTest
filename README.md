# SqlInsertPerformanceTest

The structure of the database (foreign keys, indexes, etc...) has the biggest effect to the database performance. 
But it is also does matter how the client side is implemented. The .Net framework supports several ways to implement 
database insert operations. In certain scenarios it is also possible to reach huge performance increase by choosing 
the appropriate method.

In this tests I am using a time based curve data to insert. The tests are focused to determine which way is the best
 to insert several thousands records at one time.

Check [my 'MS SQL - C# insert performance test' blog post][blog-url] about the test results.

[blog-url]:  http://szunyog.github.io/archivers/mssql-csharp-insert-performance-test
