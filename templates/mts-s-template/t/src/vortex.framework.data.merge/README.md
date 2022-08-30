# Vortex.Framework.Data.Merge #

## Introduction ##

`Vortex.Framework.Data.Merge` is a package to provide merging identical objects (entities).There are two way how to merging data, first is one to one entity and second is
 one to many entities. 

## MergeEntitiesData class ###
Data for merging are stored in any repository that implements `Vortex.Framework.Abstractions.Data.IRepository` interface `MergeEntitiesData<T>` is generic class where T is type of data in collection stored in repository. T object must implemented `Vortex.Framework.Abstractions.Data.IBrowsableDataObject`, `Vortex.Connector.IPlain`

## Implemnetation Example 1 ##
First you must declare an instance of the class

```csharp
   var merge = new MergeEntitiesData<TestData>(
                      repositorySource
                    , repositoryTarget
                    , reqTypes
                    , reqProperties
                    , Exclusion
                    , Inclusion
                    );
```
where  
 - repositorySource and repositoryTarget `IRepository<T>` - here is defined repository where eitities are stored. 

    ```csharp
    private  IRepository<TestData> repositorySource;
    private  IRepository<TestData> repositoryTarget;
    ```

    
    ```csharp
     repositorySource = new MongoDbRepository<TestData>(new MongoDbRepositorySettings<TestData>(connectionString, databaseName, "SourceData"));
            repositoryTarget = new MongoDbRepository<TestData>(new MongoDbRepositorySettings<TestData>(connectionString, databaseName, "TargetData"));
    ```
 -  reqTypes - here you can define list of types witch you want to change in `T` object
 ```csharp
    List<Type> reqTypes = new List<Type>();

    reqTypes.Add(typeof(CustomLogicCheckerData));
    reqTypes.Add(typeof(CustomAnalogCheckerData));
 ```
 -  reqProperties - here you can define list of properties witch you want to change in types defined above
 ```csharp
    List<string> reqProperties = new List<string>();

   reqProperties = PropertyHelper.GetPropertiesNames(c, p => p.IsByPassed, p => p.IsExcluded, p => p.Minimum, p => p.Maximum, p => p.NumberOfAllowedRetries);
 ```
 *You can use a method PropertyHelper.GetPropertiesNames to search properties of object includet to merging*

- Exclusion - here you can define special conditions for exlude from merge operations.
 **Can be also null ,then is irelevant.**
```csharp
 private bool Exclusion(object obj)
        {
            switch (obj)
            {
                // here is definitions of  all types and condition witch are relevat not to merge 
                case IPlainstCheckerData c:
                    return c.Result != (short)enumCheckResult.NoAction;
             
                default:
                    break;
            }

            return false;
        }
```

- Inclusion - define aditional special rules for object to change in merge operations.**Can be also null ,then is irelevant.**
```csharp
 private bool Inclusion(object obj)
        {
            switch (obj)
            {
                // here is definitions of  all types and condition witch are relevat to merge 
                case IPlainstCheckerData c:
                    return return c is IPlainstCheckerData;
             
                default:
                    break;
            }

            return false;
        }
```





### Merge method call ###
```csharp
 merge.Merge(testDataSourceId, testDataTargetId);
```
By calling a method Merge , where  SourceId and TargetId are inputs parameters of method. This parameters are identifiers of entities ( entities implemented `Vortex.Connector.IPlain`).
Merge operation consist of:

- read entities from specified repositiories defined in constructor
- search whole entity  object and find all required type and appropriate properties of this types  and change values from source to target (also is checked conditions from Exclude and Include methods)
- update entity in target repository

## Implemnetation Example 2 ##
First you must declare an instance of the class

```csharp
  var merge = new MergeEntitiesData<TestData>(
                      repositorySource
                    , repositoryTarget
                    );
```
where  
 - repositorySource and repositoryTarget `IRepository<T>` - here is defined repository where eitities are stored. 

    ```csharp
    private  IRepository<TestData> repositorySource;
    private  IRepository<TestData> repositoryTarget;
    ```

    
    ```csharp
     repositorySource = new MongoDbRepository<TestData>(new MongoDbRepositorySettings<TestData>(connectionString, databaseName, "SourceData"));
            repositoryTarget = new MongoDbRepository<TestData>(new MongoDbRepositorySettings<TestData>(connectionString, databaseName, "TargetData"));
    ```
 

### Merge method call ###

```csharp
 merger.Merge(sourceId, targetId, Exclude,Include, ReqProperty);
```
Call a method with this parameters , all rules for merging are defined  below (see examples). 
```csharp
   private IEnumerable<string> ReqProperty(object obj)
        {
            var retVal = new List<string>();
            switch (obj)
            {
                // here you define  properties witch are relevant for reqired types to change by rework 
                case PlainstAnalogueCheckerData c:
                    return PropertyHelper.GetPropertiesNames(c, p => p.IsByPassed, p => p.IsExcluded, p => p.Minimum, p => p.Maximum, p => p.NumberOfAllowedRetries);
                case PlainstLogicCheckerData c:
                    return PropertyHelper.GetPropertiesNames(c, p => p.IsByPassed, p => p.IsExcluded, p => p.NumberOfAllowedRetries);
                case PlainstDataCheckerData c:
                    return PropertyHelper.GetPropertiesNames(c, p => p.IsByPassed, p => p.IsExcluded, p => p.NumberOfAllowedRetries);
                case PlainstTimeCheckerData c:
                    return PropertyHelper.GetPropertiesNames(c, p => p.IsByPassed, p => p.IsExcluded, p => p.NumberOfAllowedRetries);
                case PlainstCu_Header c:
                    return PropertyHelper.GetPropertiesNames(c, p => p.NextOnFailed,p=>p.NextOnPassed);
                
                default:
                    break;
            }

            return new List<string>();
        }
```
```csharp
        private bool Exclude(object obj)
        {
            // some special con
            return false;
        }
```
```csharp
        private bool Include(object obj)
        {
            switch (obj)
            {
                // here is definitions of  all types and condition witch are relevat to change by rework 
                case IPlainstCheckerData c:
                    return c is IPlainstCheckerData;  //c.Result != (short)enumCheckResult.NoAction;
                //case PlainstAnalogueCheckerData c:
                //    return c is PlainstAnalogueCheckerData;
              
                default:
                    break;
            }

            return false;
        }
```
By calling a method Merge , where  SourceId and TargetId are inputs parameters of method. This parameters are identifiers of entities ( entities implemented `Vortex.Connector.IPlain`).
Merge operation consist of:

- read entities from specified repositiories defined in constructor
- search whole entity  object and find all required type and appropriate properties of this types  and change values from source to target (also is checked conditions from Exclude and Include methods)
- update entity in target repository



