# FeralExpressions

FeralExpressions is a plugin for Entity Framework which allows EF to inline the body of methods into the expression it passes to the relational store.

It works in two stages.  

a) During compile time it reads all expression bodies functions, and produces an expression equivalent.
b) During runtime, when an expression bodies function is encountered in the expression that is passed to EF, it replaces it with 
   expression that was built in the previous step.
   
There are a few limitations
a) It only works for functions with expression bodies 
- GetValue(int x) => x * x  // this will work
- GetValue(int x) { return x * x; } // this will not work

b) In order to have a place to write the expressions to, it uses partial classes.  If you don't declare your method in a partial class
   this won't work.
c) At the moment implicit this references on instance methods arent correctly interpreted.  Make sure you explicitly put this. in from of a method call.

d) Parameters are inlined.  If the expression for the parameter has side effects, strange things can result.

An example of using FeralExpressions is given below

public partial class Person
{
  public int Id { get; set; }
  public string FirstName { get; set; }
  public string LastName { get; set; }
  
  public string FullName(string seperator) => FirstName + seperator + LastName;
}

public class MyApp
{
  public MyApp(MyDbContext dbContext)
  {
    this.dbContext = dbContext;
  }
  
  public void UseFeralExpressions()
  {
    // without FeralExpressions the following line would either require the entire People DbSet to be loaded to memory, or would error.
    // with FeralExpressions, this will be run on the Relational data store
    var results = dbContext.People.Inline().Where(p => p.FullName(" ") == "Ruben Morton");  
  }
  
  private MyDbContext dbContext;
}

This allows for decomposing complex LINQ to Entities expression into functions, and splitting this decomposition across objects.  This 
results in less monolithic hard to maintain blocks of SQL (which end up having the same maintenance issues as big stored procs in SQL)
