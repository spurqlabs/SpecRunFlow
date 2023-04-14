Build
Run following command to install all dependencies
dotnet restore

The dotnet restore command uses NuGet to restore dependencies as well as project-specific tools that are specified in the project file

Run following command to build the code
dotnet build

command builds the project and its dependencies into a set of binaries. The binaries include the project's code in Intermediate Language (IL) files with a .dll extension

Run tests:
run tests using following command:

dotnet test -a:. -l:trx --filter TestCategory=smoke