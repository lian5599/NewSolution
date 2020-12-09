using System;
using System.Reflection;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;

namespace ConsoleApp1
{
    [PSerializable]
    public sealed class OnMethodBoundaryAttribute1 : OnMethodBoundaryAspect
    {
        #region Build-Time Logic

        public OnMethodBoundaryAttribute1()
        {
        }

        public override bool CompileTimeValidate(MethodBase method)
        {
            // TODO: Check that the aspect has been applied on a proper method.

            if (false)
            {
                Message.Write(method, SeverityType.Error, "MY001", "Cannot apply OnMethodBoundaryAttribute1 to method '{0}'.", method);
                return false;
            }

            return true;
        }

        public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
        {
            // TODO: Initialize any instance field whose value only depends on the method to which the aspect is applied.
        }

        #endregion


        public override void RuntimeInitialize(MethodBase method)
        {
            // This method is invoked once at run time.
        }

        public override void OnEntry(MethodExecutionArgs args)
        {
            //  This method is invoked before the execution of the method to which the current aspect is applied.

            // args.Instance contains the object whose method is being executed (null if the method is static).
            // args.Arguments contains method arguments.

            // Set args.FlowBehavior = FlowBehavior.Return to return to the caller without executing the intercepted method.
            // In this case, set args.ReturnValue to a value that is compatible to the return type of the method (null for void methods).

        }

        public override void OnExit(MethodExecutionArgs args)
        {
            // This method is invoked after the execution of the method to which the current aspect is applied (both on success and on exception).
            // It is equivalent to the 'finally' block.

            // args.Instance contains the object whose method is being executed (null if the method is static).
            // args.Arguments contains method arguments.
            // args.ReturnValue contains the return value in case of successful execution.
            // args.Exception contains the current exception (if any).

        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            // This method is invoked after successful execution of the method to which the current aspect is applied.
            // It is equivalent to the 'finally' block.

            // args.Instance contains the object whose method is being executed (null if the method is static).
            // args.Arguments contains method arguments.
            // args.ReturnValue contains the return value in case of successful execution.
        }

        public override void OnException(MethodExecutionArgs args)
        {
            // This method is invoked upon exception in the method to which the current aspect is applied.
            // It is equivelent to the 'catch' block.


            // args.Instance contains the object whose method is being executed (null if the method is static).
            // args.Arguments contains method arguments.
            // args.Exception contains the current exception.

            // By default, the exception will be rethrown after execution of the current advise.
            // Set args.FlowBehavior = FlowBehavior.Return to return to the caller without executing an exception.
            // In this case, set args.ReturnValue to a value that is compatible to the return type of the method (null for void methods).
            // Set args.FlowBehavior = FlowBehavior.ThrowException to throw another exception.
            // In this case, set args.Exception to the new exception.

        }

        // TODO: For better performance, delete any method that the aspect does not use.
    }
}
