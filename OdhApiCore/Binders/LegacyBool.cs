using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers
{
    [ModelBinder(typeof(LegacyBoolBinder))]
    public class LegacyBool
    {
        public bool? Value { get; }

        public LegacyBool(bool? value)
        {
            this.Value = value;
        }

        public static implicit operator Boolean?(LegacyBool legacyBool)
        {
            return legacyBool.Value;
        }

        public static implicit operator LegacyBool(bool? boolean)
        {
            return new LegacyBool(boolean);
        }
    }

    public class LegacyBoolBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(
                bindingContext.ModelName
            );
            var firstValue = valueProviderResult.FirstValue;
            if (firstValue == null || firstValue == "null") // "null" exists for compatibility reasons
            {
                bindingContext.Result = ModelBindingResult.Success(new LegacyBool(null));
            }
            else if (bool.TryParse(firstValue, out var value))
            {
                bindingContext.Result = ModelBindingResult.Success(new LegacyBool(value));
            }
            else
            {
                bindingContext.ModelState.TryAddModelError(
                    bindingContext.ModelName,
                    bindingContext.ModelMetadata.ModelBindingMessageProvider.ValueIsInvalidAccessor(
                        firstValue
                    )
                );
                bindingContext.Result = ModelBindingResult.Failed();
            }
            return Task.CompletedTask;
        }
    }
}
