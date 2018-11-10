# enum-parser
A simple to use lib to parse a string to a enum value. Given a set of strings you can parse them to a flagable enum. Given a string that is different to the enum value you can parse it too by using an attribute.

# Usecases

Given one or more of the following usecases you may benefit from this lib.

## Parseing single string value to enum value

This is the simplest case. You have a enum like this one:
 
```csharp
public enum Test {
	A
}
```

And you want to parse "A" to the enum value ```Test.A```. This is the simplest case which can also be achieved by using ```Enum.Parse(typeof(Test), "A")```.

## Parsing a string value that is not named like the enum value

Given a better named enum:

```csharp
public enum CarProducer {
	Volkswagen, BayerischeMotorenWerke, Toyota
}
```

And you want to parse the string "VW" or "BMW" or "T" to the respective enum value.
This can be done by using the ```EnumNameAttribute```. This must be set onto the enum type itself and onto every value.

```csharp
[EnumName]
public enum CarProducer {
	[EnumName("VW")]
	Volkswagen, 
	[EnumName("BMW")]
	BayerischeMotorenWerke, 
	[EnumName("T")]
	Toyota
}
```

## Parsing multiple strings into one enum value by using the flag feature

C# supports concatening multiple enum values into one by using an "or" operator.
To do this, we must set the attribute "Flag" onto the enum type:

```csharp
[Flags]
[EnumName]
public enum CarProducer {
	[EnumName("VW")]
	Volkswagen = 1, 
	[EnumName("BMW")]
	BayerischeMotorenWerke = 2, 
	[EnumName("T")]
	Toyota = 4
}
```

Now you can parse the string "VW;BMW" into a enum value.

## Supported Datatypes

To support flags with more than 32 values, you can inherit from "long". The lib respects your choice:
 
```csharp
public enum CarProducer : long {
...
	Volkswagen = 2^64 // this will work, too
...
}
```
## Usage

Example how to apply the "or" operator:

```csharp
object enum1 = CarProducer.Volkswagen;
object enum2 = CarProducer.Toyota;

var value = (CarProducer)EnumParser.EnumOr(enum1, enum2);

Console.WriteLine(value.HasFlag(CarProducer.Volkswagen));
```

How to parse:

```csharp
EnumParser.Parse(typeof(CarProducer), "VW;BMW");
```

## Nuget

The lib is available on nuget. 

## [Changlog](CHANGELOG.md)