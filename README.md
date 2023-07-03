# ASP.NET API practice
A collection of different API implementations using different services.

## CityInfo
API for getting and managing simple information about cities and their points of interest. Inspired by
[Kevin's Dockx](https://app.pluralsight.com/library/courses/asp-dot-net-core-6-web-api-fundamentals/table-of-contents)
course from pluralsight.

**Types of endpoints implemented:** </br>
&emsp;`get`, `post`, `put`, `patch`, `delete`

**Services used:**
- `JsonPatch` - to process the **patch** endpoint
- `NewtonsoftJson` formatter - to serialize a JsonPatch document
- `FileExtensionContentTypeProvider` - to create a `Content-Type` field for specific files
- `XmlDataContractSerializerFormatters` - output XML serialization
- `Serilog` - for logging into file and console
  - `serilog.sinks.file`
  - `serilog.sinks.console`
- Dummy mail service - custom created
  - different implementations for development and production
  - mail addresses stored in configuration file