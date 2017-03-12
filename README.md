# OData-count
This is an example of how to extend the query options for OData v4 for Web API using a custom data annotation. 

Specifically this is an implementation of the OData query option $count, which returns the number of results (after applying $filter but before applying $top/$skip) and is necessary for pagination, Kendo grids, etc.. and only available if your controllers are derived from the ODataController class. 

If you don't want to switch to ODataController (like say, all your controllers are derived from the same base controller and you don't want the responses of all your services in production to change), this could be useful. 
The relevant code is in /common/ODataHelper.cs


####Using ODataController and the standard [EnableQuery] 
```
    public class StatesController : ODataController
    {
        ...
        [EnableQuery]
        public IQueryable<State> Get()
        {
            return states.AsQueryable();
        }
    }
```
![ODataController count](https://cloud.githubusercontent.com/assets/7025212/23828128/71e11edc-067c-11e7-83d1-7870705761b2.JPG)


####Using APIController and [CustomEnableQuery] 
```
    public class StatesController : APIController
    {
        ...
        [CustomEnableQuery]
        public IQueryable<State> Get()
        {
            return states.AsQueryable();
        }
    }
```
![ODataController count](https://cloud.githubusercontent.com/assets/7025212/23828127/71de2aa6-067c-11e7-93a7-393a8c8ffdd2.JPG)

