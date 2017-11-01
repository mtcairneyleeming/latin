using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace api
{
    public class CommaDelimitedArrayModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelMetadata.IsEnumerableType)
            {
                var key = bindingContext.ModelName;
                var value = bindingContext.ValueProvider.GetValue(key).ToString();

                if (!string.IsNullOrWhiteSpace(value))
                {
                    var elementType = bindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0];
                    var converter = TypeDescriptor.GetConverter(elementType);

                    var values = value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => converter.ConvertFromString(x.Trim()))
                        .ToArray();

                    var typedValues = Array.CreateInstance(elementType, values.Length);

                    values.CopyTo(typedValues, 0);

                    bindingContext.Result = ModelBindingResult.Success(typedValues);
                }
                else
                {
                    Console.WriteLine("string was empty");
                    // change this line to null if you prefer nulls to empty arrays 
                    bindingContext.Result = ModelBindingResult.Success(Array.CreateInstance(bindingContext.ModelType.GetGenericArguments()[0], 0));
                }

                return Task.CompletedTask;
            }
            Console.WriteLine("Not enumerable");
            return Task.CompletedTask;
        }
    }
}
