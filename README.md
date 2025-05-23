# MorphMapper

# Your Effortless .NET Object-to-Object Mapper

[![.NET 9](https://img.shields.io/badge/.NET-9.0-brightgreen.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Nuget](https://img.shields.io/nuget/v/MorphMapper)](https://www.nuget.org/packages/MorphMapper)

**MorphMapper** is a lightweight and efficient object-to-object mapping library for .NET 9. It simplifies the process of transferring data between different types with minimal configuration. Say goodbye to repetitive manual mapping code and embrace the elegance of automatic property mapping.

## Features

* **Automatic Property Mapping:** Maps properties with the same name and compatible types by default.
* **Type Conversion:** Handles automatic conversion between compatible types.
* **Ignoring Properties:** Easily exclude specific properties from the mapping process.
* **Lightweight and Performant:** Designed with efficiency in mind, minimizing overhead.
* **.NET 9 Support:** Built specifically for the latest .NET runtime.

## Installation
Coming soon:
You can install MorphMapper via NuGet Package Manager:

```bash
Install-Package MorphMapper
```

## Quick Start

Initialize the mapper and the configuration
```c#

var mapperConfig = new MapperConfiguration(Assembly.GetExecutingAssembly()); 

var mapper = new Mapper(mapperConfig);

var result = mapper.Map<HouseDto, House>(sourceObjectMock);

```

In the HouseMapper.cs just override the Configure method

```c#
public class HouseMapper : Mapping<HouseDto, House>
{
    public override Mapping<HouseDto, House> Configure(Mapping<HouseDto, House> mapping)
    {
        mapping.ForMember(x => x.Id, y => y.MapFrom(x => x.Id));
        mapping.ForMember(x => x.Name, y => y.MapFrom(x => x.Name));
        mapping.ForMember(x => x.Description, y => y.MapFrom(x => x.Description));

        return mapping;
    }
}
```
## Contributing

Contributions are more then welcome, please feel free to submit pull requests, report issues, or suggest new features.

## License
MorphMapper is licensed under the MIT License.
