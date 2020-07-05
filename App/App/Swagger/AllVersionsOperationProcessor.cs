using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace App.Swagger
{
    public class AllVersionsOperationProcessor : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            var apiDesc = ((AspNetCoreOperationProcessorContext)context).ApiDescription;
            context.OperationDescription.Operation.Tags.Insert(0, apiDesc.GroupName);

            return true;
        }
    }
}
