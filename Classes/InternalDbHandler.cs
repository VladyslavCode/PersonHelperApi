using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PersonHelperApi.Classes
{
    public class InternalDbHandler
    {
        Mutex MutexObj { get; set; } = new Mutex();
        private List<ApiObjectHandler> _ObjectsList { get; set; } = new List<ApiObjectHandler>();
        public List<ApiObjectHandler> ObjectsList
        {
            get
            {
                MutexObj.WaitOne();
                var result = _ObjectsList;
                MutexObj.ReleaseMutex();
                return result;
            }
            set
            {
                MutexObj.WaitOne();
                _ObjectsList = value;
                MutexObj.ReleaseMutex();
            }
        }
    }

    public class ApiObjectHandler
    {
        public object Request { get; set; }
        public object Response { get; set; }
    }
}
