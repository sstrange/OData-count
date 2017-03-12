using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.OData;
using OData_count.Models;
using OData_count.Common;

namespace OData_count.Controllers
{
    
    //public class StatesController : ODataController
    public class StatesController : ApiController 
    {
        private List<State> states;

        public StatesController()
        {
            Seed();
        }
        
        //[EnableQuery]
        [CustomEnableQuery]
        // GET odata/states
        public IQueryable<State> Get()
        {
            return states.AsQueryable();
        }

        private void Seed()
        {
            states = new List<State>();
            states.Add(NewState(1, "Alaska", "Pacific"));
            states.Add(NewState(2, "Arizona", "Pacific"));
            states.Add(NewState(3, "California", "Pacific"));
            states.Add(NewState(4, "Hawaii", "Pacific"));
            states.Add(NewState(5, "Nevada", "Pacific"));
            states.Add(NewState(6, "Oregon", "Pacific"));
            states.Add(NewState(7, "Utah", "Pacific"));
            states.Add(NewState(8, "Washington", "Pacific"));

            states.Add(NewState(9, "Illinois", "Central"));
            states.Add(NewState(10, "Indiana", "Central"));
            states.Add(NewState(11, "Iowa", "Central"));
            states.Add(NewState(12, "Kentucky", "Central"));
            states.Add(NewState(13, "Michigan", "Central"));
            states.Add(NewState(14, "Minnesota", "Central"));
            states.Add(NewState(15, "Missouri", "Central"));
            states.Add(NewState(16, "Ohio", "Central"));
            states.Add(NewState(17, "Wisconsin", "Central"));
        }

        private State NewState(int id, string name, string region)
        {
            return new State()
            {
                StateId = id,
                Name = name,
                Region = region
            };
        }

        
    }

}
