using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace TestAppWpf
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method)]
    public class MetricInterceptionAttribute : HandlerAttribute
    {
        private readonly int order;

        public MetricInterceptionAttribute(int order)
        {
            this.order = order;
        }

        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            var handler = container.Resolve<MetricCallHandler>();
            handler.Order = order;
            return handler;
        }

    }

    public class MetricCallHandler : ICallHandler
    {        
        public int Order
        {
            get; set;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            var method = input.MethodBase as MethodInfo;     
            var value = getNext().Invoke(input, getNext);
            

            if (value.ReturnValue != null
                    && method != null
                    && typeof(Task) == method.ReturnType)
            {
                var task = (Task)value.ReturnValue;
                return input.CreateMethodReturn(CreateWrapperTask(task, input), value.Outputs);
            }

            
            return value;
        }

        private async Task CreateWrapperTask(Task task, IMethodInvocation input)
        {
            try
            {
                string methodName = $"{input.MethodBase.DeclaringType.FullName}.{input.MethodBase.Name}";
                Console.WriteLine($"** Handler Start Interception {methodName}");

                var stopWatch = Stopwatch.StartNew();

                await task.ConfigureAwait(false);

                stopWatch.Stop();
                Console.WriteLine($"** Handler End Interception {methodName} in (ss:ms) {stopWatch.Elapsed.Seconds}:{stopWatch.Elapsed.Milliseconds}");
            }
            catch (Exception e)
            {
                Trace.TraceWarning("Async operation {0} threw: {1}",
                  input.MethodBase.Name, e);
                throw;
            }
        }

    }
}