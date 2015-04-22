# RavenDb.Identity

## Introduction
The RavenDb.Identity project is a simple ASP.NET Membership and RoleProvider, with RavenDb as backend, for use in MVC applications. It allows you to simple create, manage and authenticate/authorize users and their roles. Based on conventions, it requires little setup to get started.

Although the project originally wasn't developed for primetime, this utility was used in production before I knew it. So, I've plumbed the essentials a bit and published it [to NuGet](https://www.nuget.org/packages/Hanssens.Net.Identity.RavenDb/). 

And although the name implies that it uses ASP.NET Identity: it does not (yet). 

## Getting Started

#### Install Package
In any MVC project, install the package simply through NuGet:

	Install-Package Hanssens.Net.Identity.RavenDb

#### Edit your web.config

Add the following section to the `<system.web>` node in your web.config:

    <!--START: Authentication-->

    <authentication mode="Forms">
      <forms loginUrl="~/auth/login" timeout="2880" />
    </authentication>

    <membership defaultProvider="RavenDbMembershipProvider">
      <providers>
        <clear />
        <add name="RavenDbMembershipProvider" type="Hanssens.Net.Identity.RavenDb.RavenDbMembershipProvider" />
      </providers>
    </membership>

    <roleManager enabled="true" defaultProvider="RavenDbRoleProvider">
      <providers>
        <clear />
        <add name="RavenDbRoleProvider" type="Hanssens.Net.Identity.RavenDb.RavenDbRoleProvider" />
      </providers>
    </roleManager>  

    <!--END: Authentication-->

This hooks onto

#### Add a login view

Basically, all you need to do further on is provide a custom Login view to your project. Something like the following will do just fine (:

    <!-- File: ~/Views/Auth/Login.cshtml -->
    <div class="content">
        @using (Html.BeginForm("Login", "Auth", FormMethod.Post))
        {

            <div class="fields">
                <strong>Gebruikersnaam</strong>
                <input name="Username" class="form-control" type="text" placeholder="your username" />
            </div>
            <div class="fields">
                <strong>Wachtwoord</strong>
                <input name="Password" class="form-control" type="password" placeholder="your password" />
            </div>

            <div class="actions">
                <br />
                <input type="submit" class="btn btn-default button" value="Aanmelden">
            </div>

            @Html.ValidationSummary()
        }
    </div>

Do note that in this case the default conventions are used. Of course you can override these, but the default location requires it to be in ~/Views/Auth/Login.cshtml. If you use a derived Controller, this can be anywhere you prefer. 
