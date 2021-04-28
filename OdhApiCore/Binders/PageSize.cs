using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers
{
    [ModelBinder(typeof(PageSizeBinder))]
    public class PageSize
    {
        public int? Value { get; }

        public PageSize(int? value)
        {
            this.Value = value;
        }

        public static implicit operator int?(PageSize pagesize)
        {
            return pagesize.Value;
        }

        public static implicit operator PageSize(int? value)
        {
            return new PageSize(value);
        }
    }

    public class PageSizeBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var firstValue = valueProviderResult.FirstValue;
            if (firstValue == null || firstValue == "null") // "null" exists for compatibility reasons
            {
                bindingContext.Result = ModelBindingResult.Success(new PageSize(10));
            }
            else if (firstValue == "-1" || firstValue == "0") 
            {
                bindingContext.Result = ModelBindingResult.Success(new PageSize(int.MaxValue));
            }
            else if (int.TryParse(firstValue, out var value))
            {
                bindingContext.Result = ModelBindingResult.Success(new PageSize(value));
            }
            else
            {
                bindingContext.ModelState.TryAddModelError(
                    bindingContext.ModelName,
                    bindingContext.ModelMetadata.ModelBindingMessageProvider.ValueIsInvalidAccessor(firstValue)
                );
                bindingContext.Result = ModelBindingResult.Failed();
            }
            return Task.CompletedTask;
        }
    }
}
