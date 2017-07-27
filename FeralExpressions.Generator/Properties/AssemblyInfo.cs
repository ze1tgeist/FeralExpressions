using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("FeralExpressions")]
[assembly: AssemblyDescription(
@"This project supplies compile time support for FeralExpressions.
For every.cs file which contains a partial class with one or more expression bodied method, it creates an equivalent of that
method which has the same logic, but is a static Expression property.
For example,
public static string Test(string arg1) => ""abc"" + arg1;
would produce
public static Expression&lt;Func&lt;string,string&gt;&gt; Test_Expression => (string arg1) => ""abc"" + arg1;")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("ruben morton")]
[assembly: AssemblyProduct("FeralExpressions")]
[assembly: AssemblyCopyright("Copyright ©  2017")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("0.0.1.0")]
[assembly: AssemblyFileVersion("0.0.1.0")]
[assembly: AssemblyInformationalVersion("0.0.1-alpha")]
