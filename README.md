# Tool calling

## Autorisering mot tjenester

- Når du kjører lokalt, lagre hemmeligheter i `dotnet user-secrets`:
  - `AZURE_OPENAI_API_KEY`
  - `AZURE_OPENAI_API_ENDPOINT`
  - `GOOGLE_CLOUD_API_KEY`

## YR

Lenke til lisens: [Creative Commons 4.0](https://developer.yr.no/doc/License/)

## Todos

- State i Tool-objekter
- Mange tools, klassifisering av prompt før registrering av verktøy - Sjekk i hvor stor grad input tokens påvirkes.
- les filinnhold fra en fil, mat til modell og spør om ting.

## Lokal modell

For å kjøre modeller lokalt, følg guiden her: https://learn.microsoft.com/en-us/dotnet/ai/quickstarts/quickstart-local-ai

## Diverse

- Implementasjon av Grep-verktøyet er inspirert fra denne [kodesnutten](https://github.com/dotnet/samples/blob/main/csharp/parallel/ParallelGrep/Program.cs).