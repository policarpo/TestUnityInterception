using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAppWpf
{

    //public class MetricInterceptionBehavior : IInterceptionBehavior
    //{
    //    public bool WillExecute
    //    {
    //        get
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }

    //    public IEnumerable<Type> GetRequiredInterfaces()
    //    {
    //        return Type.EmptyTypes;
    //    }

    //    public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
    //    {
    //        Console.WriteLine("Start Interception");
    //        var result = getNext().Invoke(input, getNext);
    //        Console.WriteLine("End Interception");
    //        return result;
    //    }
    //}
}