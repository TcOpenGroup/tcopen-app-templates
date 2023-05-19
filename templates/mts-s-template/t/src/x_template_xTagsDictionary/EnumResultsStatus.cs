
namespace x_template_xTagsDictionary

{
    public enum EnumResultsStatus
    {
        // these results are used when we are requesting tag
        TagFound = 0,
        TagFoundInactive = 10,
        TagFoundAssignedValueEmpty =20,
        TagFoundUnknown = 30,
        TagNotFound = 40,
        //these results are used when we are requesting insertion new into collection
        TagAlreadyExist = 50,
        TagAddedSuccessfully = 60,
        TagAddedNotSuccessfuly = 70,
        EmptyCollection = 100,
       
    }
}
