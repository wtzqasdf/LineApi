# LineApi
The simple Line Login/Notify examples.    
The example for ASP.NET Core.

## Nuget requirements
```
Newtonsoft.Json    
BCrypt.Net-Core    
Microsoft.AspNet.WebApi.Client
```

## How to Inject?
```csharp
services.AddSingleton<LineNotify>();
services.AddSingleton<LineLogin>();
```
You need to register "Line Login Api" & "Line Notify Api" and get Id/Secret keys before setting Configuration.    
They both is not the same, So you have to register twice.

## Configuration Helps
```
Client/Channel Id: The id of LineNotify/LineLogin.    
Client/Channel Secret: The secret of LineNotify/LineLogin.    
Stete: for validation, random key.
```

## How to use "Line Login"?
#### Step 1
```csharp
string url = lineLogin.GetLoginAuthUrl(Request, "/linelogin/response");
//Redirect page...
```
#### Step 2
```csharp
//The variable "parameter" is the parameter of the request, use get header.
if (lineLogin.IsStateVaild(parameter.state) == false)
{
    //forbid
}
var result = lineLogin.GetOAuthToken(Request, parameter.code, "/linelogin/response");
var profile = lineLogin.GetProfile(result.AccessToken);
//to do something...
```

## How to use "Line Notify"?
#### Step 1
```csharp
string url = lineNotify.GetNotifyAuthUrl(Request, "/lineNotify/response");
//Redirect page...
```
#### Step 2
```csharp
//The variable "parameter" is the parameter of the request, use get header.
if (lineNotify.IsStateVaild(parameter.state) == false)
{
    //forbid
}
var result = lineNotify.GetOAuthToken(Request, parameter.Code, "/lineNotify/response");
lineNotify.SendMessage(result.AccessToken, "Hello World");
```
#### If you want to delete an AccessToken
```csharp
lineNotify.RevokeToken("Access Token of user");
```
