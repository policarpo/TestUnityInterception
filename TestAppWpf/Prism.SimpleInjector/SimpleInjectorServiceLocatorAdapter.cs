using System;
using System.Collections.Generic;

using Microsoft.Practices.ServiceLocation;

using SimpleInjector;

namespace Prism.SimpleInjector
{
    public class SimpleInjectorServiceLocatorAdapter : ServiceLocatorImplBase
    {
        private readonly Container _container;
      
        public SimpleInjectorServiceLocatorAdapter(Container container)
        {
            _container = container;
        }
     
        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (key != null)
            {
                throw new NotSupportedException();
            }

            return _container.GetInstance(serviceType);
        }
     
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return _container.GetAllInstances(serviceType);
        }
    }
}