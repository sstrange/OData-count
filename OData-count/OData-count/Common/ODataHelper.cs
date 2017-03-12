using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Filters;
using System.Web.Http.Hosting;
using System.Web.OData;
using Newtonsoft.Json;

namespace OData_count.Common
{
    //Problem:      $count is not supported in OData for WebAPI
    //Discussion:   http://stackoverflow.com/questions/16649760/odata-count-doesnt-work-with-entitysetcontrollertentity-tkey-in-web-api-4
    //Solution:     Custom "[CustomEnableQuery]" annotaion is to be used in leu of "[EnableQuery]" when the service needs to return paginated results using $count
    //ex: http://dev.client.srv.edudyn.com/client/18/courses/?$count=true&$skip=1&$top=2

    //Note: This must be used with namespace System.Web.OData,  NOT System.Web.Http.OData. 
    //using System.Web.Http.OData;              //This assembly is OData v3. Do not use this.
    //using System.Web.OData;                   //This assembly is OData v4. Use this one. 

    public class ODataMetadata
    {
        [JsonProperty(PropertyName = "Results")]
        public IEnumerable<object> Value { get; set; }

        [JsonProperty(PropertyName = "Count", NullValueHandling = NullValueHandling.Ignore)]
        public int? Count { get; set; }
    }

    public class CustomEnableQuery : EnableQueryAttribute
    {
        //OnActionExecuted executes after the IQueryable is returned, but before any OData options are applied
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var queryString = actionExecutedContext.Request.GetQueryNameValuePairs().ToList();
            var countRequested = queryString.Any(q => q.Key == "$count" && q.Value == "true");
            if (countRequested)
            {
                var result = new ODataMetadata();

                //Remove top/skip from query string, saving values
                var top = queryString.Where(kvp => kvp.Key == "$top").Select(kvp => kvp.Value).FirstOrDefault();
                var skip = queryString.Where(kvp => kvp.Key == "$skip").Select(kvp => kvp.Value).FirstOrDefault();
                actionExecutedContext.Request.Properties[HttpPropertyKeys.RequestQueryNameValuePairsKey] = queryString.Where(kvp => kvp.Key != "$top" && kvp.Key != "$skip" && kvp.Key != "$count");

                //Apply OData query options, without top/skip
                base.OnActionExecuted(actionExecutedContext);
                if (!actionExecutedContext.Response.IsSuccessStatusCode) return;

                //Get count after $filter has been applied
                var contentFiltered = actionExecutedContext.Response.Content as ObjectContent;
                if (contentFiltered == null) return;
                var queryFiltered = contentFiltered.Value as IQueryable<object>;
                if (queryFiltered == null) return;
                result.Count = queryFiltered.Count();

                //Apply top & skip
                int skipNum;
                if (int.TryParse(skip, out skipNum))
                    queryFiltered = ((IOrderedQueryable<object>)queryFiltered).Skip(skipNum);  //Skip requires IQueryable to be explicitly ordered, but we don't want to change the OData ordering. 

                int topNum;
                if (int.TryParse(top, out topNum))
                    queryFiltered = queryFiltered.Take(topNum);

                result.Value = queryFiltered;
                var status = actionExecutedContext.Response.StatusCode;
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(status, result);
            }
            else
            {
                base.OnActionExecuted(actionExecutedContext);
            }
        }
    }
}




