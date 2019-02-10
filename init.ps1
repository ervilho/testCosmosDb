dotnet new xunit --name cosmosDbtests # Cr�e le projet
cd cosmosDbtests # va dans le dossier du projet

dotnet add package Microsoft.Azure.DocumentDb.Core # Ajoute le SDK CosmosDb 
dotnet add package NFluent # Ajoute la librairie d'assertions

# Ajoute les librairies de Configuration dotnet
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.Json
dotnet add package Microsoft.Extensions.Configuration.EnvironmentVariables

# cr�er Mesure.cs
# cr�er MeasureRepository.cs
# cr�er MeasureRepositoryTest.cs
# cr�er CosmosDbFixture.cs
