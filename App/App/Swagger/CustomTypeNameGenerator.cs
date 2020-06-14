using NJsonSchema;
using System.Collections.Generic;
using System.Linq;

namespace App.Swagger
{
    public class CustomTypeNameGenerator : DefaultTypeNameGenerator
    {
        /// <inheritdoc />
        public override string Generate(JsonSchema schema, string typeNameHint, IEnumerable<string> reservedTypeNames)
        {
            if (string.IsNullOrEmpty(typeNameHint) && !string.IsNullOrEmpty(schema.DocumentPath))
            {
                typeNameHint = schema.DocumentPath.Replace("\\", "/").Split('/').Last();
            }

            return typeNameHint;
        }
    }
}
